using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Naturistic.Backend.Authentication
{
	public class BearerAuthenticationOptions : AuthenticationOptions, IOptions<BearerAuthenticationOptions>
	{
		public BearerAuthenticationOptions Value { get; }
	}
}