using GemBox.Spreadsheet;
using Comisiones.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Comisiones.Common
{
    public class ReciboComision
    {
        public static Tools.ProcessResult Procesar(Stream Formato, Stream Parametros, string Para, string usuario, string app, string mailSubject, string mailBody, long logId)
        {
            OfficeOpenXml.ExcelPackage pkg = null;
            OfficeOpenXml.ExcelPackage pkgForm = null;
            OfficeOpenXml.ExcelWorksheet sheet = null;
            OfficeOpenXml.ExcelWorksheet sheetForm = null;
            OfficeOpenXml.ExcelCellAddress size = null;
            Empleado empleado = null;
            List<Empleado> empleados = new List<Empleado>();
            MemoryStream ms = null;
            MemoryStream pdf = null;
            List<System.Net.Mail.Attachment> attachments = null;
            int? startColumn = null;
            int? startRow = null;
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            ExcelFile xls = null;
            ExcelWorksheet worksheet = null;
            Tools.ProcessResult result = new ProcessResult();
            List<OfficeOpenXml.ExcelRangeBase> rangos = null;
            OfficeOpenXml.ExcelRangeBase rango = null;
            Comision comision = null;
            Deduccion deduccion = null;
            decimal dec;
            DateTime fec;
            try
            {
                pkg = new OfficeOpenXml.ExcelPackage(Parametros);
                sheet = pkg.Workbook.Worksheets[1];
                startColumn = sheet.Dimension.Start.Column;
                try
                {
                    startRow = (from cell in sheet.Cells where cell.Value is string && ((string)cell.Value).ToUpper().Contains("FILE M") select cell).FirstOrDefault().Start.Row + 1;
                }
                catch
                {
                    startRow = null;
                }
                finally
                {
                    if (startRow.HasValue)
                    {
                        while (startRow <= sheet.Dimension.End.Row)
                        {
                            if (!empleados.Any(v => v.Nombre == (string)sheet.Cells[startRow.Value, startColumn.Value].Value && v.Mes == (string)sheet.Cells[startRow.Value, startColumn.Value + 3].Value))
                            {
                                empleado = new Empleado()
                                {
                                    Nombre = (string)sheet.Cells[startRow.Value, startColumn.Value].Value,
                                    Puesto = (string)sheet.Cells[startRow.Value, startColumn.Value + 1].Value,
                                    Correo = (string)sheet.Cells[startRow.Value, startColumn.Value + 2].Value,
                                    Mes = (string)sheet.Cells[startRow.Value, startColumn.Value + 3].Value,
                                    FechaPago = (DateTime?)sheet.Cells[startRow.Value, startColumn.Value + 4].Value,
                                    //TipoCambio = (double?)sheet.Cells[startRow.Value, startColumn.Value + 5].Value,
                                    TipoCambio = null,
                                    Comisiones = new List<Comision>(),
                                    Deducciones = new List<Deduccion>(),
                                    TotalComisiones = 0,
                                    TotalDeducciones = 0,
                                    Total = 0,
                                    Procesado = false
                                };
                                size = BuscarCadena("TC", sheet);
                                if (size != null)
                                {
                                    empleado.TipoCambio = (double?)sheet.Cells[size.Row, size.Column + 1].Value;
                                }
                                rangos = sheet.Cells.Where(v => v.Value is string && ((string)v.Value) == empleado.Nombre).ToList();
                                if (rangos != null)
                                {
                                    if (rangos.Count > 0)
                                    {
                                        for (int x = 0; x < rangos.Count; x++)
                                        {
                                            rango = rangos.ElementAt(x);
                                            comision = new Comision();
                                            deduccion = new Deduccion();
                                            for (int row = rango.Start.Row; row <= rango.End.Row; row++)
                                            {
                                                comision = new Comision();
                                                comision.FileM = (string)sheet.Cells[row, startColumn.Value + 5].Value;
                                                comision.Cliente = (string)sheet.Cells[row, startColumn.Value + 6].Value;
                                                comision.Modelo = (string)sheet.Cells[row, startColumn.Value + 7].Value;
                                                comision.VentaDdp = decimal.TryParse(sheet.Cells[row, startColumn.Value + 8].Value == null ? "" : sheet.Cells[row, startColumn.Value + 8].Value.ToString(), out dec) ? dec : (decimal?)null;
                                                comision.VentaExw = decimal.TryParse(sheet.Cells[row, startColumn.Value + 9].Value == null ? "" : sheet.Cells[row, startColumn.Value + 9].Value.ToString(), out dec) ? dec : (decimal?)null;
                                                comision.Interes = decimal.TryParse(sheet.Cells[row, startColumn.Value + 10].Value == null ? "" : sheet.Cells[row, startColumn.Value + 10].Value.ToString(), out dec) ? dec : (decimal?)null;
                                                comision.FechaOC = DateTime.TryParseExact(sheet.Cells[row, startColumn.Value + 11].Value == null ? "" : sheet.Cells[row, startColumn.Value + 11].Value.ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fec) ? fec : (DateTime?)null;
                                                comision.MesCalculo = (string)sheet.Cells[row, startColumn.Value + 12].Value;
                                                comision.ComisionUsd = decimal.TryParse(sheet.Cells[row, startColumn.Value + 13].Value == null ? "" : sheet.Cells[row, startColumn.Value + 13].Value.ToString(), out dec) ? dec : (decimal?)null;
                                                comision.BonoClientesDif = decimal.TryParse(sheet.Cells[row, startColumn.Value + 14].Value == null ? "" : sheet.Cells[row, startColumn.Value + 14].Value.ToString(), out dec) ? dec : (decimal?)null;
                                                comision.IncentivoCertHerr = decimal.TryParse(sheet.Cells[row, startColumn.Value + 15].Value == null ? "" : sheet.Cells[row, startColumn.Value + 15].Value.ToString(), out dec) ? dec : (decimal?)null;
                                                comision.IncentivoDescOpc = decimal.TryParse(sheet.Cells[row, startColumn.Value + 16].Value == null ? "" : sheet.Cells[row, startColumn.Value + 16].Value.ToString(), out dec) ? dec : (decimal?)null;
                                                comision.IncentivoMesasFinder = decimal.TryParse(sheet.Cells[row, startColumn.Value + 17].Value == null ? "" : sheet.Cells[row, startColumn.Value + 17].Value.ToString(), out dec) ? dec : (decimal?)null;
                                                comision.ComisionMxp = decimal.TryParse(sheet.Cells[row, startColumn.Value + 18].Value == null ? "" : sheet.Cells[row, startColumn.Value + 18].Value.ToString(), out dec) ? dec : (decimal?)null;
                                                if (comision.ComisionMxp.HasValue)
                                                {
                                                    if (comision.ComisionMxp > 0)
                                                    {
                                                        empleado.Comisiones.Add(comision);
                                                    }
                                                    else
                                                    {
                                                        deduccion.Motivo = comision.FileM;
                                                        deduccion.MesPago = comision.MesCalculo;
                                                        deduccion.Monto = comision.ComisionMxp.HasValue ? Math.Abs(comision.ComisionMxp.Value) : (decimal?)null;
                                                        empleado.Deducciones.Add(deduccion);
                                                    }
                                                }
                                                else
                                                {
                                                    empleado.Comisiones.Add(comision);
                                                }
                                                comision = null;
                                            }
                                        }
                                        empleado.TotalComisiones = empleado.Comisiones.Sum(v => v.ComisionMxp.HasValue ? v.ComisionMxp.Value : 0);
                                        empleado.TotalDeducciones = empleado.Deducciones.Sum(v => v.Monto.HasValue ? v.Monto.Value : 0);
                                        empleado.Total = empleado.TotalComisiones - empleado.TotalDeducciones;
                                    }
                                }
                                empleados.Add(empleado);
                            }
                            startRow++;
                        }
                    }
                }
                if (empleados != null)
                {
                    if (empleados.Count > 0)
                    {
                        for (int x = 0; x < empleados.Count; x++)
                        {
                            pkgForm = new OfficeOpenXml.ExcelPackage(Formato);
                            pkgForm.Workbook.CalcMode = OfficeOpenXml.ExcelCalcMode.Automatic;
                            pkgForm.Workbook.FullCalcOnLoad = true;
                            sheetForm = pkgForm.Workbook.Worksheets[1];
                            size = BuscarCadena("NOMBRE", sheetForm);
                            if (size != null)
                            {
                                sheetForm.Cells[size.Row, size.Column + 1].Value = empleados.ElementAt(x).Nombre;
                            }
                            size = null;
                            size = BuscarCadena("{NOMBRE}", sheetForm);
                            if (size != null)
                            {
                                sheetForm.Cells[size.Row, size.Column].Value = empleados.ElementAt(x).Nombre;
                            }
                            size = null;
                            size = BuscarCadena("PUESTO", sheetForm);
                            if (size != null)
                            {
                                sheetForm.Cells[size.Row, size.Column + 1].Value = empleados.ElementAt(x).Puesto;
                            }
                            size = null;
                            size = BuscarCadena("{PUESTO}", sheetForm);
                            if (size != null)
                            {
                                sheetForm.Cells[size.Row, size.Column].Value = empleados.ElementAt(x).Puesto;
                            }
                            size = null;
                            size = BuscarCadena("MES", sheetForm);
                            if (size != null)
                            {
                                sheetForm.Cells[size.Row, size.Column + 1].Value = empleados.ElementAt(x).Mes;
                            }
                            size = null;
                            size = BuscarCadena("FECHA DE PAGO", sheetForm);
                            if (size != null)
                            {
                                sheetForm.Cells[size.Row, size.Column + 1].Value = empleados.ElementAt(x).FechaPago;
                            }
                            size = null;
                            size = BuscarCadena("TIPO DE CAMBIO", sheetForm);
                            if (size != null)
                            {
                                sheetForm.Cells[size.Row, size.Column + 1].Value = empleados.ElementAt(x).TipoCambio;
                            }
                            size = null;
                            size = BuscarCadena("FILE M", sheetForm);
                            if (size != null)
                            {
                                if (empleados.ElementAt(x).Comisiones.Count > 0)
                                {
                                    sheetForm.InsertRow(size.Row + 2, empleados.ElementAt(x).Comisiones.Count, size.Row + 1);
                                    sheetForm.Cells[size.Row + 2, size.Column].LoadFromCollection(empleados.ElementAt(x).Comisiones, false);
                                    sheetForm.DeleteRow(size.Row + 1);
                                }
                            }
                            size = null;
                            size = BuscarCadena("MOTIVO", sheetForm);
                            if (size != null)
                            {
                                if (empleados.ElementAt(x).Deducciones.Count > 0)
                                {
                                    sheetForm.InsertRow(size.Row + 2, empleados.ElementAt(x).Deducciones.Count, size.Row + 1);
                                    sheetForm.Cells[size.Row + 2, size.Column].LoadFromCollection(empleados.ElementAt(x).Deducciones, false);
                                    sheetForm.DeleteRow(size.Row + 1);
                                }
                            }
                            size = null;
                            size = BuscarCadena("SUB-TOTAL COMISIONES", sheetForm);
                            if (size != null)
                            {
                                sheetForm.Cells[size.Row, size.Column + 1].Value = empleados.ElementAt(x).TotalComisiones;
                                sheetForm.Cells[size.Row, size.Column + 1].Style.Numberformat.Format = "_($* #,##0.0_);_($* (#,##0.0);_($* \"-\"??_);_(@_)";
                            }
                            size = null;
                            size = BuscarCadena("SUB-TOTAL DEDUCCIONES", sheetForm);
                            if (size != null)
                            {
                                sheetForm.Cells[size.Row, size.Column + 1].Value = empleados.ElementAt(x).TotalDeducciones;
                                sheetForm.Cells[size.Row, size.Column + 1].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
                            }
                            size = null;
                            size = BuscarCadena("TOTAL PERCEP", sheetForm);
                            if (size != null)
                            {
                                sheetForm.Cells[size.Row, size.Column + 1].Value = empleados.ElementAt(x).Total;
                                sheetForm.Cells[size.Row, size.Column + 1].Style.Numberformat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"??_);_(@_)";
                            }
                            size = null;
                            ms = new MemoryStream();
                            pkgForm.SaveAs(ms);
                            ms.Position = 0;
                            pdf = new MemoryStream();
                            xls = ExcelFile.Load(ms, LoadOptions.XlsxDefault);
                            worksheet = xls.Worksheets.ActiveWorksheet;
                            worksheet.PrintOptions.FitWorksheetWidthToPages = 1;
                            worksheet.PrintOptions.FitWorksheetHeightToPages = 1;
                            worksheet.PrintOptions.VerticalCentered = false;
                            //worksheet.PrintOptions.TopMargin = 1;
                            xls.Save(pdf, SaveOptions.PdfDefault);
                            attachments = new List<System.Net.Mail.Attachment>();
                            attachments.Add(new System.Net.Mail.Attachment(pdf, "Recibo Comisiones.pdf", "application/pdf"));
                            if (!string.IsNullOrEmpty(Para))
                            {
                                empleados.ElementAt(x).Correo = Para;
                            }
                            empleados.ElementAt(x).Procesado = Correo.Enviar(empleados.ElementAt(x), usuario, app, mailSubject, mailBody, attachments, logId);
                            empleados.ElementAt(x).Guardar();
                            if (pdf != null)
                            {
                                pdf.Dispose();
                                pdf = null;
                            }
                            if (ms != null)
                            {
                                ms.Dispose();
                                ms = null;
                            }
                            attachments = null;
                            worksheet = null;
                            xls = null;
                            if (sheetForm != null)
                            {
                                sheetForm.Dispose();
                                sheetForm = null;
                            }
                            if (pkgForm != null)
                            {
                                pkgForm.Dispose();
                                pkgForm = null;
                            }
                            size = null;
                        }
                    }
                }
                if (sheet != null)
                {
                    sheet.Dispose();
                    sheet = null;
                }
                if (pkg != null)
                {
                    pkg.Dispose();
                    pkg = null;
                }
                if (Parametros != null)
                {
                    Parametros.Dispose();
                    Parametros = null;
                }
                if (Formato != null)
                {
                    Formato.Dispose();
                    Formato = null;
                }
                result.Collection = empleados;
                return result;
            }
            catch (Exception ex)
            {
                LogTools.RegisterLog(logId, usuario, app, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error al procesar la información: " + ex.Message + ".", DateTime.Now);
                result.HasErrors = true;
                result.Message = "Error al procesar la información: " + ex.Message + ".";
                return result;
            }
            finally
            {
                if (sheet != null)
                {
                    sheet.Dispose();
                    sheet = null;
                }
                if (pkg != null)
                {
                    pkg.Dispose();
                    pkg = null;
                }
                if (Parametros != null)
                {
                    Parametros.Dispose();
                    Parametros = null;
                }
                empleado = null;
                empleados = null;
                attachments = null;
                xls = null;
                worksheet = null;
                size = null;
                if (sheetForm != null)
                {
                    sheetForm.Dispose();
                    sheetForm = null;
                }
                if (pkgForm != null)
                {
                    pkgForm.Dispose();
                    pkgForm = null;
                }
                if (Formato != null)
                {
                    Formato.Dispose();
                    Formato = null;
                }
                if (ms != null)
                {
                    ms.Dispose();
                    ms = null;
                }
                if (pdf != null)
                {
                    pdf.Dispose();
                    pdf = null;
                }
                result = null;
            }
        }

        private static OfficeOpenXml.ExcelCellAddress BuscarCadena(string Cadena, OfficeOpenXml.ExcelWorksheet sheetForm)
        {
            try
            {
                return (from cell in sheetForm.Cells where cell.Value is string && ((string)cell.Value).ToUpper().Contains(Cadena) select cell).FirstOrDefault().Start;
            }
            catch
            {
                return null;
            }
        }
    }
}
