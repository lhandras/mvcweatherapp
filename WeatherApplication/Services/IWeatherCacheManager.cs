using System.Collections.Generic;
using WeatherApplication.Context;

namespace WeatherApplication.Services
{
    public interface IWeatherCacheManager
    {
        /// <summary>
        /// Gets the current weather from the Weather Api
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        WeatherCache GetCurrentWeatherData(int cityId);

        /// <summary>
        /// Gets daily forecasts from the Weather Api
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        IEnumerable<WeatherForecastDailyCache> GetDailyForecastWeatherData(int cityId);
    }
}
