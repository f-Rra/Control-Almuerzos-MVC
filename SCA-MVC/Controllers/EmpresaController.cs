using Microsoft.AspNetCore.Mvc;
using SCA_MVC.Models;
using SCA_MVC.Services;
using SCA_MVC.ViewModels;

namespace SCA_MVC.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly IEmpresaNegocio _empresaNegocio;
        private readonly IEmpleadoNegocio _empleadoNegocio;
        private readonly IRegistroNegocio _registroNegocio;

        public EmpresaController(
            IEmpresaNegocio empresaNegocio,
            IEmpleadoNegocio empleadoNegocio,
            IRegistroNegocio registroNegocio)
        {
            _empresaNegocio = empresaNegocio;
            _empleadoNegocio = empleadoNegocio;
            _registroNegocio = registroNegocio;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? filtro, int? idEmpresa, bool nueva = false)
        {
            var empresas = string.IsNullOrWhiteSpace(filtro)
                ? await _empresaNegocio.ListarConEmpleadosAsync()
                : await _empresaNegocio.FiltrarAsync(filtro);

            int? seleccionadaId = nueva ? null : (idEmpresa ?? empresas.FirstOrDefault()?.IdEmpresa);

            var empresaActual = seleccionadaId.HasValue
                ? await _empresaNegocio.BuscarPorIdAsync(seleccionadaId.Value)
                : null;

            empresaActual ??= new Empresa { Estado = true };

            var estadisticas = await ObtenerEstadisticasAsync(seleccionadaId);

            var vm = new EmpresaViewModel
            {
                Empresas = empresas,
                EmpresaActual = empresaActual,
                Filtro = filtro,
                EmpresaSeleccionadaId = seleccionadaId,
                TotalEmpleados = estadisticas.totalEmpleados,
                Inactivos = estadisticas.inactivos,
                AsistenciasMesActual = estadisticas.asistenciasMes,
                PromedioDiario = estadisticas.promedioDiario
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmpresaViewModel vm)
        {
            ModelState.Remove("EmpresaActual.IdEmpresa");

            if (string.IsNullOrWhiteSpace(vm.EmpresaActual.Nombre))
            {
                ModelState.AddModelError("EmpresaActual.Nombre", "El nombre de la empresa es obligatorio.");
            }

            if (!ModelState.IsValid)
            {
                return await RecargarConErrores(vm);
            }

            try
            {
                var nuevaEmpresa = new Empresa
                {
                    Nombre = vm.EmpresaActual.Nombre.Trim(),
                    Estado = vm.EmpresaActual.Estado
                };

                var idCreado = await _empresaNegocio.AgregarAsync(nuevaEmpresa);

                TempData["ToastType"] = "success";
                TempData["ToastTitle"] = "Empresa creada";
                TempData["ToastMessage"] = $"La empresa '{nuevaEmpresa.Nombre}' se creó correctamente.";

                return RedirectToAction(nameof(Index), new { idEmpresa = idCreado });
            }
            catch (Exception ex)
            {
                TempData["ToastType"] = "error";
                TempData["ToastTitle"] = "Error";
                TempData["ToastMessage"] = $"Error al crear la empresa: {ex.Message}";
                return await RecargarConErrores(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmpresaViewModel vm)
        {
            if (vm.EmpresaActual.IdEmpresa <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(vm.EmpresaActual.Nombre))
            {
                ModelState.AddModelError("EmpresaActual.Nombre", "El nombre de la empresa es obligatorio.");
            }

            if (!ModelState.IsValid)
            {
                return await RecargarConErrores(vm);
            }

            try
            {
                await _empresaNegocio.ModificarAsync(new Empresa
                {
                    IdEmpresa = vm.EmpresaActual.IdEmpresa,
                    Nombre = vm.EmpresaActual.Nombre.Trim(),
                    Estado = vm.EmpresaActual.Estado
                });

                TempData["ToastType"] = "success";
                TempData["ToastTitle"] = "Empresa actualizada";
                TempData["ToastMessage"] = $"La empresa '{vm.EmpresaActual.Nombre}' se actualizó correctamente.";

                return RedirectToAction(nameof(Index), new { idEmpresa = vm.EmpresaActual.IdEmpresa });
            }
            catch (Exception ex)
            {
                TempData["ToastType"] = "error";
                TempData["ToastTitle"] = "Error";
                TempData["ToastMessage"] = $"Error al actualizar la empresa: {ex.Message}";
                return await RecargarConErrores(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int idEmpresa)
        {
            if (idEmpresa > 0)
            {
                try
                {
                    var empresa = await _empresaNegocio.BuscarPorIdAsync(idEmpresa);

                    if (empresa != null && !empresa.Estado)
                    {
                        TempData["ToastType"] = "warning";
                        TempData["ToastTitle"] = "Empresa desactivada";
                        TempData["ToastMessage"] = $"La empresa '{empresa.Nombre}' ya se encuentra desactivada.";
                        return RedirectToAction(nameof(Index), new { idEmpresa });
                    }

                    await _empresaNegocio.EliminarAsync(idEmpresa);

                    TempData["ToastType"] = "success";
                    TempData["ToastTitle"] = "Empresa desactivada";
                    TempData["ToastMessage"] = $"La empresa '{empresa?.Nombre}' fue desactivada correctamente.";
                }
                catch (Exception ex)
                {
                    TempData["ToastType"] = "error";
                    TempData["ToastTitle"] = "Error";
                    TempData["ToastMessage"] = $"Error al eliminar la empresa: {ex.Message}";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int idEmpresa)
        {
            var empresa = await _empresaNegocio.BuscarPorIdAsync(idEmpresa);
            if (empresa == null)
            {
                return NotFound();
            }

            var estadisticas = await ObtenerEstadisticasAsync(idEmpresa);

            return Json(new
            {
                idEmpresa = empresa.IdEmpresa,
                nombre = empresa.Nombre,
                estado = empresa.Estado,
                totalEmpleados = estadisticas.totalEmpleados,
                inactivos = estadisticas.inactivos,
                asistenciasMesActual = estadisticas.asistenciasMes,
                promedioDiario = estadisticas.promedioDiario
            });
        }

        private async Task<(int totalEmpleados, int inactivos, int asistenciasMes, decimal promedioDiario)> ObtenerEstadisticasAsync(int? idEmpresa)
        {
            if (!idEmpresa.HasValue)
            {
                return (0, 0, 0, 0m);
            }

            var empleados = await _empleadoNegocio.FiltrarEmpleadosAsync(null, idEmpresa.Value);
            var totalEmpleados = empleados.Count;
            var inactivos = empleados.Count(e => !e.Estado);

            var inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var fin = DateTime.Today;
            var asistencias = await _registroNegocio.PorEmpresaYFechaAsync(idEmpresa.Value, inicioMes, fin);
            var asistenciasMes = asistencias.Count;
            var diasTranscurridos = Math.Max(1, DateTime.Today.Day);
            var promedioDiario = Math.Round(asistenciasMes / (decimal)diasTranscurridos, 1);

            return (totalEmpleados, inactivos, asistenciasMes, promedioDiario);
        }

        private async Task<IActionResult> RecargarConErrores(EmpresaViewModel vm)
        {
            var empresas = string.IsNullOrWhiteSpace(vm.Filtro)
                ? await _empresaNegocio.ListarConEmpleadosAsync()
                : await _empresaNegocio.FiltrarAsync(vm.Filtro);

            var estadisticas = await ObtenerEstadisticasAsync(vm.EmpresaActual.IdEmpresa > 0 ? vm.EmpresaActual.IdEmpresa : vm.EmpresaSeleccionadaId);

            vm.Empresas = empresas;
            vm.EmpresaSeleccionadaId = vm.EmpresaActual.IdEmpresa > 0 ? vm.EmpresaActual.IdEmpresa : vm.EmpresaSeleccionadaId;
            vm.TotalEmpleados = estadisticas.totalEmpleados;
            vm.Inactivos = estadisticas.inactivos;
            vm.AsistenciasMesActual = estadisticas.asistenciasMes;
            vm.PromedioDiario = estadisticas.promedioDiario;

            return View(nameof(Index), vm);
        }
    }
}
