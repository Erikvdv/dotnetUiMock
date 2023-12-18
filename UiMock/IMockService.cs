namespace DotnetUiMock;

public interface IMockService
{
    public ServiceMocks ServiceMocks { get; }
}

public interface IMockService<out T> : IMockService where T : class
{
    public T GenerateService(IServiceProvider serviceProvider);
}