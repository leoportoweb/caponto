using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CAPonto.Models
{
    public class Periodo
    {
        [Required]
        [Range(2000, 3000)]
        [DisplayName("Year")]
        public int Ano { get; set; }

        [Required]
        [Range(1, 12)]
        [DisplayName("Month")]
        public int Mes { get; set; }

        [Required]
        [DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Initial date")]
        public DateTime DataInicio { get; set; }

        [Required]
        [DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Final Date")]
        public DateTime DataFim { get; set; }
    }
}
