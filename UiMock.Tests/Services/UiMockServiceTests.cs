using DotnetUiMock;

namespace UiMock.Tests.Services;

public class UiMockServiceTests
{
    [Fact]
    public void Init_WhenNoSavedScenarios_UsesDefaultScenarios()
    {
        // Arrange
        var service = new UiMockService();
        var methodMocks = new List<MethodMocks>
        {
            new("TestMethod", [new MockScenario("default", (_, _) => { })], 100)
        };
        service.ServiceMocksList =
        [
            new ServiceMocks("TestService", methodMocks)
        ];

        // Act
        service.Init();

        // Assert
        Assert.Single(service.SelectedScenarios);
        Assert.Equal("TestService", service.SelectedScenarios[0].ServiceName);
        Assert.Equal("TestMethod", service.SelectedScenarios[0].MethodName);
        Assert.Equal("default", service.SelectedScenarios[0].Scenario);
    }

    [Fact]
    public void Init_WithMultipleServices_CreatesScenarioForEach()
    {
        // Arrange
        var service = new UiMockService();
        var methodMocks1 = new List<MethodMocks>
        {
            new("Method1", [new MockScenario("scenario1", (_, _) => { })], 50)
        };
        var methodMocks2 = new List<MethodMocks>
        {
            new("Method2", [new MockScenario("scenario2", (_, _) => { })], 100)
        };
        service.ServiceMocksList =
        [
            new ServiceMocks("Service1", methodMocks1),
            new ServiceMocks("Service2", methodMocks2)
        ];

        // Act
        service.Init();

        // Assert
        Assert.Equal(2, service.SelectedScenarios.Count);
    }

    [Fact]
    public void ToggleService_WhenServiceDoesNotExist_DoesNotThrow()
    {
        // Arrange
        var service = new UiMockService();
        service.ServiceMocksList = [];

        // Act & Assert - should not throw
        var exception = Record.Exception(() => service.ToggleService("NonExistentService", false));
        Assert.Null(exception);
    }

    // Note: ToggleService_WhenServiceExists_UpdatesIsMocked and UpdateSelectedScenario tests 
    // are omitted because they trigger SaveToDisk() which requires file system access.
    // These should be tested via integration tests or by refactoring UiMockService 
    // to accept an abstraction for persistence.

    [Fact]
    public void InvokeDelegates_CallsCorrectHandler()
    {
        // Arrange
        var service = new UiMockService();
        var handlerCalled = false;
        var capturedDelay = 0;

        var methodMocks = new List<MethodMocks>
        {
            new("TestMethod", [
                new MockScenario("default", (_, delay) =>
                {
                    handlerCalled = true;
                    capturedDelay = delay;
                })
            ], 100)
        };
        service.ServiceMocksList =
        [
            new ServiceMocks("TestService", methodMocks)
        ];
        service.Init();

        var mockObject = new object();

        // Act
        service.InvokeDelegates("TestService", methodMocks, mockObject);

        // Assert
        Assert.True(handlerCalled);
        Assert.Equal(100, capturedDelay);
    }
}
