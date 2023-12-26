namespace DotnetUiMock;

public record MethodMocks(string Name, List<MockScenario> Scenarios);

/// <summary>
///     Delegate for the mock handler.
/// </summary>
/// <param name="service">The service object to be processed.</param>
/// <param name="delay">The delay in milliseconds before processing.</param>
public delegate void MockHandler(object service, int delay);

public class MockedService(string name, string fullName, bool isMocked = true)
{
    public string Name { get; } = name;
    public string FullName { get; } = fullName;
    public bool IsMocked { get; set; } = isMocked;
};
public record MockScenario(string Name, MockHandler Handler);

public class ServiceMocks(string serviceName, List<MethodMocks> methodMocks, bool isMocked = true)
{
    public string ServiceName { get; init; } = serviceName;
    public List<MethodMocks> MethodMocks { get; init; } = methodMocks;
    public bool IsMocked { get; set; } = isMocked;
    
}

public class SelectedScenario(string serviceName, string methodName, string? scenario = "default", int delayMs = 100)
{
    public string ServiceName { get; } = serviceName;
    public string MethodName { get; } = methodName;
    public string? Scenario { get; set; } = scenario;
    public int DelayMs { get; set; } = delayMs;
}

public record ToggleRequest(string ServiceName, string IsMocked);