using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Parus.Core.Interfaces.Services;

namespace Parus.Core.Services
{
	public class MailKitEmailService : IEmailService
	{
		string _template = "<body> {body} </body>";

        public MailKitEmailService(MailSettings settings)
        {
            Settings = settings;
        }

        public MailSettings Settings { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<EmailResponse> SendEmailAsync(string email, string subject, string text)
		{
			//return new EmailResponse { Success = true };
			using var emailMessage = new MimeMessage();

			emailMessage.From.Add(new MailboxAddress("Администрация сайта", "ivan.safonow2012@yandex.ru"));
			emailMessage.To.Add(new MailboxAddress(email, email));
			emailMessage.Subject = subject;
			// TODO: Format with html template
			emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{
				Text = _template.Replace("{body}", text)
			};

			string reponse;
			using (var client = new SmtpClient())
			{
				await client.ConnectAsync("smtp.yandex.ru", 25, false);
				await client.AuthenticateAsync("login@yandex.ru", "password");
				reponse = await client.SendAsync(emailMessage);

				await client.DisconnectAsync(true);
			}

			return new EmailResponse { Success = true };
		}
	}
}
