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

using Parus.Infrastructure.Identity;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using Parus.Backend.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Parus.Core.Interfaces.Services;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Parus.Core.Interfaces;
using Parus.Core.Services.Localization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Parus.Infrastructure.DLA.Repositories;
using Parus.Common.Utils;
using System.Diagnostics;
using MimeKit.Cryptography;
using Parus.Infrastructure.DLA;
using Parus.Core.Authentication;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;

namespace Parus.Backend.Controllers
{
    // TODO: Rename to UserController
    [ApiController]
    public partial class IdentityController : ParusController
    {
        public new ClaimsPrincipal User
        {
            get
            {
                return HttpContext != null ? HttpContext.User : user;
            }
            set
            {
                user = value;
            }
        }

        private readonly ILogger<IdentityController> logger;
        private readonly IHostEnvironment environment;
        private readonly ParusUserIdentityService identityService;

        public IdentityController(ILogger<IdentityController> logger, 
            IHostEnvironment environment, 
            ParusUserIdentityService identityService)
        {
            this.logger = logger;
            this.environment = environment;
            this.identityService = identityService;
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

        [EnableRouteResponseCompression]
		[HttpGet]
		[Route("api/test/users")]
		public object Users(IUserRepository userRepository)
        {
            return Json(userRepository.Users);
        }

        [HttpGet]
        [Route("api/test/userslimited")]
        public object UsersLimited(IUserRepository userRepository)
        {
            return Json(userRepository.Users.Where(x => x.EmailConfirmed).Select(x =>
            {
                ParusUser usr = (ParusUser)x;

                return new { 
                    id = usr.Id, 
                    username = usr.UserName, 
                    email = usr.Email, 
                    emailConfirmed = usr.EmailConfirmed,
                    jwt = GenerateJwtForUser(usr.UserName)
                };
            }));
        }

        [HttpGet]
		[Route("api/test/getuser")]
		public object Users(string username, IUserRepository userRepository)
		{
			return Json(userRepository.FindUserByUsername(username));
		}

        #endregion

        private JwtToken CreateJWT(string username, IEnumerable<Claim> claims)
        {
            //logger.LogInformation($"Tryig to create JWT token for user {username} ...");

            DateTime now = DateTime.UtcNow;

            JwtSecurityToken jwt = new JwtSecurityToken(
                    issuer: JwtAuthOptions1.ISSUER,
                    audience: JwtAuthOptions1.AUDIENCE,
                    //notBefore: now,
                    claims: claims,
                    expires: now.Add(new TimeSpan(168, 0, 0, 0)),
                    signingCredentials: new SigningCredentials(JwtAuthOptions1.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            //logger.LogInformation(encodedJwt);

            //logger.LogInformation($"JWT token for user {username} has been created.");

            return new JwtToken
            {
                Token = encodedJwt,
                Username = username
            };
        }

        public static class APIErrorCodes
        {
            public static readonly string LOGIN_WRONG_PSWD = "Login.WrongPassword";
            internal static readonly object TWO_FA_WRONG_QR_CODE = "2FA.WrongCode";
        }

		[HttpPost]
        [Route("api/account/login")]
        public async Task<object> Login(
            string username, 
            string password,
            IPasswordHasher<ParusUser> passwordHasher, 
            ParusDbContext dbContext)
        {
            logger.LogInformation($"Attempt to login {username}");
            ParusUser user = await dbContext.Users
                .Include(x => x.CustomerKey)
                .FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null)
            {
                string errorMessage = $"Wrong username for user {username}";
                logger.LogInformation($"Failed to login {username}. Error: {errorMessage}");

                HttpContext.Response.StatusCode = 401;
                // TODO: enum for error codes
                return new { 
                    success = "false", 
                    //message = "Wrong username for user {user.UserName}", 
                    errorCode = APIErrorCodes.LOGIN_WRONG_PSWD
                };
            }

            PasswordVerificationResult signInResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            
            if (signInResult == PasswordVerificationResult.Success)
            {
                logger.LogInformation($"{username} login successfully!");

                if (user.TwoFactorEnabled)
                {
                    // TODO: Configure relations so user.TwoFactorCustomKey
                    //var customKey = dbContext.TwoFactoryCustomerKeys.FirstOrDefault(x => x.UserId == user.Id);
                    var customKey = user.CustomerKey;

                    // exceptional case
                    if (customKey == null)
                    {
                        return HandleServerError($"Identity", $"TwoFactorEnabled was set to 'true' but couldn't find any customerKey associated with the user {user}");
                    }

                    return Json(new
                    {
                        success = "true",
                        twoFactoryEnabled = true,
                        twoFactoryCustomKey = customKey.Key
                    });
                }

                string fingerPrint = HttpContext.Request.Headers.UserAgent;
                return await HandleLoginAsync(user, fingerPrint, dbContext);
            }
            else
            {
                string errorMessage = $"Wrong password for user {user.UserName}";
                logger.LogInformation($"Failed to login {username}. Error: {errorMessage}");

                HttpContext.Response.StatusCode = 401;
                return new { errorCode = APIErrorCodes.LOGIN_WRONG_PSWD };
            }
        }

        // TODO: reconsider gender enum type as something with less size
        [HttpPost]
        [Route("api/account/register")]
		public async Task<object> Register(string username, string email, string password, Gender gender)
		{
            // TODO: Add here a proper json binding. Js side have to send json too
            // the parameters must be dto
            var dto = new ParusUserRegistrationJsonDTO
            {
                Username = username,
                Email = email,
                Password = password,
                Gender = gender
            };

            var res = await identityService.RegiserAsync(dto);

            HttpContext.Response.StatusCode = res.StatusCode;
            return Json(res.JsonResponse);
        }
        
        private object CreateJsonError(string message)
		{
            return new { success = "N", message = message };
		}

        private Random random = new Random();
        private ClaimsPrincipal user;

        private int GetVerificationCode(string userId, IConfrimCodesRepository confrimCodesRepository)
        {
            IVerificationCode number = confrimCodesRepository.OneByUser(userId);

            if (number != null)
            {
                return number.Code;
            }

            return -1;
        }

        public class CustomModel
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        [HttpPost]
        [Route("api/test/modelbinding")]
        public object BindModel([FromBody] CustomModel model)
        {
            return 1;
        }

        [HttpPost]
        [Route("api/account/requestverificationcode")]
        public async Task<object> RequestVerificationCodeAsync(string username, bool forceCreate,
            [FromServices] IConfrimCodesRepository confrimCodesRepository,
            [FromServices] IEmailService emailService,
            [FromServices] UserManager<ParusUser> userManager)
        {
            var user = await userManager.FindByNameAsync(username);

            if (forceCreate) goto CreateNew;

            ConfirmCode addedCode = (ConfirmCode)confrimCodesRepository.OneByUser(user.GetId());

            // if alread exists
            if (addedCode != null)
            {
                bool expired = false;

                // exceptional case, expired code must've been already deleted
                if (expired)
                {
                    // here we must lock thread 
                    confrimCodesRepository.Remove(user.ConfirmCode);

                    // go to create new one
                    goto CreateNew;
                }
                // otherwise user will user old code
                return CreateJsonError($"{username} already has token.");
            }

            CreateNew:
            // Too overhead but more random
            string a = random.Next(1, 10).ToString();
			string b = random.Next(0, 10).ToString();
			string c = random.Next(0, 10).ToString();
			string d = random.Next(0, 10).ToString();
			string f = random.Next(0, 10).ToString();

			int verificationCode = Int32.Parse(a + b + c + d + f);

            if (user == null)
            {
                return CreateJsonError($"Can't find user {username}.");
            }

			confrimCodesRepository.Add(new ConfirmCode { Code = verificationCode, User = user, UserId = user.Id });

			logger.LogInformation($"Creating confirmation code with numbers {verificationCode} for user: {username}");

            string emailBody = "";

			var emailResponse = await emailService.SendEmailAsync(user.Email, "Email Verification", emailBody);

            if (emailResponse.Success)
            {
                if (environment.IsAnyDevelopment())
                {
                    return Json(new { success = "true", code = verificationCode });
                }
                else
                {
                    return Json(new { success = "true" });
                }
			}

			return Json(CreateJsonError(emailResponse.Mssage));
		}

        [HttpPost]
        [Route("api/account/verifyaccount")]
        public async Task<object> VerifyAccountAsync(string username, int code,
            IUserRepository userRepository,
            IConfrimCodesRepository confrimCodesRepository)
        {
			IUser user = userRepository.One(x => x.GetUsername() == username);

			if (user == null)
			{
				string errorInfo = $"Something went wrong! Couldn't find user with email = {username}";

				logger.LogInformation(errorInfo);

				return Json(CreateJsonError(errorInfo));
			}

			int exprectedCode = GetVerificationCode(user.GetId(), confrimCodesRepository);

            if (exprectedCode == code)
            {
				ParusUser appUser = (ParusUser)user;

				appUser.EmailConfirmed = true;

				if (userRepository.Update(user))
                {
					logger.LogInformation($"Account {username} has been confirmed!");

                    confrimCodesRepository.Remove(appUser.ConfirmCode);

					return Json(JsonSuccess());
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
        public object GetUsers(UserManager<ParusUser> userManager)
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
        [Route("api/account/isemailtaken")]
        public object IsEmailTaken(string email, IUserRepository userRepository)
            => userRepository.IsEmailTaken(email) ? Json(new { taken = "true" }) : (object)Json(new { taken = "false" });

        [HttpGet]
        [Route("api/account/isusernametaken")]
        public object IsUsernameTaken(string username, IUserRepository userRepository)
            => userRepository.IsUsernameTaken(username) ? Json(new { taken = "true" }) : (object)Json(new { taken = "false" });

        private string RECOVERY_MAIL1 => "RECOVERY_MAIL1";

        [HttpPost]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("api/account/sendrecoveryemail")]
		public async Task<object> SendRecoveryEmail(string username, string email, string locale,
            IPasswordRecoveryTokensRepository passwordRecoveryTokens,
            IPasswordHasher<ParusUser> passwordHasher,
            [FromServices] IServer server, 
            [FromServices] ILocalizationService localization,
            UserManager<ParusUser> userManager)
        {
            // TODO: Add protection from the case when user can 
            // request a recovery code multiple times for different mails and thus 
            // stuff up the database
            // 1. add checking if mail belongs to user with exact username entered on phase 1
            // 2. usual request count limitations
            // 3. point 2 but add IP checking as well

            ParusUser user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                logger.LogInformation($"Could't find user with name {username}");
                return BadRequest($"Could't find user with name {username}");
            }

            PasswordRecoveryToken existedToken = 
                (PasswordRecoveryToken)passwordRecoveryTokens.OneByUser(user.GetId());
            if (existedToken != null)
            {
                passwordRecoveryTokens.Delete(user.PasswordRecoveryToken);
            }

            var addresses = server.Features.Get<IServerAddressesFeature>().Addresses;

            string https = "https://localhost:5002";// addresses.First();

            string token = Guid.NewGuid().ToString().Replace("-", "AMOGUS");// passwordHasher.HashPassword(null, randomString);

            string url = https + $"/account/editpassword?token={token}";

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
                ExpireAt = DateTimeUtils.ToUnixTimeSeconds(expireDate)
            };

            passwordRecoveryTokens.Add(recoveryToken);

            logger.LogInformation($"Created password recovery token: user {user.UserName}, token {token}");
            logger.LogInformation($"Recovery URL: {url}");

            //Task sendTask = emailService.SendEmailAsync(username, email, content);

            return Ok();
        }

        // TODO: make it as private api
        // when calling this method a client must have a special private api token
        [HttpGet]
        [Route("api/account/refreshtoken")]
        public async Task<object> RefreshToken(string fingerPrint, string refreshToken,
            [FromServices] ParusDbContext identityDbContext)
        {
            //string rsUuid = HttpContext.Request.Cookies["refreshToken"];

            if (String.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized();
            }

            // if user was deleted from database but jwt cookies is still there
            // checks if user is existed
            //if (!identityDbContext.Users.Any(x => x.UserName == User.Identity.Name))
            //{
            //    //Debug.Console("User with associated with this auth token is deleted form db.");
            //    return NotFound("Can't find user.");
            //}

            RefreshSession lastRs = identityDbContext.RefreshSessions
                .Include(x => x.User)
                .AsEnumerable()
                .FirstOrDefault(x => x.Token == refreshToken);

            // exceptiona case
            // it means that this user has nver registered into our system
            if (lastRs == null)
            {
                LogInfo_Debug("token havent' created before.");
                return HandleServerError("Identity", "Detected and attempt requesting refreshToken for unregistered user");
            }

            int now = DateTimeUtils.ToUnixTimeSeconds(DateTime.UtcNow);
            if (lastRs.ExpiresAt < now)
            {
                LogInfo_Debug("token expired.");
                return Unauthorized();
            }

            if (lastRs.Fingerprint != fingerPrint)
            {
                LogInfo_Debug("finger print dismatch.");
                return Forbid();
            }

            ParusUser user = lastRs.User;

            int expTs = DateTimeUtils.ToUnixTimeSeconds(
                DateTime.UtcNow.Add(RefreshSession.LifeTime)
            );

            lastRs.Fingerprint = fingerPrint;
            lastRs.ExpiresAt = expTs;
            lastRs.Token = RefreshSession.GenerateToken();

            identityDbContext.Update(lastRs);

            if (await identityDbContext.SaveChangesAsync() < 1)
            {

            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.GetUsername())
            };
            string jwt = CreateJWT(user.GetUsername(), claims).Token;

            Console.WriteLine($"RefreshSession was updated. User: {user.UserName} , Token: {lastRs.Token}. Access Token lifetime: {JwtAuthOptions1.LIFETIME} minutes");

            HttpContext.Response.Headers.Append("Set-Cookie", $"refreshToken='{lastRs.Token}';");

            return Json(new { accessToken = jwt, refreshToken = lastRs.Token });
        }

        // TODO: Make this [Authorize]
        //[Authorize]
        [HttpGet]
        [Route("api/account/checkPassword")]
        public async Task<object> CheckPassword(string password,
            [FromServices] SignInManager<ParusUser> signInManager,
            [FromServices] IUserRepository users)
        {
            // since we require authorization we alread have username from token
            // otherwise this method won't be called
            string username = User.Identity.Name;
            IUser user = users.One(x => x.GetUsername() == username);

            var result = await signInManager.PasswordSignInAsync((ParusUser)user, password, false, false);

            if (result.Succeeded)
            {
                return Json(new { message = "Valid" });
            }
            else
            {
                return Json(new { message = "Invalid" });
            }
        }

        // TODO: Make this [Authorize]
        //[Authorize]
        [HttpPost]
        [Route("api/account/editPassword")]
        public async Task<object> EditPassword(string newPassword,
            [FromServices] IPasswordHasher<ParusUser> passwordHasher,
            [FromServices] IUserRepository users)
        {
            // since we require authorization we alread have username from token
            // otherwise this method won't be called
            string username = User.Identity.Name;

            return EditPasswordCore(username, newPassword, passwordHasher, users);
        }

        [HttpPost]
        [Route("api/account/recoverPassword")]
        public async Task<object> RecoverPassword(string token, string newPassword,
            [FromServices] IPasswordHasher<ParusUser> passwordHasher,
            [FromServices] IUserRepository users,
            [FromServices] IPasswordRecoveryTokensRepository tokens)
        {
            IPasswordRecoveryToken tokenEntry = tokens.GetTokenWithUser(token);
            
            string msg;
            if (tokenEntry.Validate(out msg))
            {
                IUser user = tokenEntry.GetUser();

                if (user == null)
                {
                    return Unauthorized();
                }

                return EditPasswordCore(user.GetUsername(), newPassword, passwordHasher, users);
            }

            return Unauthorized(new { message = "Token is outdated." });
        }

        private JsonResult EditPasswordCore(string username, string newPassword, IPasswordHasher<ParusUser> passwordHasher, IUserRepository users)
        {
            ParusUser user = (ParusUser)users.One(x => x.GetUsername() == username);

            if (user == null)
            {
                return Json(new { message = "Fail" });
            }

            string newHash = passwordHasher.HashPassword(user, newPassword);

            user.PasswordHash = newHash;

            users.Update(user);

            LogInfo_Debug($"Password of {user} has been changed.");

            return Json(new { success = "true" });
        }
    }
}
