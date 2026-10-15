using System.Text;
using System.Text.Json;
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