using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WeatherDataRefreshApplication
{
    /// <summary>
    /// Simple console app that can be scheduled to trigger the weather cache refreshing Api
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

            HttpClientHandler handler = new HttpClientHandler()
            {
                UseDefaultCredentials = true
            };

            var apiClient = new HttpClient(handler);

            apiClient.BaseAddress = new Uri(apiUrl);
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = apiClient.GetAsync("").Result;

            Console.WriteLine(response);


        }
    }
}
