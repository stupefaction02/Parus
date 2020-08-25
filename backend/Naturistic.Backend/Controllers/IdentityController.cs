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

using Microsoft.AspNetCore.Identity;

using Microsoft.AspNet.Identity.EntityFramework;

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

        public IConfiguration Configuration { get; }

        public IdentityController(IWebHostEnvironment hostEnviroment, UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ILogger<IdentityController> logger)
        {
            this.hostEnviroment = hostEnviroment;
            Configuration = configuration;
			this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        [Route("api/test")]
        public object Test()
        {
            return "Test";
        }

        [HttpPost]
        [Route("api/login")]
        public async Task<object> Login(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var signInResult = await signInManager.PasswordSignInAsync(user.UserName, password, false, false);

                if (signInResult.Succeeded)
                    return StatusCode(200, new { Success = true, Message = "Login was successful" });
            }
             
            return StatusCode(500, new { Success = false, Message = "Server Error!" });
        }

        [HttpPost]
        [Route("api/register")]
		public async Task<object> Register(string nickname, string lastname, string firstname, string email, string password)
		{
			Console.WriteLine($"User to register: {firstname} : {email}");

            var user = new ApplicationUser
            {
                Nickname = nickname,
                FirstName = firstname,
                LastName = lastname,
                Email = email
            };

            var created = await userManager.CreateAsync(user, password);
            
			return null;
		}
    }
}
