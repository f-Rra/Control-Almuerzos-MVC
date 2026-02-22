using Microsoft.AspNetCore.Mvc;
using SCA_MVC.Models;
using SCA_MVC.Services;
using SCA_MVC.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SCA_MVC.Controllers
{
    public class ServicioController : Controller
    {
        private readonly IServicioNegocio _servicioNegocio;
        private readonly ILugarNegocio _lugarNegocio;
        private readonly IRegistroNegocio _registroNegocio;
        private readonly IEmpleadoNegocio _empleadoNegocio;

        public ServicioController(
            IServicioNegocio servicioNegocio,
            ILugarNegocio lugarNegocio,
            IRegistroNegocio registroNegocio,
            IEmpleadoNegocio empleadoNegocio)
        {
            _servicioNegocio = servicioNegocio;
            _lugarNegocio = lugarNegocio;
            _registroNegocio = registroNegocio;
            _empleadoNegocio = empleadoNegocio;
        }

        // GET: Servicio
        public async Task<IActionResult> Index()
        {
            var viewModel = new ServicioActivoViewModel();
            viewModel.LugaresDisponibles = await _lugarNegocio.ListarAsync();

            // Intentar obtener el servicio actualmente activo
            var activo = await _servicioNegocio.ObtenerActivoGlobalAsync();
            if (activo != null && activo.Fecha.Date == DateTime.Today)
            {
                viewModel.ServicioActivo = activo;
                viewModel.ServicioActivo.Lugar = viewModel.LugaresDisponibles.FirstOrDefault(l => l.IdLugar == activo.IdLugar);
                
                // Cargar registros del servicio activo
                var registros = await _registroNegocio.ListarPorServicioAsync(activo.IdServicio);
                
                // Poblar navegación de registros (Empleado, Empresa)
                // Nota: Esto es necesario porque ADO.NET devuelve IDs planos
                var empleados = await _empleadoNegocio.ListarAsync();
                foreach (var reg in registros)
                {
                    reg.Empleado = empleados.FirstOrDefault(e => e.IdEmpleado == reg.IdEmpleado);
                    if (reg.Empleado != null)
                    {
                        // En un entorno productivo esto vendría de un Join o Cache
                        // Aquí simplificamos asignando el objeto Empresa desde el Empleado
                        // req.Empresa ya debería estar poblado si el SP lo devuelve, si no, lo buscamos
                    }
                }

                viewModel.UltimosRegistros = registros.OrderByDescending(r => r.Hora).Take(10).ToList();
                viewModel.RegistradosHoy = registros.Count;
                
                if (activo.Proyeccion.HasValue && activo.Proyeccion > 0)
                {
                    viewModel.PorcentajeCobertura = (double)viewModel.RegistradosHoy / activo.Proyeccion.Value * 100;
                    viewModel.PendientesHoy = Math.Max(0, activo.Proyeccion.Value - viewModel.RegistradosHoy);
                }
            }

            return View(viewModel);
        }

        // POST: Servicio/Iniciar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Iniciar(int idLugar, int? proyeccion, int invitados = 0)
        {
            // Validar que no haya otro activo
            var existente = await _servicioNegocio.ObtenerActivoAsync(idLugar);
            if (existente != null)
            {
                TempData["ToastType"] = "error";
                TempData["ToastTitle"] = "Error";
                TempData["ToastMessage"] = "Ya existe un servicio activo en este lugar.";
                return RedirectToAction(nameof(Index));
            }

            if (proyeccion < 0 || proyeccion > 1000)
            {
                TempData["ToastType"] = "warning";
                TempData["ToastMessage"] = "La proyección debe estar entre 0 y 1000.";
                return RedirectToAction(nameof(Index));
            }

            await _servicioNegocio.CrearServicioAsync(idLugar, proyeccion, invitados);

            TempData["ToastType"]    = "success";
            TempData["ToastTitle"]   = "Servicio iniciado correctamente";
            return RedirectToAction(nameof(Index));
        }

        // POST: Servicio/Finalizar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(int idServicio, int invitados, int duracionMinutos = 1)
        {
            var totalComensales = await _registroNegocio.ContarAsync(idServicio);

            await _servicioNegocio.FinalizarServicioAsync(idServicio, totalComensales, invitados, Math.Max(1, duracionMinutos));

            TempData["ToastType"]    = "success";
            TempData["ToastTitle"]   = "Servicio finalizado correctamente";
            return RedirectToAction(nameof(Index));
        }

        // POST: Servicio/Registrar (AJAX)
        [HttpPost]
        public async Task<IActionResult> Registrar(string credencial, int idServicio, int idLugar)
        {
            if (string.IsNullOrEmpty(credencial))
                return Json(new { success = false, message = "Credencial vacía" });

            var empleado = await _empleadoNegocio.BuscarPorCredencialAsync(credencial);
            if (empleado == null)
                return Json(new { success = false, message = "Empleado no encontrado" });

            if (!empleado.Estado)
                return Json(new { success = false, message = "Empleado inactivo" });

            var yaRegistrado = await _registroNegocio.YaRegistradoAsync(empleado.IdEmpleado, idServicio);
            if (yaRegistrado)
                return Json(new { success = false, message = "Ya registrado en este servicio" });

            await _registroNegocio.RegistrarAsync(empleado.IdEmpleado, empleado.IdEmpresa, idServicio, idLugar);

            return Json(new { 
                success = true, 
                nombre = $"{empleado.Nombre} {empleado.Apellido}",
                empresa = empleado.Empresa?.Nombre ?? "-",
                hora = DateTime.Now.ToString("HH:mm")
            });
        }
    }
}
