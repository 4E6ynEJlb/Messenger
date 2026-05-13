using Application.Models.Internal.Messages;
using Application.Services.Interfaces;
using MassTransit;

namespace Application.Services.Implementations
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IBus _bus;
        public MessagePublisher(IBus bus)
        {
            _bus = bus;
        }
        public async Task PublishAsync(BusMessage message, CancellationToken cancellationToken)
        {
            await _bus.Publish(message, cancellationToken);
        }
    }
}
