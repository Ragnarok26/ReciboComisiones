using Comisiones.Common;
using Comisiones.Tools;
using ReciboComisiones.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ReciboComisiones.Controllers
{
    public class HomeController : Controller
    {
        private async Task<Usuario> GetUsuario(string app)
        {
            HttpCookie myCookie = null;
            JavaScriptSerializer js = null;
            Usuario usuario = new Usuario();
            bool flagPermisos = false;
            try
            {
                myCookie = Request.Cookies["UserSettingsReciboComisiones"];
                js = new JavaScriptSerializer();
                if (myCookie != null)
                {
                    if (!string.IsNullOrEmpty(myCookie["UserData"]))
                    {
                        try
                        {
                            usuario = (Usuario)js.Deserialize(myCookie["UserData"], typeof(Usuario));
                            flagPermisos = usuario.Permisos != null ? (usuario.Permisos.Count > 0) : false;
                            if (!flagPermisos)
                            {
                                usuario = null;
                            }
                        }
                        catch
                        {
                            usuario = null;
                        }
                    }
                    else
                    {
                        usuario = null;
                    }
                }
                else
                {
                    try
                    {
                        myCookie = new HttpCookie("UserSettingsReciboComisiones");
                        usuario.Nombre = User.Identity.Name.Substring(User.Identity.Name.LastIndexOf(@"\") + 1);
                        usuario.Permisos = usuario.ObtenerPermisos(app);
                        flagPermisos = usuario.Permisos != null ? (usuario.Permisos.Count > 0) : false;
                        if (flagPermisos)
                        {
                            myCookie["UserData"] = js.Serialize(usuario);
                            myCookie.Expires = DateTime.Now.AddYears(1000);
                        }
                        else
                        {
                            usuario = null;
                            myCookie = null;
                        }
                    }
                    catch
                    {
                        myCookie = null;
                        usuario = null;
                    }
                    if (myCookie != null)
                    {
                        Response.Cookies.Add(myCookie);
                    }
                }
                return usuario;
            }
            finally
            {
                js = null;
                usuario = null;
                myCookie = null;
            }
        }

        public FileResult Ayuda()
        {
            string NombreArchivo = Server.MapPath("~/Ayuda/Manual usuario.pdf");
            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = "Manual de usuario.pdf",
                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(NombreArchivo, "application/pdf");
            //return null;
        }

        public FileResult ArchivoEjemploFormato()
        {
            string NombreArchivo = Server.MapPath("~/Ayuda/Ejemplo formato.xlsx");
            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = "Ejemplo formato.xlsx",
                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(NombreArchivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public FileResult ArchivoEjemploParametros()
        {
            string NombreArchivo = Server.MapPath("~/Ayuda/Ejemplo parametros.xlsx");
            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = "Ejemplo parametros.xlsx",
                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(NombreArchivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<ViewResult> Index()
        {
            Usuario usuario = null;
            string app = ConfigurationManager.AppSettings["App.Name"];
            try
            {
                usuario = await GetUsuario(app);
                if (usuario != null)
                {
                    ViewBag.UserName = usuario.Nombre;
                    return View();
                }
                else
                {
                    return View("ErrorPermisos");
                }
            }
            finally
            {
                usuario = null;
            }
        }

        [HttpPost]
        public async Task<ActionResult> Upload()
        {
            long logId = 0;
            ProcessResult result = null;
            string app = ConfigurationManager.AppSettings["App.Name"];
            Usuario usuario = await GetUsuario(app);
            logId = LogTools.RegisterLog(0, usuario.Nombre, app, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Procesando Archivo.", DateTime.Now);
            if (usuario != null)
            {
                HttpPostedFileBase fileForm = null;
                HttpPostedFileBase fileParam = null;
                string to = ConfigurationManager.AppSettings["MailTo"].ToString();
                try
                {
                    if (Request.Files.Count == 2)
                    {
                        fileForm = Request.Files["fileInputForm"];
                        fileParam = Request.Files["fileInputParam"];
                        result = ReciboComision.Procesar(fileForm.InputStream, fileParam.InputStream, to, usuario.Nombre, app, Request.Params["mailSubject"], Request.Params["mailBody"].Replace("\n", "<br/>"), logId);
                        if (fileParam.InputStream != null)
                        {
                            fileParam.InputStream.Dispose();
                            fileParam = null;
                        }
                        if (fileForm.InputStream != null)
                        {
                            fileForm.InputStream.Dispose();
                            fileForm = null;
                        }
                        if (!result.HasErrors)
                        {
                            return PartialView("Grid", result.Collection);
                        }
                        else
                        {
                            return Json(result);
                        }
                    }
                    else
                    {
                        result = new ProcessResult()
                        {
                            HasErrors = true,
                            Message = "No hay suficientes archivos para iniciar el proceso."
                        };
                        return Json(result);
                    }
                }
                catch (Exception ex)
                {
                    LogTools.RegisterLog(logId, usuario.Nombre, app, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error al procesar la información: " + ex.Message + ".", DateTime.Now);
                    result = new ProcessResult()
                    {
                        HasErrors = true,
                        Message = "Error al procesar la información: " + ex.Message + "."
                    };
                    return Json(result);
                }
                finally
                {
                    if (fileForm != null)
                    {
                        if (fileForm.InputStream != null)
                        {
                            fileForm.InputStream.Dispose();
                        }
                        fileForm = null;
                    }
                    if (fileParam != null)
                    {
                        if (fileParam.InputStream != null)
                        {
                            fileParam.InputStream.Dispose();
                        }
                        fileParam = null;
                    }
                }
            }
            else
            {
                LogTools.RegisterLog(logId, "", app, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No se han cargado los datos del usuario para poder procesar la carga de datos.", DateTime.Now);
                result = new ProcessResult()
                {
                    HasErrors = true,
                    Message = "No se han cargado los datos del usuario para poder procesar la carga de datos."
                };
                return Json(result);
            }
        }
    }
}