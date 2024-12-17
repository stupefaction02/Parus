using Microsoft.Extensions.Options;
using Parus.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parus.Infrastructure.Services
{
    public class ParusMailKitEmailService : MailKitEmailService
    {
        public ParusMailKitEmailService(IOptions<MailSettings> options) : base(options.Value)
        {
            
        }
    }
}
