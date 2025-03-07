using fianzas_app.Models;
using fianzas_app.Services;
using Microsoft.AspNetCore.Mvc;

namespace fianzas_app.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly EmpresaService _empresaService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EmpresaService> _logger;
        private readonly ListaService _listaService;
        public EmpresaController(EmpresaService empresaService, IHttpContextAccessor httpContextAccessor, ILogger<EmpresaService> logger, ListaService listaService)
        {
            _empresaService = empresaService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _listaService = listaService;
        }

        //[HttpGet("listar")]
        //public async Task<IActionResult> ListarEmpresas()
        //{
        //    var empresas = await _empresaService.ListarEmpresasAsync();
        //    return View(empresas);
        //}

        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// VIsta de crear empresas
        /// </summary>
        /// <returns></returns>
        [HttpGet("Registrar-Empresa")]
        public async Task<IActionResult> RegistrarEmpresa()
        {
            var tipoEmpresas = await _listaService.ListarTipoEmpresaAsync();
            ViewBag.TipoEmpresa = tipoEmpresas;
            return View();
        }

        // Endpoint para insertar una empresa
        [HttpPost("Crear-Empresa")]
        public async Task<IActionResult> CrearEmpresa([FromBody] EmpresaRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.EmpNombre) || string.IsNullOrWhiteSpace(request.EmpRUC))
            {
                return BadRequest(new { mensaje = "Datos inválidos para registrar la empresa." });
            }

            var response = await _empresaService.InsertarEmpresaAsync(request);

            if (response.Estado == "Error")
            {
                return BadRequest(new { mensaje = response.Mensaje });
            }

            // Redirigir a la lista de empresas después de la inserción
            return RedirectToAction(nameof(Index));
        }
    }
}
