using fianzas_app.Models;
using fianzas_app.Services;
using Microsoft.AspNetCore.Mvc;

namespace fianzas_app.Controllers
{
    public class SolicitudController : Controller
    {
        private readonly SolicitudService _solicitudService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SolicitudService> _logger;
        private readonly ListaService _listaService;
        private readonly AppFianzasContext _dbContext;
        private readonly EmpresaService _empresaService;
        public SolicitudController(IHttpContextAccessor httpContextAccessor, ILogger<SolicitudService> logger, AppFianzasContext dbContext, ListaService listaService, EmpresaService empresaService)
        {
            _solicitudService = new SolicitudService(dbContext, httpContextAccessor, logger, listaService);
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _listaService = listaService;
            _dbContext = dbContext;
            _empresaService = empresaService;
        }

        [HttpGet("Registro-Fianza")]
        public async Task<IActionResult> RegistrarSolicitudFianza()
        {
            ViewBag.TiposSolicitud = await _listaService.ListarTipoFianzasAsync();
            var empresas = await _empresaService.ListarEmpresasAsync();
            ViewBag.Empresas = empresas;
            ViewBag.Mensaje = "Empresas listadas correctamente";  // Opcional, si deseas mostrar el mensaje en la vista

            return View();
        }
    }
}
