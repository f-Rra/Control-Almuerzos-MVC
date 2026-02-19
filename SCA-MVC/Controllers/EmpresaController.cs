using Microsoft.AspNetCore.Mvc;
using SCA_MVC.Models;
using SCA_MVC.Services;
using SCA_MVC.ViewModels;

namespace SCA_MVC.Controllers
{
    public class EmpresaController : Controller
    {
        #region Dependencias (Inyección de dependencias)

        // Servicio de negocio para operaciones sobre Empresa
        private readonly IEmpresaNegocio _empresaNegocio;

        // Servicio de negocio para consultar empleados vinculados a una empresa
        private readonly IEmpleadoNegocio _empleadoNegocio;

        // Servicio de negocio para consultar registros/asistencias
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

        #endregion

        // =====================================================================

        #region Acciones Públicas (CRUD + Vista principal)

        // GET: Empresa
        // Muestra el listado de empresas con soporte de filtrado y el panel lateral
        // de detalle/edición de la empresa seleccionada, junto a sus estadísticas.
        // 'nueva = true' limpia el panel para dar de alta una empresa nueva.
        public async Task<IActionResult> Index(string? filtro, int? idEmpresa, bool nueva = false)
        {
            // Si hay filtro activo, se usa FiltrarAsync; de lo contrario se listan todas con conteo de empleados
            var empresas = string.IsNullOrWhiteSpace(filtro)
                ? await _empresaNegocio.ListarConEmpleadosAsync()
                : await _empresaNegocio.FiltrarAsync(filtro);

            // Si se pidió panel de "nueva empresa", no se preselecciona ninguna;
            // si no, se usa el ID recibido o el primero de la lista como fallback
            int? seleccionadaId = nueva ? null : (idEmpresa ?? empresas.FirstOrDefault()?.IdEmpresa);

            // Cargar datos de la empresa seleccionada (o null si el panel es de "nueva")
            var empresaActual = seleccionadaId.HasValue
                ? await _empresaNegocio.BuscarPorIdAsync(seleccionadaId.Value)
                : null;

            // Si no hay empresa seleccionada (lista vacía o modo "nueva"), usar un modelo vacío con Estado=true por defecto
            empresaActual ??= new Empresa { Estado = true };

            // Calcular las estadísticas del panel lateral para la empresa seleccionada
            var estadisticas = await ObtenerEstadisticasAsync(seleccionadaId);

            // Armar el ViewModel que agrupa todos los datos necesarios para la vista
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

        // ---------------------------------------------------------------------

        // POST: Empresa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmpresaViewModel vm)
        {
            // El ID no aplica para altas: se elimina del ModelState para no bloquear la validación
            ModelState.Remove("EmpresaActual.IdEmpresa");

            // Validación manual: el nombre es obligatorio
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
                // Crear el objeto de dominio limpiando espacios del nombre
                var nuevaEmpresa = new Empresa
                {
                    Nombre = vm.EmpresaActual.Nombre.Trim(),
                    Estado = vm.EmpresaActual.Estado
                };

                // Persistir y recuperar el ID autogenerado para redirigir al panel de la nueva empresa
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

        // ---------------------------------------------------------------------

        // POST: Empresa/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmpresaViewModel vm)
        {
            // Guardia: si el ID no es válido, redirigir al listado sin hacer nada
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

        // ---------------------------------------------------------------------

        // POST: Empresa/Delete
        // Realiza la baja lógica (desactivación) de la empresa. Si ya está inactiva, no opera.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int idEmpresa)
        {
            if (idEmpresa > 0)
            {
                try
                {
                    var empresa = await _empresaNegocio.BuscarPorIdAsync(idEmpresa);

                    // Guardia: si ya está desactivada, evitar operación redundante
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

        // ---------------------------------------------------------------------

        // GET: Empresa/Detalle?idEmpresa={id}
        // Endpoint AJAX: devuelve JSON con datos y estadísticas de la empresa indicada.
        // Se consume desde site.js para actualizar el panel lateral sin recargar la página.
        public async Task<IActionResult> Detalle(int idEmpresa)
        {
            if (!EmpresaExiste(idEmpresa))
            {
                return NotFound();
            }

            var empresa = await _empresaNegocio.BuscarPorIdAsync(idEmpresa);
            var estadisticas = await ObtenerEstadisticasAsync(idEmpresa);

            return Json(new
            {
                idEmpresa = empresa!.IdEmpresa,
                nombre = empresa.Nombre,
                estado = empresa.Estado,
                totalEmpleados = estadisticas.totalEmpleados,
                inactivos = estadisticas.inactivos,
                asistenciasMesActual = estadisticas.asistenciasMes,
                promedioDiario = estadisticas.promedioDiario
            });
        }

        #endregion

        // =====================================================================

        #region Métodos Privados de Soporte

        // Verifica si existe una empresa con el ID dado (equivalente al XExists de los ejemplos EF).
        // Usado como guardia antes de operar sobre un registro.
        private bool EmpresaExiste(int idEmpresa)
        {
            return _empresaNegocio.BuscarPorIdAsync(idEmpresa).Result != null;
        }

        // Calcula las estadísticas de panel lateral para una empresa:
        // total de empleados, inactivos, asistencias del mes corriente y promedio diario.
        // Retorna una tupla con nombre. Si idEmpresa es null, devuelve todo en cero.
        private async Task<(int totalEmpleados, int inactivos, int asistenciasMes, decimal promedioDiario)> ObtenerEstadisticasAsync(int? idEmpresa)
        {
            if (!idEmpresa.HasValue)
            {
                return (0, 0, 0, 0m);
            }

            var empleados = await _empleadoNegocio.FiltrarEmpleadosAsync(null, idEmpresa.Value);
            var totalEmpleados = empleados.Count;
            var inactivos = empleados.Count(e => !e.Estado);

            // Calcular el rango del mes actual (del primer día al día de hoy)
            var inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var fin = DateTime.Today;

            var asistencias = await _registroNegocio.PorEmpresaYFechaAsync(idEmpresa.Value, inicioMes, fin);
            var asistenciasMes = asistencias.Count;

            // Math.Max(1, ...) evita división por cero el primer día del mes
            var diasTranscurridos = Math.Max(1, DateTime.Today.Day);
            var promedioDiario = Math.Round(asistenciasMes / (decimal)diasTranscurridos, 1);

            return (totalEmpleados, inactivos, asistenciasMes, promedioDiario);
        }

        // ---------------------------------------------------------------------

        // Recarga la vista Index preservando los errores de validación del ModelState
        // y restaurando la lista de empresas y estadísticas para que la UI sea coherente
        // tras un formulario rechazado (Create o Edit fallido).
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

        #endregion
    }
}
