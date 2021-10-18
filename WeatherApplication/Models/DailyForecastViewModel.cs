using System;

namespace WeatherApplication.Models
{
    /// <summary>
    /// ViewModel for the daily forecasts on the details view
    /// </summary>
    public class DailyForecastViewModel : CityLight
    {
        public double MinTemperature { get; set; }

        public double MaxTemperature { get; set; }

        public double FeelsLikeMinTemp { get; set; }

        public double FeelsLikeMaxTemp { get; set; }

        public double UvIndex { get; set; }

        public double Pressure { get; set; }

        public string ValidDate { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}