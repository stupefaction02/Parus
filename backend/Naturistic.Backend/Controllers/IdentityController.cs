using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cassandra;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Naturistic.Infrastructure.Identity;
using Naturistic.Infrastructure.DLA;
using Naturistic.Core;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces.Repositories;

namespace Naturistic.Backend.Controllers
{
    [ApiController]
    public class IdentityController : Controller
    {
        private readonly IWebHostEnvironment hostEnviroment;

        private readonly ILogger<IdentityController> logger;

		private readonly UserManager<ApplicationUser> userManager;

        private readonly SignInManager<ApplicationUser> signInManager;

        private IConfiguration configuration;
        private readonly IViewerUsersRepository viewerUsersRepository;
        private readonly IChatsRepository chatsRepository;
        private readonly IBroadcastRepository broadcastRepository;

        public IdentityController(IWebHostEnvironment hostEnviroment, 
                           UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager, 
                           IConfiguration configuration,
                           IViewerUsersRepository channelRepository,
                           IChatsRepository chatsRepository,
                           ILogger<IdentityController> logger)
        {
            this.hostEnviroment = hostEnviroment;
            this.configuration = configuration;
            this.viewerUsersRepository = channelRepository;
            this.chatsRepository = chatsRepository;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [HttpGet]
        [Route("api/test")]
        public object Test()
        {
            
            return "Test";
        }

        [HttpPost]
        [Route("api/user/login")]
        public async Task<object> Login(string nickname, string password)
        {
            logger.LogInformation($"Attempt to login {nickname}");
            var user = await userManager.FindByNameAsync(nickname);

            if (user != null)
            {
                var signInResult = await signInManager.PasswordSignInAsync(user.UserName, password, false, false);

                if (signInResult.Succeeded)
                {
                    logger.LogInformation($"{nickname} login successfully!");
                    return StatusCode(200, new { Success = true, Message = "Login was successful" });
                }
            }
             
            logger.LogInformation($"Failed to login {nickname}");

            return StatusCode(500, new { Success = false, Message = "Server Error!" });
        }

        [HttpPost]
        [Route("api/user/register")]
		public async Task<object> Register(string nickname, string email,
         string password, string passwordRepeat, RegisterType registerType)
		{
			logger.LogInformation($"User to register: {email}. Register type: {registerType}");
            
            var user = new ApplicationUser
            {
                UserName = nickname,
                Email = email
            };

            var created = await userManager.CreateAsync(user, password);

            if (created.Succeeded)
            {
                switch (registerType)
                {
                    case RegisterType.BroadcastUser:
                    {
                        // As an idea, add BroadcasterUserManager or smth

                        logger.LogInformation("Create broadcast user...");

                        /* creating a channel. each identity user has its own channel */
                        var bcUser = new BroadcastUser
                        {
                            IdentityUserId = user.Id
                        };

                        logger.LogInformation("Create cha and attach to broadcast user...");

                        var bcChat = new Chat
                        {
                            BroadcastUser = bcUser
                        };

                        chatsRepository.Add(bcChat);
                    }
                    break;

                    default:
                    case RegisterType.ViewerUser:
                    {
                        /* creating a channel. each identity user has its own channel */
                        var channel = new ViewerUser
                        {
                            IdentityUserId = user.Id
                        };
                        logger.LogInformation("Creating the channel binded to user...");
                        viewerUsersRepository.Add(channel);
                    }
                    break;
                }

                logger.LogInformation("User registered successfully!");

                return Ok($"{nickname} {email} registered successfully!");
            }
            else
            {
                return null;
            }
		}

        [HttpGet]
        [Route("api/users")]
        public object GetUsers()
        {
            return Ok(userManager.Users);
        }

        [HttpGet]
        [Route("api/users/current")]
        public object GetCurrentUser()
        {
            return Ok(User);
        }

        public enum RegisterType : sbyte
        {
            ViewerUser = 2,
            BroadcastUser = 1
        }
    }
}
