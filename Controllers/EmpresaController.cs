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

        // GET: api/empresas/listar
        [HttpGet("Listado-Empresas")]
        public async Task<IActionResult> ListarEmpresas()
        {
            var empresas = await _empresaService.ListarEmpresasAsync();

            if (empresas == null || empresas.Count == 0)
            {
                TempData["ErrorMessage"] = "No se encontraron empresas registradas.";
                empresas = new List<EmpresaResponse>(); // Pasas una lista vacía del tipo correcto
            }

            return View(empresas);
        }




        /// <summary>
        /// VIsta de crear empresas
        /// </summary>
        /// <returns></returns>
        [HttpGet("Registrar-Empresa")]
        public async Task<IActionResult> RegistrarEmpresa()
        {
            // Llenas el ViewBag con los tipos de empresa para el formulario
            var tipoEmpresas = await _listaService.ListarTipoEmpresaAsync();
            var corredors = await _listaService.ListarCorredorAsync();
            ViewBag.TipoEmpresa = tipoEmpresas;
            ViewBag.Corredores = corredors;
            return View();
        }

        // POST: empresas/crear
        [HttpPost("Crwr-empresa")]
        public async Task<IActionResult> CrearEmpresa(EmpresaRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.EmpNombre) || string.IsNullOrWhiteSpace(request.EmpRUC))
            {
                TempData["ErrorMessage"] = "Datos inválidos para registrar la empresa.";
                return RedirectToAction(nameof(ListarEmpresas));
            }

            var response = await _empresaService.InsertarEmpresaAsync(request);

            if (response.Estado == "Error")
            {
                TempData["ErrorMessage"] = response.Mensaje;
                return RedirectToAction(nameof(RegistrarEmpresa));
            }

            TempData["SuccessMessage"] = response.Mensaje ?? "Empresa registrada exitosamente.";
            return RedirectToAction(nameof(ListarEmpresas));
        }

        /// <summary>
        /// Vista de actualización de empresas
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("actualizar/{empresaId}")]
        public async Task<IActionResult> ActualizarEmpresa(int empresaId)
        {
            var empresa = await _empresaService.ObtenerEmpresaPorIdAsync(empresaId);

            if (empresa == null)
                return NotFound(new { Estado = "Error", Mensaje = "Empresa no encontrada." });

            // Llenas el ViewBag con los tipos de empresa para el formulario
            var tipoEmpresas = await _listaService.ListarTipoEmpresaAsync();
            var corredors = await _listaService.ListarCorredorAsync();
            ViewBag.TipoEmpresa = tipoEmpresas;
            ViewBag.Corredores = corredors;

            // Retornas la vista para actualizar la empresa
            return View("ActualizarEmpresa", empresa); // Esta vista puede ser ActualizarEmpresa.cshtml
        }

        // POST: empresas/editar
        [HttpPost("editar")]
        public async Task<IActionResult> EditarEmpresa(EmpresaRequest request)
        {
            if (request == null || request.EmpresaId <= 0)
            {
                TempData["Error"] = "Datos inválidos para actualizar la empresa.";
                return RedirectToAction(nameof(ListarEmpresas));
            }

            var response = await _empresaService.ActualizarEmpresaAsync(request);

            if (response.Estado == "Error")
            {
                TempData["ErrorMessage"] = response.Mensaje ?? "No se pudo actualizar la empresa.";
                return RedirectToAction(nameof(ListarEmpresas));
            }

            TempData["SuccessMessage"] = response.Mensaje ?? "Empresa actualizada exitosamente.";
            return RedirectToAction(nameof(ListarEmpresas));
        }

        // GET: empresas/historial/5
        [HttpGet("historial/{empresaId}")]
        public async Task<IActionResult> HistorialEmpresa(int empresaId, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            if (empresaId <= 0)
            {
                TempData["Error"] = "El ID de empresa es inválido.";
                return RedirectToAction(nameof(ListarEmpresas));
            }

            var historial = await _empresaService.ObtenerHistorialEmpresaAsync(empresaId, fechaInicio, fechaFin);

            if (historial == null || historial.Count == 0)
            {
                TempData["ErrorMessage"] = "No se encontraron registros en el historial para la empresa.";
                return RedirectToAction(nameof(ListarEmpresas));
            }

            ViewBag.MensajeExito = TempData["SuccessMessage"];
            ViewBag.MensajeError = TempData["ErrorMessage"];

            return View(historial); // Vista: Views/Empresas/HistorialEmpresa.cshtml
        }
    }
}
