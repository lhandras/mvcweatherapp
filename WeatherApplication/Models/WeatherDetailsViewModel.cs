using System.Collections.Generic;

namespace WeatherApplication.Models
{
    /// <summary>
    /// ViewModel for the details of the selected city
    /// </summary>
    public class WeatherDetailsViewModel
    {
        public CurrentWeatherViewModel CurrentWeatherDetails { get; set; }

        public IList<DailyForecastViewModel> DailyForecasts { get; set; }
    }
}