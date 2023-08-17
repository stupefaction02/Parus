using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Naturistic.Core.Interfaces.Services;

namespace Naturistic.Core.Services
{
	public class MailKitEmailService : IEmailService
	{
		public async Task SendEmailAsync(string email, string subject, string body)
		{
			using var emailMessage = new MimeMessage();

			emailMessage.From.Add(new MailboxAddress("Администрация сайта", "ivan.safonow2012@yandex.ru"));
			emailMessage.To.Add(new MailboxAddress("flashdancer", email));
			emailMessage.Subject = subject;
			emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{
				Text = body
			};

			using (var client = new SmtpClient())
			{
				await client.ConnectAsync("smtp.yandex.ru", 25, false);
				await client.AuthenticateAsync("login@yandex.ru", "password");
				await client.SendAsync(emailMessage);

				await client.DisconnectAsync(true);
			}
		}
	}
}
