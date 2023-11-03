using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Parus.Backend.Middlewares
{
    public class SignalRCorsMiddleware
    {
        private readonly RequestDelegate _next;
        
           public SignalRCorsMiddleware(RequestDelegate next)
           {
              _next = next;
           }
        
           public Task Invoke(HttpContext httpContext)
           {
			    httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
			    httpContext.Response.Headers.Add("Access-Control-Allow-Headers", "x-requested-with");
			    httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,OPTIONS");
			    httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "https://localhost:5002");

			    //if (httpContext.Request.Method == "OPTIONS")
			    //{
				   // httpContext.Response.StatusCode = 204;
				   // return httpContext.Response.WriteAsync(string.Empty);
			    //}

			    return _next(httpContext);
           }
    }
}