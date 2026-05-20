using Application.Models.Input;
using Application.Models.Internal;
using Application.Models.Internal.Messages;
using Application.Models.Internal.Options;
using Application.Models.OptionsAndHelpers;
using Application.Models.Output;
using Application.Services.Interfaces;
using Domain.Stores;
using Infrastructure.Storage;
using Microsoft.Extensions.Options;

namespace Application.Services.Implementations
{
    public class BotChatService : IBotChatService
    {
        private readonly string _mediaPrefix;
        private readonly IBotChatStore _botChatStore;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IObjectStorage _objectStorage;
        private readonly IMessageCacheService _messageCacheService;
        public BotChatService(
            IBotChatStore botChatStore, IMessagePublisher messagePublisher,
            IObjectStorage objectStorage, IMessageCacheService messageCacheService, IOptions<ApplicationServicesOptions> options)
        {
            _botChatStore = botChatStore;
            _messagePublisher = messagePublisher;
            _objectStorage = objectStorage;
            _messageCacheService = messageCacheService;
            _mediaPrefix = options.Value.MediaPrefix;
        }
        public async Task DeleteChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            await _botChatStore.DeleteChatAsync(chatId, userId, cancellationToken);
            await _messagePublisher.PublishAsync(
                new ChatDeletedMessage()
                {
                    ChatId = chatId,
                    ChatType = Models.Internal.Constants.ChatType.Bot,
                    UserId = [userId]
                },
                cancellationToken);
        }

        public async Task DisableBotAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            await _botChatStore.DisableBotAsync(userId, chatId, cancellationToken);
        }

        public async Task EnableBotAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            await _botChatStore.EnableBotAsync(userId, chatId, cancellationToken);
        }

        public async Task<BotButton[]> GetActiveButtonsAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            Domain.Models.Types.BotButtonInfo[] buttons = await _botChatStore.GetActiveButtonsListAsync(chatId, userId, cancellationToken);
            return buttons.Select(b => new BotButton(b)).ToArray();
        }

        public async Task<Guid> GetBotIdByChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            return await _botChatStore.GetBotIdByChatIdAsync(chatId, userId, cancellationToken);
        }

        public async Task<ChatShortInfo> GetChatShortInfoAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            return new ChatShortInfo(await _botChatStore.GetChatShortInfoAsync(chatId, userId, cancellationToken), _mediaPrefix);
        }

        public async Task<Message> GetMessageAsync(Guid userId, Guid chatId, Guid messageId, CancellationToken cancellationToken)
        {
            Message? message = await _messageCacheService.GetAsync(messageId, chatId, cancellationToken);
            if (message is null)
            {
                Domain.Models.Types.Message messageData = await _botChatStore.GetMessageAsync(chatId, messageId, userId, cancellationToken);
                message = new Message(messageData, _mediaPrefix, chatId);
                await _messageCacheService.SaveAsync(message, cancellationToken);
            }
            return message;
        }

        public async Task<Message[]> GetMessagesAsync(Guid userId, Guid chatId, MessagesSelectOptions options, CancellationToken cancellationToken)
        {
            Domain.Models.Types.Message[] messages = await _botChatStore.GetMessagesAsync(chatId, userId, options.MessagesCount, options.SentBefore ?? DateTime.UtcNow, cancellationToken);
            return messages.Select(m => new Message(m, _mediaPrefix, chatId)).ToArray();
        }

        public async Task<Guid> OpenChatWithBotAsync(Guid userId, Guid botId, CancellationToken cancellationToken)
        {
            return await _botChatStore.CreateChatAsync(userId, botId, cancellationToken);
        }

        public async Task<Guid[]> ResendMessagesAsync(Guid userId, ResendMessagesModel resendMessagesModel, CancellationToken cancellationToken)
        {
            Guid[] ids = await _botChatStore.ResendMessagesAsync(resendMessagesModel.ChatId, userId, ChatTypeConverter.Convert(resendMessagesModel.SourceChatType), resendMessagesModel.SourceChatId, resendMessagesModel.Messages, cancellationToken);
            await _messagePublisher.PublishAsync(
                new MessagesSentMessage()
                {
                    ChatId = resendMessagesModel.ChatId,
                    ChatType = Models.Internal.Constants.ChatType.Bot,
                    UserId = [userId],
                    MessagesId = ids,
                },
                cancellationToken);
            return ids;
        }

        public async Task<Guid> SendMessageAsync(SendingMessage sendingMessage, CancellationToken cancellationToken)
        {
            Domain.Models.Types.MediaFile[]? attachments = null;
            if (sendingMessage.Attachments != null && sendingMessage.Attachments.Length > 0)
            {
                var list = new List<Domain.Models.Types.MediaFile>(sendingMessage.Attachments.Length);
                foreach (var a in sendingMessage.Attachments)
                {
                    var mediaId = Guid.NewGuid();
                    await _objectStorage.SaveAsync(a.Content, mediaId, cancellationToken);
                    list.Add(a.ToMediaFile(mediaId));
                }
                attachments = list.ToArray();
            }

            Guid id = await _botChatStore.SendMessageAsync(
                sendingMessage.ChatId, sendingMessage.Author,
                sendingMessage.ReplyTo, sendingMessage.MessageText,
                attachments, cancellationToken);

            await _messagePublisher.PublishAsync(
                new MessagesSentMessage()
                {
                    ChatId = sendingMessage.ChatId,
                    ChatType = Models.Internal.Constants.ChatType.Bot,
                    UserId = [sendingMessage.Author],
                    MessagesId = [id]
                },
                cancellationToken);

            return id;
        }
    }
}
