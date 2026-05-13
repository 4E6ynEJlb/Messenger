using Application.Models.Internal.Messages;
using Application.Services.Interfaces;
using MassTransit;

namespace Application.Services.Consumers
{
    public class UserIsTypingConsumer : IConsumer<UserIsTypingMessage>
    {
        private readonly IUpdatesService _updatesService;
        public UserIsTypingConsumer(IUpdatesService updatesService)
        {
            _updatesService = updatesService;
        }
        public async Task Consume(ConsumeContext<UserIsTypingMessage> context)
        {
            UserIsTypingMessage message = context.Message;
            await _updatesService.UserIsTyping(
                message.ChatId, message.TypingUserId, 
                message.DestinationUserId, message.ChatType, context.CancellationToken);
        }
    }
}
