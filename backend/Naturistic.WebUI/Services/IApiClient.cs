using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Naturistic.Core.Entities;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.WebUI.Services
{ 
    public interface IApiClient
    {
        Task<object> LoginAsync(string nickname, string password);
        Task<object> RegisterAsync(string nickname, string firstname, string lastname, string email, string password);
    }
}
