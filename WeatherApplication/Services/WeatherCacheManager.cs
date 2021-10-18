using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeatherApplication.Context;
using WeatherApplication.Models;

namespace WeatherApplication.Services
{
    public class WeatherCacheManager : IWeatherCacheManager
    {
        private string _weatherHostAddress = "https://api.weatherbit.io/v2.0/";

        private string _weatherApiKey;

        private const string ApiHostUrlField = "WeatherApiUrl";

        private const string ApiKeyField = "WeatherApiKey";

        private const string CurrentQueryField = "WeatherApiCurrentQuery";

        private const string DailyQueryField = "WeatherApiForecastDailyQuery";

        private const string ForecastDaysField = "WeatherForecastDays";

        protected HttpClient ApiClient;

        /// <summary>
        /// Gets the current weather from the Online Api
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public WeatherCache GetCurrentWeatherData(int cityId)
        {
            var nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";


            if (cityId > 0)
            {
                var dbContext = new WeatherDataEntities();

                var city = dbContext.Cities.SingleOrDefault(x => x.Id == cityId);

                if (city != null)
                {
                    InitializeApiClient();

                    var currentWeather = ConfigurationManager.AppSettings[CurrentQueryField];

                    if (ApiClient.BaseAddress.AbsolutePath != "/")
                    {
                        currentWeather = ApiClient.BaseAddress.AbsolutePath + currentWeather;
                    }

                    var response = ApiClient.GetAsync(currentWeather + "?key=" + _weatherApiKey + "&lat=" + city.Latitude.Value.ToString(nfi) + "&lon=" + city.Longitude.Value.ToString(nfi)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        // Read
                        var stream = response.Content.ReadAsStringAsync().Result;

                        // Map
                        stream = "[" + stream + "]";
                        var adatok = JsonConvert.DeserializeObject<IList<JObject>>(stream);

                        var x = adatok[0].GetValue("data");

                        var actualWeather = JsonConvert.DeserializeObject<ApiCurrentWeatherDto>(x.First.ToString());

                        var cache = new WeatherCache();
                        cache.CityId = city.Id;
                        cache.FeelsLike = actualWeather.FeelsLike;
                        cache.LastUpdated = DateTime.Now;
                        cache.ObservationTime = actualWeather.ObTime;
                        cache.Pressure = actualWeather.Pressure;
                        cache.Temperature = actualWeather.Temperature;
                        cache.UvIndex = actualWeather.UvIndex;
                        cache.WeatherDescription = actualWeather.Weather.GetValue("description").ToString();
                        cache.WeatherIcon = actualWeather.Weather.GetValue("icon").ToString();

                        return cache;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the daily forecasts from the Online Api
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public IEnumerable<WeatherForecastDailyCache> GetDailyForecastWeatherData(int cityId)
        {
            var nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";


            if (cityId > 0)
            {
                var dbContext = new WeatherDataEntities();

                var city = dbContext.Cities.SingleOrDefault(x => x.Id == cityId);

                if (city != null)
                {
                    InitializeApiClient();

                    var dailyWeather = ConfigurationManager.AppSettings[DailyQueryField];

                    var forecastDays = ConfigurationManager.AppSettings[ForecastDaysField];

                    var result = new List<WeatherForecastDailyCache>();

                    if (ApiClient.BaseAddress.AbsolutePath != "/")
                    {
                        dailyWeather = ApiClient.BaseAddress.AbsolutePath + dailyWeather;
                    }

                    var response = ApiClient.GetAsync(dailyWeather + "?key=" + _weatherApiKey + "&days=" + forecastDays + "&lat=" + city.Latitude.Value.ToString(nfi) + "&lon=" + city.Longitude.Value.ToString(nfi)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        // Read
                        var stream = response.Content.ReadAsStringAsync().Result;

                        // Map
                        stream = "[" + stream + "]";
                        var adatok = JsonConvert.DeserializeObject<IList<JObject>>(stream);

                        var x = adatok[0].GetValue("data");

                        var forecasts = JsonConvert.DeserializeObject<IList<JObject>>(x.ToString());

                        foreach (var forecast in forecasts)
                        {
                            var forecastWeather = JsonConvert.DeserializeObject<ApiDailyForecastWeatherDto>(forecast.ToString());

                            var cache = new WeatherForecastDailyCache();

                            cache.CityId = city.Id;
                            cache.FeelsLikeMinTemp = forecastWeather.FeelsLikeMinTemp;
                            cache.FeelsLikeMaxTemp = forecastWeather.FeelsLikeMaxTemp;
                            cache.LastUpdated = DateTime.Now;
                            cache.ValidDate = forecastWeather.ValidDate;
                            cache.Pressure = forecastWeather.Pressure;
                            cache.MinTemperature = forecastWeather.MinTemperature;
                            cache.MaxTemperature = forecastWeather.MaxTemperature;
                            cache.UvIndex = forecastWeather.UvIndex;
                            cache.WeatherDescription = forecastWeather.Weather.GetValue("description").ToString();
                            cache.WeatherIcon = forecastWeather.Weather.GetValue("icon").ToString();

                            result.Add(cache);
                        }

                        return result;
                    }
                }
            }

            return new List<WeatherForecastDailyCache>();
        }

        private void InitializeApiClient()
        {
            var apiUrl = ConfigurationManager.AppSettings[ApiHostUrlField];

            var apiKey = ConfigurationManager.AppSettings[ApiKeyField];

            if (!string.IsNullOrWhiteSpace(apiUrl))
            {
                _weatherHostAddress = apiUrl;
            }

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                _weatherApiKey = apiKey;
            }

            HttpClientHandler handler = new HttpClientHandler()
            {
                UseDefaultCredentials = true
            };

            ApiClient = new HttpClient(handler);

            ApiClient.BaseAddress = new Uri(_weatherHostAddress);
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}