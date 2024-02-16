using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

                    // go to create new one
                    goto CreateNew;
                }
                // otherwise user will user old code
                return CreateJsonError($"{user.UserName} already has token.");
            }

        CreateNew:
            // Too overheading but more random

            int code = Parus.Core.Utils.CodesUtils.RandomizeEmailVerificatioCode();

            context.TwoFAVerificationCodes
                .Add(new TwoFactoryEmailVerificationCode { Code = code, User = user, UserId = user.Id });

            //logger.LogInformation($"Creating confirmation code with numbers {confirmNumber} for user: {username}");

            string emailBody = "";
            var emailResponse = await emailService.SendEmailAsync(user.Email, "Email Verification", emailBody);

            if (emailResponse.Success)
            {
                return Json(CreateJsonSuccess());
            }

            return Json(CreateJsonError(emailResponse.Mssage));
        }
    }
}