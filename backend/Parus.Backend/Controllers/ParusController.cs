using Microsoft.AspNetCore.Mvc;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;

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

        // TODO: Format appropriate to 'API Best Practices'
		protected JsonResult JsonSuccess()
		{
			return Json(new { success = "Y" });
		}

        // TODO: Format appropriate to 'API Best Practices'
        protected JsonResult JsonFail()
        {
            return Json(new { success = "N" });
        }
    }
}
