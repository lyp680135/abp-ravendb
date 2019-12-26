using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IAsyncDocumentSession _session;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IAsyncDocumentSession session)
        {
            _logger = logger;
            _session = session;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            IEnumerable<WeatherForecast> weatherList = await GetWeatherList();
            return weatherList;
        }

        private async Task<IEnumerable<WeatherForecast>> GetWeatherList(bool rnd = false)
        {
            if (rnd)
            {
                var rng = new Random();
                return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
            }
            var result = await _session.Query<WeatherForecast>().ToListAsync();
            if (result.Count == 0)
            {
                await SaveWeather();
            }
            return await _session.Query<WeatherForecast>().ToListAsync();
        }

        private async Task SaveWeather()
        {
            var rng = new Random();
            await _session.StoreAsync(new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
            await _session.SaveChangesAsync();
        }
    }
}
