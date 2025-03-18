using System;
using System.IO;
using System.Threading.Tasks;
using fianzas_app.Models;
using fianzas_app.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fianzas_app.Controllers
{
    public class DocumentController : Controller
    {
        private readonly DocumentosService _sfdService;
        private readonly string _documentsFolder;

        public DocumentController(DocumentosService sfdService)
        {
            _sfdService = sfdService;
            _documentsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "plantillas");

        }


        // Carpeta donde se encuentran los documentos PDF para descarga.

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
            {
                return NotFound("El documento no existe.");
            }

            string fileName = fileNames[id - 1];
            string filePath = Path.Combine(_documentsFolder, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("El documento no se encontró.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", fileName);
        }


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

                // Llamada al servicio pasando los bytes en lugar de rutas de archivo
                int insertedId = await _sfdService.InsertSfdAsync(
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
