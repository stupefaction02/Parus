using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Naturistic.Core.Interfaces.Services
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string email, string subject, string body);
    }
}
