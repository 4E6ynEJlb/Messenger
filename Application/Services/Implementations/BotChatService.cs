using Application.Models.Input;
using Application.Models.Internal;
using Application.Models.Internal.Constants;
using Application.Models.Internal.Messages;
using Application.Models.Internal.Options;
using Application.Models.OptionsAndHelpers;
using Application.Models.Output;
using Application.Services.Interfaces;
using Domain.Stores;
using Domain.Stores.MongoDB;
using Microsoft.Extensions.Options;

namespace Application.Services.Implementations
{
    public class BotChatService : IBotChatService
    {
        private readonly string _mediaPrefix;
        private readonly IBotChatStore _botChatStore;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMessageCacheService _messageCacheService;
        private readonly IMessagesService _messagesService;
        private readonly IDeletedChatStore _deletedChatStore;
        public BotChatService(
            IBotChatStore botChatStore, IMessagePublisher messagePublisher, 
            IMessageCacheService messageCacheService, IMessagesService messagesService, 
            IDeletedChatStore deletedChatStore, IOptions<ApplicationServicesOptions> options)
        {
            _botChatStore = botChatStore;
            _messagePublisher = messagePublisher;
            _messageCacheService = messageCacheService;
            _messagesService = messagesService;
            _deletedChatStore = deletedChatStore;
            _mediaPrefix = options.Value.MediaPrefix;
        }
        public async Task DeleteChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            bool completed = false;
            try
            {
                await _deletedChatStore.SaveAsync(chatId, ChatTypeConverter.Convert(ChatType.Bot), cancellationToken);
                await _botChatStore.DeleteChatAsync(chatId, userId, cancellationToken);
                completed = true;
            }
            catch (Exception ex)
            {
                if (!completed)
                    await _deletedChatStore.DeleteAsync(chatId, ChatTypeConverter.Convert(ChatType.Bot), CancellationToken.None);
                else
                    await _messagePublisher.PublishAsync(
                        new ChatDeletedMessage()
                        {
                            ChatId = chatId,
                            ChatType = ChatType.Bot,
                            UserId = [userId]
                        },
                        CancellationToken.None);
                throw ex;
            }
            await _messagePublisher.PublishAsync(
                new ChatDeletedMessage()
                {
                    ChatId = chatId,
                    ChatType = ChatType.Bot,
                    UserId = [userId]
                },
                CancellationToken.None);
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
                message = await _messagesService.GetMessageByIdAsync(userId, ChatType.Bot, chatId, messageId, cancellationToken);
                await _messageCacheService.SaveAsync(message, cancellationToken);
            }
            return message;
        }

        public async Task<Message[]> GetMessagesAsync(Guid userId, Guid chatId, MessagesSelectOptions options, CancellationToken cancellationToken)
        {            
            return await _messagesService.GetMessagesAsync(userId, ChatType.Bot, chatId, options, cancellationToken);
        }

        public async Task<Guid> OpenChatWithBotAsync(Guid userId, Guid botId, CancellationToken cancellationToken)
        {
            return await _botChatStore.CreateChatAsync(userId, botId, cancellationToken);
        }

        public async Task<Guid[]> ResendMessagesAsync(Guid userId, ResendMessagesModel resendMessagesModel, CancellationToken cancellationToken)
        {
            bool completed = false;
            Guid[] ids = Array.Empty<Guid>();
            try
            {
                ids = await _messagesService.ResendMessagesAsync(userId, ChatType.Bot, resendMessagesModel, cancellationToken);
                completed = true;
            }
            finally
            {
                if (completed)
                    await _messagePublisher.PublishAsync(
                        new MessagesSentMessage()
                        {
                            ChatId = resendMessagesModel.ChatId,
                            ChatType = ChatType.Bot,
                            UserId = [userId],
                            MessagesId = ids,
                        },
                        CancellationToken.None);                
            }
            return ids;
        }

        public async Task<Guid> SendMessageAsync(SendingMessage sendingMessage, CancellationToken cancellationToken)
        {
            bool completed = false;
            Guid id = Guid.Empty;
            try
            {
                id = await _messagesService.SendUserMessageAsync(ChatType.Bot, sendingMessage, cancellationToken);
                completed = true;
            }
            finally
            {
                if (completed)
                    await _messagePublisher.PublishAsync(
                        new MessagesSentMessage()
                        {
                            ChatId = sendingMessage.ChatId,
                            ChatType = ChatType.Bot,
                            UserId = [sendingMessage.Author],
                           MessagesId = [id]
                        },
                      CancellationToken.None);
            }
            return id;
        }
    }
}
