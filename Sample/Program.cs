using DotnetUiMock;
using sample.Mocks;
using sample.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IWeatherService, WeatherService>();

#if DEBUG
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IWeatherService>(provider => new WeatherServiceMock().GenerateService(provider));
    builder.Services.UseUiMock();
}
#endif


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();



app.MapRazorComponents<sample.Components.App>()
    .AddInteractiveServerRenderMode();
#if DEBUG
app.UseMockUi();
#endif

app.Run();

namespace sample
{
    public partial class Program
    {

    }
}



