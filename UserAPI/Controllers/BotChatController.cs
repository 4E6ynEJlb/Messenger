using Application.Models.Input;
using Application.Models.Internal.Constants;
using Application.Models.Output;
using Domain.Models.Types;
using Domain.Stores;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Extensions;
using UserAPI.Models;
using Application.Services.Interfaces;

namespace UserAPI.Controllers
{
    /// <summary>
    /// Only for not banned users
    /// </summary>
    [Authorize(Policy = Policies.USER_POLICY)]
    [Route("[controller]")]
    [ApiController]
    public class BotChatController : ControllerBase
    {
        string mediaPrefix = "https://localhost:10102/Media";
        private readonly IBotChatStore _botChatStore;
        private readonly IUpdatesService _updatesService;
        private readonly IObjectStorage _objectStorage;
        public BotChatController(IBotChatStore botChatStore, IUpdatesService updatesService, IObjectStorage objectStorage)
        {
            _botChatStore = botChatStore;
            _updatesService = updatesService;
            _objectStorage = objectStorage;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>404 if is not belonging to current user</returns>
        [ProducesResponseType(typeof(ChatShortInfo), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChatShortInfo(Guid chatId, CancellationToken cancellationToken)
        {
            var chatShortInfo = await _botChatStore.GetChatShortInfoAsync(chatId, HttpContext.GetUserId(), cancellationToken);
            var result = new ChatShortInfo
            {
                ChatId = chatShortInfo.ChatId,
                ChatName = chatShortInfo.ChatName,
                ChatType = chatShortInfo.ChatType switch
                {
                    EnChatType.Bot => ChatType.Bot,
                    EnChatType.Personal => ChatType.Personal,
                    EnChatType.Public => ChatType.Group,
                    _ => throw new Exception("Unknown chat type")
                },
                NewMessagesCount = chatShortInfo.NewMessagesCount,
                ChatImage = chatShortInfo.ChatImage == null ? null : $"{mediaPrefix}/{chatShortInfo.ChatImage}"
            };
            return Ok();
        }        

        /// <summary>
        /// Call when user opens a chat
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>list of buttons with inner command, outer text or emoji and background color (optional)</returns>
        [ProducesResponseType(typeof(BotButton[]), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetActiveButtonsList(Guid chatId, CancellationToken cancellationToken)
        {
            var buttons = await _botChatStore.GetActiveButtonsListAsync(chatId, HttpContext.GetUserId(), cancellationToken);
            var result = buttons.Select(b => new BotButton
            {
                Command = b.InnerCommand,
                Text = b.ButtonText,
                BackgroundColor = b.BackgroundColor is not null ? new Color
                {
                    R = b.BackgroundColor[0],
                    G = b.BackgroundColor[1],
                    B = b.BackgroundColor[2]
                } : null
            }).ToArray();
            return Ok(result);
        }

        /// <summary>
        /// Call when user opens or scrolls the chat
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="messagesSelectOptions"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>404 if chat is not belonging to current user</returns>
        [ProducesResponseType(typeof(Application.Models.Output.Message[]), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("{chatId}/[action]")]
        public async Task<IActionResult> GetMessages(Guid chatId, MessagesSelectOptions messagesSelectOptions, CancellationToken cancellationToken)
        {
            var messages = await _botChatStore.GetMessagesAsync(chatId, HttpContext.GetUserId(), messagesSelectOptions.MessagesCount, messagesSelectOptions.SentBefore ?? DateTime.UtcNow, cancellationToken);
            var result = messages.Select(m => new Application.Models.Output.Message
            {
                MessageId = m.MessageId,
                Author = m.Author,
                SentAt = m.SentAt,
                IsUpdated = m.IsUpdated,
                UpdatedAt = m.UpdatedAt,
                ReplyTo = m.ReplyTo,
                ResentFrom = m.ResentFrom,
                IsBotResend = m.IsBotResend,
                MessageText = m.MessageText,
                AttachedMedia = m.AttachedMedia.Select(am => $"{mediaPrefix}/{am}").ToArray(),
                ChatId = chatId
            }).ToArray();
            return Ok(result);
        }

        /// <summary>
        /// Call when you catch an update about a new or updated message
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="messageId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>404 if chat is not belonging to current user or message does not exist</returns>
        [ProducesResponseType(typeof(Application.Models.Output.Message), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetMessage(Guid chatId, Guid messageId, CancellationToken cancellationToken)
        {
            var message = await _botChatStore.GetMessageAsync(chatId, messageId, HttpContext.GetUserId(), cancellationToken);
            var result = new Application.Models.Output.Message
                {
                    MessageId = message.MessageId,
                    Author = message.Author,
                    SentAt = message.SentAt,
                    IsUpdated = message.IsUpdated,
                    UpdatedAt = message.UpdatedAt,
                    ReplyTo = message.ReplyTo,
                    ResentFrom = message.ResentFrom,
                    IsBotResend = message.IsBotResend,
                    MessageText = message.MessageText,
                    AttachedMedia = message.AttachedMedia.Select(am => $"{mediaPrefix}/{am}").ToArray(),
                    ChatId = chatId
                };
            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>404 if chat is not belonging to current user</returns>
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetBotIdByChat(Guid chatId, CancellationToken cancellationToken)
        {
            var botId = await _botChatStore.GetBotIdByChatIdAsync(chatId, HttpContext.GetUserId(), cancellationToken);
            return Ok(botId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationBotId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>returns id of existing chat or creates a new one</returns>
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> OpenChatWithBot(Guid destinationBotId, CancellationToken cancellationToken)
        {
            var chatId = await _botChatStore.CreateChatAsync(destinationBotId, HttpContext.GetUserId(), cancellationToken);
            return Ok(chatId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendingMessageBody"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>404 if chat is not belonging to current user or replying message not found</returns>
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("[action]")]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageForm sendingMessageBody, CancellationToken cancellationToken)
        {
            var messageId = await _botChatStore.SendMessageAsync(sendingMessageBody.ChatId, HttpContext.GetUserId(),
                sendingMessageBody.ReplyTo, sendingMessageBody.MessageText,
                sendingMessageBody.Attachments.Select(a => new MediaFile
                {
                    ContentType = a.ContentType,
                    FileName = a.FileName,
                    MediaId = new Func<Guid>(() =>
                    {
                        var id = Guid.NewGuid();
                        _objectStorage.SaveAsync(a.OpenReadStream(), id, cancellationToken).Wait();
                        return id;
                    })()
                }).ToArray(), cancellationToken);
            await _updatesService.MessagesSent(sendingMessageBody.ChatId, [messageId],
                [
                HttpContext.GetUserId()
                ],
                ChatType.Personal, cancellationToken);
            return Ok(messageId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resendMessagesModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>404 if chat is not belonging to current user or user has no access to resending messages</returns>
        /// <exception cref="NotImplementedException"></exception>
        [ProducesResponseType(typeof(Guid[]), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("[action]")]
        public async Task<IActionResult> ResendMessages(ResendMessagesModel resendMessagesModel, CancellationToken cancellationToken)
        {
            var messageIds = await _botChatStore.ResendMessagesAsync(resendMessagesModel.ChatId,
                HttpContext.GetUserId(), resendMessagesModel.SourceChatType switch
                {
                    ChatType.Personal => EnChatType.Personal,
                    ChatType.Group => EnChatType.Public,
                    ChatType.Bot => EnChatType.Bot,
                    _ => throw new NotImplementedException()
                }, resendMessagesModel.SourceChatId,
                resendMessagesModel.Messages, cancellationToken);
            await _updatesService.MessagesSent(resendMessagesModel.ChatId, messageIds,
                [
                HttpContext.GetUserId()
                ],
                ChatType.Personal, cancellationToken);
            return Ok(messageIds);
        }

        /// <summary>
        /// works like blocking a bot - user will not receive messages from it
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> DisableBot(Guid botId, CancellationToken cancellationToken)
        {
            await _botChatStore.DisableBotAsync(botId, HttpContext.GetUserId(), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// unblocking bot
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> EnableBot(Guid botId, CancellationToken cancellationToken)
        {
            await _botChatStore.EnableBotAsync(botId, HttpContext.GetUserId(), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteChat(Guid chatId, CancellationToken cancellationToken)
        {
            await _botChatStore.DeleteChatAsync(chatId, HttpContext.GetUserId(), cancellationToken);
            return Ok();
        }
    }
}