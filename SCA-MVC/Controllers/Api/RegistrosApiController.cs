using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCA_MVC.Api;
using SCA_MVC.Helpers;
using SCA_MVC.Services;

namespace SCA_MVC.Controllers.Api
{
    // El registro de un comensal desde un lector RFID de red. A diferencia de las pantallas, no hay
    // sesión de navegador: el lector se identifica con su clave (X-Api-Key) y, como la clave
    // pertenece a un lugar, registra siempre en el servicio activo de ese lugar.
    //
    // No lleva token antiforgery: ese token existe para defender a los usuarios que entran con
    // cookie, y esta API no acepta cookies, solo la clave.
    [ApiController]
    [Route("api/registros")]
    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationHandler.Esquema)]
    public class RegistrosApiController : ControllerBase
    {
        private readonly IEmpleadoNegocio _empleados;
        private readonly IServicioNegocio _servicios;
        private readonly IRegistroNegocio _registros;
        private readonly TimeProvider _reloj;

        public RegistrosApiController(
            IEmpleadoNegocio empleados,
            IServicioNegocio servicios,
            IRegistroNegocio registros,
            TimeProvider reloj)
        {
            _empleados = empleados;
            _servicios = servicios;
            _registros = registros;
            _reloj = reloj;
        }

        /// <summary>Registra el almuerzo del dueño de una credencial en el servicio activo del lugar del lector.</summary>
        /// <response code="201">Registrado.</response>
        /// <response code="400">Falta la credencial o es demasiado larga.</response>
        /// <response code="401">Falta la clave del lector o no es válida.</response>
        /// <response code="404">Ninguna persona tiene esa credencial.</response>
        /// <response code="409">Esa persona ya está registrada en este servicio.</response>
        /// <response code="422">Hay una credencial, pero no se puede registrar ahora: el empleado está inactivo o no hay servicio activo.</response>
        [HttpPost]
        [ProducesResponseType(typeof(RegistroCreadoRespuesta), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Registrar([FromBody] RegistrarLecturaSolicitud solicitud)
        {
            var idLugar = int.Parse(User.FindFirstValue(ApiKeyAuthenticationHandler.ClaimLugar)!);

            // [Required] deja pasar una credencial hecha solo de espacios, así que se revisa acá también
            var credencial = solicitud.Credencial.Trim();
            if (credencial.Length == 0)
            {
                ModelState.AddModelError(nameof(solicitud.Credencial), "La credencial es obligatoria.");
                return ValidationProblem(ModelState);
            }

            var empleado = await _empleados.BuscarPorCredencialAsync(credencial);
            if (empleado == null)
                return Error(StatusCodes.Status404NotFound, "Credencial desconocida",
                    "Ninguna persona tiene esa credencial.");

            if (!empleado.Estado)
                return Error(StatusCodes.Status422UnprocessableEntity, "Empleado inactivo",
                    "La persona está dada de baja y no puede registrarse.");

            // Igual que las pantallas web: un servicio que quedó abierto de otro día no cuenta
            var servicio = await _servicios.ObtenerActivoAsync(idLugar);
            if (servicio == null || servicio.Fecha.Date != _reloj.Hoy())
                return Error(StatusCodes.Status422UnprocessableEntity, "No hay servicio activo",
                    "No hay un servicio activo hoy en el lugar de este lector.");

            var resultado = await _registros.RegistrarAsync(empleado.IdEmpleado, empleado.IdEmpresa, servicio.IdServicio, idLugar);

            if (resultado == ResultadoRegistro.YaRegistrado)
                return Error(StatusCodes.Status409Conflict, "Ya registrado",
                    "Esa persona ya está registrada en este servicio.");

            // Cualquier otro motivo (el servicio se cerró en el medio, por ejemplo) se explica igual
            if (resultado != ResultadoRegistro.Registrado)
                return Error(StatusCodes.Status422UnprocessableEntity, "No se pudo registrar", resultado.Mensaje());

            var ahora = _reloj.Ahora();
            return StatusCode(StatusCodes.Status201Created, new RegistroCreadoRespuesta
            {
                IdServicio = servicio.IdServicio,
                Empleado = empleado.NombreCompleto,
                Empresa = empleado.Empresa?.Nombre ?? "-",
                Lugar = servicio.Lugar?.Nombre ?? "-",
                Fecha = DateOnly.FromDateTime(ahora),
                Hora = ahora.ToString("HH:mm")
            });
        }

        // Un error en el formato estándar de las APIs (ProblemDetails): estado, título corto y detalle
        private ObjectResult Error(int estado, string titulo, string detalle) =>
            Problem(detail: detalle, statusCode: estado, title: titulo);
    }
}
