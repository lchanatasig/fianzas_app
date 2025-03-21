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


        // GET: solicitudes/listar
        [HttpGet("Listado-Solicitudes")]
        public async Task<IActionResult> ListarSolicitudes()
        {
            var solicitudes = await _solicitudService.ListarSolicitudesCompletasAsync();

            // Leer TempData para mensajes de éxito/error que vengan de otras acciones (crear/editar/eliminar)
            ViewBag.MensajeExito = TempData["SuccessMessage"];
            ViewBag.MensajeError = TempData["ErrorMessage"];

            return View("ListarSolicitudes", solicitudes); // Vista Razor en Views/Solicitudes/ListarSolicitudes.cshtml
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
                TempData["ErrorMessage"] = "Datos inválidos para registrar la solicitud.";
                return RedirectToAction(nameof(RegistrarSolicitudFianza));
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
                    TempData["ErrorMessage"] = "Error al procesar las prendas: " + ex.Message;
                    return RedirectToAction(nameof(RegistrarSolicitudFianza));
                }
            }

            // ✅ LÓGICA PARA VERIFICAR SI REQUIERE PRENDA
            bool requierePrenda = request.SfMontoFianza > 416000; // Aquí la regla de negocio, ajústala si cambia.

            if (requierePrenda)
            {
                // Si requiere prenda y no hay ninguna asociada, error.
                if (request.Prendas == null || request.Prendas.Count == 0)
                {
                    TempData["ErrorMessage"] = "Debe registrar al menos una prenda para este tipo de solicitud.";
                    return RedirectToAction(nameof(RegistrarSolicitudFianza));
                }

                // Si requiere archivo y no se subió, error.
                if (PrenArchivo == null || PrenArchivo.Length == 0)
                {
                    TempData["ErrorMessage"] = "Debe adjuntar un archivo para la prenda.";
                    return RedirectToAction(nameof(RegistrarSolicitudFianza));
                }

                // ✅ Si todo bien, procesamos el archivo y lo asignamos a la prenda (a la primera en este caso).
                using (var memoryStream = new MemoryStream())
                {
                    await PrenArchivo.CopyToAsync(memoryStream);
                    var archivoBytes = memoryStream.ToArray();

                    request.Prendas[0].PrenArchivo = archivoBytes; // Puedes cambiar el índice según lógica.
                }
            }
            else
            {
                // Si NO requiere prenda, pero subieron un archivo, es opcional hacer algo o ignorarlo.
                if (PrenArchivo != null && PrenArchivo.Length > 0)
                {
                    // Puedes ignorarlo o dar una advertencia.
                    TempData["WarningMessage"] = "Archivo adjunto cargado, pero no es necesario para esta solicitud.";
                    // O si quieres, puedes eliminar el archivo: no hacemos nada.
                }

                // Y puedes también ignorar el request.Prendas si no es necesario.
                request.Prendas = new List<PrendaDto>(); // Aseguramos que no pase null si el servicio lo espera como lista vacía.
            }

            // ✅ LLAMAR AL SERVICIO
            var response = await _solicitudService.InsertarSolicitudCompletaAsync(request);

            if (response.Estado != "Exito")
            {
                TempData["Error"] = response.Mensaje ?? "No se pudo registrar la solicitud.";
                return RedirectToAction(nameof(RegistrarSolicitudFianza));
            }

            TempData["Success"] = response.Mensaje;
            return RedirectToAction(nameof(ListarSolicitudes));
        }

        [HttpGet("editar-solicitud/{id}")]
        public async Task<IActionResult> EditarSolicitudFianza(int id)
        {
            var solicitud = await _solicitudService.ObtenerSolicitudPorIdAsync(id);
            var empresas = await _empresaService.ListarEmpresasAsync();
            ViewBag.TiposSolicitud = await _listaService.ListarTipoFianzasAsync();

            ViewBag.Empresas = empresas;
            
            if (solicitud == null || solicitud.SfId == 0)
            {
                TempData["ErrorMessage"] = "Solicitud no encontrada.";
                return RedirectToAction("Index");
            }
            ModelState.Clear();

            return View(solicitud);
        }



    }
}
