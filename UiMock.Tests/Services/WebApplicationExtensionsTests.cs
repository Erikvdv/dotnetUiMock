using System.Reflection;
using DotnetUiMock;
using Sample.Mocks;
using UiMock.Tests.Mocks;

namespace UiMock.Tests.Services;

public class WebApplicationExtensionsTests
{
    [Fact]
    public void GetMockedServicesList_WithSingleAssembly_ReturnsAllMocks()
    {
        // Arrange
        var assembly = Assembly.GetAssembly(typeof(WeatherServiceMock));
        var assemblies = new[] { assembly! };

        // Act
        var result = WebApplicationExtensions.GetMockedServicesList(assemblies);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, t => t.Name == nameof(WeatherServiceMock));
        Assert.Contains(result, t => t.Name == nameof(AuthenticationMock));
        Assert.Contains(result, t => t.Name == nameof(GetWeatherForecastHandlerMock));
    }

    [Fact]
    public void GetMockedServicesList_WithMultipleAssemblies_ReturnsAllMocksFromAllAssemblies()
    {
        // Arrange
        var sampleMocksAssembly = Assembly.GetAssembly(typeof(WeatherServiceMock));
        var testAssembly = Assembly.GetAssembly(typeof(TestMockService));
        var testmocksAssembly = Assembly.GetAssembly(typeof(TestRequestHandlerMock));
        var assemblies = new[] { sampleMocksAssembly!, testAssembly!, testmocksAssembly! };

        // Act
        var result = WebApplicationExtensions.GetMockedServicesList(assemblies);

        // Assert
        Assert.NotEmpty(result);

        // Verify mocks from Sample.Mocks assembly
        Assert.Contains(result, t => t.Name == nameof(WeatherServiceMock));
        Assert.Contains(result, t => t.Name == nameof(AuthenticationMock));
        Assert.Contains(result, t => t.Name == nameof(GetWeatherForecastHandlerMock));

        // Verify mocks from test assembly
        Assert.Contains(result, t => t.Name == nameof(TestMockService));
        Assert.Contains(result, t => t.Name == nameof(TestRequestHandlerMock));

        // Verify total count includes mocks from both assemblies
        var sampleMocksCount = result.Count(t => t.Assembly == sampleMocksAssembly);
        var testMocksCount = result.Count(t => t.Assembly == testAssembly);

        Assert.True(sampleMocksCount >= 3, "Should have at least 3 mocks from Sample.Mocks");
        Assert.True(testMocksCount >= 1, "Should have at least 1 mock from test assembly");
    }

    [Fact]
    public void GetMockedServicesList_WithEmptyAssemblyArray_ReturnsEmptyList()
    {
        // Arrange
        var assemblies = Array.Empty<Assembly>();

        // Act
        var result = WebApplicationExtensions.GetMockedServicesList(assemblies);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetMockedServicesList_WithAssemblyContainingNoMocks_ReturnsEmptyList()
    {
        // Arrange - use System assembly which has no IMockService implementations
        var assemblies = new[] { typeof(string).Assembly };

        // Act
        var result = WebApplicationExtensions.GetMockedServicesList(assemblies);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetMockedServicesList_WithDuplicateAssemblies_ReturnsAllMocksIncludingDuplicates()
    {
        // Arrange
        var assembly = Assembly.GetAssembly(typeof(WeatherServiceMock));
        var assemblies = new[] { assembly!, assembly! };

        // Act
        var result = WebApplicationExtensions.GetMockedServicesList(assemblies);

        // Assert
        Assert.NotEmpty(result);

        // Should return duplicates (6 = 3 mocks * 2 assemblies)
        var weatherMockCount = result.Count(t => t.Name == nameof(WeatherServiceMock));
        Assert.Equal(2, weatherMockCount);
    }
}
