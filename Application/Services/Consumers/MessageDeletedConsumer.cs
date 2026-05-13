using Application.Models.Internal.Messages;
using Application.Services.Interfaces;
using MassTransit;

namespace Application.Services.Consumers
{
    public class MessageDeletedConsumer : IConsumer<MessageDeletedMessage>
    {
        private readonly IUpdatesService _updatesService;
        public MessageDeletedConsumer(IUpdatesService updatesService)
        {
            _updatesService = updatesService;
        }
        public async Task Consume(ConsumeContext<MessageDeletedMessage> context)
        {
            MessageDeletedMessage message = context.Message;
            await _updatesService.MessageDeleted(
                message.ChatId, message.MessageId, 
                message.UserId, message.ChatType, context.CancellationToken);
        }
    }
}
