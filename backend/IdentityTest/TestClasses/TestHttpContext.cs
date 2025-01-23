using System.Security.Claims;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityTest
{
    public class MyServerAddressesFeature : IServerAddressesFeature
    {
        public ICollection<string> Addresses { get; } = new string[2]
        {
                "", ""
        };

        public bool PreferHostingUrls { get; set; }
    }

    public class TestContexts
	{
		public class TestHttpContext : HttpContext
		{
			public override IFeatureCollection Features { get; }
			public override HttpRequest Request { get => request; }
			public override HttpResponse Response { get => response; }
			public override ConnectionInfo Connection { get; }
			public override WebSocketManager WebSockets { get; }
			public override ClaimsPrincipal User { get; set; }
			public override IDictionary<object, object?> Items { get; set; }
			public override IServiceProvider RequestServices { get; set; }
			public override CancellationToken RequestAborted { get; set; }
			public override string TraceIdentifier { get; set; }
			public override ISession Session { get; set; }

			public override void Abort()
			{
			}

			TestHttpRequest request;
			TestHttpResponse response;

			public void SetRequest(TestHttpRequest request)
            {
				this.request = request;
			}

			public void SetResponse(TestHttpResponse response)
			{
				this.response = response;
			}
		}

		public static PageContext CreatePageContext(Dictionary<string, string> cookies)
		{
			TestHttpContext ctx = new TestHttpContext();
			TestHttpRequest request = new TestHttpRequest();
			TestHttpResponse response = new TestHttpResponse();

			foreach (var cookie in cookies)
			{
				request.AddCookie(cookie.Key, cookie.Value);
			}

			//foreach (var cookie in cookies)
			//{
			//	response.AddCookie(cookie.Key, cookie.Value);
			//}

			ctx.SetRequest(request);
			ctx.SetResponse(response);

			PageContext pageCtx = new PageContext
			{
				HttpContext = ctx
			};
			return pageCtx;
		}

        public static TestHttpContext CreateContext1(Dictionary<string, string> cookies,
            Dictionary<string, string> headers)
        {
            TestHttpContext ctx = new TestHttpContext();
            TestHttpRequest request = new TestHttpRequest();
            TestHttpResponse response = new TestHttpResponse();

            foreach (var cookie in cookies)
            {
                request.AddCookie(cookie.Key, cookie.Value);
            }

			foreach (var h in headers)
			{
				request.AddHeader(h.Key, h.Value);
			}

			ctx.SetRequest(request);
            ctx.SetResponse(response);

            return ctx;
        }
    }

	public class TestIdentity : ClaimsIdentity
	{
        public TestIdentity(IEnumerable<Claim>? claims) : base(claims)
        {
        }

        public TestIdentity() : base()
        {
            
        }

		public bool _IsAuthenticated = true;
        public override bool IsAuthenticated => _IsAuthenticated;

		
    }
}