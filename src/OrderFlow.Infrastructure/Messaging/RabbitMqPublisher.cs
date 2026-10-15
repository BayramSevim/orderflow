using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace OrderFlow.Infrastructure.Messaging
{
    public class RabbitMqPublisher : IAsyncDisposable
    {
        public const string Exchange = "orderflow.events";

        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMqPublisher(IConfiguration configuration)
        {
            _factory = new ConnectionFactory { HostName = configuration["RabbitMq:Host"]! };
        }

        public async Task PublishAsync(string routingKey, string payload, Guid messageId, CancellationToken ct)
        {
            if (_channel is null || _channel.IsClosed)
            {
                _connection = await _factory.CreateConnectionAsync(ct);
                _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
                await _channel.ExchangeDeclareAsync(Exchange, ExchangeType.Topic, durable: true, cancellationToken: ct);
            }

            var properties = new BasicProperties
            {
                MessageId = messageId.ToString(),
                Persistent = true,
                ContentType = "application/json"
            };

            await _channel.BasicPublishAsync(Exchange, routingKey, mandatory: false, properties,
                Encoding.UTF8.GetBytes(payload), ct);
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel is not null) await _channel.DisposeAsync();
            if (_connection is not null) await _connection.DisposeAsync();
        }
    }
}