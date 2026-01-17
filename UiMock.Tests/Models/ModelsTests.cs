using DotnetUiMock;

namespace UiMock.Tests.Models;

public class ServiceMocksTests
{
    [Fact]
    public void ServiceMocks_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var methodMocks = new List<MethodMocks>
        {
            new("TestMethod", [new MockScenario("default", (_, _) => { })])
        };
        var serviceMocks = new ServiceMocks("TestService", methodMocks);

        // Assert
        Assert.Equal("TestService", serviceMocks.ServiceName);
        Assert.True(serviceMocks.IsMocked);
        Assert.Null(serviceMocks.FriendlyName);
        Assert.Single(serviceMocks.MethodMocks);
    }

    [Fact]
    public void ServiceMocks_WithFriendlyName_StoresCorrectly()
    {
        // Arrange & Act
        var methodMocks = new List<MethodMocks>
        {
            new("TestMethod", [new MockScenario("default", (_, _) => { })])
        };
        var serviceMocks = new ServiceMocks("TestService", methodMocks, true, "Friendly Test Service");

        // Assert
        Assert.Equal("Friendly Test Service", serviceMocks.FriendlyName);
    }

    [Fact]
    public void ServiceMocks_IsMocked_CanBeToggled()
    {
        // Arrange
        var methodMocks = new List<MethodMocks>
        {
            new("TestMethod", [new MockScenario("default", (_, _) => { })])
        };
        var serviceMocks = new ServiceMocks("TestService", methodMocks, true);

        // Act
        serviceMocks.IsMocked = false;

        // Assert
        Assert.False(serviceMocks.IsMocked);
    }
}

public class SelectedScenarioTests
{
    [Fact]
    public void SelectedScenario_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var scenario = new SelectedScenario("TestService", "TestMethod");

        // Assert
        Assert.Equal("TestService", scenario.ServiceName);
        Assert.Equal("TestMethod", scenario.MethodName);
        Assert.Equal("default", scenario.Scenario);
        Assert.Equal(0, scenario.DelayMs);
    }

    [Fact]
    public void SelectedScenario_WithCustomValues_StoresCorrectly()
    {
        // Arrange & Act
        var scenario = new SelectedScenario("TestService", "TestMethod", "custom", 500);

        // Assert
        Assert.Equal("custom", scenario.Scenario);
        Assert.Equal(500, scenario.DelayMs);
    }

    [Fact]
    public void SelectedScenario_CanUpdateScenarioAndDelay()
    {
        // Arrange
        var scenario = new SelectedScenario("TestService", "TestMethod", "default", 100);

        // Act
        scenario.Scenario = "updated";
        scenario.DelayMs = 300;

        // Assert
        Assert.Equal("updated", scenario.Scenario);
        Assert.Equal(300, scenario.DelayMs);
    }
}

public class MethodMocksTests
{
    [Fact]
    public void MethodMocks_DefaultDelayMs_IsZero()
    {
        // Arrange & Act
        var methodMocks = new MethodMocks("TestMethod", [new MockScenario("default", (_, _) => { })]);

        // Assert
        Assert.Equal(0, methodMocks.DelayMs);
    }

    [Fact]
    public void MethodMocks_WithCustomDelay_StoresCorrectly()
    {
        // Arrange & Act
        var methodMocks = new MethodMocks("TestMethod", [new MockScenario("default", (_, _) => { })], 250);

        // Assert
        Assert.Equal(250, methodMocks.DelayMs);
    }
}

public class MockScenarioTests
{
    [Fact]
    public void MockScenario_StoresNameAndHandler()
    {
        // Arrange
        var handlerCalled = false;
        MockHandler handler = (_, _) => { handlerCalled = true; };

        // Act
        var scenario = new MockScenario("TestScenario", handler);
        scenario.Handler(new object(), 0);

        // Assert
        Assert.Equal("TestScenario", scenario.Name);
        Assert.True(handlerCalled);
    }
}

public class MockedServiceTests
{
    [Fact]
    public void MockedService_DefaultIsMocked_IsTrue()
    {
        // Arrange & Act
        var service = new MockedService("TestName", "TestFullName");

        // Assert
        Assert.Equal("TestName", service.Name);
        Assert.Equal("TestFullName", service.FullName);
        Assert.True(service.IsMocked);
    }

    [Fact]
    public void MockedService_IsMocked_CanBeToggled()
    {
        // Arrange
        var service = new MockedService("TestName", "TestFullName", true);

        // Act
        service.IsMocked = false;

        // Assert
        Assert.False(service.IsMocked);
    }
}
