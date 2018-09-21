using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CAPonto.Models
{
    public class Config
    {
        [Required]
        [Range(1, 24)]
        [DisplayName("Number of hours to work per day")]
        public int HorasDia { get; set; }

        [Required]
        [Range(0, 120)]
        [DisplayName("Number of minutes of tolerance in the total hours of the day")]
        public int LimiteAjuste { get; set; }
    }
}
