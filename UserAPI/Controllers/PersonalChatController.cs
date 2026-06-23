using Application.Models.Input;
using Application.Models.Output;
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
        private readonly IPersonalChatService _personalChatService;
        public PersonalChatController(IPersonalChatService personalChatService)
        {
            _personalChatService = personalChatService;
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
            Guid userId = HttpContext.GetUserId();
            return Ok(await _personalChatService.GetChatShortInfoAsync(userId, chatId, cancellationToken));
        }

        /// <summary>
        /// Call when user opens or scrolls the chat
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="messagesSelectOptions"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>404 if chat is not belonging to current user</returns>
        [ProducesResponseType(typeof(Message[]), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("{chatId}/[action]")]
        public async Task<IActionResult> GetMessages(Guid chatId, MessagesSelectOptions messagesSelectOptions, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _personalChatService.GetMessagesAsync(userId, chatId, messagesSelectOptions, cancellationToken));
        }

        /// <summary>
        /// Call when you catch an update about a new or updated message
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="messageId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>404 if chat is not belonging to current user or message does not exist</returns>
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetMessage(Guid chatId, Guid messageId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _personalChatService.GetMessageAsync(userId, chatId, messageId, cancellationToken));
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
            Guid userId = HttpContext.GetUserId();            
            return Ok(await _personalChatService.GetUserIdByChatAsync(userId, chatId, cancellationToken));
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
            Guid userId = HttpContext.GetUserId();
            return Ok(await _personalChatService.OpenChatWithUserAsync(userId, destinationUserId, cancellationToken));
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
            Guid userId = HttpContext.GetUserId();
            return Ok(await _personalChatService.SendMessageAsync(sendingMessageBody.ToSendingMessage(userId), cancellationToken));
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
            Guid userId = HttpContext.GetUserId();
            return Ok(await _personalChatService.ResendMessagesAsync(userId, resendMessagesModel, cancellationToken));
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
            Guid userId = HttpContext.GetUserId();
            await _personalChatService.HandleUserTypingEventAsync(userId, chatId, cancellationToken);
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
            Guid userId = HttpContext.GetUserId();
            await _personalChatService.EditMessageTextAsync(userId, messageText, cancellationToken);
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
            Guid currentUserId = HttpContext.GetUserId();
            await _personalChatService.BlockUserAsync(currentUserId, userId, cancellationToken);
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
            Guid currentUserId = HttpContext.GetUserId();
            await _personalChatService.UnblockUserAsync(currentUserId, userId, cancellationToken);
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
            Guid userId = HttpContext.GetUserId();
            await _personalChatService.DeleteMessageAsync(userId, chatId, messageId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="messageId"></param>
        /// <param name="mediaLink"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>404 if chat or message is not belonging to current user, 403 if message is resend</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteFileFromMessage(Guid chatId, Guid messageId, string mediaLink, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _personalChatService.DeleteFileFromMessageAsync(userId, chatId, messageId, mediaLink, cancellationToken);
            return Ok();
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteChat(Guid chatId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _personalChatService.DeleteChatAsync(userId, chatId, cancellationToken);
            return Ok();
        }
    }
}
