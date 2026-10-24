using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCA_MVC.Helpers;
using SCA_MVC.Services;
using SCA_MVC.ViewModels;

namespace SCA_MVC.Controllers
{
    // Alta, lista y revocación de los lectores RFID que llaman a la API. Solo el administrador.
    [Authorize(Roles = "Admin")]
    public class LectorController : Controller
    {
        private readonly ILectorNegocio _lectores;
        private readonly ILugarNegocio _lugares;

        public LectorController(ILectorNegocio lectores, ILugarNegocio lugares)
        {
            _lectores = lectores;
            _lugares = lugares;
        }

        // GET: Lector
        public async Task<IActionResult> Index() => View(await ArmarAsync(new LectorFormViewModel()));

        // POST: Lector/Crear
        // La respuesta es la propia página con la clave, sin redirigir: así la clave no pasa por
        // una dirección ni por TempData (que viaja en una cookie) y no hay forma de volver a verla.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind(Prefix = nameof(LectoresViewModel.Nuevo))] LectorFormViewModel nuevo)
        {
            // [Required] deja pasar un nombre hecho solo de espacios
            if (string.IsNullOrWhiteSpace(nuevo.Nombre))
                ModelState.AddModelError($"{nameof(LectoresViewModel.Nuevo)}.{nameof(nuevo.Nombre)}", "El nombre es obligatorio.");

            if (!ModelState.IsValid)
                return View(nameof(Index), await ArmarAsync(nuevo));

            try
            {
                var (lector, clave) = await _lectores.CrearAsync(nuevo.Nombre, nuevo.IdLugar!.Value);

                // Que ni el navegador ni un proxy guarden una copia de la página con la clave
                Response.Headers.CacheControl = "no-store";

                var lugar = (await _lugares.ListarAsync()).FirstOrDefault(l => l.IdLugar == lector.IdLugar);
                return View("Creado", new LectorCreadoViewModel
                {
                    Nombre = lector.Nombre,
                    Lugar = lugar?.Nombre ?? "-",
                    Clave = clave
                });
            }
            catch (InvalidOperationException)
            {
                ModelState.AddModelError($"{nameof(LectoresViewModel.Nuevo)}.{nameof(nuevo.IdLugar)}", "El lugar no existe.");
                return View(nameof(Index), await ArmarAsync(nuevo));
            }
        }

        // POST: Lector/Revocar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revocar(int idLector)
        {
            if (await _lectores.RevocarAsync(idLector))
                TempData.MostrarExito("Lector revocado. Su clave ya no funciona.");
            else
                TempData.MostrarError(MensajesConstantes.ERROR_NOT_FOUND);

            return RedirectToAction(nameof(Index));
        }

        private async Task<LectoresViewModel> ArmarAsync(LectorFormViewModel nuevo) => new()
        {
            Lectores = await _lectores.ListarAsync(),
            Lugares = await _lugares.ListarAsync(),
            Nuevo = nuevo
        };
    }
}
