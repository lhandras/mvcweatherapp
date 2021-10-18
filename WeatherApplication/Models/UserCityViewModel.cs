using System.Collections.Generic;

namespace WeatherApplication.Models
{
    /// <summary>
    /// ViewModel for the user's dashboard
    /// </summary>
    public class UserCityViewModel
    {
        public IList<CityLight> FavouriteUserCities { get; set; }
        public IList<CityLight> NonFavouriteUserCities { get; set; }
    }
}