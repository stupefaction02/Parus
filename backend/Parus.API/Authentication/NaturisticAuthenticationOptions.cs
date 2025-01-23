using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Parus.API.Authentication
{
	public class BearerAuthenticationOptions : AuthenticationOptions, IOptions<BearerAuthenticationOptions>
	{
		public BearerAuthenticationOptions Value { get; }
	}
}