using fianzas_app.Models;
using fianzas_app.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        [HttpPost("crear")]
        public async Task<IActionResult> CrearSolicitud(SolicitudCompletaRequest request, IFormFile PrenArchivo)
        {
            if (request == null || request.SfEmpId <= 0 || request.SfMontoFianza <= 0)
            {
                TempData["Error"] = "Datos inválidos para registrar la solicitud.";
                return RedirectToAction(nameof(CrearSolicitud));
            }

            // ✅ DESERIALIZACIÓN DEL JSON (PrendasJson viene del input hidden)
            if (!string.IsNullOrWhiteSpace(request.PrendasJson))
            {
                try
                {
                    request.Prendas = JsonConvert.DeserializeObject<List<PrendaDto>>(request.PrendasJson);
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al procesar las prendas: " + ex.Message;
                    return RedirectToAction(nameof(CrearSolicitud));
                }
            }

            // ✅ PROCESAR ARCHIVO SUBIDO
            if (PrenArchivo != null && PrenArchivo.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await PrenArchivo.CopyToAsync(memoryStream);
                    var archivoBytes = memoryStream.ToArray();

                    // 🔥 Aquí decides a qué prenda asignarlo.
                    // Si hay una prenda, se lo asignamos a la primera.
                    if (request.Prendas != null && request.Prendas.Count > 0)
                    {
                        request.Prendas[0].PrenArchivo = archivoBytes;  // Puedes cambiar el índice según la lógica que prefieras.
                    }
                    else
                    {
                        TempData["Error"] = "No se encontraron prendas para asociar el archivo.";
                        return RedirectToAction(nameof(RegistrarSolicitudFianza));
                    }
                }
            }
            else
            {
                // Opcional: validar si es obligatorio subir el archivo
                TempData["Error"] = "Debe adjuntar un archivo para la prenda.";
                return RedirectToAction(nameof(RegistrarSolicitudFianza));
            }

            // ✅ LLAMAR AL SERVICIO
            var response = await _solicitudService.InsertarSolicitudCompletaAsync(request);

            if (response.Estado != "Exito")
            {
                TempData["Error"] = response.Mensaje ?? "No se pudo registrar la solicitud.";
                return RedirectToAction(nameof(RegistrarSolicitudFianza));
            }

            TempData["Success"] = response.Mensaje;
            return RedirectToAction(nameof(RegistrarSolicitudFianza));
        }


    }
}
