namespace Application.Models.Input
{
    public record ChatsSelectOptions
    {
        public required uint Page { get; init; }
        public required uint PageSize { get; init; }
    }
}
