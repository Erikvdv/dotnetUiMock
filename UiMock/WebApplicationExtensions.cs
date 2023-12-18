using System.Reflection;
using DotnetUiMock.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace DotnetUiMock;

public static class WebApplicationExtensions
{
    public static void UseUiMock(this IServiceCollection services)
    {
        var mockedServices = Assembly.GetEntryAssembly()?.GetTypes()
            .Where(x => x.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMockService<>)))
            .ToList();
        if (mockedServices != null)
            foreach (var mockedServiceType in mockedServices)
            {
                var interfaceType = mockedServiceType.GetInterfaces().First().GetGenericArguments().First()
                    .UnderlyingSystemType;
                var mockedService = Activator.CreateInstance(mockedServiceType) as IMockService;

                if (mockedService.GetType().FullName != "Castle.Proxies.ObjectProxy")
                    continue;
            }

        services.AddSingleton<UiMockService>();
    }

    public static void UseMockUi(this WebApplication app)
    {
        List<ServiceMocks> serviceMocksList = [];
        var mockedServices = Assembly.GetEntryAssembly()?.GetTypes()
            .Where(x => x.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMockService<>)))
            .ToList();
        using var scope = app.Services.CreateScope();
        if (mockedServices != null)
            foreach (var mockedServiceType in mockedServices)
            {
                var interfaceType = mockedServiceType.GetInterfaces().First().GetGenericArguments().First()
                    .UnderlyingSystemType;
                var mockedService = scope.ServiceProvider.GetRequiredService(interfaceType);

                if (mockedService.GetType().FullName != "Castle.Proxies.ObjectProxy")
                    continue;
                var mockedServiceInstance = Activator.CreateInstance(mockedServiceType) as IMockService;
                serviceMocksList.Add(mockedServiceInstance.ServiceMocks);
            }

        var uiService = scope.ServiceProvider.GetRequiredService<UiMockService>();
        uiService.ServiceMocksList = serviceMocksList;
        uiService.SetDefaults();


        app.MapUiMockEndpoints();

        app.UseDefaultFiles();

        var embeddedFileProvider = new EmbeddedFileProvider(
            Assembly.GetExecutingAssembly(),
            "DotnetUiMock.StaticAssets"
        );
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = embeddedFileProvider,
            RequestPath = new PathString("/uimock/static")
        });
    }
}