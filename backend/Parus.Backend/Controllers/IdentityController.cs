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

namespace Parus.Backend.Controllers
{
    [ApiController]
    public class IdentityController : ParusController
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

        public IdentityController(ILogger<IdentityController> logger)
        {
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

        [EnableRouteResponseCompression]
		[HttpGet]
		[Route("api/test/users")]
		public object Users(IUserRepository userRepository)
        {
            return Json(userRepository.Users);
        }

        private string GetJWT(string username)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, username)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims);

            return CreateJWT(identity).Token;
        }

        [HttpGet]
        [Route("api/test/userslimited")]
        public object UsersLimited(IUserRepository userRepository)
        {
            return Json(userRepository.Users.Where(x => x.EmailConfirmed).Select(x =>
            {
                ApplicationUser usr = (ApplicationUser)x;

                return new { 
                    id = usr.Id, 
                    username = usr.UserName, 
                    email = usr.Email, 
                    emailConfirmed = usr.EmailConfirmed,
                    jwt = GetJWT(usr.UserName)
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

        private JwtToken CreateJWT(string username, IEnumerable<Claim> claims)
        {
            logger.LogInformation($"Tryig to create JWT token for user {username} ...");

            DateTime now = DateTime.UtcNow;

            JwtSecurityToken jwt = new JwtSecurityToken(
                    issuer: JwtAuthOptions.ISSUER,
                    audience: JwtAuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: claims,
                    expires: now.Add(TimeSpan.FromMinutes(JwtAuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(JwtAuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            logger.LogInformation(encodedJwt);

            logger.LogInformation($"JWT token for user {username} has been created.");

            return new JwtToken
            {
                Token = encodedJwt,
                Username = username
            };
        }

        private struct JwtToken
        {
            public string Token { get; set; }
            public string Username { get; set; }
        }

        private async Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user)
        {
            List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.GetUsername())
                };

            return new ClaimsIdentity(claims);
        }

		[HttpPost]
        [Route("api/account/login")]
        public async Task<object> Login(
            string username, 
            string password,
            UserManager<ApplicationUser> userManager,
            IPasswordHasher<ApplicationUser> passwordHasher)
        {
            logger.LogInformation($"Attempt to login {username}");
            ApplicationUser user = await userManager.FindByNameAsync(username);

            PasswordVerificationResult signInResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            
            if (signInResult == PasswordVerificationResult.Success)
            {
                logger.LogInformation($"{username} login successfully!");

                ClaimsIdentity identity = await CreateIdentityAsync(user);

                JwtToken newToken = CreateJWT(identity);
                return Json(new { 
                    success = "true", 
                    payload = newToken.Token, 
                    twoFactoryEnabled = user.TwoFactorEnabled 
                });
            }
            else
            {
                string errorMessage = $"Wrong password for user {user.UserName}";
                logger.LogInformation($"Failed to login {username}. Error: {errorMessage}");

                HttpContext.Response.StatusCode = 401;
                return new { success = "N", errorCode = "LOGIN_WRONG_PSWD" };
            }
        }

        // TODO: reconsider gender enum type as something with less size
        [HttpPost]
        [Route("api/account/register")]
		public object Register(
            string username, 
            string email,
            string password, 
            Gender gender, 
            RegisterType registerType,
            [FromServices] UserManager<ApplicationUser> userManager,
            IUserRepository userRepository, 
            ApplicationIdentityDbContext identityDbContext)
		{
			logger.LogInformation($"User to register: {email}. Register type: {registerType}");
            
            var user = new ApplicationUser
            {
                UserName = username,
                Email = email
            };

            var created = userManager.CreateAsync(user, password).GetAwaiter().GetResult();

            if (created.Succeeded)
            {
                var createdUsr = userRepository.FindUserByUsername(username);
                logger.LogInformation(createdUsr.GetUsername());

                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.GetUsername())
                };

                ClaimsIdentity identity = new ClaimsIdentity(claims);

                JwtToken jwt = CreateJWT(identity);
                RefreshSession refreshSession = AddRefreshToken(user, "fingerprint01", identityDbContext);

                logger.LogInformation("User registered successfully!");

                // jwt expires timestamp
                int jwtExpires = DateTimeUtils.ToUnixTimeSeconds(
                        DateTime.Now.Add( JwtAuthOptions.Lifetime )
                    );
                var response = new
                {
                    success = true,
                    access_token = new { jwt = jwt.Token, expires = jwtExpires }, 
                    refresh_token = new { token = refreshSession.Token, expires = refreshSession.ExpiresAt }
                };

                identityDbContext.SaveChanges();

                return Json(response);
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

        // TODO: more effiecent way ot create tokens, with passwordHasher maybe ?
        private RefreshSession AddRefreshToken(ApplicationUser user, string fingerPrint, 
            ApplicationIdentityDbContext identityDbContext)
        {
            //identityDbContext.RefreshSessions.Remove();

            RefreshSession rs = new RefreshSession
            {
                // TODO: Replace with something more efficeint
                Token = Guid.NewGuid().ToString().Replace("-", ""),
                Fingerprint = fingerPrint,
                ExpiresAt = DateTimeUtils.ToUnixTimeSeconds( DateTime.Now.Add(RefreshSession.LifeTime) ),
                User = user
            };

            identityDbContext.RefreshSessions.Add(rs);

            return rs;
        }

        private object CreateJsonError(string message)
		{
            return new { success = "N", message = message };
		}

		private object CreateJsonSuccess(string message)
		{
			return new { success = "Y", message = message };
		}

        private object CreateJsonSuccess(object additionalParameter)
        {
            return new { success = "Y", payload = additionalParameter };
        }

        private Dictionary<string, int> confirmatonsTable = new Dictionary<string, int>();
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

        [HttpPost]
        [Route("api/account/requestverificationcode")]
        public async Task<object> CreateVerificationCodeAsync(string username, bool force,
            IConfrimCodesRepository confrimCodesRepository,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByNameAsync(username);

            if (force) goto CreateNew;

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
            // Too overheading but more random
            string a = random.Next(1, 10).ToString();
			string b = random.Next(0, 10).ToString();
			string c = random.Next(0, 10).ToString();
			string d = random.Next(0, 10).ToString();
			string f = random.Next(0, 10).ToString();
			int confirmNumber = Int32.Parse(a + b + c + d + f);

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
				return Json(JsonSuccess());
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
				ApplicationUser appUser = (ApplicationUser)user;

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
        public object GetUsers(UserManager<ApplicationUser> userManager)
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
        public async Task<object> CheckIfEmailExists(string email, IUserRepository userRepository)
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
        public async Task<object> CheckIfNicknameExists(string nickname, IUserRepository userRepository)
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

        [HttpPost]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("api/account/sendrecoveryemail")]
		public async Task<object> SendRecoveryEmail(string username, string email, string locale,
            IPasswordRecoveryTokensRepository passwordRecoveryTokens,
            IPasswordHasher<ApplicationUser> passwordHasher,
            [FromServices] IServer server, 
            [FromServices] ILocalizationService localization,
            UserManager<ApplicationUser> userManager)
        {
            // TODO: Add protection from the case when user can 
            // request a recovery code multiple times for different mails and thus 
            // stuff up the database
            // 1. add checking if mail belongs to user with exact username entered on phase 1
            // 2. usual request count limitations
            // 3. point 2 but add IP checking as well

            ApplicationUser user = await userManager.FindByNameAsync(username);

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

        [HttpGet]
        [Route("api/account/refreshtoken")]
        public async Task<object> RefreshToken(string fingerPrint,
            [FromServices] ApplicationIdentityDbContext identityDbContext)
        {
            string rsUuid = HttpContext.Request.Cookies["refreshToken"];

            if (String.IsNullOrEmpty(rsUuid))
            {
                return Unauthorized();
            }

            RefreshSession lastRs = identityDbContext.RefreshSessions
                .Include(x => x.User)
                .AsEnumerable()
                .SingleOrDefault(x => x.Token == rsUuid);

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

            ApplicationUser user = lastRs.User;

            int expTs = DateTimeUtils.ToUnixTimeSeconds(
                DateTime.UtcNow.Add(RefreshSession.LifeTime)
            );
            RefreshSession newRefreshSession = new RefreshSession
            {
                // TODO: Replace with something more efficeint
                Token = Guid.NewGuid().ToString().Replace("-", ""),
                Fingerprint = fingerPrint,
                ExpiresAt = expTs,
                User = user
            };
            Console.WriteLine(newRefreshSession.Token);

            identityDbContext.RefreshSessions.Remove(lastRs);

            await identityDbContext.RefreshSessions.AddAsync(newRefreshSession);

            await identityDbContext.SaveChangesAsync();

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.GetUsername())
            };
            string jwt = CreateJWT(user.GetUsername(), claims).Token;

            HttpContext.Response.Headers.Append("Set-Cookie", $"refreshToken='{newRefreshSession.Token}';");

            return Json(new { accessToken = jwt, refreshToken = newRefreshSession.Token });
        }

        // TODO: Make this [Authorize]
        //[Authorize]
        [HttpGet]
        [Route("api/account/checkPassword")]
        public async Task<object> CheckPassword(string password,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromServices] IUserRepository users)
        {
            // since we require authorization we alread have username from token
            // otherwise this method won't be called
            string username = User.Identity.Name;
            IUser user = users.One(x => x.GetUsername() == username);

            var result = await signInManager.PasswordSignInAsync((ApplicationUser)user, password, false, false);

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
            [FromServices] IPasswordHasher<ApplicationUser> passwordHasher,
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
            [FromServices] IPasswordHasher<ApplicationUser> passwordHasher,
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

            return Unauthorized(new { message = "Token is outdated" });
        }

        private JsonResult EditPasswordCore(string username, string newPassword, IPasswordHasher<ApplicationUser> passwordHasher, IUserRepository users)
        {
            ApplicationUser user = (ApplicationUser)users.One(x => x.GetUsername() == username);

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

        public enum RegisterType : sbyte
        {
            ViewerUser = 2,
            BroadcastUser = 1
        }
    }
}
