using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Naturistic.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

using NETCore.MailKit.Core;
using Org.BouncyCastle.Ocsp;

public class IdentityController : Controller
{
    private readonly ILogger<IdentityController> logger;
    private readonly EmailService emailService;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly UserManager<ApplicationUser> userManager;

    public IdentityController(ILogger<IdentityController> logger, EmailService emailService,
        SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
		this.logger = logger;
        this.emailService = emailService;
        this.signInManager = signInManager;
        this.userManager = userManager;
    }

    public IActionResult Index()
    {
        return View("Views/Index.cshtml");
    }

    [Authorize]
    public void Secret()
    {
        Console.WriteLine("Secret page...");
    }

    public async Task Authenticate()
    {
        // Creating user, but it's not like an entity user
        // This user will be based on Cookies claims

        var customClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Ivan"),
                new Claim(ClaimTypes.Email, "ivan@test.net")
            };

        var claimsIdentity = new ClaimsIdentity(customClaims, "Ivan Identity");

        var userPrincipal = new ClaimsPrincipal(new[] { claimsIdentity });

        // With Cookie Auth Scheme it creates cookeie configured in this scheme
        await HttpContext.SignInAsync(userPrincipal);
    }

    public IActionResult Register() => View("Views/Identity/Register/Index.cshtml");

    public IActionResult Login() => View("Views/Identity/Login/Index.cshtml");

    public IActionResult EmailVerification() => View("Views/Identity/EmailVerification.cshtml");

    [HttpPost]
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

        if (created.Succeeded)
        {
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

            var link = Url.Action(nameof(VerifyEmail), "Identity", new { userId = user.Id, code = code }, Request.Scheme.ToString(), Request.Host.ToString());

            await emailService.SendAsync(user.Email, "Naturistic Email Verification", $"<a href={link}>Verify Email</a>", true);

            return RedirectToAction("EmailVerification");
        }

        return StatusCode(500);
    }

    public async Task<IActionResult> VerifyEmail(string userId, string code)
    {
        var user = await userManager.FindByIdAsync(userId);
        var result = await userManager.ConfirmEmailAsync(user, code);

        if (user == null)
            return BadRequest();

        if (result.Succeeded)
            return View("Views/Identity/VarifyedEmail.cshtml");

        return BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user != null)
        {
            var signInResult = await signInManager.PasswordSignInAsync(user.UserName, password, false, false);

            if (signInResult.Succeeded)
                return RedirectToAction("Index");
        }

        return StatusCode(500);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        // TODO: Back url
        return RedirectToAction("Index");
    }
}