using Cassandra;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Naturistic.Core.Entities;
using Naturistic.Infrastructure.DLA.Repositories;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.Backend.Controllers
{
    [ApiController]
    public class ChatController : Controller
    {
        private readonly IChatsRepository chatsRepository;

        public ChatController(IChatsRepository chatsRepository)
        {
            this.chatsRepository = chatsRepository;
        }
        
        [HttpGet]
        [Route("api/chats")]
        public ActionResult GetChatInfo(int chatId)
        {
            var chat = chatsRepository.Get(chatId);

            if (chat != null)
            {
                return Ok(chat);
            }
            else return NotFound();
        }

        [HttpPost]
        [Route("api/chat/messages/send")]
        public ActionResult SendMessage(string body, string senderName, string senderNameColor, 
                                  long viewerUserId, int chatId)
        {
            var message = new Message
            {
                Body = body,
                SenderId = viewerUserId,
                TimeStamp = System.DateTime.Now,
                SenderName = senderName,
                SenderNameColor = senderNameColor
            };

            var chat = chatsRepository.Get(chatId);

            if (chat == null) return NotFound($"Can't find Chat info with id={chatId}");

            //chat.Messages.Add(message);

            // TODO: Consider adding async
            //chat.Me

            return Ok();
        }

        [HttpGet]
        [Route("api/chat/messages")]
        public JsonResult GetMessages(int chatId)
        { 
            var result = chatsRepository.GetMessages(chatId);

            return new JsonResult(result);
        }
    }
}