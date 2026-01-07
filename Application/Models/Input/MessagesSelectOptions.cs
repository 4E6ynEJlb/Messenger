namespace Application.Models.Input
{
    public record MessagesSelectOptions
    {
        public required uint MessagesCount { get; init; }
        public required DateTime? SentBefore { get; init; }
    }
}
