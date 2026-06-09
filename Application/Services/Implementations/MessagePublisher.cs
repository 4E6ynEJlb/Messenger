using Application.Models.Internal.Messages;
using Application.Services.Interfaces;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services.Implementations
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IServiceProvider _serviceProvider;
        public MessagePublisher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : BusMessage
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                ITopicProducer<T> producer = scope.ServiceProvider.GetRequiredService<ITopicProducer<T>>();
                await producer.Produce(message, cancellationToken);
            }
        }
    }
}
