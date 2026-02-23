using Microsoft.AspNetCore.Mvc;
using SCA_MVC.Models;
using SCA_MVC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCA_MVC.Controllers
{
    public class RegistroController : Controller
    {
        #region Atributos y Constructor
        private readonly IEmpleadoNegocio _empleadoNegocio;
        private readonly IEmpresaNegocio _empresaNegocio;
        private readonly IRegistroNegocio _registroNegocio;
        private readonly IServicioNegocio _servicioNegocio;

        public RegistroController(
            IEmpleadoNegocio empleadoNegocio,
            IEmpresaNegocio empresaNegocio,
            IRegistroNegocio registroNegocio,
            IServicioNegocio servicioNegocio)
        {
            _empleadoNegocio = empleadoNegocio;
            _empresaNegocio = empresaNegocio;
            _registroNegocio = registroNegocio;
            _servicioNegocio = servicioNegocio;
        }
        #endregion

        #region Acciones Principales

        // GET: Registro
        public async Task<IActionResult> Index()
        {
            // Obtener servicio activo (donde DuracionMinutos es NULL)
            var servicioActivo = await _servicioNegocio.ObtenerActivoGlobalAsync();
            
            // Si no hay servicio activo hoy, mostrar advertencia
            if (servicioActivo == null || servicioActivo.Fecha.Date != DateTime.Today)
            {
                ViewBag.NoServicio = true;
                return View(new List<Empleado>());
            }

            ViewBag.IdServicio = servicioActivo.IdServicio;
            ViewBag.IdLugar = servicioActivo.IdLugar;
            ViewBag.Empresas = await _empresaNegocio.ListarAsync();
            
            // Datos para las 4 cards superiores (usando conteo real de la tabla Registros)
            int proyectados = servicioActivo.Proyeccion ?? 0;
            int registradosNormales = await _registroNegocio.ContarAsync(servicioActivo.IdServicio);
            int registrados = registradosNormales + servicioActivo.TotalInvitados;
            int pendientes = Math.Max(0, proyectados - registrados);
            double progreso = proyectados > 0 ? (registrados * 100.0 / proyectados) : 0;

            ViewBag.Proyectados = proyectados;
            ViewBag.Registrados = registrados;
            ViewBag.Pendientes = pendientes;
            ViewBag.Progreso = progreso;

            // Por defecto cargamos todos los que faltan almorzar
            var empleados = await _empleadoNegocio.FiltrarSinAlmorzarAsync(servicioActivo.IdServicio, null, null);
            return View(empleados);
        }

        #endregion

        #region Acciones AJAX

        // GET: Registro/Filtrar (AJAX)
        // Permite la búsqueda dinámica en el panel de registro manual
        [HttpGet]
        public async Task<IActionResult> Filtrar(int? idEmpresa, string? nombre)
        {
            var servicioActivo = await _servicioNegocio.ObtenerActivoGlobalAsync();
            if (servicioActivo == null)
                return Json(new { success = false, message = "No hay servicio activo" });

            var empleados = await _empleadoNegocio.FiltrarSinAlmorzarAsync(servicioActivo.IdServicio, idEmpresa, nombre);
            
            // Mapeamos a un objeto anónimo para facilitar el JS
            var result = empleados.Select(e => new {
                idEmpleado = e.IdEmpleado,
                idEmpresa = e.IdEmpresa,
                nombre = e.Nombre,
                apellido = e.Apellido,
                empresaNombre = e.Empresa?.Nombre ?? "Sin Empresa",
                credencial = e.IdCredencial
            });

            return Json(result);
        }

        // POST: Registro/RegistrarMultiples (AJAX)
        // Procesa la selección múltiple de empleados para registrar sus almuerzos masivamente
        [HttpPost]
        public async Task<IActionResult> RegistrarMultiples([FromBody] RegistroMultipleRequest request)
        {
            if (request == null || request.IdsEmpleados == null || request.IdsEmpleados.Count == 0)
                return Json(new { success = false, message = "No se seleccionaron empleados" });

            try
            {
                foreach (var idEmp in request.IdsEmpleados)
                {
                    // Obtener datos del empleado para tener el ID de empresa
                    var emp = await _empleadoNegocio.BuscarPorIdAsync(idEmp);
                    if (emp != null)
                    {
                        await _registroNegocio.RegistrarAsync(idEmp, emp.IdEmpresa, request.IdServicio, request.IdLugar);
                    }
                }

                // Sincronización de KPIs: Recalculamos los totales después de la inserción
                // Esto permite que el cliente actualice las tarjetas superiores sin recargar la página.
                var servicioActualizado = await _servicioNegocio.ObtenerActivoGlobalAsync();
                int proyectados = servicioActualizado?.Proyeccion ?? 0;
                int registradosNormales = servicioActualizado != null ? await _registroNegocio.ContarAsync(servicioActualizado.IdServicio) : 0;
                int registrados = registradosNormales + (servicioActualizado?.TotalInvitados ?? 0);
                int pendientes = Math.Max(0, proyectados - registrados);
                double progreso = proyectados > 0 ? (registrados * 100.0 / proyectados) : 0;

                return Json(new { 
                    success = true, 
                    count = request.IdsEmpleados.Count,
                    registrados = registrados,
                    pendientes = pendientes,
                    progreso = progreso
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }

    /// <summary>
    /// Estructura para capturar la petición masiva de registro por AJAX
    /// </summary>
    public class RegistroMultipleRequest
    {
        public int IdServicio { get; set; }
        public int IdLugar { get; set; }
        public List<int> IdsEmpleados { get; set; } = new List<int>();
    }
}
