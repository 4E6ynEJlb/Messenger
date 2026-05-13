using Application.Models.Input;
using Application.Models.Internal;
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
    public class PublicChatController : ControllerBase
    {
        string mediaPrefix = "https://localhost:10102/Media";
        private readonly IPublicChatStore _publicChatStore;
        private readonly IObjectStorage _objectStorage;
        private readonly IUpdatesService _updatesService;
        public PublicChatController(IPublicChatStore publicChatStore, IObjectStorage objectStorage, IUpdatesService updatesService)
        {
            _publicChatStore = publicChatStore;
            _objectStorage = objectStorage;
            _updatesService = updatesService;
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
        [HttpGet("[action]")]
        public async Task<IActionResult> Search(string chatName, PageOptions pageOptions, CancellationToken cancellationToken)
        {
            var publicChats = await _publicChatStore.SearchChatsAsync(chatName, HttpContext.GetUserId(), pageOptions.Page, pageOptions.PageSize, cancellationToken);
            var result = publicChats.Select(c => new PublicChatShortInfo
            {
                ChatId = c.ChatId,
                ChatImage = $"{mediaPrefix}/{c.Avatar}",
                ChatName = c.ChatName,
                MembersCount = (uint)c.Members.Count()
            }).ToArray();
            return Ok(result);
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
            var chat = await _publicChatStore.GetChatShortInfoAsync(chatId, HttpContext.GetUserId(), cancellationToken);
            var result = new ChatShortInfo
            {
                ChatId = chat.ChatId,
                ChatImage = $"{mediaPrefix}/{chat.ChatImage}",
                ChatName = chat.ChatName,
                ChatType = chat.ChatType switch 
                { 
                    EnChatType.Public => ChatType.Group, 
                    EnChatType.Personal => ChatType.Personal, 
                    EnChatType.Bot => ChatType.Bot ,
                    _ => throw new ArgumentOutOfRangeException()
                },
                NewMessagesCount = chat.NewMessagesCount
            };
            return Ok(result);
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
            var chat = await _publicChatStore.GetChatFullInfoAsync(chatId, HttpContext.GetUserId(), cancellationToken);
            var result = new PublicChatFullInfo
                {
                    ChatId = chat.ChatId,
                    ChatImage = $"{mediaPrefix}/{chat.Avatar}",
                    ChatName = chat.ChatName,
                    Members = chat.Members.Select(m => new PublicChatMemberInfo
                    {
                        UserId = m.UserId,
                        FullName = m.FullName,
                        Role = m.Role switch
                        {
                            EnPublicChatMemberRole.Creator => PublicChatMemberRole.Owner,
                            EnPublicChatMemberRole.Administrator => PublicChatMemberRole.Administrator,
                            EnPublicChatMemberRole.Member => PublicChatMemberRole.Member,
                            EnPublicChatMemberRole.Reader => PublicChatMemberRole.Reader,
                            _ => throw new ArgumentOutOfRangeException()
                        },
                        Avatar = $"{mediaPrefix}/{m.Avatar}"
                    }).ToArray()
                };
            return Ok(result);
        }

        /// <summary>
        /// Available for owner and admins
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [ProducesResponseType(typeof(Application.Models.Input.PublicChatOptions), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetChatOptions(Guid chatId, CancellationToken cancellationToken)
        {
            var chat = await _publicChatStore.GetChatOptionsAsync(chatId, HttpContext.GetUserId(), cancellationToken);
            var result = new Application.Models.Input.PublicChatOptions
            {
                Searchable = chat.IsSearchable,
                DefaultMemberRole = chat.DefaultMemberRole switch
                {
                    EnPublicChatMemberRole.Creator => PublicChatMemberRole.Owner,
                    EnPublicChatMemberRole.Administrator => PublicChatMemberRole.Administrator,
                    EnPublicChatMemberRole.Member => PublicChatMemberRole.Member,
                    EnPublicChatMemberRole.Reader => PublicChatMemberRole.Reader,
                    _ => throw new ArgumentOutOfRangeException()
                },
                ChatImage = chat.Avatar ==  null ? null : $"{mediaPrefix}/{chat.Avatar}",
                ChatName = chat.ChatName
            };
            return Ok(result);
        }

        /// <summary>
        /// Avai
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>id of users banned from chat, 403 if current user is not admin/owner, 404 if not a member</returns>
        [ProducesResponseType(typeof(Guid[]), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetBannedUsers(Guid chatId, CancellationToken cancellationToken)
        {
            var bannedUsers = await _publicChatStore.GetBannedUsersAsync(chatId, HttpContext.GetUserId(), cancellationToken);
            return Ok(bannedUsers);
        }

        /// <summary>
        /// Call when user opens or scrolls chat
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
            var messages = await _publicChatStore.GetMessagesAsync(chatId, HttpContext.GetUserId(), messagesSelectOptions.MessagesCount, messagesSelectOptions.SentBefore ?? DateTime.UtcNow, cancellationToken);
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
        /// Call when you catch an update about new or updated message
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
            var message = await _publicChatStore.GetMessageAsync(chatId, messageId, HttpContext.GetUserId(), cancellationToken);
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
        /// Available for owner and admins
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="auditOptions">page and page size >=1</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>400 if invalid options, 403 if not owner/admin</returns>
        [ProducesResponseType(typeof(PublicChatAuditRecord), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> AuditChat(Guid chatId, PageOptions auditOptions, CancellationToken cancellationToken)
        {
            var auditRecords = await _publicChatStore.AuditChatAsync(chatId, HttpContext.GetUserId(), auditOptions.Page, auditOptions.PageSize, cancellationToken);
            var result = auditRecords.Select(ar => new PublicChatAuditRecord
            {
                Action = ar.Action switch
                {
                    EnPublicChatAuditRecordAction.Join => AuditRecordAction.Join,
                    EnPublicChatAuditRecordAction.UpdateMessage => AuditRecordAction.UpdateMessage,
                    EnPublicChatAuditRecordAction.ChangeRole => AuditRecordAction.ChangeRole,
                    EnPublicChatAuditRecordAction.UpdateSettings => AuditRecordAction.UpdateSettings,
                    EnPublicChatAuditRecordAction.Ban => AuditRecordAction.Ban,
                    EnPublicChatAuditRecordAction.Unban => AuditRecordAction.Unban,
                    EnPublicChatAuditRecordAction.Leave => AuditRecordAction.Leave,
                    EnPublicChatAuditRecordAction.Kick => AuditRecordAction.Kick,
                    EnPublicChatAuditRecordAction.DeleteMessage => AuditRecordAction.DeleteMessage,
                    EnPublicChatAuditRecordAction.DeleteAttachment => AuditRecordAction.DeleteAttachment,
                    _ => throw new ArgumentOutOfRangeException()
                },
                UserId = ar.SourceUserId,
                TargetUserId = ar.DestinationUserId,
                ActionDate = ar.ActionDatetime,
            }).ToArray();
            return Ok(result);
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
        [HttpGet("[action]")]
        public async Task<IActionResult> CreateNewChat([FromForm] CreatePublicChatForm createPublicChatBody, CancellationToken cancellationToken)
        {
            Guid? avatarId = createPublicChatBody.ChatImage is not null ? Guid.NewGuid() : null;
            var chatId = await _publicChatStore.CreateNewChatAsync(createPublicChatBody.ChatName, HttpContext.GetUserId(), createPublicChatBody.Searchable, avatarId.HasValue ? new MediaFile { ContentType = createPublicChatBody.ChatImage.ContentType, FileName = createPublicChatBody.ChatImage.FileName, MediaId = avatarId.Value} : null, createPublicChatBody.DefaultMemberRole switch
            {
                PublicChatMemberRole.Owner => EnPublicChatMemberRole.Creator,
                PublicChatMemberRole.Administrator => EnPublicChatMemberRole.Administrator,
                PublicChatMemberRole.Member => EnPublicChatMemberRole.Member,
                PublicChatMemberRole.Reader => EnPublicChatMemberRole.Reader,
                _ => throw new ArgumentOutOfRangeException()
            }, cancellationToken);
            if (avatarId.HasValue)
            {
                await _objectStorage.SaveAsync(createPublicChatBody.ChatImage.OpenReadStream(), avatarId.Value, cancellationToken);
            }
            return Ok(chatId);
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
            await _publicChatStore.JoinChatAsync(chatId, HttpContext.GetUserId(), cancellationToken);
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
        [HttpPut("[action]")]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageForm form, CancellationToken cancellationToken)
        {
            var messageId = await _publicChatStore.SendMessageAsync(form.ChatId, HttpContext.GetUserId(),
                form.ReplyTo, form.MessageText,
                form.Attachments.Select(a => new MediaFile
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
            await _updatesService.MessagesSent(form.ChatId, [messageId],
                (await _publicChatStore.GetChatFullInfoAsync(form.ChatId, HttpContext.GetUserId(), cancellationToken)).Members.Select(m=>m.UserId).ToArray(),
                ChatType.Group, cancellationToken);
            return Ok(messageId);
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
        [HttpPut("[action]")]
        public async Task<IActionResult> ResendMessages(ResendMessagesModel resendMessagesModel, CancellationToken cancellationToken)
        {
            var messageIds = await _publicChatStore.ResendMessagesAsync(resendMessagesModel.ChatId,
                HttpContext.GetUserId(), resendMessagesModel.SourceChatType switch
                {
                    ChatType.Personal => EnChatType.Personal,
                    ChatType.Group => EnChatType.Public,
                    ChatType.Bot => EnChatType.Bot,
                    _ => throw new NotImplementedException()
                }, resendMessagesModel.SourceChatId,
                resendMessagesModel.Messages, cancellationToken);
            await _updatesService.MessagesSent(resendMessagesModel.ChatId, messageIds,
                (await _publicChatStore.GetChatFullInfoAsync(resendMessagesModel.ChatId, HttpContext.GetUserId(), cancellationToken)).Members.Select(m=>m.UserId).ToArray(),
                ChatType.Group, cancellationToken);
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
                (await _publicChatStore.GetChatFullInfoAsync(chatId, HttpContext.GetUserId(), cancellationToken)).Members.Select(m => m.UserId).ToArray(),
                ChatType.Group, cancellationToken);
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
        [HttpPost("[action]")]
        public async Task<IActionResult> EditMessageText(UpdatingMessage messageText, CancellationToken cancellationToken)
        {
            await _publicChatStore.UpdateMessageTextAsync(messageText.ChatId, messageText.MessageId, HttpContext.GetUserId(), messageText.MessageText, cancellationToken);
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
        [HttpPost("[action]")]
        public async Task<IActionResult> GiveMemberRole(Guid member, Guid chatId, PublicChatMemberRole role, CancellationToken cancellationToken)
        {
            await _publicChatStore.GiveMemberRoleAsync(member, chatId, HttpContext.GetUserId(), role switch
            {
                PublicChatMemberRole.Owner => EnPublicChatMemberRole.Creator,
                PublicChatMemberRole.Administrator => EnPublicChatMemberRole.Administrator,
                PublicChatMemberRole.Member => EnPublicChatMemberRole.Member,
                PublicChatMemberRole.Reader => EnPublicChatMemberRole.Reader,
                _ => throw new ArgumentOutOfRangeException()
            }, cancellationToken);
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
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateChat(Guid chatId, [FromForm] UpdatePublicChatForm form, CancellationToken cancellationToken)
        {
            Guid? newAvatarId = form.NewChatImage is not null ? Guid.NewGuid() : null;
            await _publicChatStore.UpdateChatAsync(chatId, HttpContext.GetUserId(), form.NewChatName, form.NewSearchable, form.UpdateAvatar, form.NewChatImage is not null ? new MediaFile { ContentType = form.NewChatImage.ContentType, FileName = form.NewChatImage.FileName, MediaId = newAvatarId.Value } : null, form.NewDefaultMemberRole switch
            {
                PublicChatMemberRole.Owner => EnPublicChatMemberRole.Creator,
                PublicChatMemberRole.Administrator => EnPublicChatMemberRole.Administrator,
                PublicChatMemberRole.Member => EnPublicChatMemberRole.Member,
                PublicChatMemberRole.Reader => EnPublicChatMemberRole.Reader,
                _ => throw new ArgumentOutOfRangeException()
            }, cancellationToken);
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
        [HttpDelete("[action]")]
        public async Task<IActionResult> BanUser(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            await _publicChatStore.BanUserAsync(chatId, userId, HttpContext.GetUserId(), cancellationToken);
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
        [HttpDelete("[action]")]
        public async Task<IActionResult> UnbanUser(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            await _publicChatStore.UnbanUserAsync(chatId, userId, HttpContext.GetUserId(), cancellationToken);
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
            await _publicChatStore.DeleteMessageAsync(chatId, messageId, HttpContext.GetUserId(), cancellationToken);
            await _updatesService.MessageDeleted(chatId, messageId,
                (await _publicChatStore.GetChatFullInfoAsync(chatId, HttpContext.GetUserId(), cancellationToken)).Members.Select(m => m.UserId).ToArray(),
                ChatType.Group, cancellationToken);
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
            Guid mediaId = Guid.Parse(mediaLink.Replace($"{mediaPrefix}/", ""));
            await _publicChatStore.DeleteFileFromMessageAsync(chatId, messageId, mediaId, HttpContext.GetUserId(), cancellationToken);
            await _updatesService.FileDeleted(chatId, mediaLink, chatId,
                (await _publicChatStore.GetChatFullInfoAsync(chatId, HttpContext.GetUserId(), cancellationToken)).Members.Select(m => m.UserId).ToArray(),
                ChatType.Group, cancellationToken);
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
            await _publicChatStore.LeaveChatAsync(chatId, HttpContext.GetUserId(), cancellationToken);
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
            await _publicChatStore.DeleteChatMemberAsync(chatId, memberId, HttpContext.GetUserId(), cancellationToken);
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
            await _publicChatStore.DeleteAndBanChatMemberAsync(chatId, memberId, HttpContext.GetUserId(), cancellationToken);
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
            await _publicChatStore.DeleteChatAsync(chatId, HttpContext.GetUserId(), cancellationToken);
            await _updatesService.ChatDeleted(chatId,
                (await _publicChatStore.GetChatFullInfoAsync(chatId, HttpContext.GetUserId(), cancellationToken)).Members.Select(m => m.UserId).ToArray(),
                ChatType.Group, cancellationToken);
            return Ok();
        }
    }
}
