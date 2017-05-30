using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Comisiones.Common
{
    public class Empleado
    {
        [Display(Name = "Empleado")]
        public string Nombre { get; set; }

        [Display(Name = "Puesto")]
        public string Puesto { get; set; }

        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; }

        [Display(Name = "Mes")]
        public string Mes { get; set; }

        [Display(Name = "Fecha de Pago")]
        public DateTime? FechaPago { get; set; }

        [Display(Name = "Tipo de Cambio")]
        public double? TipoCambio { get; set; }

        [Display(Name = "Estatus")]
        public bool Procesado { get; set; }

        [Display(Name = "Sub-Total Comisiones")]
        public decimal TotalComisiones { get; set; }

        [Display(Name = "Sub-Total Deducciones")]
        public decimal TotalDeducciones { get; set; }

        [Display(Name = "Total Percepción por Comisión")]
        public decimal Total { get; set; }

        public List<Comision> Comisiones { get; set; }

        public List<Deduccion> Deducciones { get; set; }

        public bool Guardar()
        {
            int? rowsAffected = null;
            DataTable comision = null;
            DataTable deduccion = null;
            DataColumn column = null;
            try
            {
                comision = new DataTable();
                column = new DataColumn("FileM", typeof(string));
                column.AllowDBNull = true;
                comision.Columns.Add(column);
                column = new DataColumn("Cliente", typeof(string));
                column.AllowDBNull = true;
                comision.Columns.Add(column);
                column = new DataColumn("Modelo", typeof(string));
                column.AllowDBNull = true;
                comision.Columns.Add(column);
                column = new DataColumn("VentaDDP", typeof(decimal));
                column.AllowDBNull = true;
                comision.Columns.Add(column);
                column = new DataColumn("VentaEXW", typeof(decimal));
                column.AllowDBNull = true;
                comision.Columns.Add(column);
                column = new DataColumn("Interes", typeof(string));
                column.AllowDBNull = true;
                comision.Columns.Add(column);
                column = new DataColumn("FechaOC", typeof(DateTime));
                column.AllowDBNull = true;
                comision.Columns.Add(column);
                column = new DataColumn("MesCalculo", typeof(string));
                column.AllowDBNull = true;
                comision.Columns.Add(column);
                column = new DataColumn("ComisionUSD", typeof(decimal));
                column.AllowDBNull = true;
                comision.Columns.Add(column);
                column = new DataColumn("BonoClientesDiferentes", typeof(decimal));
                column.AllowDBNull = true;
                comision.Columns.Add(column);
                column = new DataColumn("IncentivoCertHerr", typeof(decimal));
                column.AllowDBNull = true;
                comision.Columns.Add(column);
                column = new DataColumn("IncentivoDescOpcionales", typeof(decimal));
                column.AllowDBNull = true;
                comision.Columns.Add(column);
                column = new DataColumn("IncentivoMesasFinder", typeof(decimal));
                column.AllowDBNull = true;
                comision.Columns.Add(column);
                column = new DataColumn("ComisionMXP", typeof(decimal));
                column.AllowDBNull = true;
                comision.Columns.Add(column);
                column = null;
                if (this.Comisiones != null)
                {
                    if (this.Comisiones.Count > 0)
                    {
                        for (int x = 0; x < this.Comisiones.Count; x++)
                        {
                            comision.Rows.Add(
                                new object[] {
                                    this.Comisiones[x].FileM,
                                    this.Comisiones[x].Cliente,
                                    this.Comisiones[x].Modelo,
                                    this.Comisiones[x].VentaDdp,
                                    this.Comisiones[x].VentaExw,
                                    this.Comisiones[x].Interes,
                                    this.Comisiones[x].FechaOC,
                                    this.Comisiones[x].MesCalculo,
                                    this.Comisiones[x].ComisionUsd,
                                    this.Comisiones[x].BonoClientesDif,
                                    this.Comisiones[x].IncentivoCertHerr,
                                    this.Comisiones[x].IncentivoDescOpc,
                                    this.Comisiones[x].IncentivoMesasFinder,
                                    this.Comisiones[x].ComisionMxp
                                }
                            );
                        }
                    }
                }
                deduccion = new DataTable();
                column = new DataColumn("Motivo", typeof(string));
                column.AllowDBNull = true;
                deduccion.Columns.Add(column);
                column = new DataColumn("MesPago", typeof(string));
                column.AllowDBNull = true;
                deduccion.Columns.Add(column);
                column = new DataColumn("Monto", typeof(decimal));
                column.AllowDBNull = true;
                deduccion.Columns.Add(column);
                if (this.Deducciones != null)
                {
                    if (this.Deducciones.Count > 0)
                    {
                        for (int x = 0; x < this.Deducciones.Count; x++)
                        {
                            deduccion.Rows.Add(
                                new object[] {
                                    this.Deducciones[x].Motivo,
                                    this.Deducciones[x].MesPago,
                                    this.Deducciones[x].Monto
                                }
                            );
                        }
                    }
                }
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["FinanzasDB"].ConnectionString))
                {
                    conn.Open();
                    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("[comision].[InsertEmployee]", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if (!string.IsNullOrEmpty(this.Nombre))
                        {
                            command.Parameters.Add("Nombre", SqlDbType.NVarChar).Value = this.Nombre;
                        }
                        if (!string.IsNullOrEmpty(this.Puesto))
                        {
                            command.Parameters.Add("Puesto", SqlDbType.NVarChar).Value = this.Puesto;
                        }
                        if (!string.IsNullOrEmpty(this.Correo))
                        {
                            command.Parameters.Add("Correo", SqlDbType.NVarChar).Value = this.Correo;
                        }
                        if (!string.IsNullOrEmpty(this.Mes))
                        {
                            command.Parameters.Add("Mes", SqlDbType.NVarChar).Value = this.Mes;
                        }
                        if (this.FechaPago.HasValue)
                        {
                            command.Parameters.Add("FechaPago", SqlDbType.DateTime).Value = this.FechaPago.Value;
                        }
                        if (this.TipoCambio.HasValue)
                        {
                            command.Parameters.Add("TipoCambio", SqlDbType.Decimal).Value = this.TipoCambio.Value;
                        }
                        command.Parameters.Add("Procesado", SqlDbType.Bit).Value = this.Procesado;
                        command.Parameters.Add("TotalComisiones", SqlDbType.Decimal).Value = this.TotalComisiones;
                        command.Parameters.Add("TotalDeducciones", SqlDbType.Decimal).Value = this.TotalDeducciones;
                        command.Parameters.Add("Total", SqlDbType.Decimal).Value = this.Total;
                        command.Parameters.Add("Comisiones", SqlDbType.Structured).Value = comision;
                        command.Parameters["Comisiones"].TypeName = "comision.ComisionList";
                        command.Parameters.Add("Deducciones", SqlDbType.Structured).Value = deduccion;
                        command.Parameters["Deducciones"].TypeName = "comision.DeduccionList";
                        rowsAffected = (int)command.ExecuteScalar();
                    }
                    conn.Close();
                }
                return rowsAffected.HasValue ? rowsAffected.Value > 0 : rowsAffected.HasValue;
            }
            catch
            {
                return false;
            }
            finally
            {
                comision = null;
                deduccion = null;
                rowsAffected = null;
            }
        }
    }
}
