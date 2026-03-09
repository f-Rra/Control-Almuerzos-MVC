using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SCA_MVC.Helpers;
using SCA_MVC.Models;
using SCA_MVC.ViewModels;

namespace SCA_MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuarioController : Controller
    {
        #region Dependencias

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuarioController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #endregion

        // =====================================================================

        #region Acciones Públicas

        // GET: Usuario
        // Vista principal: tabla de usuarios con búsqueda y panel lateral de alta/edición.
        public async Task<IActionResult> Index(string? filtro, string? idUsuario, bool nuevo = false)
        {
            var roles = _roleManager.Roles
                .Select(r => r.Name!)
                .OrderBy(n => n)
                .ToList();

            var todosUsuarios = _userManager.Users.ToList();

            // Filtro por nombre de usuario, nombre completo o email
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                todosUsuarios = todosUsuarios.Where(u =>
                    u.NombreUsuario.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                    u.NombreCompleto.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                    (u.Email ?? "").Contains(filtro, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Construir ítems de lista con el rol de cada usuario
            var items = new List<UsuarioListItem>();
            foreach (var u in todosUsuarios)
            {
                var userRoles = await _userManager.GetRolesAsync(u);
                items.Add(new UsuarioListItem
                {
                    Id             = u.Id,
                    NombreUsuario  = u.NombreUsuario,
                    NombreCompleto = u.NombreCompleto,
                    Email          = u.Email ?? "",
                    Rol            = userRoles.FirstOrDefault() ?? "Sin rol",
                    Activo         = !u.LockoutEnd.HasValue || u.LockoutEnd <= DateTimeOffset.UtcNow
                });
            }

            // Determinar el usuario a mostrar en el panel lateral
            string? seleccionadoId = nuevo ? null : (idUsuario ?? items.FirstOrDefault()?.Id);

            UsuarioFormViewModel formModel = new();
            if (!string.IsNullOrEmpty(seleccionadoId))
            {
                var user = await _userManager.FindByIdAsync(seleccionadoId);
                if (user != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    formModel = new UsuarioFormViewModel
                    {
                        Id            = user.Id,
                        NombreUsuario = user.NombreUsuario,
                        Nombre        = user.Nombre,
                        Apellido      = user.Apellido,
                        Email         = user.Email ?? "",
                        Rol           = userRoles.FirstOrDefault() ?? ""
                    };
                }
            }

            var vm = new UsuarioViewModel
            {
                Usuarios             = items,
                UsuarioActual        = formModel,
                Filtro               = filtro,
                UsuarioSeleccionadoId = seleccionadoId,
                RolesDisponibles     = roles
            };

            return View(vm);
        }

        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Prefix = "UsuarioActual")] UsuarioFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NuevaContrasena))
                ModelState.AddModelError(nameof(model.NuevaContrasena), "La contraseña es obligatoria al crear un usuario.");

            if (!ModelState.IsValid)
                return await RebuildAndReturn(model, null);

            var user = new ApplicationUser
            {
                UserName       = model.Email,
                Email          = model.Email,
                NombreUsuario  = model.NombreUsuario,
                Nombre         = model.Nombre,
                Apellido       = model.Apellido,
                EmailConfirmed = true
            };

            try
            {
                var resultado = await _userManager.CreateAsync(user, model.NuevaContrasena!);

                if (!resultado.Succeeded)
                {
                    foreach (var error in resultado.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return await RebuildAndReturn(model, null);
                }

                if (!string.IsNullOrEmpty(model.Rol))
                    await _userManager.AddToRoleAsync(user, model.Rol);

                TempData.MostrarExito($"Usuario '{model.NombreUsuario}' creado correctamente.", "¡Creado!");
                return RedirectToAction(nameof(Index), new { idUsuario = user.Id });
            }
            catch
            {
                TempData.MostrarError("Ocurrió un error al crear el usuario.", "Error");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Usuario/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind(Prefix = "UsuarioActual")] UsuarioFormViewModel model)
        {
            // La contraseña es opcional en edición — se elimina del estado de modelo
            ModelState.Remove(nameof(model.NuevaContrasena));

            if (!ModelState.IsValid)
                return await RebuildAndReturn(model, model.Id);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                TempData.MostrarError("Usuario no encontrado.", "Error");
                return RedirectToAction(nameof(Index));
            }

            try
            {
                user.NombreUsuario = model.NombreUsuario;
                user.Nombre        = model.Nombre;
                user.Apellido      = model.Apellido;
                user.Email         = model.Email;
                user.UserName      = model.Email;
                await _userManager.UpdateAsync(user);

                // Actualizar rol: quitar los actuales y asignar el nuevo
                var rolesActuales = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, rolesActuales);
                if (!string.IsNullOrEmpty(model.Rol))
                    await _userManager.AddToRoleAsync(user, model.Rol);

                // Resetear contraseña solo si se ingresó una nueva
                if (!string.IsNullOrWhiteSpace(model.NuevaContrasena))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, model.NuevaContrasena);
                }

                TempData.MostrarExito($"Usuario '{model.NombreUsuario}' actualizado correctamente.", "¡Guardado!");
                return RedirectToAction(nameof(Index), new { idUsuario = user.Id });
            }
            catch
            {
                TempData.MostrarError("Ocurrió un error al actualizar el usuario.", "Error");
                return RedirectToAction(nameof(Index), new { idUsuario = model.Id });
            }
        }

        // POST: Usuario/Delete
        // Alterna el estado del usuario: desactiva si está activo, reactiva si está desactivado.
        // Usa LockoutEnd = DateTimeOffset.MaxValue para bloquear la cuenta sin eliminar datos.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string idUsuario)
        {
            var user = await _userManager.FindByIdAsync(idUsuario);
            if (user == null)
            {
                TempData.MostrarError("Usuario no encontrado.", "Error");
                return RedirectToAction(nameof(Index));
            }

            try
            {
                bool activo = !user.LockoutEnd.HasValue || user.LockoutEnd <= DateTimeOffset.UtcNow;

                if (activo)
                {
                    await _userManager.SetLockoutEnabledAsync(user, true);
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                    TempData.MostrarAdvertencia($"Usuario '{user.NombreUsuario}' desactivado.", "Desactivado");
                }
                else
                {
                    await _userManager.SetLockoutEndDateAsync(user, null);
                    TempData.MostrarExito($"Usuario '{user.NombreUsuario}' reactivado.", "¡Reactivado!");
                }

                return RedirectToAction(nameof(Index), new { idUsuario });
            }
            catch
            {
                TempData.MostrarError("Ocurrió un error al cambiar el estado del usuario.", "Error");
                return RedirectToAction(nameof(Index), new { idUsuario });
            }
        }

        #endregion

        // =====================================================================

        #region Métodos Privados de Soporte

        private bool UsuarioExiste(string id) =>
            _userManager.Users.Any(u => u.Id == id);

        // Reconstruye el ViewModel completo para volver a mostrar el Index con errores de validación.
        private async Task<IActionResult> RebuildAndReturn(UsuarioFormViewModel model, string? seleccionadoId)
        {
            var roles = _roleManager.Roles
                .Select(r => r.Name!)
                .OrderBy(n => n)
                .ToList();

            var todos = _userManager.Users.ToList();
            var items = new List<UsuarioListItem>();

            foreach (var u in todos)
            {
                var userRoles = await _userManager.GetRolesAsync(u);
                items.Add(new UsuarioListItem
                {
                    Id             = u.Id,
                    NombreUsuario  = u.NombreUsuario,
                    NombreCompleto = u.NombreCompleto,
                    Email          = u.Email ?? "",
                    Rol            = userRoles.FirstOrDefault() ?? "Sin rol",
                    Activo         = !u.LockoutEnd.HasValue || u.LockoutEnd <= DateTimeOffset.UtcNow
                });
            }

            var vm = new UsuarioViewModel
            {
                Usuarios             = items,
                UsuarioActual        = model,
                UsuarioSeleccionadoId = seleccionadoId,
                RolesDisponibles     = roles
            };

            return View("Index", vm);
        }

        #endregion
    }
}
