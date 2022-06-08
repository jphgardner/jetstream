using Jetstream;
using Jetstream.Context;
using Microservice.Events;
using Microsoft.AspNetCore.Mvc;
using NATS.Client;

namespace Microservice.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IJetstreamContext _jetstreamContext;

    public WeatherForecastController(IJetstreamContext jetstreamContext, ILogger<WeatherForecastController> logger)
    {
        _jetstreamContext = jetstreamContext;
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
    
    [HttpPost]
    public IActionResult Do()
    {
        var jetStream = _jetstreamContext.GetJetStream();
        jetStream.Publish(new Msg()
        {
            Subject = "Test.Hello",
            Data = Utility.Encode(new TestEvent()
            {
                Name = "From Controller"
            })
        });
        return Ok();
    }
}