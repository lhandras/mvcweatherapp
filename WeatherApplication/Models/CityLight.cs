namespace WeatherApplication.Models
{
    /// <summary>
    /// Dto for the selected cities on the dashboard
    /// </summary>
    public class CityLight
    {
        public int Id { get; set; }

        public string CityName { get; set; }

        public string CountryName { get; set; }

        public string WeatherIcon { get; set; }

        public string WeatherDescription { get; set; }

        public string ObservationTime { get; set; }

        public bool IsFavourite { get; set; }

    }
}