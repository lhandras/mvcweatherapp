using System;
using System.Linq;
using System.Threading;
using System.Web.Http;
using WeatherApplication.Context;
using WeatherApplication.Services;

namespace WeatherApplication.Controllers
{
    /// <summary>
    /// Api that can be called regularly to update the cached weather data
    /// </summary>
    public class WeatherDataController : ApiController
    {
        private IWeatherCacheManager _cacheManager;

        /// <summary>
        /// Refreshes the cached weather data if necessary
        /// </summary>
        /// <returns></returns>
        [System.Web.Mvc.Route("api/WeatherData/WeatherDataRefreshCache")]
        [HttpGet]
        public dynamic WeatherDataRefreshCache()
        {
            var dbContext = new WeatherDataEntities();

            var cacheExpirationTime = DateTime.Now.AddHours(-2);

            var refreshCities = dbContext.WeatherCaches.Where(x => x.LastUpdated <= cacheExpirationTime);

            var refreshCityForecasts = dbContext.WeatherForecastDailyCaches.Where(x => x.LastUpdated <= cacheExpirationTime).ToList();

            if (refreshCities.Any() || refreshCityForecasts.Any())
            {
                foreach (var refreshCity in refreshCities)
                {
                    var newCurrentWeather = _cacheManager.GetCurrentWeatherData(refreshCity.CityId);

                    if (newCurrentWeather != null)
                    {
                        refreshCity.FeelsLike = newCurrentWeather.FeelsLike;
                        refreshCity.ObservationTime = newCurrentWeather.ObservationTime;
                        refreshCity.Pressure = newCurrentWeather.Pressure;
                        refreshCity.Temperature = newCurrentWeather.Temperature;
                        refreshCity.UvIndex = newCurrentWeather.UvIndex;
                        refreshCity.WeatherDescription = newCurrentWeather.WeatherDescription;
                        refreshCity.WeatherIcon = newCurrentWeather.WeatherIcon;
                        refreshCity.LastUpdated = newCurrentWeather.LastUpdated;
                    }

                    Thread.Sleep(1500); //Because of the 1 second rate limit in the free plan
                }

                dbContext.SaveChanges();

                var refreshDailyCities = refreshCityForecasts.Select(x => x.CityId).Distinct();

                foreach (var cityId in refreshDailyCities)
                {
                    var newForecasts = _cacheManager.GetDailyForecastWeatherData(cityId).ToList();

                    if (newForecasts.Any())
                    {
                        dbContext.WeatherForecastDailyCaches.RemoveRange(refreshCityForecasts.Where(x => x.CityId == cityId));

                        dbContext.WeatherForecastDailyCaches.AddRange(newForecasts);

                        dbContext.SaveChanges();
                    }

                    Thread.Sleep(1500); //Because of the 1 second rate limit in the free plan
                }
            }

            return new
            {
                data = "OK"
            };
        }

        public WeatherDataController()
        {
            _cacheManager = new WeatherCacheManager();
        }
    }
}