using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Sample.Mocks;

public class MockCookieAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IAuthenticationMock _authMock;

    public MockCookieAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IAuthenticationMock authMock)
        : base(options, logger, encoder, clock)
    {
        _authMock = authMock;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var result = _authMock.GetCookieAuthResult();
        return Task.FromResult(result);
    }
    
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        return Task.CompletedTask;
    }
}