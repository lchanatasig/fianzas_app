using fianzas_app.Services;
using Microsoft.AspNetCore.Mvc;

namespace fianzas_app.Controllers
{
    public class AutenticacionController : Controller
    {

        private readonly AutenticacionService _authService;

        public AutenticacionController(AutenticacionService authService)
        {
            _authService = authService;
        }
        public IActionResult Inicio_sesion()
        {
            return View();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthResponse request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UsuarioCI) || string.IsNullOrWhiteSpace(request.UsuarioPassword))
            {
                return BadRequest(new { mensaje = "Credenciales inválidas." });
            }

            var response = await _authService.ValidarCredencialesAsync(request.UsuarioCI, request.UsuarioPassword);

            if (response.Estado == "Error")
            {
                return Unauthorized(new { mensaje = response.Mensaje });
            }

            return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
        }
    }
}
