using fianzas_app.Models;
using fianzas_app.Services;
using Microsoft.AspNetCore.Mvc;

namespace fianzas_app.Controllers
{
    public class RevisionController : Controller
    {private readonly SolicitudService _solicitudService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SolicitudService> _logger;
        private readonly ListaService _listaService;
        private readonly AppFianzasContext _dbContext;

        public RevisionController(IHttpContextAccessor httpContextAccessor, ILogger<SolicitudService> logger, AppFianzasContext dbContext, ListaService listaService)
        {
            _solicitudService = new SolicitudService(dbContext, httpContextAccessor, logger, listaService);
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _listaService = listaService;
            _dbContext = dbContext;
        }

        // GET: solicitudes/listar
        [HttpGet("Aprobar-Solicitudes")]
        public async Task<IActionResult> RevisarSolicitudes()
        {
            var solicitudes = await _solicitudService.ListarSolicitudesCompletasAsync();

            // Leer TempData para mensajes de éxito/error que vengan de otras acciones (crear/editar/eliminar)
            ViewBag.MensajeExito = TempData["SuccessMessage"];
            ViewBag.MensajeError = TempData["ErrorMessage"];

            return View("RevisarSolicitudes", solicitudes); // Vista Razor en Views/Solicitudes/ListarSolicitudes.cshtml
        }

        [HttpPost("aprobar-solicitud")]
        public async Task<IActionResult> AprobarSolicitud([FromBody] AprobarSolicitudRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.TipoAprobacion))
            {
                return BadRequest(new { Estado = "Error", Mensaje = "Datos inválidos." });
            }

            var response = await _solicitudService.AprobarSolicitudFianzaAsync(request);

            if (response.Estado == "Exito")
                return Ok(response);

            return BadRequest(response);
        }

    }
}
