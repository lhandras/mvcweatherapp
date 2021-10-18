using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherApplication.Models
{
    /// <summary>
    /// Dto to manage the received data easily from the Api
    /// </summary>
    public class ApiDailyForecastWeatherDto
    {
        [JsonProperty("pres")]
        public double Pressure { get; set; }

        [JsonProperty("app_min_temp")]
        public double FeelsLikeMinTemp { get; set; }

        [JsonProperty("valid_date")]
        public string ValidDate { get; set; }

        [JsonProperty("app_max_temp")]
        public double FeelsLikeMaxTemp { get; set; }

        [JsonProperty("uv")]
        public double UvIndex { get; set; }

        [JsonProperty("weather")]
        public JObject Weather { get; set; }

        [JsonProperty("max_temp")]
        public double MaxTemperature { get; set; }

        [JsonProperty("min_temp")]
        public double MinTemperature { get; set; }
    }
}