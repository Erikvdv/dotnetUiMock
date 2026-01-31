using System.Reflection;
using DotnetUiMock.Endpoints;
using Microsoft.Extensions.FileProviders;


namespace DotnetUiMock;

public static class WebApplicationExtensions
{
    private static Assembly[] UiMockAssemblies { get; set; } = [];

    public static void AddUiMock(this IServiceCollection services, Assembly[]? assemblies = null)
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            assemblies = [Assembly.GetEntryAssembly()!];
        }

        UiMockAssemblies = assemblies;

        services.AddSingleton<UiMockService>();
    }

    public static void UseMockUi(this WebApplication app)
    {
        var mockedServices = GetMockedServicesList(UiMockAssemblies);

        using var scope = app.Services.CreateScope();

        var uiService = scope.ServiceProvider.GetRequiredService<UiMockService>();
        uiService.ServiceMocksList = GetServiceMocks(mockedServices, scope);
        uiService.Init();

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

    private static List<ServiceMocks> GetServiceMocks(List<Type> mockedServices, IServiceScope scope)
    {
        List<ServiceMocks> serviceMocksList = [];
        foreach (var mockedServiceType in mockedServices)
        {
            var interfaceType = mockedServiceType.GetInterfaces().First().GetGenericArguments().First()
                .UnderlyingSystemType;
            var mockedService = scope.ServiceProvider.GetRequiredService(interfaceType);

            if (mockedService is null || !mockedService.GetType().FullName!.StartsWith("Castle.Proxies"))
                continue;
            var mockedServiceInstance = Activator.CreateInstance(mockedServiceType) as IMockService;
            var fullTypeName = interfaceType.FullName!;
            var displayName = mockedServiceInstance?.FriendlyName;
            serviceMocksList.Add(new(fullTypeName, mockedServiceInstance!.MethodMocks, true, displayName));
        }

        return serviceMocksList;
    }

    internal static List<Type> GetMockedServicesList(Assembly[] assemblies)
    {
        List<Type> mockedServices = [];
        foreach (var assembly in assemblies)
        {
            var assemblyMocks = assembly.GetTypes()
                .Where(x => x.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMockService<>)))
                .ToList();
            mockedServices.AddRange(assemblyMocks);
        }

        return mockedServices;
    }
}