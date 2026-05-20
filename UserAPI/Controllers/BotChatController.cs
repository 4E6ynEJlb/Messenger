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
    public class BotChatController : ControllerBase
    {
        private readonly IBotChatService _botChatService;
        public BotChatController(IBotChatService botChatService)
        {
            _botChatService = botChatService;
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
            return Ok(await _botChatService.GetChatShortInfoAsync(userId, chatId, cancellationToken));
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
            Guid userId = HttpContext.GetUserId();
            return Ok(await _botChatService.GetActiveButtonsAsync(userId, chatId, cancellationToken));
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
            return Ok(await _botChatService.GetMessagesAsync(userId, chatId, messagesSelectOptions, cancellationToken));
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
            return Ok(await _botChatService.GetMessageAsync(userId, chatId, messageId, cancellationToken));
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
            Guid userId = HttpContext.GetUserId();
            return Ok(await _botChatService.GetBotIdByChatAsync(userId, chatId, cancellationToken));
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
            Guid userId = HttpContext.GetUserId();
            return Ok(await _botChatService.OpenChatWithBotAsync(userId, destinationBotId, cancellationToken));
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
            Guid userId = HttpContext.GetUserId();
            return Ok(await _botChatService.SendMessageAsync(sendingMessageBody.ToSendingMessage(userId), cancellationToken));
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
            Guid userId = HttpContext.GetUserId();
            return Ok(await _botChatService.ResendMessagesAsync(userId, resendMessagesModel, cancellationToken));
        }

        /// <summary>
        /// works like blocking a bot - user will not receive messages from it
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> DisableBot(Guid chatId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _botChatService.DisableBotAsync(userId, chatId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// unblocking bot
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> EnableBot(Guid chatId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _botChatService.EnableBotAsync(userId, chatId, cancellationToken);
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
            Guid userId = HttpContext.GetUserId();
            await _botChatService.DeleteChatAsync(userId, chatId, cancellationToken);
            return Ok();
        }
    }
}