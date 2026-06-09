using Application.Models.Internal.Messages;
using Application.Services.Interfaces;
using MassTransit;

namespace Application.Services.Consumers
{
    public class ChatDeletedConsumer : IConsumer<ChatDeletedMessage>
    {
        private readonly IUpdatesService _updatesService;
        public ChatDeletedConsumer(IUpdatesService updatesService)
        {
            _updatesService = updatesService;
        }
        public async Task Consume(ConsumeContext<ChatDeletedMessage> context)
        {
            ChatDeletedMessage message = context.Message;
            await _updatesService.ChatDeleted(
                message.ChatId, message.UserId, 
                message.ChatType, context.CancellationToken);
        }
    }
}
