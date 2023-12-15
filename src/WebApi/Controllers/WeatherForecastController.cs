using System.Net;
using Grpc.Net.Client;
using GrpcService;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching"
    ];

    private readonly Greeter.GreeterClient _greeterClient;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(Greeter.GreeterClient greeterClient, ILogger<WeatherForecastController> logger)
    {
        _greeterClient = greeterClient;
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get()
    {
        var res = await _greeterClient.SayHelloAsync(
            new HelloRequest() { Name = $"{Guid.NewGuid()}" },
            cancellationToken: this.HttpContext.RequestAborted
        );

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
