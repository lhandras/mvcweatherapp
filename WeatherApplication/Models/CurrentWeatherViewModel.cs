using System;

namespace WeatherApplication.Models
{
    /// <summary>
    /// ViewModel for the current weather on the details view
    /// </summary>
    public class CurrentWeatherViewModel : CityLight
    {
        public double Temperature { get; set; }

        public double FeelsLike { get; set; }

        public double UvIndex { get; set; }

        public double Pressure { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}