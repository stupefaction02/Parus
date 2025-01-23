using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Parus.Infrastructure.Extensions
{
    public static class HostingExtensions
    {
        public static bool IsAnyDevelopment(this IHostingEnvironment env)
        {
            if (env.IsEnvironment("Development_Localhost"))
            {
                return true;
            }
            else if (env.IsDevelopment())
            {
                return true;
            }

            return false;
        }
    }
}
