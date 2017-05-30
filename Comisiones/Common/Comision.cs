using System;
using System.ComponentModel.DataAnnotations;

namespace Comisiones.Common
{
    public class Comision
    {
        [Display(Name = "File M")]
        public string FileM { get; set; }

        [Display(Name = "Cliente")]
        public string Cliente { get; set; }

        [Display(Name = "Modelo")]
        public string Modelo { get; set; }

        [Display(Name = "Venta DDP")]
        public decimal? VentaDdp { get; set; }

        [Display(Name = "Venta EXW")]
        public decimal? VentaExw { get; set; }

        [Display(Name = "Intereses")]
        public decimal? Interes { get; set; }

        [Display(Name = "Fecha OC")]
        public DateTime? FechaOC { get; set; }

        [Display(Name = "Mes de Cálculo")]
        public string MesCalculo { get; set; }

        [Display(Name = "Comisión USD")]
        public decimal? ComisionUsd { get; set; }

        [Display(Name = "Bono Clientes Diferentes")]
        public decimal? BonoClientesDif { get; set; }

        [Display(Name = "Incentivo Cert. Herr.")]
        public decimal? IncentivoCertHerr { get; set; }

        [Display(Name = "Incentivo Desc. Opcionales")]
        public decimal? IncentivoDescOpc { get; set; }

        [Display(Name = "Incentivo Mesas/Finder")]
        public decimal? IncentivoMesasFinder { get; set; }

        [Display(Name = "Comisión MXP")]
        public decimal? ComisionMxp { get; set; }
    }
}
