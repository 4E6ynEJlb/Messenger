using Application.Models.Internal.Messages;
using Application.Services.Interfaces;
using MassTransit;

namespace Application.Services.Consumers
{
    public class BotButtonsUpdatedConsumer : IConsumer<BotButtonsUpdatedMessage>
    {
        private readonly IUpdatesService _updatesService;
        public BotButtonsUpdatedConsumer(IUpdatesService updatesService)
        {
            _updatesService = updatesService;
        }
        public async Task Consume(ConsumeContext<BotButtonsUpdatedMessage> context)
        {
            BotButtonsUpdatedMessage message = context.Message;
            await _updatesService.BotButtonsUpdated(
                message.ChatId, message.UserId, context.CancellationToken);
        }
    }
}
