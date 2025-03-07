using fianzas_app.Models;
using fianzas_app.Services;
using Microsoft.AspNetCore.Mvc;

namespace fianzas_app.Controllers
{
    public class UsuarioController : Controller
    {

        private readonly UsuarioService _usuarioService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UsuarioService> _logger;
        private readonly ListaService _listaService;
        public UsuarioController(IHttpContextAccessor httpContextAccessor, ILogger<UsuarioService> logger, AppFianzasContext dbContext, ListaService listaService)
        {
            _usuarioService = new UsuarioService(httpContextAccessor, logger, dbContext);
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _listaService = listaService;
        }

        /// <summary>
        /// Vista  lista usuarios
        /// </summary>
        /// <returns></returns>
        [HttpGet("Listar-Usuario")]
        public async Task<ActionResult<List<AuthResponse>>> ListarUsuarios()
        {
            var usuarios = await _usuarioService.ListarUsuariosAsync();
            return View(usuarios);
        }

        [HttpGet("Nuevo-Usuario")]
        public async Task<IActionResult> NuevoUsuario()
        {
            var perfiles = await _listaService.ListarPerfilesAsync();
            ViewBag.Perfiles = perfiles;
            return View();
        }
        /// <summary>
        /// Registrar un nuevo usuario
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Registrar-Usuario")]
        public async Task<IActionResult> CrearUsuario([FromBody] Usuario request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UsuarioNombres) ||
                string.IsNullOrWhiteSpace(request.UsuarioPassword) || string.IsNullOrWhiteSpace(request.UsuarioCi))
            {
                return BadRequest(new { success = false, mensaje = "Datos inválidos para crear el usuario." });
            }

            var response = await _usuarioService.InsertarUsuarioAsync(request);

            if (response.Estado == "Error")
            {
                return BadRequest(new { success = false, mensaje = response.Mensaje });
            }

            return Ok(new { success = true, mensaje = "Usuario creado correctamente." });
        }


        /// <summary>
        /// Obtener usuario por ID para mostrar en la vista de actualización
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns></returns>

        [HttpGet("Actualizacion-Usuario/{usuarioId}")]
        public async Task<IActionResult> ActualizarUsuario(int usuarioId)
        {
            var perfiles = await _listaService.ListarPerfilesAsync();
            ViewBag.Perfiles = perfiles;

            var usuario = await _usuarioService.ObtenerUsuarioPorIdAsync(usuarioId);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado." });
            }

            return View(usuario);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Actualizar-Usuario")]
        public async Task<IActionResult> ActualizacionUsuario([FromBody] Usuario request)
        {
            if (request == null || request.UsuarioId == null || string.IsNullOrWhiteSpace(request.UsuarioNombres) ||
                string.IsNullOrWhiteSpace(request.UsuarioCi))
            {
                return BadRequest(new { success = false, mensaje = "Datos inválidos para actualizar el usuario." });
            }

            var response = await _usuarioService.ActualizarUsuarioAsync(request);

            if (response.Estado == "Error")
            {
                return BadRequest(new { success = false, mensaje = response.Mensaje });
            }

            return Ok(new { success = true, mensaje = "Usuario actualizado correctamente." });
        }

        /// <summary>
        ///  Endpoint para cambiar el estado de un usuario
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        [HttpPost("CambiarEstadoUsuario")]
        public async Task<IActionResult> CambiarEstadoUsuario(int usuarioId)
        {
            if (usuarioId <= 0)
            {
                return BadRequest(new { mensaje = "ID de usuario inválido." });
            }

            var response = await _usuarioService.CambiarEstadoUsuarioAsync(usuarioId);

            if (response.Estado == "Error")
            {
                return BadRequest(new { mensaje = response.Mensaje });
            }

            // Redirigir a la lista de usuarios después de cambiar el estado
            return RedirectToAction(nameof(ListarUsuarios));
        }

    }
}
