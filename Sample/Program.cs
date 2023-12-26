using System.Reflection;
using DotnetUiMock;
using Sample.Core.Services;
using sample.Mocks;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IWeatherService, WeatherService>();

#if DEBUG // typically you don't want your mocks in production
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IWeatherService>(provider => new WeatherServiceMock().GenerateService(provider));
    //builder.Services.AddUiMock(); // by default, it will use the executing assembly
    builder.Services.AddUiMock([Assembly.GetAssembly(typeof(WeatherServiceMock))]); 
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
#if DEBUG // typically you don't want your mocks in production
app.UseMockUi();
#endif

app.Run();



