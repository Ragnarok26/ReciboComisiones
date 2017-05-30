using System;
using System.ComponentModel.DataAnnotations;

namespace Comisiones.Common
{
    public class Deduccion
    {
        public string Vacio01 { get { return string.Empty; } }

        public string Vacio02 { get { return string.Empty; } }

        public string Vacio03 { get { return string.Empty; } }

        public string Vacio04 { get { return string.Empty; } }

        [Display(Name = "Motivo")]
        public string Motivo { get; set; }

        public string Vacio05 { get { return string.Empty; } }

        public string Vacio06 { get { return string.Empty; } }

        public string Vacio07 { get { return string.Empty; } }

        public string Vacio08 { get { return string.Empty; } }

        public string Vacio09 { get { return string.Empty; } }

        public string Vacio10 { get { return string.Empty; } }

        public string Vacio11 { get { return string.Empty; } }

        [Display(Name = "Mes de Pago")]
        public string MesPago { get; set; }

        [Display(Name = "Monto")]
        public decimal? Monto { get; set; }
    }
}
