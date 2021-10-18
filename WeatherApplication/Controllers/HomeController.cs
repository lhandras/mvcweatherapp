using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WeatherApplication.Context;
using WeatherApplication.Models;
using WeatherApplication.Services;

namespace WeatherApplication.Controllers
{
    /// <summary>
    /// Main controller to manage all weather stuff
    /// </summary>
    public class HomeController : Controller
    {

        private IWeatherCacheManager _cacheManager;

        /// <summary>
        /// Creates the main page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var dbContext = new WeatherDataEntities();

            var currentUserId = User.Identity.GetUserId();

            var userCities = dbContext.UserCities.Where(x => x.UserId == currentUserId).Select(x => new CityLight
            {
                Id = x.CityId,
                IsFavourite = x.IsFavourite,
                CityName = x.City.Name,
                CountryName = x.City.CountryName,
                ObservationTime = x.City.WeatherCache.ObservationTime,
                WeatherDescription = x.City.WeatherCache.WeatherDescription,
                WeatherIcon = x.City.WeatherCache.WeatherIcon
            }).ToList();

            return View(new UserCityViewModel
            {
                FavouriteUserCities = userCities.Where(x => x.IsFavourite).OrderBy(x => x.CityName).ToList(),
                NonFavouriteUserCities = userCities.Where(x => !x.IsFavourite).OrderBy(x => x.CityName).ToList()
            });
        }

        /// <summary>
        /// Creates the city list for the user to choose from
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ShowCityList()
        {
            var dbContext = new WeatherDataEntities();
            var cities = dbContext.Cities
                .Select(x => new CitySelectLight { Id = x.Id, DisplayName = x.Name + " (" + x.CountryCode + ")" })
                .ToList().OrderBy(x => x.DisplayName);

            ViewData["CityList"] = new SelectList(cities, "Id", "DisplayName");

            return PartialView();
        }

        /// <summary>
        /// Adds the selected city to the user's dashboard and retrieves weather data if necessary
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShowCityList([Bind] CitySelectLight city)
        {
            if (city == null)
            {
                return PartialView("Error");
            }

            var dbContext = new WeatherDataEntities();

            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity.GetUserId();
                var item = new UserCity { CityId = city.Id, UserId = currentUserId };

                dbContext.UserCities.Add(item);
                dbContext.SaveChanges();

                if (!dbContext.WeatherCaches.Any(x => x.CityId == item.CityId))
                {
                    var result = _cacheManager.GetCurrentWeatherData(item.CityId);

                    dbContext.WeatherCaches.Add(result);

                    dbContext.SaveChanges();
                }

                if (!dbContext.WeatherForecastDailyCaches.Any(x => x.CityId == item.CityId))
                {
                    var result = _cacheManager.GetDailyForecastWeatherData(item.CityId);

                    dbContext.WeatherForecastDailyCaches.AddRange(result);

                    dbContext.SaveChanges();
                }

                return PartialView("SuccessAddCity");
            }
            else
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList();
                ViewData["ErrorMessages"] = allErrors;
            }

            var cities = dbContext.Cities
                .Select(x => new CitySelectLight { Id = x.Id, DisplayName = x.Name + " (" + x.CountryCode + ")" })
                .ToList().OrderBy(x => x.DisplayName);

            ViewData["CityList"] = new SelectList(cities, "Id", "DisplayName");

            return PartialView(city);
        }

        /// <summary>
        /// Creates the details view about the selected city
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult WeatherDetails(int cityId)
        {
            var dbContext = new WeatherDataEntities();

            var currentWeather = dbContext.WeatherCaches.Where(x => x.CityId == cityId).Select(x => new CurrentWeatherViewModel()
            {
                WeatherIcon = x.WeatherIcon,
                FeelsLike = x.FeelsLike,
                ObservationTime = x.ObservationTime,
                Pressure = x.Pressure,
                Temperature = x.Temperature,
                UvIndex = x.UvIndex,
                WeatherDescription = x.WeatherDescription
            }).SingleOrDefault();

            var dailyForecasts = dbContext.WeatherForecastDailyCaches.Where(x => x.CityId == cityId).Select(x => new DailyForecastViewModel()
            {
                FeelsLikeMaxTemp = x.FeelsLikeMaxTemp,
                FeelsLikeMinTemp = x.FeelsLikeMinTemp,
                LastUpdated = x.LastUpdated,
                MaxTemperature = x.MaxTemperature,
                MinTemperature = x.MinTemperature,
                Pressure = x.Pressure,
                UvIndex = x.UvIndex,
                ValidDate = x.ValidDate,
                WeatherDescription = x.WeatherDescription,
                WeatherIcon = x.WeatherIcon
            }).ToList();

            return PartialView(new WeatherDetailsViewModel()
            {
                CurrentWeatherDetails = currentWeather,
                DailyForecasts = dailyForecasts
            });
        }

        /// <summary>
        /// Removes the selected city from the user's dashboard
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RemoveCity(int cityId)
        {
            var dbContext = new WeatherDataEntities();
            var currentUserId = User.Identity.GetUserId();

            var city = dbContext.UserCities.SingleOrDefault(x => x.CityId == cityId && x.UserId == currentUserId);

            if (city != null)
            {
                dbContext.UserCities.Remove(city);
                dbContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Changes the favourite state of the given city
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Favourite(int cityId)
        {
            var dbContext = new WeatherDataEntities();
            var currentUserId = User.Identity.GetUserId();

            var city = dbContext.UserCities.SingleOrDefault(x => x.CityId == cityId && x.UserId == currentUserId);

            if (city != null)
            {
                city.IsFavourite = !city.IsFavourite;
                dbContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public HomeController()
        {
            _cacheManager = new WeatherCacheManager();
        }
    }
}