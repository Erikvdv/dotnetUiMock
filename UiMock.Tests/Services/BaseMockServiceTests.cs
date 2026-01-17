using DotnetUiMock;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace UiMock.Tests.Services;

public interface ITestService
{
    string GetData();
    Task<int> GetValueAsync();
}

public class TestMockService : BaseMockService<ITestService>
{
    public TestMockService()
    {
        FriendlyName = "Test Mock Service";
        MethodMocks =
        [
            new("GetData", [
                new MockScenario("default", (service, _) =>
                {
                    (service as ITestService)?.GetData().Returns("mocked data");
                })
            ], 100)
        ];
    }
}

public class BaseMockServiceTests
{
    [Fact]
    public void BaseMockService_FriendlyName_CanBeSet()
    {
        // Arrange & Act
        var mockService = new TestMockService();

        // Assert
        Assert.Equal("Test Mock Service", mockService.FriendlyName);
    }

    [Fact]
    public void BaseMockService_MethodMocks_ContainsDefinedMocks()
    {
        // Arrange & Act
        var mockService = new TestMockService();

        // Assert
        Assert.Single(mockService.MethodMocks);
        Assert.Equal("GetData", mockService.MethodMocks[0].Name);
        Assert.Equal(100, mockService.MethodMocks[0].DelayMs);
    }

    [Fact]
    public void BaseMockService_MethodMocks_ContainsScenarios()
    {
        // Arrange & Act
        var mockService = new TestMockService();

        // Assert
        Assert.Single(mockService.MethodMocks[0].Scenarios);
        Assert.Equal("default", mockService.MethodMocks[0].Scenarios[0].Name);
    }

    [Fact]
    public void GenerateService_WhenMocked_ReturnsSubstitute()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<UiMockService>();
        var serviceProvider = services.BuildServiceProvider();

        var uiMockService = serviceProvider.GetRequiredService<UiMockService>();
        uiMockService.ServiceMocksList =
        [
            new ServiceMocks(typeof(ITestService).FullName!, [
                new("GetData", [
                    new MockScenario("default", (service, _) =>
                    {
                        (service as ITestService)?.GetData().Returns("mocked data");
                    })
                ], 100)
            ], true)
        ];
        uiMockService.Init();

        var mockService = new TestMockService();

        // Act
        var generatedService = mockService.GenerateService(serviceProvider);

        // Assert
        Assert.NotNull(generatedService);
        Assert.Equal("mocked data", generatedService.GetData());
    }
}
