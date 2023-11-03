using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Parus.Core.Interfaces.Services
{
    public interface IEmailService
    {
        public Task<EmailResponse> SendEmailAsync(string email, string subject, string body);
    }

    public class EmailResponse
    {
        public bool Success { get; set; }

        public string Mssage { get; set; }
    }
}
