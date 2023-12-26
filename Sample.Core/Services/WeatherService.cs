namespace Sample.Core.Services;
public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
    public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);
}

public interface IWeatherService
{
    Task<WeatherForecast[]> GetForecastAsync();
    Task<WeatherForecast> GetForecastAsync(string location);
}

public class WeatherService : IWeatherService
{
    public async Task<WeatherForecast[]> GetForecastAsync()
    {
        await Task.Delay(500);

        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var summaries = new[] {"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"};
        var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)]
        }).ToArray();
        return forecasts;
    }

    public async Task<WeatherForecast> GetForecastAsync(string location)
    {
        await Task.Delay(500);
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var summaries = new[] {"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"};
        var forecast = new WeatherForecast
        {
            Date = startDate.AddDays(1),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)]
        };
        return forecast;
    }
}

    