using System;
using System.Collections.Generic;

namespace CAPonto.Models
{
    public class Consolidado
    {
        public string Matricula { get; set; }

        public int Ano { get; set; }

        public int Mes { get; set; }

        public string MesDescricao { get; set; }

        public double Extra { get; set; }

        public double Devendo { get; set; }

        public double ExtraAjustado { get; set; }

        public double DevendoAjustado { get; set; }

        public double ExtraDecimal { get; set; }

        public double DevendoDecimal { get; set; }

        public double ExtraAjustadoDecimal { get; set; }

        public double DevendoAjustadoDecimal { get; set; }

    }
}
