using System;
using System.Collections.Generic;

namespace CAPonto.Models
{
    public class LancamentoDia
    {
        public string Matricula { get; set; }

        public DateTime Data { get; set; }

        public string DiaSemana { get; set; }

        public string DiaSemanaReduzido { get; set; }

        public bool EhFeriado { get; set; }

        public List<Lancamento> Lancamentos { get; set; }

        public double Total { get; set; }

        public double Extra { get; set; }

        public double Devendo { get; set; }

        public double ExtraAjustado { get; set; }

        public double DevendoAjustado { get; set; }
        
    }
}
