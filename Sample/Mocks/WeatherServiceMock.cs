using DotnetUiMock;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.ExceptionExtensions;
using sample.Services;

namespace sample.Mocks;

public class WeatherServiceMock : BaseMockService<IWeatherService>
{
    public WeatherServiceMock()
    {
        MethodMocks =
        [
            new("GetForecastAsync",
            [
                new MockScenario("oneitem", GetForecastOneItem),
                new MockScenario("threeitems", GetForecastThreeItems),
                new MockScenario("exception", GetForecastAsyncException),
            ]),
            new("GetForecastAsync(string location)",
            [
                new MockScenario("withlocation", GetForecastWithLocation),
                new MockScenario("exception", GetForecastWithLocationException),
            ]),
        ];
    }

    private static void GetForecastOneItem(object service, int delay)
    {
        (service as IWeatherService)?.GetForecastAsync().Returns(async _ =>
        {
            await Task.Delay(delay);
            return new WeatherForecast[]
            {
                new() {Date = new DateOnly(2021, 1, 1), TemperatureC = 10, Summary = "Freezing"}
            };
        });
    }

    private static void GetForecastThreeItems(object service, int delay)
    {
        (service as IWeatherService)?.GetForecastAsync().Returns(async _ =>
        {
            await Task.Delay(delay);
            return new WeatherForecast[]
            {
                new() {Date = new DateOnly(2021, 1, 1), TemperatureC = 10, Summary = "Freezing"},
                new() {Date = new DateOnly(2021, 1, 1), TemperatureC = 10, Summary = "Freezing"},
                new() {Date = new DateOnly(2021, 1, 1), TemperatureC = 10, Summary = "Freezing"}
            };
        });
    }

    private static void GetForecastAsyncException(object service, int delay)
    {
        (service as IWeatherService)?.GetForecastAsync().Returns<Task<WeatherForecast[]>>(async _ =>
        {
            await Task.Delay(delay);
            throw new Exception("Exception from mock");
        });
    }

    private static void GetForecastWithLocation(object service, int delay)
    {
        (service as IWeatherService)?.GetForecastAsync(Arg.Any<string>()).Returns(async _ =>
        {
            await Task.Delay(delay);
            return new WeatherForecast
            { Date = new DateOnly(2021, 1, 1), TemperatureC = 10, Summary = "Freezing" };
        });
    }

    private static void GetForecastWithLocationException(object service, int delay)
    {
        (service as IWeatherService)?.GetForecastAsync(Arg.Any<string>()).Returns<Task<WeatherForecast>>(async _ =>
        {
            await Task.Delay(delay);
            throw new Exception("Exception from mock");
        });
    }

}