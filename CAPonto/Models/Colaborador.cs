using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CAPonto.Models
{
    public class Colaborador
    {
        [Required]
        [DisplayName("Registration")]
        public string Matricula { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Nome { get; set; }

        [Required]
        [DisplayName("Administrator")]
        public bool Administrador { get; set; }
    }
}
