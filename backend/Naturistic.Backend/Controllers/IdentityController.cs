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
using Naturistic.Core.Interfaces.Services;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Naturistic.Core.Interfaces;
using Naturistic.Core.Services.Localization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Common.Utils;

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
        private readonly IPasswordRecoveryTokensRepository passwordRecoveryTokensRepository;
        private readonly IEmailService emailService;
        private readonly IPasswordHasher<ApplicationUser> passwordHasher;
        private readonly IBroadcastRepository broadcastRepository;

        public IdentityController(IWebHostEnvironment hostEnviroment, 
                           UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager, 
                           IConfiguration configuration,
                           IViewerUsersRepository channelRepository,
                           IChatsRepository chatsRepository,
                           IUserRepository userRepository,
                           IConfrimCodesRepository confrimCodesRepository,
                           IPasswordRecoveryTokensRepository passwordRecoveryTokensRepository,
						   IEmailService emailService,
                           IPasswordHasher<ApplicationUser> passwordHasher,
						   ILogger<IdentityController> logger)
        {
            this.hostEnviroment = hostEnviroment;
            this.configuration = configuration;
            this.viewerUsersRepository = channelRepository;
            this.chatsRepository = chatsRepository;
            this.userRepository = userRepository;
            this.confrimCodesRepository = confrimCodesRepository;
            this.passwordRecoveryTokensRepository = passwordRecoveryTokensRepository;
            this.emailService = emailService;
            this.passwordHasher = passwordHasher;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

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

        private async Task<int> CreateVerificaionCodeAsync(string username)
        {
            string a = random.Next(1, 10).ToString();
            string b = random.Next(0, 10).ToString();
            string c = random.Next(0, 10).ToString();
            string d = random.Next(0, 10).ToString();
            string f = random.Next(0, 10).ToString();
            int confirmNumber = Int32.Parse(a + b + c + d + f);

            var user = await userManager.FindByNameAsync(username);

            confrimCodesRepository.Add(new ConfirmCode { Code = confirmNumber, User = user, UserId = username });

            logger.LogInformation($"Creating digit confirmation with numbers {confirmNumber} for user: {username}");

            return confirmNumber;
        }

        private int GetVerificationCode(string userId)
        {
            IConfirmCode number = confrimCodesRepository.OneByUser(userId);

            if (number != null)
            {
                return number.Code;
            }

            return -1;
        }

        [HttpPost]
        [Route("api/account/requestverificationcode")]
        public async Task<object> CreateVerificationCodeAsync(string username)
        {
			string a = random.Next(1, 10).ToString();
			string b = random.Next(0, 10).ToString();
			string c = random.Next(0, 10).ToString();
			string d = random.Next(0, 10).ToString();
			string f = random.Next(0, 10).ToString();
			int confirmNumber = Int32.Parse(a + b + c + d + f);

			var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                return CreateJsonError($"Can't find user {username}.");
            }

			confrimCodesRepository.Add(new ConfirmCode { Code = confirmNumber, User = user, UserId = user.Id });

			logger.LogInformation($"Creating confirmation code with numbers {confirmNumber} for user: {username}");

            string emailBody = "";

			var emailResponse = await emailService.SendEmailAsync(user.Email, "Email Verification", emailBody);

            if (emailResponse.Success)
            {
				return Json(CreateJsonSuccess());
			}

			return Json(CreateJsonError(emailResponse.Mssage));
		}

        [HttpPost]
        [Route("api/account/verifyaccount")]
        public async Task<object> VerifyAccountAsync(string username, int code)
        {
			IUser user = this.userRepository.One(x => x.GetUsername() == username);

			if (user == null)
			{
				string errorInfo = $"Something went wrong! Couldn't find user with email = {username}";

				logger.LogInformation(errorInfo);

				return Json(CreateJsonError(errorInfo));
			}

			int exprectedCode = GetVerificationCode(user.GetId());

            if (exprectedCode == code)
            {
				ApplicationUser appUser = (ApplicationUser)user;

				appUser.EmailConfirmed = true;

				if (userRepository.Update(user))
                {
					logger.LogInformation($"Account {username} has been confirmed!");

                    confrimCodesRepository.Remove(appUser.ConfirmCode);

					return Json(CreateJsonSuccess());
				}

                // if database has fucked up
				string errorInfo = $"Server Error.";

				logger.LogInformation(errorInfo);

				return Json(CreateJsonError(errorInfo));
			}
            else
            {
                string errorInfo = $"Number required for {username} confirmation are wrong! Waiting for client to send right number.";

				logger.LogInformation(errorInfo);

                return Json(CreateJsonError(errorInfo));
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

        private string RECOVERY_MAIL1 => "RECOVERY_MAIL1";

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("api/account/sendrecoveryemail")]
		public async Task<object> SendRecoveryEmail(string username, string email, string locale,
            [FromServices] IServer server, [FromServices] ILocalizationService localization)
        {
            ApplicationUser user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                logger.LogInformation($"Could't find user with name {username}");
                return BadRequest($"Could't find user with name {username}");
            }

            var addresses = server.Features.Get<IServerAddressesFeature>().Addresses;

            string https = addresses.First();

            string randomString = Guid.NewGuid().ToString();
            string token = passwordHasher.HashPassword(null, randomString);

            string url = https + $"/api/account/sendrecoveryemail?token={token}";

            if (String.IsNullOrEmpty(locale))
            {
                localization.SetLocale("ru");
            }
            else
            {
                localization.SetLocale(locale);
            }

            string content = localization.RetrievePhrase(RECOVERY_MAIL1) + ": " + url;

            DateTime expireDate = DateTime.Now.AddHours(PasswordRecoveryToken.LifetimeHours);
            PasswordRecoveryToken recoveryToken = new PasswordRecoveryToken
            {
                Token = token,
                User = user,
                UserId = user.Id,
                ExpireAt = TimeUtils.ToUnixTimeSeconds(expireDate)
            };

            if (passwordRecoveryTokensRepository.Contains(user.Id))
            {
                passwordRecoveryTokensRepository.DeleteAll(x => x.GetUsername() == user.Id);
			}

            passwordRecoveryTokensRepository.Add(recoveryToken);

            logger.LogInformation($"Created password recovery token: user {user.UserName}, token {token}");

            //Task sendTask = emailService.SendEmailAsync(username, email, content);

            return Ok();
        }
        
        public enum RegisterType : sbyte
        {
            ViewerUser = 2,
            BroadcastUser = 1
        }
    }
}
