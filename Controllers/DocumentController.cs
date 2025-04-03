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
using System.Globalization;

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
        [HttpGet("Download")]
        public async Task<IActionResult> Download(int solicitudId, int docTypeId)
        {
            // Se obtiene el detalle de la solicitud usando el id.
            var solicitudDetalle = await _sfdService.ObtenerSolicitudPorIdAsync(solicitudId);
            if (solicitudDetalle == null)
                return NotFound("No se encontró la solicitud.");

            // Selección del template y nombre base según docTypeId.
            string templateFile = string.Empty;
            string docName = string.Empty;

            if (docTypeId == 1 || docTypeId == 5 || docTypeId == 8)
            {
                templateFile = "CONVENIO_DE_FIANZAS_Form.pdf";
                docName = "Convenio";
            }
            else if (docTypeId == 3 || docTypeId == 6 || docTypeId == 9)
            {
                templateFile = "pagare_form.pdf";
                docName = "Pagare";
            }
            else if (docTypeId == 4 || docTypeId == 7 || docTypeId == 10)
            {
                templateFile = "prenda_form.pdf";
                docName = "Prenda";
            }
            else
            {
                return NotFound("Documento no reconocido.");
            }

            string templatePath = Path.Combine(_templateFolder, templateFile);
            if (!System.IO.File.Exists(templatePath))
                return NotFound("El template PDF no se encontró.");

            // Usamos MemoryStream para generar el PDF al vuelo.
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (PdfReader pdfReader = new PdfReader(templatePath))
                using (PdfStamper pdfStamper = new PdfStamper(pdfReader, outputStream))
                {
                    AcroFields formFields = pdfStamper.AcroFields;

                    // Relleno de campos según el tipo de documento
                    if (docTypeId == 1 || docTypeId == 5 || docTypeId == 8)
                    {
                        // Relleno para Convenio
                        formFields.SetField("sf_id", solicitudDetalle.SfId.ToString());
                        formFields.SetField("txt_empresa_nombre", solicitudDetalle.EmpresaNombre);
                        formFields.SetField("txt_ci_empresa", solicitudDetalle.EmpRuc);
                        formFields.SetField("txt_direccion_empresa", solicitudDetalle.EmpUbicacion);
                        formFields.SetField("txt_telefono_empresa", solicitudDetalle.EmpTelefono);
                        formFields.SetField("txt_beneficiario_nombre", solicitudDetalle.BenNombre);
                        formFields.SetField("txt_direccion_beneficiario", solicitudDetalle.BenDireccion);
                        formFields.SetField("txt_objeto_contrato", solicitudDetalle.SfObjetoContrato);
                        formFields.SetField("txt_telefono_beneficiario", solicitudDetalle.BenTelefono);
                        formFields.SetField("txt_monto_garantia", solicitudDetalle.SfMontoFianza.HasValue
                            ? solicitudDetalle.SfMontoFianza.Value.ToString("C")
                            : "0");
                        formFields.SetField("txt_dias_plazo", solicitudDetalle.SfPlazoGarantiaDias.ToString());
                        formFields.SetField("txt_fecha_vigencia", solicitudDetalle.SfInicioVigencia.ToString("dd/MM/yyyy"));
                        formFields.SetField("txt_legal", (solicitudDetalle.SfAprobacionLegal.HasValue && solicitudDetalle.SfAprobacionLegal.Value == 1) ? "X" : "");
                        formFields.SetField("txt_tecnica", (solicitudDetalle.SfAprobacionTecnica.HasValue && solicitudDetalle.SfAprobacionTecnica.Value == 1) ? "X" : "");
                        formFields.SetField("txt_email_empresa", solicitudDetalle.EmpEmail);
                        formFields.SetField("txt_email_beneficiario", solicitudDetalle.BenEmail);
                        formFields.SetField("txt_lugar_fecha", $"{solicitudDetalle.EmpUbicacion} {solicitudDetalle.SfFechaSolicitud:dd/MM/yyyy}");
                        var fechaActual = DateOnly.FromDateTime(DateTime.Now);
                        string fechaFormateada = fechaActual.ToString("d 'de' MMMM 'de' yyyy", new CultureInfo("es-ES"));
                        formFields.SetField("txt_fecha_emision", fechaFormateada);
                    }
                    else if (docTypeId == 3 || docTypeId == 6 || docTypeId == 9)
                    {
                        // Relleno para Pagaré
                        formFields.SetField("sf_id", solicitudDetalle.SfId.ToString());
                        formFields.SetField("txt_empresa_nombre", solicitudDetalle.EmpresaNombre);
                        formFields.SetField("txt_ci_empresa", solicitudDetalle.EmpRuc);
                        formFields.SetField("txt_direccion_empresa", solicitudDetalle.EmpUbicacion);
                        formFields.SetField("txt_telefono_empresa", solicitudDetalle.EmpTelefono);
                        formFields.SetField("txt_beneficiario_nombre", solicitudDetalle.BenNombre);
                        formFields.SetField("txt_direccion_beneficiario", solicitudDetalle.BenDireccion);
                        formFields.SetField("txt_telefono_beneficiario", solicitudDetalle.BenTelefono);
                        formFields.SetField("txt_monto_garantia", solicitudDetalle.SfMontoFianza.HasValue
                            ? solicitudDetalle.SfMontoFianza.Value.ToString("C")
                            : "0");
                        formFields.SetField("txt_monto_contrato", solicitudDetalle.SfMontoContrato.HasValue
                            ? solicitudDetalle.SfMontoContrato.Value.ToString("C")
                            : "0");
                        formFields.SetField("txt_dias_plazo", solicitudDetalle.SfPlazoGarantiaDias.ToString());
                        formFields.SetField("txt_fecha_vigencia", solicitudDetalle.SfInicioVigencia.ToString("dd/MM/yyyy"));
                        formFields.SetField("txt_legal", (solicitudDetalle.SfAprobacionLegal.HasValue && solicitudDetalle.SfAprobacionLegal.Value == 1) ? "X" : "");
                        formFields.SetField("txt_tecnica", (solicitudDetalle.SfAprobacionTecnica.HasValue && solicitudDetalle.SfAprobacionTecnica.Value == 1) ? "X" : "");
                        formFields.SetField("txt_email_empresa", solicitudDetalle.EmpEmail);
                        formFields.SetField("txt_email_beneficiario", solicitudDetalle.BenEmail);
                        formFields.SetField("txt_lugar_fecha", $"{solicitudDetalle.EmpUbicacion} {solicitudDetalle.SfFechaSolicitud:dd/MM/yyyy}");
                        var fechaActual = DateOnly.FromDateTime(DateTime.Now);
                        string fechaFormateada = fechaActual.ToString("d 'de' MMMM 'de' yyyy", new CultureInfo("es-ES"));
                        formFields.SetField("txt_fecha_emision", fechaFormateada);
                    }
                    else if (docTypeId == 4 || docTypeId == 7 || docTypeId == 10)
                    {
                        // Relleno para Prenda
                        formFields.SetField("sf_id", solicitudDetalle.SfId.ToString());
                        formFields.SetField("txt_empresa_nombre", solicitudDetalle.EmpresaNombre);
                        formFields.SetField("txt_ci_empresa", solicitudDetalle.EmpRuc);
                        formFields.SetField("txt_nombre_fianza", solicitudDetalle.TipoSolicitudNombre);
                        formFields.SetField("txt_direccion_empresa", solicitudDetalle.EmpUbicacion);
                        formFields.SetField("txt_telefono_empresa", solicitudDetalle.EmpTelefono);
                        formFields.SetField("txt_beneficiario_nombre", solicitudDetalle.BenNombre);
                        formFields.SetField("txt_direccion_beneficiario", solicitudDetalle.BenDireccion);
                        formFields.SetField("txt_telefono_beneficiario", solicitudDetalle.BenTelefono);
                        formFields.SetField("txt_fecha_emision", DateOnly.FromDateTime(DateTime.Now).ToString("dd/MM/yyyy"));
                        formFields.SetField("txt_monto_garantia", solicitudDetalle.SfMontoFianza.HasValue
                            ? solicitudDetalle.SfMontoFianza.Value.ToString("C")
                            : "0");
                        formFields.SetField("txt_dias_plazo", solicitudDetalle.SfPlazoGarantiaDias.ToString());
                        formFields.SetField("txt_fecha_vigencia", solicitudDetalle.SfInicioVigencia.ToString("dd/MM/yyyy"));
                        formFields.SetField("txt_legal", (solicitudDetalle.SfAprobacionLegal.HasValue && solicitudDetalle.SfAprobacionLegal.Value == 1) ? "X" : "");
                        formFields.SetField("txt_tecnica", (solicitudDetalle.SfAprobacionTecnica.HasValue && solicitudDetalle.SfAprobacionTecnica.Value == 1) ? "X" : "");
                        formFields.SetField("txt_email_empresa", solicitudDetalle.EmpEmail);
                        formFields.SetField("txt_email_beneficiario", solicitudDetalle.BenEmail);
                        formFields.SetField("txt_lugar_fecha", $"{solicitudDetalle.EmpUbicacion} {solicitudDetalle.SfFechaSolicitud:dd/MM/yyyy}");

                        // Asignar información de la prenda
                        if (solicitudDetalle.Prendas != null && solicitudDetalle.Prendas.Any())
                        {
                            var prenda = solicitudDetalle.Prendas.First();
                            formFields.SetField("txt_prenda_tipo", prenda.PrenTipo);
                            formFields.SetField("txt_prenda_bien", prenda.PrenBien);
                            formFields.SetField("txt_prenda_descripcion", prenda.PrenDescripcion);
                            formFields.SetField("txt_prenda_valor", prenda.PrenValor.ToString("C"));
                            formFields.SetField("txt_prenda_ubicacion", prenda.PrenUbicacion);
                            formFields.SetField("txt_prenda_custodio", prenda.PrenCustodio);
                            formFields.SetField("txt_prenda_fecha_constatacion", prenda.PrenFechaConstatacion.ToString("dd/MM/yyyy"));
                            formFields.SetField("txt_prenda_responsable", prenda.PrenResponsableConstatacion);
                        }
                        else
                        {
                            // En caso de no tener datos de prenda
                            formFields.SetField("txt_prenda_tipo", "Sin datos");
                            formFields.SetField("txt_prenda_bien", "Sin datos");
                            formFields.SetField("txt_prenda_descripcion", "Sin datos");
                            formFields.SetField("txt_prenda_valor", "Sin datos");
                            formFields.SetField("txt_prenda_ubicacion", "Sin datos");
                            formFields.SetField("txt_prenda_custodio", "Sin datos");
                            formFields.SetField("txt_prenda_fecha_constatacion", "Sin datos");
                            formFields.SetField("txt_prenda_responsable", "Sin datos");
                        }

                        // Si SfFinVigencia está definido, separamos día, mes y año
                        if (solicitudDetalle.SfFinVigencia.HasValue)
                        {
                            string dia = solicitudDetalle.SfFinVigencia.Value.Day.ToString("D2");
                            string mes = solicitudDetalle.SfFinVigencia.Value.Month.ToString("D2");
                            string anio = solicitudDetalle.SfFinVigencia.Value.Year.ToString();
                            formFields.SetField("txt_dias", dia);
                            formFields.SetField("txt_mes", mes);
                            formFields.SetField("txt_anio", anio);
                        }
                        else
                        {
                            formFields.SetField("txt_dias", string.Empty);
                            formFields.SetField("txt_mes", string.Empty);
                            formFields.SetField("txt_anio", string.Empty);
                        }
                    }

                    // Aplanamos el formulario para evitar ediciones posteriores.
                    pdfStamper.FormFlattening = true;
                }

                byte[] pdfBytes = outputStream.ToArray();

                // Se genera un número aleatorio para el nombre del archivo.
                Random rnd = new Random();
                int randomNumber = rnd.Next(1000, 9999);
                string fileDownloadName = $"{docName}_{solicitudId}_{randomNumber}.pdf";
                return File(pdfBytes, "application/pdf", fileDownloadName);
            }
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

            if (solicitudDetalle.SfSectorFianza.Equals("Público", StringComparison.OrdinalIgnoreCase))
            {
                formFields.SetField("txt_publico", "X");
                formFields.SetField("txt_privado", "");
            }
            else if (solicitudDetalle.SfSectorFianza.Equals("Privado", StringComparison.OrdinalIgnoreCase))
            {
                formFields.SetField("txt_publico", "");
                formFields.SetField("txt_privado", "X");
            }

            formFields.SetField("txt_legal", (solicitudDetalle.SfAprobacionLegal.HasValue && solicitudDetalle.SfAprobacionLegal.Value == 1) ? "X" : "");
            formFields.SetField("txt_tecnica", (solicitudDetalle.SfAprobacionTecnica.HasValue && solicitudDetalle.SfAprobacionTecnica.Value == 1) ? "X" : "");
            formFields.SetField("txt_empresa_nombre", solicitudDetalle.EmpresaNombre);
            formFields.SetField("txt_direccion_empresa", solicitudDetalle.EmpUbicacion);
            formFields.SetField("txt_ci_empresa", solicitudDetalle.EmpRuc);
            formFields.SetField("txt_email_empresa", solicitudDetalle.EmpEmail);
            formFields.SetField("txt_telefono_empresa", solicitudDetalle.EmpTelefono);
            formFields.SetField("txt_beneficiario_nombre", solicitudDetalle.BenNombre);
            formFields.SetField("txt_direccion_beneficiario", solicitudDetalle.BenDireccion);
            formFields.SetField("txt_ci_beneficiario", solicitudDetalle.BenCiRuc);
            formFields.SetField("txt_email_beneficiario", solicitudDetalle.BenEmail);
            formFields.SetField("txt_lugar_fecha", $"{solicitudDetalle.EmpUbicacion} {solicitudDetalle.SfFechaSolicitud:dd/MM/yyyy}");
            formFields.SetField("txt_telefono_beneficiario", solicitudDetalle.BenTelefono);
            // Extraemos día, mes y año con formato de dos dígitos para día y mes
            if (solicitudDetalle.SfFinVigencia.HasValue)
            {
                string dia = solicitudDetalle.SfFinVigencia.Value.Day.ToString("D2");
                string mes = solicitudDetalle.SfFinVigencia.Value.Month.ToString("D2");
                string anio = solicitudDetalle.SfFinVigencia.Value.Year.ToString();

                formFields.SetField("txt_dias", dia);
                formFields.SetField("txt_mes", mes);
                formFields.SetField("txt_anio", anio);
            }
            else
            {
                // Manejar el caso en el que SfFinVigencia es null, por ejemplo:
                formFields.SetField("txt_dias", string.Empty);
                formFields.SetField("txt_mes", string.Empty);
                formFields.SetField("txt_anio", string.Empty);
            }


            formFields.SetField("txt_objeto_contrato", solicitudDetalle.SfObjetoContrato);
            formFields.SetField("txt_plazo_dias", solicitudDetalle.SfPlazoGarantiaDias.ToString());
            // O, si prefieres mostrar solo números con dos decimales:
            formFields.SetField("txt_monto_contrato", solicitudDetalle.SfMontoFianza.HasValue
      ? solicitudDetalle.SfMontoFianza.Value.ToString("C")
      : "0");
            formFields.SetField("txt_monto_garantia", solicitudDetalle.SfMontoContrato.HasValue
                ? solicitudDetalle.SfMontoContrato.Value.ToString("C")
                : "0");

            if (solicitudDetalle.SfMontoContrato.HasValue &&
    solicitudDetalle.SfMontoFianza.HasValue &&
    solicitudDetalle.SfMontoContrato.Value != 0)
            {
                decimal porcentaje = (solicitudDetalle.SfMontoContrato.Value / solicitudDetalle.SfMontoFianza.Value) * 100;
                formFields.SetField("txt_porcentaje_monto", porcentaje.ToString("N2") + "%");
            }
            else
            {
                formFields.SetField("txt_porcentaje_monto", "0%");
            }

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
