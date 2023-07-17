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
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Linq.Expressions;
using Naturistic.Infrastructure.DLA.Repositories;
using Naturistic.Backend.Extensions;

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
        private readonly IUserRepository userRepository;
        private readonly IConfrimCodesRepository confrimCodesRepository;
        private readonly IBroadcastRepository broadcastRepository;

        public IdentityController(IWebHostEnvironment hostEnviroment, 
                           UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager, 
                           IConfiguration configuration,
                           IViewerUsersRepository channelRepository,
                           IChatsRepository chatsRepository,
                           IUserRepository userRepository,
                           IConfrimCodesRepository confrimCodesRepository,
                           ILogger<IdentityController> logger)
        {
            this.hostEnviroment = hostEnviroment;
            this.configuration = configuration;
            this.viewerUsersRepository = channelRepository;
            this.chatsRepository = chatsRepository;
            this.userRepository = userRepository;
            this.confrimCodesRepository = confrimCodesRepository;
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
        [Route("api/account/login")]
        public async Task<object> Login(string nickname, string password)
        {
            logger.LogInformation($"Attempt to login {nickname}");
            var user = await userManager.FindByNameAsync(nickname);

            var signInResult = await signInManager.PasswordSignInAsync(user.UserName, password, false, false);

            if (signInResult.Succeeded)
            {
                logger.LogInformation($"{nickname} login successfully!");

                return StatusCode(200);
            }
            else
            {
                string errorMessage = $"Wrong password for user {user.UserName}";
                logger.LogInformation($"Failed to login {nickname}. Error: {errorMessage}");

                return Json(errorMessage);
            }
        }

        // TODO: reconsider gender enum type as something with less size
        [HttpPost]
        [Route("api/account/register")]
		public async Task<object> Register(string nickname, string email,
         string password, Gender gender, RegisterType registerType)
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

                        bool channelCreated = viewerUsersRepository.Add(channel);
                        if (!channelCreated)
                        {
                            logger.LogInformation($"Unable to create a bind channel to user: {user}");
                        }
                    }
                    break;
                }

                logger.LogInformation("User registered successfully!");

                // TODO: put on another thread
                
                //CreateVerificaionCode(user.Email);

                return Ok($"{nickname} {email} registered successfully!");
            }
            else
            {
                string errors = "\t" + Environment.NewLine;
                foreach (var e in created.Errors)
                {
                    errors += e.Description + Environment.NewLine;
                }
                logger.LogError($"Unable to create user: {user}. Errors: {errors}");
                return null;
            }
		}

        private Dictionary<string, int> confirmatonsTable = new Dictionary<string, int>();
        private Random random = new Random();

        private void CreateVerificaionCode(string email)
        {
            string a = random.Next(0, 10).ToString();
            string b = random.Next(0, 10).ToString();
            string c = random.Next(0, 10).ToString();
            string d = random.Next(0, 10).ToString();
            string f = random.Next(0, 10).ToString();
            int confirmNumber = Int32.Parse(a + b + c + d + f);

            confrimCodesRepository.Add(new ConfirmCodeEntity { Code = confirmNumber, UserEmail = email });

            logger.LogInformation($"Creating digit confirmation with numbers {confirmNumber} for user: {email}");
        }

        private int GetVerificationCode(string email)
        {
            ConfirmCodeEntity number = confrimCodesRepository.Codes.SingleOrDefault(x => x.UserEmail == email);

            if (number != null)
            {
                return number.Code;
            }

            return -1;
        }

        [HttpPost]
        [Route("api/account/createverificationcode")]
        public async Task<object> CreateVerificationCode(string email)
        {
            CreateVerificaionCode(email);

            return Ok("");
        }

        [HttpPost]
        [Route("api/account/verifyaccount")]
        public async Task<object> VerifyAccount(string email, int code)
        {
            int exprectedCode = GetVerificationCode(email);

            if (exprectedCode == code)
            {
                var user = userManager.FindUserByEmail(email);

                if (user != null)
                {
                    userRepository.Update(() => user.EmailConfirmed = true);

                    logger.LogInformation($"Account {email} has been confirmed!");
                }
                else
                {
                    return NotFound($"Something went wrong! Couldn't find user with email = {email}");
                }

                return Ok("Y");
            }
            else
            {
                logger.LogInformation($"Number required for {email} confirmation are wrong! Waiting for client to send right number.");

                return Ok("N");
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

        [HttpGet]
        [Route("api/account/checkifemailexists")]
        public async Task<object> CheckIfEmailExists(string email)
        {
            if (userRepository.CheckIfEmailExists(email))
            {
                return Ok("Y");
            }
            else
            {
                return Ok("N");
            }
        }

        [HttpGet]
        [Route("api/account/checkifnicknameexists")]
        public async Task<object> CheckIfNicknameExists(string nickname)
        {
            if (userRepository.CheckIfNicknameExists(nickname))
            {
                return Ok("Y");
            }
            else
            {
                return Ok("N");
            }
        }

        public enum RegisterType : sbyte
        {
            ViewerUser = 2,
            BroadcastUser = 1
        }
    }
}
