using System;
using System.IO;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using fianzas_app.Models;
using fianzas_app.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;

namespace fianzas_app.Controllers
{
    public class DocumentController : Controller
    {
        private readonly SolicitudService _sfdService;
        private readonly DocumentosService _documentService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _templateFolder;

        public DocumentController(SolicitudService sfdService, IWebHostEnvironment webHostEnvironment,DocumentosService documentosService)
        {
            _sfdService = sfdService;
            _documentService = documentosService;
            _webHostEnvironment = webHostEnvironment;
            _templateFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "plantillas");
        }

        /// <summary>
        /// Descarga documentos fijos según un mapeo de id a nombre de archivo.
        /// id = 1: Convenio, id = 2: Solicitud, id = 3: Pagaré, id = 4: Prenda.
        /// </summary>
        [HttpGet]
        public IActionResult Download(int id)
        {
            // Mapeo de id a nombres de archivo (con extensión .pdf)
            string[] fileNames = new string[]
            {
                "CONVENIO_DE_FIANZAS_Form.pdf", // id = 1
                "solicitud_form.pdf",           // id = 2
                "pagare_form.pdf",              // id = 3
                "prenda_form.pdf"               // id = 4
            };

            if (id < 1 || id > fileNames.Length)
                return NotFound("El documento no existe.");

            string fileName = fileNames[id - 1];
            string filePath = Path.Combine(_templateFolder, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("El documento no se encontró.");

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", fileName);
        }

        /// <summary>
        /// Genera dinámicamente un PDF a partir de una plantilla, usando iTextSharp.
        /// El parámetro 'type' determina el tipo de fianza: "anticipo", "cumplimiento" o "aduanera".
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GeneratePdf(string type, int id)
        {
            // El parámetro "type" indica el tipo de fianza.
            string templateFile;
            string outputFileName;

            switch (type.ToLower())
            {
                case "anticipo":
                    templateFile = "bua_solicitud_form.pdf";
                    outputFileName = "Solicitud_Buen_Uso_del_Anticipo.pdf";
                    break;
                case "cumplimiento":
                    templateFile = "fcc_solicitud_form.pdf";
                    outputFileName = "Solicitud_Fiel_Cumplimiento_del_Contrato.pdf";
                    break;
                case "aduanera":
                    templateFile = "ga_solicitud_form.pdf";
                    outputFileName = "Solicitud_Garantia_Aduanera.pdf";
                    break;
                default:
                    return BadRequest("Tipo de fianza no soportado.");
            }

            // Obtener los datos reales de la solicitud mediante el servicio.
            SolicitudFianzaDetalleResponse solicitudDetalle = await _sfdService.ObtenerSolicitudPorIdAsync(id);
            if (solicitudDetalle == null)
            {
                return NotFound("Solicitud no encontrada.");
            }

            string templatePath = Path.Combine(_templateFolder, templateFile);
            if (!System.IO.File.Exists(templatePath))
            {
                return NotFound("La plantilla no se encontró.");
            }

            using var memoryStream = new MemoryStream();
            PdfReader pdfReader = new PdfReader(templatePath);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, memoryStream);
            AcroFields formFields = pdfStamper.AcroFields;

            // Rellenar los campos de la plantilla con los datos obtenidos.
            formFields.SetField("txt_publico", solicitudDetalle.SfSectorFianza);
            formFields.SetField("txt_privado", solicitudDetalle.SfSectorFianza);
            formFields.SetField("txt_legal", (solicitudDetalle.SfAprobacionLegal.HasValue && solicitudDetalle.SfAprobacionLegal.Value == 1) ? "X" : "");
            formFields.SetField("txt_tecnica", (solicitudDetalle.SfAprobacionTecnica.HasValue && solicitudDetalle.SfAprobacionTecnica.Value == 1) ? "X" : "");
            formFields.SetField("txt_empresa_nombre", solicitudDetalle.EmpresaNombre);
            formFields.SetField("campo_fecha", solicitudDetalle.SfFechaSolicitud.ToShortDateString());
            formFields.SetField("campo_tipo_fianza", solicitudDetalle.TipoSolicitudNombre);
            // O, si prefieres mostrar solo números con dos decimales:
            formFields.SetField("campo_monto_fianza", solicitudDetalle.SfMontoFianza.HasValue
      ? solicitudDetalle.SfMontoFianza.Value.ToString("C")
      : "0");
            formFields.SetField("campo_monto_contrato", solicitudDetalle.SfMontoContrato.HasValue
                ? solicitudDetalle.SfMontoContrato.Value.ToString("C")
                : "0");


            // Puedes agregar más campos, por ejemplo:
            // formFields.SetField("campo_direccion", solicitudDetalle.BenDireccion);
            // formFields.SetField("campo_beneficiario", solicitudDetalle.BenNombre);

            pdfStamper.FormFlattening = true;
            pdfStamper.Close();
            pdfReader.Close();

            return File(memoryStream.ToArray(), "application/pdf", outputFileName);
        }
        /// <summary>
        /// Inserta los documentos subidos (convertidos a bytes) mediante un servicio.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> InsertSfd(
            [FromForm] int sfdId,
            [FromForm] DateTime? sfdFechaSubida,
            IFormFile documento1,
            IFormFile documento2,
            IFormFile documento3,
            IFormFile documento4)
        {
            try
            {
                // Convertir cada archivo a arreglo de bytes
                byte[] documento1Bytes = null;
                byte[] documento2Bytes = null;
                byte[] documento3Bytes = null;
                byte[] documento4Bytes = null;

                if (documento1 != null && documento1.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await documento1.CopyToAsync(ms);
                        documento1Bytes = ms.ToArray();
                    }
                }

                if (documento2 != null && documento2.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await documento2.CopyToAsync(ms);
                        documento2Bytes = ms.ToArray();
                    }
                }

                if (documento3 != null && documento3.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await documento3.CopyToAsync(ms);
                        documento3Bytes = ms.ToArray();
                    }
                }

                if (documento4 != null && documento4.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await documento4.CopyToAsync(ms);
                        documento4Bytes = ms.ToArray();
                    }
                }

                // Llamada al servicio pasando los bytes en lugar de rutas de archivo.
                int insertedId = await _documentService.InsertSfdAsync(
                    sfdId,
                    documento1Bytes,
                    documento2Bytes,
                    documento3Bytes,
                    documento4Bytes,
                    sfdFechaSubida ?? DateTime.Now,
                    DateTime.Now, // Ejemplo: fecha de vencimiento
                    null);      // Ejemplo: sfdPoliza (ajusta según tu modelo)

                if (insertedId > 0)
                {
                    TempData["SuccessMessage"] = "El documento se insertó correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se pudo insertar el documento.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }
            // Redirigir a la acción de Listar Solicitudes
            return RedirectToAction("ListarSolicitudes", "Solicitud");
        }
    }
}
