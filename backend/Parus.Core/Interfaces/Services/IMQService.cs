using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Parus.Core.Interfaces.Services
{
    public interface IMQService
    {
        string RequestRefreshTokenAsync();
        void Send();
    }
}
