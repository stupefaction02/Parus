using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Parus.Infrastructure.Identity;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Parus.WebUI.Middlewares
{
    public class IpRestrictionRuleEntry 
    {
        public static explicit operator IpRestrictionRuleEntry(IPAddress address)
            => new IPAddressRuleEntry() { Address = address };
    }

    public class IPAddressRuleEntry : IpRestrictionRuleEntry
    {
        public IPAddress Address { get; set; }
    }

    public class IPv4Range : IpRestrictionRuleEntry
    {
        public Tuple<byte, byte> Octet1 { get; set; }
        public Tuple<byte, byte> Octet2 { get; set; }
        public Tuple<byte, byte> Octet3 { get; set; }
        public Tuple<byte, byte> Octet4 { get; set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

    }

    public abstract class ConnectionRestrictionRule
    {
        public abstract bool Check(HttpContext httpContext);
    }

    public class AllowOnlyIpRestrictionRule : IpRestrictionRule
    {
        public AllowOnlyIpRestrictionRule(List<IPAddress> addresses) : base(addresses)
        {
        }

        public override bool Check(HttpContext httpContext)
        {
            IPAddress iPAddress = httpContext.Connection.RemoteIpAddress;
            
            return Addresses.Any(x => x.Equals(iPAddress));
        }
    }

    public class IpRestrictionRule : ConnectionRestrictionRule
    {
        public string FailReason { get; set; }
        public string SuccessReason { get; set; }

        protected List<IPAddress> Addresses { get; set; }

        public IpRestrictionRule(List<IPAddress> addresses)
        {
            Addresses = addresses;
        }

        public override bool Check(HttpContext httpContext)
        {
            return true;
        }
    }

    public class ConnectionRestrictionBuilder
    {
        private readonly List<ConnectionRestrictionRule> _rules = new List<ConnectionRestrictionRule>();

        public void AddRule(ConnectionRestrictionRule rule)
        {
            _rules.Add(rule);
        }
        
        public IEnumerable<ConnectionRestrictionRule> Build()
        {
            return _rules;
        }
    }

    public class ConnectionRestrictionMiddlewareOptions
    {
        public IEnumerable<ConnectionRestrictionRule> Rules { get; set; }
    }

    public static class ConnectionRestrictionMiddlewareExtensions
    {
        public static IApplicationBuilder UseConnectionRestriction(
        this IApplicationBuilder app,
        Action<ConnectionRestrictionBuilder> configurePolicy)
        {
            ArgumentNullException.ThrowIfNull(app);
            ArgumentNullException.ThrowIfNull(configurePolicy);

            var policyBuilder = new ConnectionRestrictionBuilder();
            configurePolicy(policyBuilder);

            var rules = new ConnectionRestrictionMiddlewareOptions
            {
                Rules = policyBuilder.Build()
            };

            var options = Options.Create<ConnectionRestrictionMiddlewareOptions>(rules);

            return app.UseMiddleware<ConnectionRestrictionMiddleware>(options);
        }
    }

	public class ConnectionRestrictionMiddleware
    {
        private readonly RequestDelegate _next;
		private readonly IServiceProvider serviceProvider;
        private readonly ILogger<DebugMiddleware> logger;
        private readonly IEnumerable<ConnectionRestrictionRule> rules;

        public ConnectionRestrictionMiddleware(RequestDelegate next, IServiceProvider serviceProvider, ILogger<DebugMiddleware> logger, IOptions<ConnectionRestrictionMiddlewareOptions> rules)
        {
            _next = next;
			this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.rules = rules.Value.Rules;
        }

		public Task Invoke(HttpContext httpContext)
		{
            foreach (ConnectionRestrictionRule rule in rules)
            {
                if (!rule.Check(httpContext))
                {
                    httpContext.Response.StatusCode = 403;

                    httpContext.Response.WriteAsync("your ip is forbidden.");

                    //Console.WriteLine($"{h}");

                    return Task.CompletedTask;
                }
            }

			return _next(httpContext);
		}

    }
}