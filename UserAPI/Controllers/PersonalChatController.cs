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
    public class PersonalChatController : ControllerBase
    {
        string mediaPrefix = "https://localhost:10102/Media";
        private readonly IPersonalChatStore _personalChatStore;
        private readonly IUpdatesService _updatesService;
        private readonly IObjectStorage _objectStorage;
        public PersonalChatController(IPersonalChatStore personalChatStore, IUpdatesService updatesService, IObjectStorage objectStorage)
        {
            _personalChatStore = personalChatStore;
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
            var chatShortInfo = await _personalChatStore.GetChatShortInfoAsync(chatId, HttpContext.GetUserId(), cancellationToken);
            ChatShortInfo result = new ChatShortInfo
            {
                ChatId = chatShortInfo.ChatId,
                ChatName = chatShortInfo.ChatName,
                ChatType = chatShortInfo.ChatType switch
                {
                    EnChatType.Personal => ChatType.Personal,
                    EnChatType.Public => ChatType.Group,
                    EnChatType.Bot => ChatType.Bot,
                    _ => throw new ArgumentOutOfRangeException()
                },
                NewMessagesCount = chatShortInfo.NewMessagesCount,
                ChatImage = chatShortInfo.ChatImage != null ? $"{mediaPrefix}/{chatShortInfo.ChatImage}" : null
            };
            return Ok();
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
            var messages = await _personalChatStore.GetMessagesAsync(chatId, HttpContext.GetUserId(), messagesSelectOptions.MessagesCount, messagesSelectOptions.SentBefore ?? DateTime.UtcNow, cancellationToken);
            Application.Models.Output.Message[] result = messages.Select(m => new Application.Models.Output.Message
            {
                Author = m.Author,
                SentAt = m.SentAt,
                IsUpdated = m.IsUpdated,
                UpdatedAt = m.UpdatedAt,
                ReplyTo = m.ReplyTo,
                ResentFrom = m.ResentFrom,
                IsBotResend = m.IsBotResend,
                AttachedMedia = m.AttachedMedia.Select(am => $"{mediaPrefix}/{am}").ToArray(),
                MessageId = m.MessageId,
                ChatId = chatId,
                MessageText = m.MessageText
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
            var message = await _personalChatStore.GetMessageAsync(chatId, messageId, HttpContext.GetUserId(), cancellationToken);
            Application.Models.Output.Message result = new Application.Models.Output.Message
            {
                Author = message.Author,
                SentAt = message.SentAt,
                IsUpdated = message.IsUpdated,
                UpdatedAt = message.UpdatedAt,
                ReplyTo = message.ReplyTo,
                ResentFrom = message.ResentFrom,
                IsBotResend = message.IsBotResend,
                AttachedMedia = message.AttachedMedia.Select(am => $"{mediaPrefix}/{am}").ToArray(),
                MessageId = message.MessageId,
                ChatId = chatId,
                MessageText = message.MessageText
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
        public async Task<IActionResult> GetUserIdByChat(Guid chatId, CancellationToken cancellationToken)
        {
            var userId = await _personalChatStore.GetUserIdByChatIdAsync(chatId, HttpContext.GetUserId(), cancellationToken);
            return Ok(userId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationUserId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>returns id of existing chat or creates a new one</returns>
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> OpenChatWithUser(Guid destinationUserId, CancellationToken cancellationToken)
        {
            var chatId = await _personalChatStore.CreateChatAsync(destinationUserId, HttpContext.GetUserId(), cancellationToken);
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
        public async Task<IActionResult> SendMessage([FromForm]SendMessageForm sendingMessageBody, CancellationToken cancellationToken)
        {
            var messageId = await _personalChatStore.SendMessageAsync(sendingMessageBody.ChatId, HttpContext.GetUserId(),
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
                HttpContext.GetUserId(), 
                await _personalChatStore.GetUserIdByChatIdAsync(sendingMessageBody.ChatId, HttpContext.GetUserId(), cancellationToken)
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
        [ProducesResponseType(typeof(Guid[]), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("[action]")]
        public async Task<IActionResult> ResendMessages(ResendMessagesModel resendMessagesModel, CancellationToken cancellationToken)
        {
            var messageIds = await _personalChatStore.ResendMessagesAsync(resendMessagesModel.ChatId, 
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
                HttpContext.GetUserId(),
                await _personalChatStore.GetUserIdByChatIdAsync(resendMessagesModel.ChatId, HttpContext.GetUserId(), cancellationToken)
                ],
                ChatType.Personal, cancellationToken);
            return Ok(messageIds);
        }

        /// <summary>
        /// call, when user is typing a message with 5s timeout
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("[action]")]
        public async Task<IActionResult> Typing(Guid chatId, CancellationToken cancellationToken)
        {
            await _updatesService.UserIsTyping(chatId, HttpContext.GetUserId(),
                [
                await _personalChatStore.GetUserIdByChatIdAsync(chatId, HttpContext.GetUserId(), cancellationToken)
                ],
                ChatType.Personal, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageText"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>404 if chat or message is not belonging to current user</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> EditMessageText(UpdatingMessage messageText, CancellationToken cancellationToken)
        {
            await _personalChatStore.UpdateMessageTextAsync(messageText.ChatId, messageText.MessageId, HttpContext.GetUserId(), messageText.MessageText, cancellationToken);
            await _updatesService.MessageUpdated(messageText.ChatId, messageText.MessageId,
                [
                HttpContext.GetUserId(),
                await _personalChatStore.GetUserIdByChatIdAsync(messageText.ChatId, HttpContext.GetUserId(), cancellationToken)
                ],
                ChatType.Personal, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> BlockUser(Guid userId, CancellationToken cancellationToken)
        {
            await _personalChatStore.BlockUserAsync(userId, HttpContext.GetUserId(), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> UnblockUser(Guid userId, CancellationToken cancellationToken)
        {
            await _personalChatStore.UnblockUserAsync(userId, HttpContext.GetUserId(), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="messageId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>404 if chat or message is not belonging to current user</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteMessage(Guid chatId, Guid messageId, CancellationToken cancellationToken)
        {
            await _personalChatStore.DeleteMessageAsync(chatId, messageId, HttpContext.GetUserId(), cancellationToken);
            await _updatesService.MessageDeleted(chatId, messageId,
                [
                HttpContext.GetUserId(),
                await _personalChatStore.GetUserIdByChatIdAsync(chatId, HttpContext.GetUserId(), cancellationToken)
                ],
                ChatType.Personal, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="mediaLink"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>404 if chat or message is not belonging to current user, 403 if message is resend</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteFileFromMessage(Guid chatId, string mediaLink, CancellationToken cancellationToken)
        {
            Guid mediaId = Guid.Parse(mediaLink.Split('/').Last());
            await _personalChatStore.DeleteFileFromMessageAsync(chatId, mediaId, HttpContext.GetUserId(), cancellationToken);
            await _updatesService.FileDeleted(chatId, mediaLink, chatId,
                [
                HttpContext.GetUserId(),
                await _personalChatStore.GetUserIdByChatIdAsync(chatId, HttpContext.GetUserId(), cancellationToken)
                ],
                ChatType.Personal, cancellationToken);
            return Ok();
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteChat(Guid chatId, CancellationToken cancellationToken)
        {
            await _personalChatStore.DeleteChatAsync(chatId, HttpContext.GetUserId(), cancellationToken);
            await _updatesService.ChatDeleted(chatId,
                [
                HttpContext.GetUserId(),
                await _personalChatStore.GetUserIdByChatIdAsync(chatId, HttpContext.GetUserId(), cancellationToken)
                ],
                ChatType.Personal, cancellationToken);
            return Ok();
        }
    }
}
