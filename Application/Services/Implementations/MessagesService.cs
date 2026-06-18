using Application.Models.Input;
using Application.Models.Internal;
using Application.Models.Internal.Constants;
using Application.Models.Internal.Options;
using Application.Models.OptionsAndHelpers;
using Application.Models.Output;
using Application.Services.Interfaces;
using Domain;
using Domain.Models.Documents;
using Domain.Stores;
using Domain.Stores.MongoDB;
using Infrastructure.Storage;
using Microsoft.Extensions.Options;
using Persistence.Exceptions;

namespace Application.Services.Implementations
{
    public class MessagesService : IMessagesService
    {
        private readonly string _mediaPrefix;
        private readonly IGenericChatStore _genericChatStore;
        private readonly IPersonalChatStore _personalChatStore;
        private readonly IPublicChatStore _publicChatStore;
        private readonly IBotChatStore _botChatStore;
        private readonly IDeletedAttachmentStore _deletedAttachmentStore;
        private readonly IDeletedMessageStore _deletedMessageStore;
        private readonly IMessageUpdateStore _messageUpdateStore;
        private readonly INewMediaStore _newMediaStore;
        private readonly INewMessageStore _newMessageStore;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IObjectStorage _objectStorage;
        public MessagesService(IDeletedMessageStore deletedMessageStore, IMessageUpdateStore messageUpdateStore,
            INewMediaStore newMediaStore, INewMessageStore newMessageStore, IDeletedAttachmentStore deletedAttachmentStore,
            IPersonalChatStore personalChatStore, IPublicChatStore publicChatStore,
            IBotChatStore botChatStore, IUnitOfWork unitOfWork, IObjectStorage objectStorage, 
            IOptions<ApplicationServicesOptions> options)
        {
            _deletedMessageStore = deletedMessageStore;
            _messageUpdateStore = messageUpdateStore;
            _newMediaStore = newMediaStore;
            _newMessageStore = newMessageStore;
            _genericChatStore = unitOfWork.GenericChat;
            _personalChatStore = personalChatStore;
            _publicChatStore = publicChatStore;
            _botChatStore = botChatStore;
            _deletedAttachmentStore = deletedAttachmentStore;
            _unitOfWork = unitOfWork;
            _objectStorage = objectStorage;
            _mediaPrefix = options.Value.MediaPrefix;
        }

        public async Task DeleteAttachmentAsync(Guid userId, Guid chatId, ChatType chatType, Guid messageId, Guid mediaId, CancellationToken cancellationToken)
        {
            bool canCreate = false;
            try
            {
                canCreate = await _genericChatStore.CheckAccessToMessageAsync(ChatTypeConverter.Convert(chatType), chatId, userId, messageId, cancellationToken)
                    && await _genericChatStore.CheckAccessToAttachmentAsync(ChatTypeConverter.Convert(chatType), chatId, messageId, mediaId, cancellationToken);
            }
            catch (ResourceNotFoundException ex)
            {
                NewMessage? message = await _newMessageStore.GetOneByIdAsync(messageId, cancellationToken);

                if (message is null
                    || !message.AttachedMedia.Contains(mediaId)
                    || message.Author != userId && chatType != ChatType.Group) throw ex;

                if (message.ResentFrom is not null)
                    throw new ForbiddenOperationException(
                        new Exception("Attachments cannot be removed from forwarded messages."),
                        ErrorMessages.FORBIDDEN_OPERATION);

                if (chatType == ChatType.Group)
                    canCreate = await _publicChatStore.CheckMessageDeleteAbility(chatId, userId, message.Author, cancellationToken);
            }
            finally
            {
                if (canCreate)
                {
                    await _deletedAttachmentStore.CreateAsync(new DeletedAttachment()
                    {
                        Id = new Domain.Models.Documents.Keys.DeletedAttachmentKey()
                        {
                            ChatId = chatId,
                            ChatType = ChatTypeConverter.Convert(chatType),
                            MessageId = messageId,
                            MediaId = mediaId
                        },
                        DeletedAt = DateTime.UtcNow,
                        DeletedBy = userId
                    }, cancellationToken);
                }
            }
        }

        public async Task DeleteMessageAsync(Guid userId, ChatType chatType, Guid chatId, Guid messageId, CancellationToken cancellationToken)
        {
            bool canCreate = false;
            DateTime? sentAt = null;
            try
            {
                bool access =
                    await _genericChatStore.CheckAccessToMessageAsync(ChatTypeConverter.Convert(chatType),
                    chatId, userId, messageId, cancellationToken);

                Domain.Models.Types.Message[] sqlMessage =
                    await _genericChatStore.GetMessagesByIdAsync(chatId, [messageId], userId,
                    ChatTypeConverter.Convert(chatType), cancellationToken);

                if (sqlMessage.Length != 0)
                    sentAt = sqlMessage[0].SentAt;

                canCreate = access;
            }
            catch (ResourceNotFoundException ex)
            {
                NewMessage? message = await _newMessageStore.GetOneByIdAsync(messageId, cancellationToken);

                if (message is null
                    || message.Author != userId && chatType != ChatType.Group) throw ex;

                if (chatType == ChatType.Group)
                    canCreate =
                        await _publicChatStore.CheckMessageDeleteAbility(chatId, userId,
                        message.Author, cancellationToken);

                sentAt = message.SentAt;
            }
            finally
            {
                if (canCreate)
                {
                    await _deletedMessageStore.CreateAsync(new DeletedMessage()
                    {
                        ChatId = chatId,
                        ChatType = ChatTypeConverter.Convert(chatType),
                        MessageId = messageId,
                        DeletedAt = DateTime.UtcNow,
                        DeletedBy = userId,
                        SentAt = sentAt ?? DateTime.MinValue
                    }, cancellationToken);
                }
            }
        }

        public async Task<Message> GetMessageByIdAsync(Guid userId, ChatType chatType, Guid chatId, Guid messageId, CancellationToken cancellationToken)
        {
            ResourceNotFoundException notFoundEx = new ResourceNotFoundException(new Exception("Message not found"));

            if (await _deletedMessageStore.CheckDeletionByIdAsync(messageId, cancellationToken))
                throw notFoundEx;

            Domain.Models.Types.Message[] sqlResult = await _genericChatStore.GetMessagesByIdAsync(chatId, [messageId], userId, ChatTypeConverter.Convert(chatType), cancellationToken);

            if (sqlResult.Length != 0)
            {
                Domain.Models.Types.Message preparingSqlResult = sqlResult[0];
                preparingSqlResult.AttachedMedia =
                    (await PrepareMediaAsync(preparingSqlResult.AttachedMedia, messageId,
                    chatType, chatId, cancellationToken)).ToArray();

                if (preparingSqlResult.AttachedMedia.Length == 0 && String.IsNullOrEmpty(preparingSqlResult.MessageText))
                    throw notFoundEx;

                return new Message(preparingSqlResult, _mediaPrefix, chatId);
            }

            NewMessage mongoResult = await _newMessageStore.GetOneByIdAsync(messageId, cancellationToken) ?? throw notFoundEx;

            mongoResult.AttachedMedia = (await PrepareMediaAsync(mongoResult.AttachedMedia, messageId, chatType, chatId, cancellationToken)).ToList();

            if (mongoResult.AttachedMedia.Count == 0 && String.IsNullOrEmpty(mongoResult.MessageText))
                throw notFoundEx;

            return new Message(mongoResult, _mediaPrefix);
        }

        public async Task<Message[]> GetMessagesAsync(Guid userId, ChatType chatType, Guid chatId, MessagesSelectOptions options, CancellationToken cancellationToken)
        {
            try
            {
                switch (chatType)
                {
                    case ChatType.Personal:
                        await _personalChatStore.CheckMessageSendingAbilityAsync(chatId, userId, null, cancellationToken);
                        break;
                    case ChatType.Group:
                        await _publicChatStore.CheckMessageSendingAbilityAsync(chatId, userId, null, cancellationToken);
                        break;
                    case ChatType.Bot:
                        await _botChatStore.CheckMessageSendingAbilityAsync(chatId, userId, null, cancellationToken);
                        break;
                }
            }
            catch (Exception e) when (e is not ResourceNotFoundException && e is HttpException)
            { }

            uint additionalMessagesCount = await _deletedMessageStore.GetDeletedCountAsync(chatId,
                ChatTypeConverter.Convert(chatType),
                options.SentBefore ?? DateTime.UtcNow,
                cancellationToken);

            List<NewMessage> mongoResult = await _newMessageStore.GetListByChatAsync(ChatTypeConverter.Convert(chatType),
                chatId, options.SentBefore ?? DateTime.UtcNow,
                options.MessagesCount + additionalMessagesCount, cancellationToken);

            Domain.Models.Types.Message[] sqlResult = chatType switch
            {
                ChatType.Personal => await _personalChatStore.GetMessagesAsync(chatId,
                userId, options.MessagesCount + additionalMessagesCount,
                options.SentBefore ?? DateTime.UtcNow, cancellationToken),

                ChatType.Group => await _publicChatStore.GetMessagesAsync(chatId,
                userId, options.MessagesCount + additionalMessagesCount,
                options.SentBefore ?? DateTime.UtcNow, cancellationToken),

                ChatType.Bot => await _botChatStore.GetMessagesAsync(chatId,
                userId, options.MessagesCount + additionalMessagesCount,
                options.SentBefore ?? DateTime.UtcNow, cancellationToken),

                _ => throw new ArgumentException(nameof(ChatType))
            };

            return (await CombineMessagesAsync(sqlResult, mongoResult, options.MessagesCount, chatType, chatId, cancellationToken)).ToArray();
        }

        public async Task<Guid[]> ResendMessagesAsync(Guid userId, ChatType chatType, ResendMessagesModel resendMessagesModel, CancellationToken cancellationToken)
        {
            switch (chatType)
            {
                case ChatType.Personal:
                    await _personalChatStore.CheckMessageSendingAbilityAsync(resendMessagesModel.ChatId, userId, null, cancellationToken);
                    break;
                case ChatType.Group:
                    await _publicChatStore.CheckMessageSendingAbilityAsync(resendMessagesModel.ChatId, userId, null, cancellationToken);
                    break;
                case ChatType.Bot:
                    await _botChatStore.CheckMessageSendingAbilityAsync(resendMessagesModel.ChatId, userId, null, cancellationToken);
                    break;
            }

            try
            {
                switch (chatType)
                {
                    case ChatType.Personal:
                        await _personalChatStore.CheckMessageSendingAbilityAsync(resendMessagesModel.SourceChatId, userId, null, cancellationToken);
                        break;
                    case ChatType.Group:
                        await _publicChatStore.CheckMessageSendingAbilityAsync(resendMessagesModel.SourceChatId, userId, null, cancellationToken);
                        break;
                    case ChatType.Bot:
                        await _botChatStore.CheckMessageSendingAbilityAsync(resendMessagesModel.SourceChatId, userId, null, cancellationToken);
                        break;
                }
            }
            catch (Exception ex) when (ex is not ResourceNotFoundException && ex is HttpException)
            { }

            bool mongoIncrementExecuted = false;
            Dictionary<Guid, int> increments = new Dictionary<Guid, int>();
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                await _genericChatStore.PrepareAttachmentsForResending(resendMessagesModel.SourceChatId,
                    resendMessagesModel.Messages, userId,
                    ChatTypeConverter.Convert(resendMessagesModel.SourceChatType), cancellationToken);

                List<NewMessage> mongoResult = await _newMessageStore.GetListByIdAsync(
                    ChatTypeConverter.Convert(resendMessagesModel.SourceChatType), resendMessagesModel.SourceChatId,
                    resendMessagesModel.Messages, cancellationToken);

                Domain.Models.Types.Message[] sqlResult = await _genericChatStore.GetMessagesByIdAsync(resendMessagesModel.SourceChatId,
                    resendMessagesModel.Messages, userId,
                    ChatTypeConverter.Convert(resendMessagesModel.SourceChatType), cancellationToken);

                IEnumerable<Message> messages = await CombineMessagesAsync(sqlResult, mongoResult, (uint)(mongoResult.Count + sqlResult.Length), resendMessagesModel.SourceChatType, resendMessagesModel.SourceChatId, cancellationToken);

                mongoIncrementExecuted = true;
                List<NewMessage> newMessages = await MapRepliesId(resendMessagesModel.ChatId, chatType,
                    resendMessagesModel.SourceChatType == ChatType.Bot,
                    userId, messages.ToList(), increments,
                    cancellationToken);

                if (!await _newMessageStore.SaveManyAsync(newMessages, cancellationToken))
                    throw new Exception("Mongo keys duplication");

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return newMessages.Select(m => m.MessageId).ToArray();
            }
            catch (Exception ex)
            {
                if (mongoIncrementExecuted)
                    await _newMediaStore.DecrementLinksAsync(increments, CancellationToken.None);
                await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw ex;
            }
            finally
            {
                await _unitOfWork.DisposeAsync();
            }
        }

        public async Task<Guid> SendUserMessageAsync(ChatType chatType, SendingMessage sendingMessage, CancellationToken cancellationToken)
        {
            try
            {
                switch (chatType)
                {
                    case ChatType.Personal:
                        await _personalChatStore.CheckMessageSendingAbilityAsync(sendingMessage.ChatId, sendingMessage.Author,
                            sendingMessage.ReplyTo, cancellationToken);
                        break;
                    case ChatType.Group:
                        await _publicChatStore.CheckMessageSendingAbilityAsync(sendingMessage.ChatId, sendingMessage.Author,
                            sendingMessage.ReplyTo, cancellationToken);
                        break;
                    case ChatType.Bot:
                        await _botChatStore.CheckMessageSendingAbilityAsync(sendingMessage.ChatId, sendingMessage.Author,
                            sendingMessage.ReplyTo, cancellationToken);
                        break;
                }
            }
            catch (ResourceNotFoundException e) when (sendingMessage.ReplyTo is not null && e.Message == "Replying message not found")
            {
                NewMessage? replyingMessage = await _newMessageStore.GetOneByIdAsync(sendingMessage.ReplyTo.Value, cancellationToken);
                if (replyingMessage is null ||
                    replyingMessage.ChatId != sendingMessage.ChatId ||
                    ChatTypeConverter.Convert(replyingMessage.ChatType) != chatType)
                    throw e;
            }
            Guid messageId = Guid.NewGuid();
            List<Guid>? mediaList = null;
            try
            {
                if (sendingMessage.Attachments is not null)
                {
                    mediaList = new List<Guid>(sendingMessage.Attachments.Length);
                    foreach (FileUpload media in sendingMessage.Attachments)
                    {
                        Guid id = Guid.NewGuid();
                        mediaList.Add(id);

                        await _newMediaStore.CreateAsync(new NewMedia()
                        {
                            LinksCount = 1,
                            ContentType = media.ContentType,
                            MediaId = id,
                            FileName = media.FileName
                        }, cancellationToken);

                        await _objectStorage.SaveAsync(media.Content, id, cancellationToken);
                    }
                }

                if (!await _newMessageStore.SaveAsync(new NewMessage()
                {
                    AttachedMedia = mediaList ?? new List<Guid>(),
                    ChatId = sendingMessage.ChatId,
                    ChatType = ChatTypeConverter.Convert(chatType),
                    Author = sendingMessage.Author,
                    IsBot = false,
                    MessageText = (sendingMessage.MessageText ?? String.Empty).Trim(' '),
                    SentAt = DateTime.UtcNow,
                    IsBotResend = null,
                    ReplyTo = sendingMessage.ReplyTo,
                    IsUpdated = false,
                    UpdatedAt = null,
                    ResentFrom = null,
                    MessageId = messageId
                }, cancellationToken))                    
                    throw new Exception("Mongo keys duplication");                
                else
                    return messageId;
            }
            catch (Exception ex)
            {
                if (mediaList is not null)
                { 
                    await _newMediaStore.DeleteManyAsync(mediaList, CancellationToken.None);
                    await _objectStorage.DeleteManyAsync(mediaList, CancellationToken.None);
                }
                throw ex;
            }
        }

        public async Task UpdateMessageTextAsync(Guid userId, ChatType chatType, UpdatingMessage updatingMessage, CancellationToken cancellationToken)
        {
            switch (chatType)
            {
                case ChatType.Personal:
                    await _personalChatStore.CheckMessageSendingAbilityAsync(updatingMessage.ChatId, userId, null, cancellationToken);
                    break;
                case ChatType.Group:
                    await _publicChatStore.CheckMessageSendingAbilityAsync(updatingMessage.ChatId, userId, null, cancellationToken);
                    break;
                case ChatType.Bot:
                    await _botChatStore.CheckMessageSendingAbilityAsync(updatingMessage.ChatId, userId, null, cancellationToken);
                    break;
            }

            try
            {
                switch (chatType)
                {
                    case ChatType.Personal:
                        await _personalChatStore.UpdateMessageTextAsync(updatingMessage.ChatId, updatingMessage.MessageId, userId, updatingMessage.MessageText, cancellationToken);
                        break;
                    case ChatType.Group:
                        await _publicChatStore.UpdateMessageTextAsync(updatingMessage.ChatId, updatingMessage.MessageId, userId, updatingMessage.MessageText, cancellationToken);
                        break;
                    case ChatType.Bot:
                        throw new NotImplementedException("Editing messages in bot chats not implemented");
                }
            }
            catch (ResourceNotFoundException ex)
            {
                NewMessage? message = await _newMessageStore.GetOneByIdAsync(updatingMessage.MessageId, cancellationToken);

                if (message is null 
                    || message.Author != userId 
                    || ChatTypeConverter.Convert(message.ChatType) != chatType 
                    || message.ChatId != updatingMessage.ChatId)
                    throw ex;

                if(message.ResentFrom is not null)
                    throw new ForbiddenOperationException(
                        new Exception("Forwarded public messages cannot be edited."),
                        ErrorMessages.FORBIDDEN_OPERATION);

                if (message.AttachedMedia.Count == 0 && String.IsNullOrEmpty(updatingMessage.MessageText?.Trim()))
                    throw new InvalidUserInputException(new Exception("The request data is invalid."));

                message.MessageText = updatingMessage.MessageText?.Trim();
                message.UpdatedAt = DateTime.UtcNow;
                message.IsUpdated = true;

                if (!await _newMessageStore.UpdateAsync(message, cancellationToken))
                    throw ex;

                if (chatType == ChatType.Group)
                    await _messageUpdateStore.CreateAsync(new PublicMessageUpdate()
                    {
                        ChatId = updatingMessage.ChatId,
                        UpdatedBy = userId,
                        UpdatedAt = DateTime.UtcNow,
                        UpdateId = Guid.NewGuid()
                    }, CancellationToken.None);
            }
        }

        private async Task<IEnumerable<Guid>> PrepareMediaAsync(IEnumerable<Guid> media, Guid messageId, ChatType chatType, Guid chatId, CancellationToken cancellationToken)
        {
            return media.Except(
                await _deletedAttachmentStore.GetDeletedByMessageIdAsync(messageId, 
                ChatTypeConverter.Convert(chatType), chatId, 
                cancellationToken));
        }

        private async Task<IEnumerable<Message>> CombineMessagesAsync(Domain.Models.Types.Message[] sqlInput, List<NewMessage> mongoInput, uint count, ChatType sourceChatType, Guid sourceChatId, CancellationToken cancellationToken)
        {
            List<Message> result = new List<Message>((int)count);

            int mongoI = 0, sqlI = 0;
            Message? mongoCurrent, sqlCurrent;

            mongoCurrent = mongoI < mongoInput.Count
                ? new Message(mongoInput[mongoI],
                    _mediaPrefix,
                    (await PrepareMediaAsync(mongoInput[mongoI].AttachedMedia, mongoInput[mongoI].MessageId, sourceChatType, sourceChatId, cancellationToken)).ToArray())
                : null;

            sqlCurrent = sqlI < sqlInput.Length
                ? new Message(sqlInput[sqlI],
                    _mediaPrefix,
                    sourceChatId,
                    (await PrepareMediaAsync(sqlInput[sqlI].AttachedMedia, sqlInput[sqlI].MessageId, sourceChatType, sourceChatId, cancellationToken)).ToArray())
                : null;

            while (mongoI < mongoInput.Count && sqlI < sqlInput.Length || result.Count < count)
            {
                if (mongoCurrent is null && sqlCurrent is null)
                    break;

                if (mongoCurrent is not null && sqlCurrent is not null && mongoCurrent.MessageId == sqlCurrent.MessageId)
                {
                    if (!await _deletedMessageStore.CheckDeletionByIdAsync(mongoCurrent.MessageId, cancellationToken))
                        result.Add(mongoCurrent);

                    mongoI++;
                    mongoCurrent = mongoI < mongoInput.Count
                        ? new Message(mongoInput[mongoI],
                            _mediaPrefix,
                            (await PrepareMediaAsync(mongoInput[mongoI].AttachedMedia, mongoInput[mongoI].MessageId, sourceChatType, sourceChatId, cancellationToken)).ToArray())
                        : null;

                    sqlI++;
                    sqlCurrent = sqlI < sqlInput.Length
                        ? new Message(sqlInput[sqlI],
                            _mediaPrefix,
                            sourceChatId,
                            (await PrepareMediaAsync(sqlInput[sqlI].AttachedMedia, sqlInput[sqlI].MessageId, sourceChatType, sourceChatId, cancellationToken)).ToArray())
                        : null;

                    continue;
                }

                if (mongoCurrent is null || mongoCurrent.SentAt < sqlCurrent!.SentAt)
                {
                    if (!await _deletedMessageStore.CheckDeletionByIdAsync(sqlCurrent!.MessageId, cancellationToken))
                        result.Add(sqlCurrent!);

                    sqlI++;
                    sqlCurrent = sqlI < sqlInput.Length
                        ? new Message(sqlInput[sqlI],
                            _mediaPrefix,
                            sourceChatId,
                            (await PrepareMediaAsync(sqlInput[sqlI].AttachedMedia, sqlInput[sqlI].MessageId, sourceChatType, sourceChatId, cancellationToken)).ToArray())
                        : null;

                    continue;
                }

                if (sqlCurrent is null || sqlCurrent.SentAt > mongoCurrent.SentAt)
                {
                    if (!await _deletedMessageStore.CheckDeletionByIdAsync(mongoCurrent.MessageId, cancellationToken))
                        result.Add(mongoCurrent);

                    mongoI++;
                    mongoCurrent = mongoI < mongoInput.Count
                        ? new Message(mongoInput[mongoI],
                            _mediaPrefix,
                            (await PrepareMediaAsync(mongoInput[mongoI].AttachedMedia, mongoInput[mongoI].MessageId, sourceChatType, sourceChatId, cancellationToken)).ToArray())
                        : null;

                    continue;
                }
            }
            return result.AsEnumerable();
        }

        private async Task<List<NewMessage>> MapRepliesId(Guid chatId, ChatType chatType, bool isBot, Guid author, List<Message> messages, Dictionary<Guid, int> increments, CancellationToken cancellationToken)
        {
            Dictionary<Guid, Guid> idMap = messages.Select(m => m.MessageId).ToDictionary(k => Guid.NewGuid());
            List<NewMessage> newMessages = new List<NewMessage>(messages.Count);
            DateTime sentAt = DateTime.UtcNow;

            foreach (Message message in messages)
            {
                List<Guid> media = ParseMedia(message.AttachedMedia);

                foreach (Guid mediaId in media)
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw new TaskCanceledException();
                    if (!increments.ContainsKey(mediaId))
                        increments.Add(mediaId, 1);
                    else
                        increments[mediaId]++;
                }

                await _newMediaStore.IncrementMediaLinksAsync(media, cancellationToken);
                NewMessage adding = new NewMessage()
                {
                    MessageId = idMap[message.MessageId],
                    ChatId = chatId,
                    ChatType = ChatTypeConverter.Convert(chatType),
                    Author = author,
                    ResentFrom = message.ResentFrom ?? message.Author,
                    MessageText = message.MessageText,
                    IsBot = false,
                    IsBotResend = message.IsBotResend ?? false || isBot,
                    SentAt = sentAt,
                    IsUpdated = false,
                    UpdatedAt = null,
                    AttachedMedia = media,
                    ReplyTo = idMap.TryGetValue(message.ReplyTo ?? Guid.Empty, out Guid replyTo) ? replyTo : null
                };
                newMessages.Add(adding);
            }

            return newMessages;
        }

        private List<Guid> ParseMedia(string[] links) => links.Select(l => Guid.Parse(l.Replace(_mediaPrefix, ""))).ToList();
    }
}
