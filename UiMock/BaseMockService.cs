using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace DotnetUiMock;

public abstract class BaseMockService<T> : IMockService<T> where T : class
{
    public List<MethodMocks> MethodMocks { get; protected init;} = [];

    public T GenerateService(IServiceProvider serviceProvider)
    {
        var uiService = serviceProvider.GetRequiredService<UiMockService>();
        var stackTrace = new StackTrace();
        var currentMethodName = new StackFrame(0).GetMethod()?.Name;
        var isRecursive = stackTrace.GetFrames().Count(frame => frame.GetMethod()?.Name == currentMethodName) > 1;

        var isMocked = uiService.ServiceMocksList.FirstOrDefault(x => x.ServiceName == typeof(T).FullName)?.IsMocked ?? true;
        if (!isRecursive && !isMocked)
        {
            using var serviceScope = serviceProvider.CreateScope();
            return serviceProvider.CreateScope().ServiceProvider.GetServices<T>().First();
        }
        
        
        var service = Substitute.For<T>();
        uiService.InvokeDelegates(typeof(T).FullName!, MethodMocks, service);
        return service;
    }
}