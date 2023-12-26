using System.Security.Claims;
using DotnetUiMock;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NSubstitute;

namespace Sample.Mocks;

public interface IAuthenticationMock
{
    public bool CookieAuthMock { get; set; }
    public AuthenticateResult GetCookieAuthResult();
}

public class AuthenticationMock : BaseMockService<IAuthenticationMock>
{
    
    public AuthenticationMock()
    {
        MethodMocks =
        [
            new("EnableCookieAuthMock",
            [
                new MockScenario("enabled", EnableCookieAuthMock),
                new MockScenario("disabled", DisableCookieAuthMock)
            ]),
            new("CookieAuthResultMock",
            [
                new MockScenario("user1", CookieAuthUser1),
                new MockScenario("user2", CookieAuthUser2),
                new MockScenario("failure", CookieAuthFailure)
            ])
        ];
    }

    private static void EnableCookieAuthMock(object service, int delay)
    {
        (service as IAuthenticationMock)?.CookieAuthMock.Returns(true);
    }
    
    private static void DisableCookieAuthMock(object service, int delay)
    {
        (service as IAuthenticationMock)?.CookieAuthMock.Returns(false);
    }
    
    private static void CookieAuthUser1(object service, int delay)
    {
        (service as IAuthenticationMock)?.GetCookieAuthResult().Returns(_ =>
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "Test user") };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, CookieAuthenticationDefaults.AuthenticationScheme);
            return AuthenticateResult.Success(ticket);
        });
    }
    
    private static void CookieAuthUser2(object service, int delay)
    {
        (service as IAuthenticationMock)?.GetCookieAuthResult().Returns(_ =>
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "Test  2") };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, CookieAuthenticationDefaults.AuthenticationScheme);
            return AuthenticateResult.Success(ticket);
        });
    }
    
    private static void CookieAuthFailure(object service, int delay)
    {
        (service as IAuthenticationMock)?.GetCookieAuthResult().Returns(_ =>
        {
            return AuthenticateResult.Fail("Invalid cookie");
        });
    }
    
}