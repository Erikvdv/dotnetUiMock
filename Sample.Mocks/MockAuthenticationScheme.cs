using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace Sample.Mocks;

public class MockSchemeProvider : AuthenticationSchemeProvider
{
    private readonly IAuthenticationMock _authMock;

    public MockSchemeProvider(IOptions<AuthenticationOptions> options, IAuthenticationMock authMock)
        : base(options)
    {
        _authMock = authMock;
    }

    protected MockSchemeProvider(
        IOptions<AuthenticationOptions> options,
        IDictionary<string, AuthenticationScheme> schemes
    )
        : base(options, schemes)
    {
    }

    public override Task<AuthenticationScheme> GetSchemeAsync(string name)
    {
        if (name == CookieAuthenticationDefaults.AuthenticationScheme && _authMock.CookieAuthMock)
        {
            var scheme = new AuthenticationScheme(
                CookieAuthenticationDefaults.AuthenticationScheme,
                CookieAuthenticationDefaults.AuthenticationScheme,
                typeof(MockCookieAuthenticationHandler)
            );
            return Task.FromResult(scheme);
        }
        
        return base.GetSchemeAsync(name);
    }
}