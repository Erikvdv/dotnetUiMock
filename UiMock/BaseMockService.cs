using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace DotnetUiMock;

public abstract class BaseMockService<T> : IMockService<T> where T : class
{
    public ServiceMocks ServiceMocks { get; init; } = new(typeof(T).Name, new List<MethodMocks>());

    public T GenerateService(IServiceProvider serviceProvider)
    {
        var uiService = serviceProvider.GetRequiredService<UiMockService>();
        var service = Substitute.For<T>();
        uiService.InvokeDelegates(typeof(T).Name, ServiceMocks, service);
        return service;
    }
}