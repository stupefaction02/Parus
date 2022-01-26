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

        public IdentityController(IWebHostEnvironment hostEnviroment, UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ILogger<IdentityController> logger)
        {
            this.hostEnviroment = hostEnviroment;
            this.configuration = configuration;
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
		public async Task<object> Register(string nickname, string email, string password)
		{
			logger.LogInformation($"User to register: {email}");
            
            var user = new ApplicationUser
            {
                UserName = nickname,
                Email = email
            };

            var created = await userManager.CreateAsync(user, password);

            if (created.Succeeded)
            {
                logger.LogInformation("User registered successfully!");
                return Ok($"{nickname} {email} registered successfully!");
            }
            else
            {
                return null;
            }
		}
    }
}
