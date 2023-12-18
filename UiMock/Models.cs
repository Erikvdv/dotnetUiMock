namespace DotnetUiMock;

public record MethodMocks(string Name, List<MockScenario> Scenarios);

/// <summary>
///     Delegate for the mock handler.
/// </summary>
/// <param name="service">The service object to be processed.</param>
/// <param name="delay">The delay in milliseconds before processing.</param>
public delegate void MockHandler(object service, int delay);

public record MockScenario(string Name, MockHandler Handler);

public record ServiceMocks(string ServiceName, List<MethodMocks> Methods);

public class SelectedScenario(string serviceName, string methodName, string? scenario = "default", int delayMs = 100)
{
    public string ServiceName { get; } = serviceName;
    public string MethodName { get; } = methodName;
    public string? Scenario { get; set; } = scenario;
    public int DelayMs { get; set; } = delayMs;
}