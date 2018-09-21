using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CAPonto.Models
{
    public class Lancamento
    {
        [Required]
        [DisplayName("Order")]
        public int Ordem { get; set; }
        
        [Required]
        [DataType(DataType.DateTime)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Date")]
        public DateTime Data { get; set; }
    }
}
