using System.Text;
using System.Text.Json;
using Dapper;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderFlow.ShipmentWorker;

public class OrderApprovedConsumer : BackgroundService
{
    private const string Exchange = "orderflow.events";
    private const string Queue = "shipment.order-approved";
    private const string RoutingKey = "OrderApprovedEvent";

    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderApprovedConsumer> _logger;

    public OrderApprovedConsumer(IConfiguration configuration, ILogger<OrderApprovedConsumer> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var db = new NpgsqlConnection(_configuration.GetConnectionString("Shipment"));
        await db.ExecuteAsync("""
                CREATE TABLE IF NOT EXISTS processed_messages (
                    message_id   uuid PRIMARY KEY,
                    processed_at timestamptz NOT NULL DEFAULT now()
                )
                """);

        var factory = new ConnectionFactory { HostName = _configuration["RabbitMq:Host"] ?? "localhost" };
        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(Exchange, ExchangeType.Topic, durable: true, cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync(Queue, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await channel.QueueBindAsync(Queue, Exchange, RoutingKey, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, args) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(args.Body.Span);
                var message = JsonSerializer.Deserialize<OrderApprovedMessage>(json)!;

                var messageId = Guid.Parse(args.BasicProperties.MessageId!);

                var inserted = await db.ExecuteAsync(
                    "INSERT INTO processed_messages (message_id) VALUES (@Id) ON CONFLICT (message_id) DO NOTHING",
                    new { Id = messageId });

                if (inserted == 0)
                {
                    _logger.LogInformation("Message {MessageId} already processed, skipping", messageId);
                    await channel.BasicAckAsync(args.DeliveryTag, multiple: false);
                    return;
                }

                _logger.LogInformation("Preparing shipment for order {OrderId}, dealer {DealerId}, total {Total}",
                    message.OrderId, message.DealerId, message.Total);

                await channel.BasicAckAsync(args.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message {MessageId}", args.BasicProperties.MessageId);
                await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await channel.BasicConsumeAsync(Queue, autoAck: false, consumer, stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}