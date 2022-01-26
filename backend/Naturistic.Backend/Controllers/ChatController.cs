using Cassandra;
using Microsoft.AspNetCore.Mvc;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.Backend.Controllers
{
    [ApiController]
    public class ChatController : Controller
    {
        public ChatController()
        {
        }
        
        [HttpGet]
        [Route("")]
        public void Entrance()
        {
            
        }
    }
}