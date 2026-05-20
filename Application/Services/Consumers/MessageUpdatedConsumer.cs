using Application.Models.Internal.Messages;
using Application.Services.Interfaces;
using MassTransit;

namespace Application.Services.Consumers
{
    public class MessageUpdatedConsumer : IConsumer<MessageUpdatedMessage>
    {
        private readonly IUpdatesService _updatesService;
        public MessageUpdatedConsumer(IUpdatesService updatesService)
        {
            _updatesService = updatesService;
        }
        public async Task Consume(ConsumeContext<MessageUpdatedMessage> context)
        {
            MessageUpdatedMessage message = context.Message;
            await _updatesService.MessageUpdated(
                message.ChatId, message.MessageId, 
                message.UserId, message.ChatType, context.CancellationToken);
        }
    }
}
