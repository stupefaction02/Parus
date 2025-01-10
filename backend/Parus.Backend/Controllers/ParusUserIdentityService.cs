using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Parus.Infrastructure.Identity;
using Parus.Core.Interfaces.Repositories;
using Parus.Backend.Authentication;
using Microsoft.AspNetCore.Http;
using Parus.Common.Utils;
using Parus.Core.Authentication;
using Org.BouncyCastle.Ocsp;
using static Parus.Backend.Controllers.IdentityController;
using Microsoft.Extensions.Options;
using Parus.Core.Identity;
using Parus.Infrastructure.Extensions;

namespace Parus.Backend.Controllers
{
    public struct ParusUserIdentityServiceResult
    {
        /// <summary>
        /// Used in controllers as an json response
        /// </summary>
        public object JsonResponse { get; set; }

        public IEnumerable<ParusUser> RegisteredUsers { get; set; }

        public int StatusCode { get; set; }
    }

    public class ParusUserIdentityService
    {
        private readonly UserManager<ParusUser> userManager;
        private readonly IUserRepository userRepository;
        private readonly ParusDbContext identityDbContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IOptions<JwtAuthOptions> authOptions;
        private readonly ILogger<ParusUserIdentityService> logger;

        public ParusUserIdentityService(
            UserManager<ParusUser> userManager,
            IUserRepository userRepository,
            ParusDbContext identityDbContext, 
            IHttpContextAccessor httpContextAccessor, 
            IOptions<JwtAuthOptions> authOptions,
            ILogger<ParusUserIdentityService> logger)
        {
            this.userManager = userManager;
            this.userRepository = userRepository;
            this.identityDbContext = identityDbContext;
            this.httpContextAccessor = httpContextAccessor;
            this.authOptions = authOptions;
            this.logger = logger;
        }

        protected HttpContext HttpContext
        {
            get
            {
                return httpContextAccessor.HttpContext;
            }
        }

        public async Task<ParusUserIdentityServiceResult> RegiserAsync(ParusUserRegistrationJsonDTO model, string[] roles = null)
        {
            ParusUser user = new ParusUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            var created = await userManager.CreateAsync(user, model.Password);
            
            if (created.Succeeded)
            {
                ParusUser createdUsr = (ParusUser)userRepository.FindUserByUsername(model.Username);
                logger.LogInformation(createdUsr.GetUsername());

                //List<Claim> claims = new List<Claim>
                //{
                //    new Claim(ClaimsIdentity.DefaultNameClaimType, user.GetUsername())
                //};

                JwtToken jwt = createdUsr.JwtTokenFromUser(authOptions.Value);

                //ClaimsIdentity identity = new ClaimsIdentity(claims);

                //JwtToken jwt = JwtAuthUtils.CreateDefault(identity, authOptions.Value);
                // TODO: checks if token is already created
                string fingerPrint = HttpContext.Request.Headers.UserAgent;
                RefreshSession refreshSession = RefreshSession.CreateDefault(fingerPrint, user);

                await identityDbContext.RefreshSessions.AddAsync(refreshSession);

                // jwt expires timestamp
                int jwtExpires = DateTimeUtils.ToUnixTimeSeconds(DateTime.Now.Add(JwtAuthOptions1.Lifetime));

                string logRoles = "no roles";
                if (roles != null)
                {
                    foreach (var role in roles)
                    {
                        var rar = await userManager.AddToRoleAsync(createdUsr, role);

                        if (rar.Succeeded)
                        {
                            logRoles += $"{role} ";
                        }
                    }
                }

                logger.LogInformation($"User {model.Username}:{model.Email} has been registered. JWT token: {jwt.Token}, RS token: {refreshSession.Token}, Roles: {logRoles.TrimEnd()}");

                var jsonResponse = new
                {
                    success = "true",
                    access_token = new { jwt = jwt.Token, expires = jwtExpires },
                    refresh_token = new { token = refreshSession.Token, expires = refreshSession.ExpiresAt }
                };

                await identityDbContext.SaveChangesAsync();

                return new ParusUserIdentityServiceResult
                {
                    JsonResponse = jsonResponse,
                    RegisteredUsers = new ParusUser[1] { createdUsr },
                    StatusCode = StatusCodes.Status200OK
                };
            }
            else
            {
                string errors = Environment.NewLine + "\t";
                foreach (var e in created.Errors)
                {
                    errors += e.Description + Environment.NewLine;
                }

                string err = $"Unable to create user: {user}. Errors: {errors}";

                logger.LogError(err);

                var jsonResponse = new { success = "false", };

                return new ParusUserIdentityServiceResult 
                {
                    JsonResponse = jsonResponse,
                    StatusCode = 400
                };
            }
        }
    }
}
