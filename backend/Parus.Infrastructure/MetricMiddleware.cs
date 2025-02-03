using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Parus.Infrastructure.Identity;
using static System.Formats.Asn1.AsnWriter;

namespace Parus.Infrastructure.Middlewares
{
    public class MetricService
    {
        private int requestsCount;

        public int RequestsCount { get { Thread.Sleep(1000); return requestsCount; } set => requestsCount = value; }
    }

    public class MetricMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<MetricMiddleware> logger;
        private readonly MetricService metrics;

        public MetricMiddleware(RequestDelegate next, 
            IServiceProvider serviceProvider, 
            ILogger<MetricMiddleware> logger,
            MetricService metrics)
        {
            _next = next;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.metrics = metrics;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Interlocked.Increment(metrics.RequestsCount);

            try
            {
                await _next(httpContext);
            }
            finally
            {
                metrics.RequestsCount -= 1;
            }

            logger.LogInformation($"Total requests: {metrics.RequestsCount}");
        }
    }
}