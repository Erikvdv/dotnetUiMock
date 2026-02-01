using Mediator;

namespace UiMock.Tests.Mocks;

/// <summary>
/// MediatR Response type for testing handlers not in startup assembly
/// </summary>
public class TestResponse
{
    public string Result { get; set; } = "";
    public int Count { get; set; }
}
