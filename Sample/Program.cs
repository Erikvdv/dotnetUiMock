using System.Reflection;
using DotnetUiMock;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Sample.Core.Services;
using sample.Mocks;
using Sample.Mocks;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,options =>
    {
        options.Cookie.Name = "__sample";
        options.Cookie.SameSite = SameSiteMode.Strict;
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = "https://demo.duendesoftware.com";

        // confidential client using code flow + PKCE
        options.ClientId = "interactive.confidential";
        options.ClientSecret = "secret";
        options.ResponseType = "code";
        options.ResponseMode = "query";

        options.MapInboundClaims = false;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;

        // request scopes + refresh tokens
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("api");
        options.Scope.Add("offline_access");
    });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    
});
builder.Services.AddScoped<IWeatherService, WeatherService>();


#if DEBUG // typically you don't want your mocks in production
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddTransient<IAuthenticationMock>(provider => new AuthenticationMock().GenerateService(provider));
    builder.Services.AddScoped<IWeatherService>(provider => new WeatherServiceMock().GenerateService(provider));
    builder.Services.AddTransient<IAuthenticationSchemeProvider, MockSchemeProvider>();

    //builder.Services.AddUiMock(); // by default, it will use the executing assembly
    builder.Services.AddUiMock([Assembly.GetAssembly(typeof(WeatherServiceMock))]); 
}
#endif


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


app.UseStaticFiles();
app.UseAntiforgery();



app.MapRazorComponents<sample.Components.App>()
    .AddInteractiveServerRenderMode();
#if DEBUG // typically you don't want your mocks in production
app.UseMockUi();
#endif

app.Run();



