using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

public class EnableRouteResponseCompressionAttribute : MiddlewareFilterAttribute
{
    public EnableRouteResponseCompressionAttribute()
        : base(typeof(EnableRouteResponseCompressionAttribute))
    { }

    public void Configure(IApplicationBuilder applicationBuilder)
        => applicationBuilder.UseResponseCompression();
}