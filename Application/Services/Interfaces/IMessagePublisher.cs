using Application.Models.Internal.Messages;

namespace Application.Services.Interfaces
{
    public interface IMessagePublisher
    {
        public Task PublishAsync(BusMessage message, CancellationToken cancellationToken);
    }
}
