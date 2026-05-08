using Application.Models.Input;
using Application.Models.Output;
using Microsoft.AspNetCore.Mvc;

namespace UserAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BotChatController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns>404 if is not belonging to current user</returns>
        [ProducesResponseType(typeof(ChatShortInfo), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChatShortInfo(Guid chatId)
        {
            return Ok();
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns>list of buttons with inner command, outer text or emoji and background color (optional)</returns>
        [ProducesResponseType(typeof(BotButton[]), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetActiveButtonsList(Guid chatId)
        {
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="messagesSelectOptions"></param>
        /// <returns>404 if chat is not belonging to current user</returns>
        [ProducesResponseType(typeof(Message[]), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("{chatId}/[action]")]
        public async Task<IActionResult> GetMessages(Guid chatId, MessagesSelectOptions messagesSelectOptions)
        {
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="messageId"></param>
        /// <returns>404 if chat is not belonging to current user or message does not exist</returns>
        [ProducesResponseType(typeof(Message), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetMessage(Guid chatId, Guid messageId)
        {
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns>404 if chat is not belonging to current user</returns>
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetBotIdByChat(Guid chatId)
        {
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationBotId"></param>
        /// <returns>returns id of existing chat or creates a new one</returns>
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> OpenChatWithBot(Guid destinationBotId)
        {
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendingMessageBody"></param>
        /// <returns>404 if chat is not belonging to current user or user has no access to replying or resending message</returns>
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("[action]")]
        public async Task<IActionResult> SendMessage([FromForm] SendingMessageForm sendingMessageBody)
        {
            return Ok();
        }

        /// <summary>
        /// works like blocking a bot - user will not receive messages from it
        /// </summary>
        /// <param name="botId"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> DisableBot(Guid botId)
        {
            return Ok();
        }

        /// <summary>
        /// unblocking bot
        /// </summary>
        /// <param name="botId"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> EnableBot(Guid botId)
        {
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteChat(Guid chatId)
        {
            return Ok();
        }
    }
}