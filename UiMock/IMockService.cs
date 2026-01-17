namespace DotnetUiMock;

public interface IMockService
{
    public List<MethodMocks> MethodMocks { get; }
    public string? FriendlyName { get; }
}

public interface IMockService<out T> : IMockService where T : class
{
    public T GenerateService(IServiceProvider serviceProvider);
}