using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Parus.Core.Entities;
using Parus.Infrastructure.Identity;

namespace Parus.WebUI.Services
{ 
    public interface IApiClient
    {
        Task<object> LoginAsync(string nickname, string password);
		Task<object> LoginJwtAsync(string nickname, string password);
		Task<object> RegisterAsync(string nickname, string firstname, string lastname, string email, string password);
    }
}
