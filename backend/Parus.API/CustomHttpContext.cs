using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Primitives;
using System.IO;
using System.Threading.Tasks;

namespace Parus.API
{
    public class TestHttpResponse : HttpResponse
    {
        public override HttpContext HttpContext { get; }
        public override int StatusCode { get; set; }
        public override IHeaderDictionary Headers { get => headers; }
        public override Stream Body { get; set; }
        public override long? ContentLength { get; set; }
        public override string? ContentType { get; set; }
        public override IResponseCookies Cookies { get; } = new TestReponseCookieCollection();
        public override bool HasStarted { get; }

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
        }

        public override void OnStarting(Func<object, Task> callback, object state)
        {
        }

        public override void Redirect(string location, bool permanent)
        {
        }

        public void AddCookie(string key, string value)
        {
            Cookies.Append(key, value);
        }

        TestHeadersCollection headers = new TestHeadersCollection();
        public void AddHeader(string key, string value)
        {
            headers.Add(key, value);
        }
    }

    public class TestHttpRequest : HttpRequest
    {
        #region Useless
        public override HttpContext HttpContext { get; }
        public override string Method { get; set; }
        public override string Scheme { get; set; }
        public override bool IsHttps { get; set; }
        public override HostString Host { get; set; }
        public override PathString PathBase { get; set; }
        public override PathString Path { get; set; }
        public override QueryString QueryString { get; set; }
        public override IQueryCollection Query { get; set; }
        public override string Protocol { get; set; }

        public override long? ContentLength { get; set; }
        public override string? ContentType { get; set; }
        public override Stream Body { get; set; }
        public override bool HasFormContentType { get; }
        public override IFormCollection Form { get; set; }

        public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default)
        {
            return null;
        }

        #endregion

        public override IHeaderDictionary Headers { get => headers; }


        public override IRequestCookieCollection Cookies
        {
            get => cookies; set { }
        }

        TestCookieCollection cookies;
        TestHeadersCollection headers = new TestHeadersCollection();
        public TestHttpRequest()
        {
            cookies = new TestCookieCollection();
        }

        public void AddCookie(string key, string value)
        {
            cookies.Add(key, value);
        }

        public void AddHeader(string key, string value)
        {
            headers.Add(key, value);
        }
    }

    public class TestCookieCollection : IRequestCookieCollection
    {
        private Dictionary<string, string> cookies = new Dictionary<string, string>();
        public string? this[string key] { get => cookies[key]; }

        public void Add(string key, string value)
        {
            cookies.Add(key, value);
        }

        #region Useless
        public int Count { get; }
        public ICollection<string> Keys { get; }

        public bool ContainsKey(string key)
        {
            return false;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return null;
        }

        public bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
        {
            value = "";
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }

        #endregion
    }

    public class TestHeadersCollection : IHeaderDictionary
    {
        public StringValues this[string key]
        {
            get
            {
                StringValues ret;
                TryGetValue(key, out ret);

                return ret;
            }
            set { }
        }

        public long? ContentLength { get; set; }
        public ICollection<string> Keys { get; }
        public ICollection<StringValues> Values { get; }
        public int Count { get; }
        public bool IsReadOnly { get; }

        public void Add(string key, StringValues value)
        {
            keys.Add(key);
            values.Add(value);
        }

        public void Add(KeyValuePair<string, StringValues> item)
        {

        }

        public void Clear()
        {
        }

        public bool Contains(KeyValuePair<string, StringValues> item)
        {
            return default;
        }

        public bool ContainsKey(string key)
        {
            return default;
        }

        public void CopyTo(KeyValuePair<string, StringValues>[] array, int arrayIndex)
        {
        }

        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
        {
            return default;
        }

        public bool Remove(string key)
        {
            return default;
        }

        public bool Remove(KeyValuePair<string, StringValues> item)
        {
            return default;
        }

        List<string> keys = new List<string>();
        List<StringValues> values = new List<StringValues>();

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out StringValues value)
        {
            int index = keys.IndexOf(key);

            if (index != -1)
            {
                value = values[index];

                return true;
            }

            value = default;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return default;
        }
    }


    public class TestReponseCookieCollection : IResponseCookies
    {
        public void Append(string key, string value)
        {
        }

        public void Append(string key, string value, CookieOptions options)
        {
        }

        public void Delete(string key)
        {
        }

        public void Delete(string key, CookieOptions options)
        {
        }
    }

    public class MyServerAddressesFeature : IServerAddressesFeature
    {
        public ICollection<string> Addresses { get; } = new string[2]
        {
                "", ""
        };

        public bool PreferHostingUrls { get; set; }
    }

    public class CustomHttpContexts
    {
        public class CustomHttpContext : HttpContext
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
            CustomHttpContext ctx = new CustomHttpContext();
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

        public static CustomHttpContext CreateContext1(Dictionary<string, string> cookies,
            Dictionary<string, string> headers)
        {
            CustomHttpContext ctx = new CustomHttpContext();
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
}
