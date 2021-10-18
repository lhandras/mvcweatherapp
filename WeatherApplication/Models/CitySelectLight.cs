using System.ComponentModel.DataAnnotations;

namespace WeatherApplication.Models
{
    /// <summary>
    /// Dto for the city select list
    /// </summary>
    public class CitySelectLight
    {
        [Required(ErrorMessage = "Please choose a city!")]
        public int Id { get; set; }

        public string DisplayName { get; set; }
    }
}