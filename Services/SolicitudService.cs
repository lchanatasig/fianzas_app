using fianzas_app.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlTypes;

namespace fianzas_app.Services
{
    public class SolicitudService
    {
        private readonly AppFianzasContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SolicitudService> _logger;

        private readonly ListaService _listaService;

        public SolicitudService(AppFianzasContext dbContext, IHttpContextAccessor httpContextAccessor, ILogger<SolicitudService> logger, ListaService listaService)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _listaService = listaService;
        }


        public async Task<AuthResponse> InsertarSolicitudCompletaAsync(SolicitudCompletaRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_insertar_solicitud_completa", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // --- Agregar parámetros ---
                        command.Parameters.AddWithValue("@ben_id", request.BenId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ben_nombre", request.BenNombre ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ben_direccion", request.BenDireccion ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ben_ci_ruc", request.BenCiRuc ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ben_email", request.BenEmail ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ben_telefono", request.BenTelefono ?? (object)DBNull.Value);

                        command.Parameters.AddWithValue("@sf_emp_id", request.SfEmpId);
                        command.Parameters.AddWithValue("@sf_tpos_id", request.SfTposId);
                        command.Parameters.AddWithValue("@sf_estf_id", request.SfEstfId);
                        command.Parameters.AddWithValue("@sf_fecha_solicitud", DateTime.Now);
                        command.Parameters.AddWithValue("@sf_inicio_vigencia", request.SfInicioVigencia);
                        command.Parameters.AddWithValue("@sf_fin_vigencia", request.SfFinVigencia);
                        command.Parameters.AddWithValue("@sf_plazo_garantia_dias", request.SfPlazoGarantiaDias);
                        command.Parameters.AddWithValue("@sf_sector_fianza", request.SfSectorFianza ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@sf_monto_fianza", request.SfMontoFianza);
                        command.Parameters.AddWithValue("@sf_monto_contrato", request.SfMontoContrato);
                        command.Parameters.AddWithValue("@sf_objeto_contrato", request.SfObjetoContrato ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@sf_aprobacion_legal", request.SfAprobacionLegal ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@sf_aprobacion_tecnica", request.SfAprobacionTecnica ?? (object)DBNull.Value);

                        command.Parameters.AddWithValue("@sfd_solicitud", request.SfdSolicitud ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@sfd_convenio", request.SfdConvenio ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@sfd_pagare", request.SfdPagare ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@sfd_prenda", request.SfdPrenda ?? (object)DBNull.Value);

                        command.Parameters.AddWithValue("@sfd_fecha_subida",
                            request.SfdFechaSubida.HasValue && request.SfdFechaSubida.Value > (DateTime)SqlDateTime.MinValue
                            ? request.SfdFechaSubida.Value
                            : (object)DBNull.Value);

                        command.Parameters.AddWithValue("@sfd_fecha_vencimiento",
                            request.SfdFechaVencimiento.HasValue && request.SfdFechaVencimiento.Value > (DateTime)SqlDateTime.MinValue
                            ? request.SfdFechaVencimiento.Value
                            : (object)DBNull.Value);

                        command.Parameters.AddWithValue("@sfd_poliza", request.SfdPoliza ?? (object)DBNull.Value);

                        var tablePrendas = CrearDataTablePrendas(request.Prendas);
                        var prendasParam = command.Parameters.AddWithValue("@Prendas", tablePrendas);
                        prendasParam.SqlDbType = SqlDbType.Structured;
                        prendasParam.TypeName = "TVP_PRENDA";

                        var sfIdOutParam = new SqlParameter("@sf_id_out", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(sfIdOutParam);

                        // --- LOG PARÁMETROS antes de ejecutar ---
                        foreach (SqlParameter p in command.Parameters)
                        {
                            Console.WriteLine($"Nombre: {p.ParameterName}, Tipo SQL: {p.SqlDbType}, Valor .NET: {p.Value?.GetType().Name ?? "NULL"}");
                        }

                        // --- Ejecutar ---
                        await command.ExecuteNonQueryAsync();

                        var solicitudId = (int)sfIdOutParam.Value;

                        return new AuthResponse
                        {
                            Estado = "Exito",
                            Mensaje = $"Solicitud completa registrada correctamente. ID: {solicitudId}",
                            IdGenerado = solicitudId
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("************** SQL EXCEPTION **************");
                Console.WriteLine($"Mensaje      : {ex.Message}");
                Console.WriteLine($"Número       : {ex.Number}");
                Console.WriteLine($"Procedimiento: {ex.Procedure}");
                Console.WriteLine($"Origen       : {ex.Source}");
                Console.WriteLine($"Estado       : {ex.State}");
                Console.WriteLine($"Línea        : {ex.LineNumber}");
                Console.WriteLine($"StackTrace   : {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine("************** INNER EXCEPTION **************");
                    Console.WriteLine($"Mensaje      : {ex.InnerException.Message}");
                }

                throw; // Opcional, para que el error suba a capas superiores si lo necesitas
            }
            catch (Exception ex)
            {
                Console.WriteLine("************** EXCEPTION **************");
                Console.WriteLine($"Mensaje      : {ex.Message}");
                Console.WriteLine($"StackTrace   : {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine("************** INNER EXCEPTION **************");
                    Console.WriteLine($"Mensaje      : {ex.InnerException.Message}");
                }

                throw;
            }
        }

        private DataTable CrearDataTablePrendas(List<PrendaDto> prendas)
        {
            var dt = new DataTable();

            dt.Columns.Add("pren_fecha_creacion", typeof(DateTime));
            dt.Columns.Add("pren_tipo", typeof(string));
            dt.Columns.Add("pren_bien", typeof(string));
            dt.Columns.Add("pren_descripcion", typeof(string));
            dt.Columns.Add("pren_valor", typeof(decimal));
            dt.Columns.Add("pren_ubicacion", typeof(string));
            dt.Columns.Add("pren_custodio", typeof(string));
            dt.Columns.Add("pren_fecha_constatacion", typeof(DateTime));
            dt.Columns.Add("pren_responsable_constatacion", typeof(string));
            dt.Columns.Add("pren_archivo", typeof(byte[]));  // VARBINARY(MAX)

            foreach (var prenda in prendas)
            {
                var fechaCreacion = prenda.PrenFechaCreacion.HasValue &&
                                    prenda.PrenFechaCreacion.Value >= SqlDateTime.MinValue.Value
                                    ? prenda.PrenFechaCreacion.Value
                                    : (object)DBNull.Value;

                var fechaConstatacion = prenda.PrenFechaConstatacion.HasValue &&
                                        prenda.PrenFechaConstatacion.Value >= SqlDateTime.MinValue.Value
                                        ? prenda.PrenFechaConstatacion.Value
                                        : (object)DBNull.Value;


                dt.Rows.Add(
                    fechaCreacion,
                    prenda.PrenTipo,
                    prenda.PrenBien,
                    prenda.PrenDescripcion,
                    prenda.PrenValor ?? 0,
                    prenda.PrenUbicacion,
                    prenda.PrenCustodio,
                    fechaConstatacion,
                    prenda.PrenResponsableConstatacion,
        prenda.PrenArchivo ?? (object)DBNull.Value
                );
            }

            return dt;
        }


    }
}
