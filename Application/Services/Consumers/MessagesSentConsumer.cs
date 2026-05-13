using Application.Models.Internal.Messages;
using Application.Services.Interfaces;
using MassTransit;

namespace Application.Services.Consumers
{
    public class MessagesSentConsumer : IConsumer<MessagesSentMessage>
    {
        private readonly IUpdatesService _updatesService;
        public MessagesSentConsumer(IUpdatesService updatesService)
        {
            _updatesService = updatesService;
        }
        public async Task Consume(ConsumeContext<MessagesSentMessage> context)
        {
            MessagesSentMessage message = context.Message;
            await _updatesService.MessagesSent(
                message.ChatId, message.MessagesId, 
                message.UserId, message.ChatType, context.CancellationToken);
        }
    }
}
