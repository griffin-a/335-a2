using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using A2.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace A2.Handler;

public class A2AuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IA2Repo _repository;
    public A2AuthHandler( 
        IA2Repo repository,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _repository = repository;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            Response.Headers.Add("WWW-Authenticate", "Basic");
            return Task.FromResult(AuthenticateResult.Fail("Authorization header not found."));
        }

        var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
        if (authHeader.Parameter == null) return Task.FromResult(AuthenticateResult.Fail("userName and password do not match"));
        var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(":");
        var username = credentials[0];
        var password = credentials[1];

        if (!_repository.ValidLogin(username, password))
            return Task.FromResult(AuthenticateResult.Fail("userName and password do not match"));
        var claims = new[] { new Claim("userName", username) };

        ClaimsIdentity identity = new ClaimsIdentity(claims, "Basic");
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

        AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}