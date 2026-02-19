using Microsoft.AspNetCore.Mvc;
using SCA_MVC.Models;
using SCA_MVC.Services;
using SCA_MVC.ViewModels;

namespace SCA_MVC.Controllers
{
    public class EmpleadoController : Controller
    {
        #region Dependencias (Inyección de dependencias)

        // Servicio de negocio para operaciones sobre Empleado
        private readonly IEmpleadoNegocio _empleadoNegocio;

        // Servicio de negocio para obtener empresas (combo de asignación y filtro)
        private readonly IEmpresaNegocio _empresaNegocio;

        public EmpleadoController(IEmpleadoNegocio empleadoNegocio, IEmpresaNegocio empresaNegocio)
        {
            _empleadoNegocio = empleadoNegocio;
            _empresaNegocio = empresaNegocio;
        }

        #endregion

        // =====================================================================

        #region Acciones Públicas (CRUD + Vista principal)

        // GET: Empleado
        // Vista principal del módulo. Muestra la tabla de empleados con filtros
        // (búsqueda por nombre/credencial y filtro por empresa) y el panel lateral
        // de alta/edición del empleado seleccionado.
        public async Task<IActionResult> Index(string? filtro, int? empresaFiltroId, int? idEmpleado, bool nuevo = false)
        {
            // Cargar las empresas activas para poblar el combo del formulario y el filtro
            var empresas = await _empresaNegocio.ListarConEmpleadosAsync();

            // Obtener TODOS los empleados (incluyendo inactivos) y filtrar en memoria
            // para no depender de que el SP de filtro excluya inactivos
            var todosEmpleados = await _empleadoNegocio.ListarAsync();

            // Aplicar filtro de texto (nombre, apellido o credencial)
            IEnumerable<Empleado> empleadosFiltrados = todosEmpleados;
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                empleadosFiltrados = empleadosFiltrados.Where(e =>
                    e.Nombre.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                    e.Apellido.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                    e.IdCredencial.Contains(filtro, StringComparison.OrdinalIgnoreCase));
            }

            // Aplicar filtro de empresa
            if (empresaFiltroId.HasValue && empresaFiltroId.Value > 0)
                empleadosFiltrados = empleadosFiltrados.Where(e => e.IdEmpresa == empresaFiltroId.Value);

            var empleados = empleadosFiltrados.ToList();

            // Join en memoria: poblar la propiedad de navegación Empresa de cada empleado
            // (ADO.NET no hace joins automáticos; el SP devuelve solo el IdEmpresa)
            foreach (var emp in empleados)
                emp.Empresa = empresas.FirstOrDefault(e => e.IdEmpresa == emp.IdEmpresa);

            // Determinar el empleado a mostrar en el panel lateral
            int? seleccionadoId = nuevo ? null : (idEmpleado ?? empleados.FirstOrDefault()?.IdEmpleado);

            var empleadoActual = seleccionadoId.HasValue
                ? await _empleadoNegocio.BuscarPorIdAsync(seleccionadoId.Value)
                : null;

            // Si no hay selección (lista vacía o modo "nuevo"), usar un modelo vacío con Estado=true por defecto
            empleadoActual ??= new Empleado { Estado = true };

            var vm = new EmpleadoViewModel
            {
                Empleados = empleados,
                EmpleadoActual = empleadoActual,
                Filtro = filtro,
                EmpresaFiltroId = empresaFiltroId,
                EmpleadoSeleccionadoId = seleccionadoId,
                Empresas = empresas
            };

            return View(vm);
        }

        // ---------------------------------------------------------------------

        // POST: Empleado/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmpleadoViewModel vm)
        {
            // El ID no aplica para altas: se elimina del ModelState para no bloquear la validación
            ModelState.Remove("EmpleadoActual.IdEmpleado");
            // La empresa de navegación es una propiedad de EF, no viene del form
            ModelState.Remove("EmpleadoActual.Empresa");

            // Validaciones manuales adicionales a las Data Annotations
            if (string.IsNullOrWhiteSpace(vm.EmpleadoActual.IdCredencial))
                ModelState.AddModelError("EmpleadoActual.IdCredencial", "La credencial RFID es obligatoria.");

            if (string.IsNullOrWhiteSpace(vm.EmpleadoActual.Nombre))
                ModelState.AddModelError("EmpleadoActual.Nombre", "El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(vm.EmpleadoActual.Apellido))
                ModelState.AddModelError("EmpleadoActual.Apellido", "El apellido es obligatorio.");

            if (vm.EmpleadoActual.IdEmpresa <= 0)
                ModelState.AddModelError("EmpleadoActual.IdEmpresa", "Debe seleccionar una empresa.");

            if (!ModelState.IsValid)
                return await RecargarConErrores(vm);

            try
            {
                // Verificar que la credencial no esté ya en uso por otro empleado
                var credencialOcupada = await _empleadoNegocio.ExisteCredencialAsync(vm.EmpleadoActual.IdCredencial.Trim());
                if (credencialOcupada)
                {
                    ModelState.AddModelError("EmpleadoActual.IdCredencial", $"La credencial '{vm.EmpleadoActual.IdCredencial}' ya está asignada a otro empleado.");
                    return await RecargarConErrores(vm);
                }

                var nuevoEmpleado = new Empleado
                {
                    IdCredencial = vm.EmpleadoActual.IdCredencial.Trim().ToUpper(),
                    Nombre = vm.EmpleadoActual.Nombre.Trim(),
                    Apellido = vm.EmpleadoActual.Apellido.Trim(),
                    IdEmpresa = vm.EmpleadoActual.IdEmpresa,
                    Estado = vm.EmpleadoActual.Estado
                };

                var idCreado = await _empleadoNegocio.AgregarAsync(nuevoEmpleado);

                TempData["ToastType"] = "success";
                TempData["ToastTitle"] = "Empleado creado";
                TempData["ToastMessage"] = $"El empleado '{nuevoEmpleado.Nombre} {nuevoEmpleado.Apellido}' se creó correctamente.";

                return RedirectToAction(nameof(Index), new { idEmpleado = idCreado });
            }
            catch (Exception ex)
            {
                TempData["ToastType"] = "error";
                TempData["ToastTitle"] = "Error";
                TempData["ToastMessage"] = $"Error al crear el empleado: {ex.Message}";
                return await RecargarConErrores(vm);
            }
        }

        // ---------------------------------------------------------------------

        // POST: Empleado/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmpleadoViewModel vm)
        {
            // Guardia: si el ID no es válido, redirigir al listado sin hacer nada
            if (vm.EmpleadoActual.IdEmpleado <= 0)
                return RedirectToAction(nameof(Index));

            ModelState.Remove("EmpleadoActual.Empresa");

            if (string.IsNullOrWhiteSpace(vm.EmpleadoActual.IdCredencial))
                ModelState.AddModelError("EmpleadoActual.IdCredencial", "La credencial RFID es obligatoria.");

            if (string.IsNullOrWhiteSpace(vm.EmpleadoActual.Nombre))
                ModelState.AddModelError("EmpleadoActual.Nombre", "El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(vm.EmpleadoActual.Apellido))
                ModelState.AddModelError("EmpleadoActual.Apellido", "El apellido es obligatorio.");

            if (vm.EmpleadoActual.IdEmpresa <= 0)
                ModelState.AddModelError("EmpleadoActual.IdEmpresa", "Debe seleccionar una empresa.");

            if (!ModelState.IsValid)
                return await RecargarConErrores(vm);

            try
            {
                // Verificar que la credencial no esté en uso por OTRO empleado distinto al actual
                var empleadoConEsaCredencial = await _empleadoNegocio.BuscarPorCredencialAsync(vm.EmpleadoActual.IdCredencial.Trim());
                if (empleadoConEsaCredencial != null && empleadoConEsaCredencial.IdEmpleado != vm.EmpleadoActual.IdEmpleado)
                {
                    ModelState.AddModelError("EmpleadoActual.IdCredencial", $"La credencial '{vm.EmpleadoActual.IdCredencial}' ya está asignada a otro empleado.");
                    return await RecargarConErrores(vm);
                }

                await _empleadoNegocio.ModificarAsync(new Empleado
                {
                    IdEmpleado = vm.EmpleadoActual.IdEmpleado,
                    IdCredencial = vm.EmpleadoActual.IdCredencial.Trim().ToUpper(),
                    Nombre = vm.EmpleadoActual.Nombre.Trim(),
                    Apellido = vm.EmpleadoActual.Apellido.Trim(),
                    IdEmpresa = vm.EmpleadoActual.IdEmpresa,
                    Estado = vm.EmpleadoActual.Estado
                });

                TempData["ToastType"] = "success";
                TempData["ToastTitle"] = "Empleado actualizado";
                TempData["ToastMessage"] = $"El empleado '{vm.EmpleadoActual.Nombre} {vm.EmpleadoActual.Apellido}' se actualizó correctamente.";

                return RedirectToAction(nameof(Index), new { idEmpleado = vm.EmpleadoActual.IdEmpleado });
            }
            catch (Exception ex)
            {
                TempData["ToastType"] = "error";
                TempData["ToastTitle"] = "Error";
                TempData["ToastMessage"] = $"Error al actualizar el empleado: {ex.Message}";
                return await RecargarConErrores(vm);
            }
        }

        // ---------------------------------------------------------------------

        // POST: Empleado/Delete
        // Realiza la baja lógica (desactivación) del empleado. Si ya está inactivo, no opera.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int idEmpleado)
        {
            if (idEmpleado > 0)
            {
                try
                {
                    var empleado = await _empleadoNegocio.BuscarPorIdAsync(idEmpleado);

                    // Guardia: si ya está desactivado, evitar operación redundante
                    if (empleado != null && !empleado.Estado)
                    {
                        TempData["ToastType"] = "warning";
                        TempData["ToastTitle"] = "Empleado ya inactivo";
                        TempData["ToastMessage"] = $"El empleado '{empleado.NombreCompleto}' ya se encuentra desactivado.";
                        return RedirectToAction(nameof(Index), new { idEmpleado });
                    }

                    await _empleadoNegocio.EliminarAsync(idEmpleado);

                    TempData["ToastType"] = "success";
                    TempData["ToastTitle"] = "Empleado desactivado";
                    TempData["ToastMessage"] = $"El empleado '{empleado?.NombreCompleto}' fue desactivado correctamente.";
                }
                catch (Exception ex)
                {
                    TempData["ToastType"] = "error";
                    TempData["ToastTitle"] = "Error";
                    TempData["ToastMessage"] = $"Error al eliminar el empleado: {ex.Message}";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------------------------------

        // GET: Empleado/VerificarCredencial?credencial=RF001
        // Endpoint AJAX: verifica si una credencial RFID ya está en uso.
        // Devuelve JSON con { estado: 'disponible' | 'propia' | 'ocupada' } para
        // que el JS pueda mostrar mensajes diferenciados en el formulario.
        public async Task<IActionResult> VerificarCredencial(string credencial, int? idEmpleadoActual)
        {
            if (string.IsNullOrWhiteSpace(credencial))
                return Json(new { estado = "disponible" });

            var credencialNorm = credencial.Trim().ToUpper();

            // Paso 1: verificar si la credencial existe en la base de datos
            var existe = await _empleadoNegocio.ExisteCredencialAsync(credencialNorm);

            if (!existe)
                return Json(new { estado = "disponible" });

            // Paso 2: la credencial existe — determinar a quién pertenece
            var propietario = await _empleadoNegocio.BuscarPorCredencialAsync(credencialNorm);

            // Si estamos en modo edición y la credencial pertenece al mismo empleado
            if (idEmpleadoActual.HasValue && idEmpleadoActual.Value > 0
                && propietario?.IdEmpleado == idEmpleadoActual.Value)
            {
                return Json(new { estado = "propia" });
            }

            // La credencial está asignada a otro empleado distinto
            return Json(new { estado = "ocupada" });
        }

        #endregion

        // =====================================================================

        #region Métodos Privados de Soporte

        // Verifica si existe un empleado con el ID dado.
        // Usado como guardia antes de operar sobre un registro.
        private bool EmpleadoExiste(int idEmpleado)
        {
            return _empleadoNegocio.BuscarPorIdAsync(idEmpleado).Result != null;
        }

        // Recarga la vista Index preservando los errores de validación del ModelState
        // y restaurando la lista de empleados y empresas para que la UI sea coherente
        // tras un formulario rechazado (Create o Edit fallido).
        private async Task<IActionResult> RecargarConErrores(EmpleadoViewModel vm)
        {
            // Re-obtener todos los empleados y filtrar en memoria (igual que en Index)
            var todosEmpleados = await _empleadoNegocio.ListarAsync();
            var empresas = await _empresaNegocio.ListarConEmpleadosAsync();

            IEnumerable<Empleado> empleadosFiltrados = todosEmpleados;
            if (!string.IsNullOrWhiteSpace(vm.Filtro))
                empleadosFiltrados = empleadosFiltrados.Where(e =>
                    e.Nombre.Contains(vm.Filtro, StringComparison.OrdinalIgnoreCase) ||
                    e.Apellido.Contains(vm.Filtro, StringComparison.OrdinalIgnoreCase) ||
                    e.IdCredencial.Contains(vm.Filtro, StringComparison.OrdinalIgnoreCase));

            if (vm.EmpresaFiltroId.HasValue && vm.EmpresaFiltroId.Value > 0)
                empleadosFiltrados = empleadosFiltrados.Where(e => e.IdEmpresa == vm.EmpresaFiltroId.Value);

            var empleados = empleadosFiltrados.ToList();
            foreach (var emp in empleados)
                emp.Empresa = empresas.FirstOrDefault(e => e.IdEmpresa == emp.IdEmpresa);

            vm.Empleados = empleados;
            vm.Empresas = empresas;
            vm.EmpleadoSeleccionadoId = vm.EmpleadoActual.IdEmpleado > 0
                ? vm.EmpleadoActual.IdEmpleado
                : vm.EmpleadoSeleccionadoId;

            return View(nameof(Index), vm);
        }

        #endregion
    }
}
