using Application.Models.Input;
using Application.Models.Internal.Constants;
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
    public class PublicChatController : ControllerBase
    {
        private readonly IPublicChatService _publicChatService;
        public PublicChatController(IPublicChatService publicChatService)
        {
            _publicChatService = publicChatService;
        }
        /// <summary>
        /// works with only searchable public chats
        /// </summary>
        /// <param name="chatName">beginning of chat name, length >= 3 chars</param>
        /// <param name="pageOptions">page and page size >=1</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(PublicChatShortInfo[]), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Search(string chatName, PageOptions pageOptions, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _publicChatService.GetChatsByNameAsync(chatName, userId, pageOptions, cancellationToken));
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
        [HttpGet("[action]")]
        public async Task<IActionResult> GetChatShortInfo(Guid chatId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _publicChatService.GetChatInfoAsync(chatId, userId, cancellationToken));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>404 if is not belonging to current user</returns>
        [ProducesResponseType(typeof(PublicChatFullInfo), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChatFullInfo(Guid chatId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _publicChatService.GetChatFullInfoAsync(chatId, userId, cancellationToken));
        }

        /// <summary>
        /// Available for owner and admins
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [ProducesResponseType(typeof(PublicChatOptions), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetChatOptions(Guid chatId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _publicChatService.GetChatOptionsAsync(chatId, userId, cancellationToken));
        }

        /// <summary>
        /// Avai
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>id of users banned from chat, 403 if current user is not admin/owner, 404 if not a member</returns>
        [ProducesResponseType(typeof(PublicChatBannedUser[]), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetBannedUsers(Guid chatId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _publicChatService.GetBannedUsersAsync(chatId, userId, cancellationToken));
        }

        /// <summary>
        /// Call when user opens or scrolls chat
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
            return Ok(await _publicChatService.GetMessagesAsync(userId, chatId, messagesSelectOptions, cancellationToken));
        }

        /// <summary>
        /// Call when you catch an update about new or updated message
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
            return Ok(await _publicChatService.GetMessageAsync(userId, chatId, messageId, cancellationToken));
        }

        /// <summary>
        /// Available for owner and admins
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="auditOptions">page and page size >=1</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>400 if invalid options, 403 if not owner/admin</returns>
        [ProducesResponseType(typeof(PublicChatAuditRecord[]), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> AuditChat(Guid chatId, PageOptions auditOptions, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _publicChatService.AuditChatAsync(userId, chatId, auditOptions, cancellationToken));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createPublicChatBody"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>400 if chat name shorter than 3 chars</returns>
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateNewChat([FromForm] CreatePublicChatForm createPublicChatBody, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _publicChatService.CreateChatAsync(userId, createPublicChatBody.ChatName, createPublicChatBody.Searchable, createPublicChatBody.ChatImage?.ToFileUpload(), createPublicChatBody.DefaultMemberRole, cancellationToken));
        }

        /// <summary>
        /// Should be called when user clicks "Join" button on chat in search results or opens invite link.
        /// Link should be generated by chat members from chat id on client side
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>403 if current user banned from chat</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]/{chatId}")]
        public async Task<IActionResult> JoinChat(Guid chatId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _publicChatService.JoinChatAsync(chatId, userId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>404 if chat is not belonging to current user or replying message not found</returns>
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageForm form, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _publicChatService.SendMessageAsync(form.ToSendingMessage(userId), cancellationToken));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resendMessagesModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>404 if chat is not belonging for user or user have no access to resending messages</returns>
        [ProducesResponseType(typeof(Guid[]), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ResendMessages(ResendMessagesModel resendMessagesModel, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _publicChatService.ResendMessagesAsync(userId, resendMessagesModel, cancellationToken));
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
            await _publicChatService.HandleUserTypingEventAsync(userId, chatId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageText"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>403 if message is resend, 404 if chat or message is not belonging to current user</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("[action]")]
        public async Task<IActionResult> EditMessageText(UpdatingMessage messageText, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _publicChatService.EditMessageTextAsync(userId, messageText, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="chatId"></param>
        /// <param name="role">must be lower than current user role</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>403 if current user is not owner or admin, if action violates role hierarchy; 404 if member or chat is not found</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("[action]")]
        public async Task<IActionResult> GiveMemberRole(Guid member, Guid chatId, PublicChatMemberRole role, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _publicChatService.GiveMemberRoleAsync(userId, chatId, member, role, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="form"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>400 if chat name shorter than 3 chars, 403 if user is not owner or creator</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateChat(Guid chatId, [FromForm] UpdatePublicChatForm form, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _publicChatService.UpdateChatAsync(userId, chatId, 
                form.NewChatName, form.NewSearchable, 
                form.UpdateAvatar, form.NewChatImage?.ToFileUpload(), 
                form.NewDefaultMemberRole, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>403 if current user not owner or administrator, 404 if banning user not a member</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> BanUser(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            Guid sourceUserId = HttpContext.GetUserId();
            await _publicChatService.BanUserAsync(sourceUserId, chatId, userId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>403 if not owner or administrator, 404 if banning user not a member</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> UnbanUser(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            Guid sourceUserId = HttpContext.GetUserId();
            await _publicChatService.UnbanUserAsync(sourceUserId, chatId, userId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Available for owner or admins or message author
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
            await _publicChatService.DeleteMessageAsync(userId, chatId, messageId, cancellationToken);
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
            await _publicChatService.DeleteFileFromMessageAsync(userId, chatId, mediaLink, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>403 if owner (must make another user as owner at first)</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> LeaveChat(Guid chatId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _publicChatService.LeaveChatAsync(userId, chatId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="memberId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>403 if not owner or admin or trying delete self</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteChatMember(Guid chatId, Guid memberId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _publicChatService.RemoveMemberAsync(userId, chatId, memberId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="memberId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>403 if not owner or admin or trying delete self</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteAndBanMember(Guid chatId, Guid memberId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _publicChatService.RemoveAndBanChatMemberAsync(userId, chatId, memberId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>403 if not owner</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteChat(Guid chatId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _publicChatService.DeleteChatAsync(userId, chatId, cancellationToken);
            return Ok();
        }
    }
}
