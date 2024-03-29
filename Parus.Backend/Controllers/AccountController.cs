﻿using Google.Authenticator;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
                .TwoFactoryVerificationCodes
                    .SingleOrDefault(x => x.UserId == user.GetId());

            // if alread exists
            if (addedCode != null)
            {
                bool expired = false;

                // exceptional case, expired code must've been already deleted
                if (expired)
                {
                    // here we must lock thread 
                    context.TwoFactoryVerificationCodes.Remove(user.TwoFAEmailVerificationCode);

                    // proceed to create new one
                }
                // otherwise user will user old code
                else { return Accepted($"{user.UserName} already has a token."); }
            }

            // Too overheading but more random
            int code = Parus.Core.Utils.CodesUtils.RandomizeEmailVerificatioCode();

            context.TwoFactoryVerificationCodes
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

                return Json( CreateJsonError(errorInfo) );
            }

            var codeEntry = context.TwoFactoryVerificationCodes.SingleOrDefault
                (x => x.UserId == user.GetId());

            if (codeEntry == null)
            {
                return NotFound(new { error = "Code is expired." });
            }

            int exprectedCode = codeEntry.Code;

            if (exprectedCode == code)
            {
                ApplicationUser appUser = (ApplicationUser)user;

                context.TwoFactoryVerificationCodes.Remove(codeEntry);

                await context.SaveChangesAsync();

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
        // TODO: Pull from configuration 
        // set up during server setup
        private readonly static int googleCodeLength = 6;

        [HttpPost]
        [Route("api/account/2FA/verify2FACode")]
        public async Task<JsonResult> Verify2FACode(int code, string customerKey, ApplicationIdentityDbContext context)
        {
            // 5 levels of security :)
            if (User.Identity.IsAuthenticated)
            {
                if (customerKey.Length == uidLegnth)
                {
                    TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();

                    string codeStr = code.ToString();
                    if (codeStr.Length == googleCodeLength)
                    {
                        if (twoFactor.ValidateTwoFactorPIN(customerKey, codeStr, TimeSpan.FromSeconds(30)))
                        {
                            var appUser = context.Users.SingleOrDefault(x => x.UserName == User.Identity.Name);

                            if (appUser == null)
                            {
                                HttpContext.Response.StatusCode = 401;
                                return Json(new { success = "N", error = "Couldn't find user with this username." });
                            }

                            appUser.TwoFactorEnabled = true;

                            context.Users.Update(appUser);

                            string userId = appUser.Id;

                            var existedCustomerKey = context.TwoFactoryCustomerKeys.SingleOrDefault(x => x.UserId == userId);
                            if (existedCustomerKey != null)
                            {
                                existedCustomerKey.Key = customerKey;
                                context.Update(existedCustomerKey);
                            }
                            else
                            {
                                await context.TwoFactoryCustomerKeys.AddAsync(
                                    new TwoFactoryCustomerKey() { Key = customerKey, UserId = userId });
                            }

                            await context.SaveChangesAsync();

                            Console.WriteLine($"2FA was enabled for user {appUser.GetUsername()}");

                            HttpContext.Response.StatusCode = 200;
                            return Json(new { success = "Y" });
                        }
                    }
                }
            }

            HttpContext.Response.StatusCode = 401;
            return Json(new { success = "N", error = "Forbidden." });
        }

        [HttpPut]
        [Route("api/account/2FA/disable")]
        public async Task<JsonResult> Disable2FA(int code, ApplicationIdentityDbContext context)
        {
            if (User.Identity.IsAuthenticated)
            {
                // TODO: Database.Users.IdEmail
                var appUser = context.Users
                    .Include(x => x.CustomerKey)
                    .SingleOrDefault(x => x.UserName == User.Identity.Name);

                if (appUser == null)
                {
                    HttpContext.Response.StatusCode = 404;
                    return Json(new { success = "N", error = "Couldn't find user with this username." });
                }

                var customerKey = appUser.CustomerKey;
                if (customerKey == null)
                {
                    HttpContext.Response.StatusCode = 500;
                    return Json(new { success = "N", error = "Server Error. Contact the Webmaster." });
                }

                TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();

                string codeStr = code.ToString();
                if (codeStr.Length == googleCodeLength)
                {
                    if (twoFactor.ValidateTwoFactorPIN(customerKey.Key, codeStr, TimeSpan.FromSeconds(30)))
                    {
                        appUser.TwoFactorEnabled = false;

                        context.Users.Update(appUser);

                        Console.WriteLine($"2FA was disabled for user {appUser.GetUsername()}");

                        await context.SaveChangesAsync();

                        HttpContext.Response.StatusCode = 200;
                        return Json(new { success = "Y" });
                    }
                }
            }

            HttpContext.Response.StatusCode = 401;
            return Json(new { success = "N", error = "Forbidden." });
        }
    }
}