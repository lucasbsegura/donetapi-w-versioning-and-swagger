using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace SwaggerAndVersioning.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]
    [Route("v{version:apiVersion}/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Weather Forecast 
        /// </summary>
        /// <remarks>List of weather forecast for the last five days</remarks>
        /// <response code="200">Returns weather forecast fot the last five days</response>
        /// <response code="404">Not Found, the resource could not be found</response>		
        [HttpGet]
        public IActionResult Get()
        {
            var rng = new Random();
            var weatherForecastList = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = WeatherHelper.Summaries[rng.Next(WeatherHelper.Summaries.Length)]
            })
            .ToArray();

            if (weatherForecastList == null || !weatherForecastList.Any()) return Unauthorized();

            return Ok(weatherForecastList);
        }
    }

    public static class WeatherHelper
    {
        public static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
    }
}
