namespace UserAPI.Services.Interfaces
{
    public interface IUpdatesService
    {
        Task PrivateMessagesRead(Guid chatId, Guid[] messagesId, Guid receiver);
        Task PublicMessagesRead(Guid chatId, Guid userId, Guid[] messagesId, Guid receiver);
        Task UserTypingPrivate(Guid chatId, Guid userId, Guid receiver);
        Task UserTypingPublic(Guid chatId, Guid userId, Guid[] receivers);
        Task NewPrivateMessage(Guid chatId, Guid messageId, Guid[] receivers);
        Task NewPublicMessage(Guid chatId, Guid messageId, Guid[] receivers);
    }
}
