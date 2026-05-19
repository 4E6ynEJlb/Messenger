namespace Application.Models.Internal.Messages
{
    public record BotButtonsUpdatedMessage : BusMessage
    {
        public required Guid ChatId {  get; init; }
        public required Guid UserId { get; init; }
    }
}
