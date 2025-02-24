using ClubMembership_Authentication.Events;
using System.Text.Json;
using RabbitMQ.Client;
using System.Text;

namespace ClubMembership_Authentication.Infrastructure.Messaging
{
    public class EventBusPublisher : IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public EventBusPublisher(ConnectionFactory factory)
        {
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

            _channel.ExchangeDeclareAsync(exchange: "membership_exchange", type: ExchangeType.Fanout);
        }

        public async Task PublishMembershipCreatedEventAsync(MembershipCreatedEvent membershipEvent)
        {
            try
            {
                var message = JsonSerializer.Serialize(membershipEvent);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = new BasicProperties();

                await _channel.BasicPublishAsync(exchange: "membership_exchange",
                                                 routingKey: "",
                                                 mandatory: false,
                                                 basicProperties: properties,
                                                 body: body);

                Console.WriteLine($"[✓] Published membership event: {membershipEvent.MembershipType}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error publishing event: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }
    }
}
