using Application.Models.Internal.Messages;

namespace Application.Services.Interfaces
{
    public interface IMessagePublisher
    {
        public Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : BusMessage;
    }
}
