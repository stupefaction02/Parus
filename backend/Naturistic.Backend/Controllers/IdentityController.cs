using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Naturistic.Infrastructure.Identity;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Backend.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

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

        //public async Task<object> 

        #region Testing

        // breaere
        [Authorize]
        [HttpGet]
        [Route("api/test")]
        public object Test()
        {
            return "Secret!";
        }

		[HttpGet]
		[Route("api/test/users")]
		public object Users()
        {
            return Json(userRepository.Users);
        }

		[HttpGet]
		[Route("api/test/getuser")]
		public object Users(string username)
		{
			return Json(userRepository.FindUserByUsername(username));
		}

		#endregion

		[HttpPost]
		private JwtToken CreateJWT(ClaimsIdentity user)
        {
			logger.LogInformation($"Tryig to create JWT token for user {user.Name} ...");

			DateTime now = DateTime.UtcNow;

			JwtSecurityToken jwt = new JwtSecurityToken(
					issuer: JwtAuthOptions.ISSUER,
					audience: JwtAuthOptions.AUDIENCE,
					notBefore: now,
					claims: user.Claims,
					expires: now.Add(TimeSpan.FromMinutes(JwtAuthOptions.LIFETIME)),
					signingCredentials: new SigningCredentials(JwtAuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            logger.LogInformation(encodedJwt);

            logger.LogInformation($"JWT token for user {user.Name} has been created.");

            return new JwtToken
            {
                Token = encodedJwt,
                Username = user.Name
			};
		}

        private struct JwtToken
        {
            public string Token { get; set; }
            public string Username { get; set; }
        }

        private async Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user)
        {
            if (await userManager.IsInRoleAsync(user, "admin"))
            {
                return null;
            }
            else
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.GetUsername())
                };

                return new ClaimsIdentity(claims);
            }
        }

		[HttpPost]
        [Route("api/account/login")]
        public async Task<object> Login(string nickname, string password)
        {
            logger.LogInformation($"Attempt to login {nickname}");
            ApplicationUser user = await userManager.FindByNameAsync(nickname);

            var signInResult = await signInManager.PasswordSignInAsync(user.UserName, password, false, false);
            
            if (signInResult.Succeeded)
            {
                logger.LogInformation($"{nickname} login successfully!");

                var identity = await CreateIdentityAsync(user);

                JwtToken newToken = CreateJWT(identity);
                return CreateJsonSuccess(additionalParameter: newToken.Token);
            }
            else
            {
                string errorMessage = $"Wrong password for user {user.UserName}";
                logger.LogInformation($"Failed to login {nickname}. Error: {errorMessage}");

                return CreateJsonError(errorMessage);
            }
        }

        // TODO: reconsider gender enum type as something with less size
        [HttpPost]
        [Route("api/account/register")]
		public async Task<object> Register(string username, string email,
         string password, Gender gender, RegisterType registerType)
		{
			logger.LogInformation($"User to register: {email}. Register type: {registerType}");
            
            var user = new ApplicationUser
            {
                UserName = username,
                Email = email
            };

            var created = await userManager.CreateAsync(user, password);

            if (created.Succeeded)
            {
                var createdUsr = userRepository.FindUserByUsername(username);
                logger.LogInformation(createdUsr.GetUsername());
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

                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.GetUsername())
                };

                ClaimsIdentity identity = new ClaimsIdentity(claims);

                JwtToken jwt = CreateJWT(identity);

                logger.LogInformation("User registered successfully!");

                // TODO: put on another thread

                return Json(CreateJsonSuccess(new { access_token = jwt.Token }));
			}
            else
            {
                string errors = "\t" + Environment.NewLine;
                foreach (var e in created.Errors)
                {
                    errors += e.Description + Environment.NewLine;
                }

                string err = $"Unable to create user: {user}. Errors: {errors}";

				logger.LogError(err);

                return Json(CreateJsonError(err));
            }
		}

		private object CreateJsonError(string message)
		{
            return new { success = "N", message = message };
		}

		private object CreateJsonSuccess(string message)
		{
			return new { success = "Y", message = message };
		}

		private object CreateJsonSuccess()
		{
			return new { success = "Y" };
		}

        private object CreateJsonSuccess(object additionalParameter)
        {
            return new { success = "Y", payload = additionalParameter };
        }

        private Dictionary<string, int> confirmatonsTable = new Dictionary<string, int>();
        private Random random = new Random();

        private void CreateVerificaionCode(string username)
        {
            string a = random.Next(1, 10).ToString();
            string b = random.Next(0, 10).ToString();
            string c = random.Next(0, 10).ToString();
            string d = random.Next(0, 10).ToString();
            string f = random.Next(0, 10).ToString();
            int confirmNumber = Int32.Parse(a + b + c + d + f);

            confrimCodesRepository.Add(new ConfirmCodeEntity { Code = confirmNumber, Username = username });

            logger.LogInformation($"Creating digit confirmation with numbers {confirmNumber} for user: {username}");
        }

        private int GetVerificationCode(string email)
        {
            ConfirmCodeEntity number = confrimCodesRepository.Codes.SingleOrDefault(x => x.Username == email);

            if (number != null)
            {
                return number.Code;
            }

            return -1;
        }

        [HttpPost]
        [Route("api/account/requestverificationcode")]
        public async Task<object> CreateVerificationCode(string username)
        {
            CreateVerificaionCode(username);

            return CreateJsonSuccess();
        }

        [HttpPost]
        [Route("api/account/verifyaccount")]
        public async Task<object> VerifyAccount(string username, int code)
        {
            int exprectedCode = GetVerificationCode(username);

            if (exprectedCode == code)
            {
                IUser user = this.userRepository.FindUserByUsername(username);

                if (user != null)
                {
                    userRepository.Update(() => user.EmailConfirmed = true);

                    logger.LogInformation($"Account {username} has been confirmed!");
                }
                else
                {
                    return NotFound($"Something went wrong! Couldn't find user with email = {username}");
                }

                return Ok("Y");
            }
            else
            {
                logger.LogInformation($"Number required for {username} confirmation are wrong! Waiting for client to send right number.");

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
