using Microsoft.AspNetCore.Mvc;
using SCA_MVC.Models;
using SCA_MVC.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCA_MVC.Controllers
{
    public class RegistroController : Controller
    {
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

        // GET: Registro
        public async Task<IActionResult> Index()
        {
            // Obtener servicio activo de hoy
            var servicioActivo = await _servicioNegocio.ObtenerUltimoAsync();
            
            // Si no hay servicio activo hoy, mostrar advertencia
            if (servicioActivo == null || servicioActivo.Estado != "Activo" || servicioActivo.Fecha.Date != DateTime.Today)
            {
                ViewBag.NoServicio = true;
                return View(new List<Empleado>());
            }

            ViewBag.IdServicio = servicioActivo.IdServicio;
            ViewBag.IdLugar = servicioActivo.IdLugar;
            ViewBag.Empresas = await _empresaNegocio.ListarAsync();

            // Por defecto cargamos todos los que faltan almorzar
            var empleados = await _empleadoNegocio.FiltrarSinAlmorzarAsync(servicioActivo.IdServicio, null, null);
            return View(empleados);
        }

        // GET: Registro/Filtrar (AJAX)
        [HttpGet]
        public async Task<IActionResult> Filtrar(int? idEmpresa, string? nombre)
        {
            var servicioActivo = await _servicioNegocio.ObtenerUltimoAsync();
            if (servicioActivo == null || servicioActivo.Estado != "Activo")
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
                return Json(new { success = true, count = request.IdsEmpleados.Count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class RegistroMultipleRequest
    {
        public int IdServicio { get; set; }
        public int IdLugar { get; set; }
        public List<int> IdsEmpleados { get; set; } = new List<int>();
    }
}
