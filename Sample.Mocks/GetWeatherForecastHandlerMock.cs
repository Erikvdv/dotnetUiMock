using DotnetUiMock;
using Mediator;
using NSubstitute;
using Sample.Core.Services;

namespace Sample.Mocks;

public class GetWeatherForecastHandlerMock : BaseMockService<IRequestHandler<GetWeatherForecastRequest, WeatherForecast[]>>
{
  public GetWeatherForecastHandlerMock()
  {
    FriendlyName = "Get Weather Forecast";
    MethodMocks =
    [
      new("Handle",
      [
        new MockScenario("oneitem", HandleOneItem),
        new MockScenario("threeitems", HandleThreeItems),
        new MockScenario("exception", HandleException)
      ], 200)
    ];
  }

  private static void HandleOneItem(object service, int delay)
  {
    (service as IRequestHandler<GetWeatherForecastRequest, WeatherForecast[]>)?
      .Handle(Arg.Any<GetWeatherForecastRequest>(), Arg.Any<CancellationToken>())
      .Returns<ValueTask<WeatherForecast[]>>(async _ =>
      {
        await Task.Delay(delay);
        return new[]
        {
          new WeatherForecast { Date = new DateOnly(2021, 1, 1), TemperatureC = 10, Summary = "Freezing" }
        };
      });
  }

  private static void HandleThreeItems(object service, int delay)
  {
    (service as IRequestHandler<GetWeatherForecastRequest, WeatherForecast[]>)?
      .Handle(Arg.Any<GetWeatherForecastRequest>(), Arg.Any<CancellationToken>())
      .Returns<ValueTask<WeatherForecast[]>>(async _ =>
      {
        await Task.Delay(delay);
        return new[]
        {
          new WeatherForecast { Date = new DateOnly(2021, 1, 1), TemperatureC = 10, Summary = "Freezing" },
          new WeatherForecast { Date = new DateOnly(2021, 1, 2), TemperatureC = 12, Summary = "Bracing" },
          new WeatherForecast { Date = new DateOnly(2021, 1, 3), TemperatureC = 8, Summary = "Chilly" }
        };
      });
  }

  private static void HandleException(object service, int delay)
  {
    (service as IRequestHandler<GetWeatherForecastRequest, WeatherForecast[]>)?
      .Handle(Arg.Any<GetWeatherForecastRequest>(), Arg.Any<CancellationToken>())
      .Returns<ValueTask<WeatherForecast[]>>(async _ =>
      {
        await Task.Delay(delay);
        throw new Exception("Exception from mock");
      });
  }
}