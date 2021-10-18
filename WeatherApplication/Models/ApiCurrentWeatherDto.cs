using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherApplication.Models
{
    /// <summary>
    /// Dto to manage the received data easily from the Api
    /// </summary>
    public class ApiCurrentWeatherDto
    {
        [JsonProperty("pod")]
        public char Pod { get; set; }

        [JsonProperty("pres")]
        public double Pressure { get; set; }

        [JsonProperty("ob_time")]
        public string ObTime { get; set; }

        [JsonProperty("uv")]
        public double UvIndex { get; set; }

        [JsonProperty("weather")]
        public JObject Weather { get; set; }

        [JsonProperty("temp")]
        public double Temperature { get; set; }

        [JsonProperty("app_temp")]
        public double FeelsLike { get; set; }
    }
}