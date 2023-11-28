using Microsoft.AspNetCore.Mvc;
using System;

namespace Parus.Backend.Controllers
{
    public class ParusController : Controller
    {

        protected void LogInfo_Debug(string message)
        {
#if DEBUG
            Console.WriteLine(message);    
#endif
        }
    }
}
