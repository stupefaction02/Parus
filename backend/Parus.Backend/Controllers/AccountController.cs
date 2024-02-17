using Google.Authenticator;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using Parus.Core.Interfaces.Services;
using Parus.Infrastructure.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Parus.Backend.Controllers
{
    [ApiController]
	public class AccountController : ParusController
	{
        private object CreateJsonError(string message)
        {
            return new { success = "N", message = message };
        }

        [HttpPost]
        [Route("api/account/2FA/request2FAVerificationEmailCode")]
        public async Task<object> Request2FAVerificationEmailCode(
            [FromServices] IUserRepository users,
            IEmailService emailService,
            [FromServices] ApplicationIdentityDbContext context)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "Forbidden." });
            }

            ApplicationUser user = (ApplicationUser)users.One(x => x.GetUsername() == User.Identity.Name);

            TwoFactoryEmailVerificationCode addedCode = (TwoFactoryEmailVerificationCode)context
                .TwoFAVerificationCodes
                    .SingleOrDefault(x => x.UserId == user.GetId());

            // if alread exists
            if (addedCode != null)
            {
                bool expired = false;

                // exceptional case, expired code must've been already deleted
                if (expired)
                {
                    // here we must lock thread 
                    context.TwoFAVerificationCodes.Remove(user.TwoFAEmailVerificationCode);

                    // proceed to create new one
                }
                // otherwise user will user old code
                else { return Accepted($"{user.UserName} already has a token."); }
            }

            // Too overheading but more random
            int code = Parus.Core.Utils.CodesUtils.RandomizeEmailVerificatioCode();

            context.TwoFAVerificationCodes
                .Add(new TwoFactoryEmailVerificationCode { Code = code, User = user, UserId = user.Id });

            Console.WriteLine($"Creating confirmation code with numbers {code} for user: {User.Identity.Name}");

            string emailBody = "";
            var emailResponse = await emailService.SendEmailAsync(user.Email, "Email Verification", emailBody);

            if (emailResponse.Success)
            {
                context.SaveChanges();

                return Json(CreateJsonSuccess());
            }

            return Json(CreateJsonError(emailResponse.Mssage));
        }

        [NonAction]
        public JsonResult GetTwoFactorAuthenticationData(string applicationName, ApplicationUser user)
        {
            // we literally pull the whole user to solely get email
            // We could manually send command (select email from users) and that's it
            
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();

            // We need a unique PER USER key to identify this Setup
            // must be saved: you need this value later to verify a validation code
            string customerSecretKey = Guid.NewGuid().ToString();

            SetupCode setupInfo = twoFactor.GenerateSetupCode(
                // name of the application - the name shown in the Authenticator
                applicationName,
                // an account identifier - shouldn't have spaces
                user.Email,
                // the secret key that also is used to validate later
                customerSecretKey,
                // Base32 Encoding (odd this was left in)
                false,
                // resolution for the QR Code - larger number means bigger image
                10);

            return Json(new
            {
                success = "Y",
                str_key = setupInfo.ManualEntryKey,
                qr_image = setupInfo.QrCodeSetupImageUrl,
                customer_key = customerSecretKey
            });
        }

        // var requestHandleTask = HandleRequestAsync(params);
        // doing things
        // var requestHandleTaskResult = requestHandleTask.Wait().Result;

        [HttpPost]
        [Route("api/account/2FA/verify")]
        public async Task<object> VerifyAccount(int code,
            [FromServices] IUserRepository users,
            [FromServices] ILogger<AccountController> logger,
            [FromServices] ApplicationIdentityDbContext context,
            [FromServices] IConfiguration configuration)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "Forbidden." });
            }
            
            string username = User.Identity.Name;

            IUser user = users.One(x => x.GetUsername() == username);

            if (user == null)
            {
                string errorInfo = $"Something went wrong! Couldn't find user with email = {username}";

                logger.LogInformation(errorInfo);

                return Json(CreateJsonError(errorInfo));
            }

            var codeEntry = context.TwoFAVerificationCodes.SingleOrDefault
                (x => x.UserId == user.GetId());

            if (codeEntry == null)
            {
                return NotFound(new { error = "Code is expired." });
            }

            int exprectedCode = codeEntry.Code;

            if (exprectedCode == code)
            {
                ApplicationUser appUser = (ApplicationUser)user;

                return GetTwoFactorAuthenticationData(configuration["ApplicationName"], appUser);
            }
            else
            {
                string errorInfo = $"Number required for {username} confirmation are wrong! Waiting for client to send right number.";

                logger.LogInformation(errorInfo);

                return BadRequest(new { error = errorInfo });
            }
        }

        // Guid.New() generates 36 chars
        private const int uidLegnth = 36;

        [HttpPost]
        [Route("api/account/2FA/verify2FACode")]
        public JsonResult Verify2FACode(int code, string key)
        {
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();

            if (key.Length == uidLegnth)
            {
                string codeStr = code.ToString();
                if (twoFactor.ValidateTwoFactorPIN(key, codeStr, TimeSpan.FromSeconds(30)))
                {
                    HttpContext.Response.StatusCode = 200;
                    return Json(new { success = "Y" });
                }
            }

            HttpContext.Response.StatusCode = 401;
            return Json(new { success = "N", error = "Forbidden." });
        }
    }
}