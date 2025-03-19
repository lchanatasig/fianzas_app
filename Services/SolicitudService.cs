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

                        // 1. Beneficiario
                        command.Parameters.Add("@ben_id", SqlDbType.Int).Value = (object?)request.BenId ?? DBNull.Value;
                        command.Parameters.Add("@ben_nombre", SqlDbType.NVarChar, 255).Value = (object?)request.BenNombre ?? DBNull.Value;
                        command.Parameters.Add("@ben_direccion", SqlDbType.NVarChar, 255).Value = (object?)request.BenDireccion ?? DBNull.Value;
                        command.Parameters.Add("@ben_ci_ruc", SqlDbType.NVarChar, 13).Value = (object?)request.BenCiRuc ?? DBNull.Value;
                        command.Parameters.Add("@ben_email", SqlDbType.NVarChar, 255).Value = (object?)request.BenEmail ?? DBNull.Value;
                        command.Parameters.Add("@ben_telefono", SqlDbType.NVarChar, 10).Value = (object?)request.BenTelefono ?? DBNull.Value;

                        // 2. Solicitud Fianza
                        command.Parameters.Add("@sf_emp_id", SqlDbType.Int).Value = request.SfEmpId;
                        command.Parameters.Add("@sf_tpos_id", SqlDbType.Int).Value = request.SfTposId;
                        command.Parameters.Add("@sf_fecha_solicitud", SqlDbType.DateTime).Value = DateTime.Now;
                        command.Parameters.Add("@sf_inicio_vigencia", SqlDbType.DateTime).Value = request.SfInicioVigencia;
                        command.Parameters.Add("@sf_fin_vigencia", SqlDbType.DateTime).Value = request.SfFinVigencia;
                        command.Parameters.Add("@sf_plazo_garantia_dias", SqlDbType.Int).Value = request.SfPlazoGarantiaDias;
                        command.Parameters.Add("@sf_sector_fianza", SqlDbType.NVarChar, 255).Value = (object?)request.SfSectorFianza ?? DBNull.Value;
                        command.Parameters.Add("@sf_monto_fianza", SqlDbType.Decimal).Value = request.SfMontoFianza;
                        command.Parameters.Add("@sf_monto_contrato", SqlDbType.Decimal).Value = request.SfMontoContrato;
                        command.Parameters.Add("@sf_objeto_contrato", SqlDbType.NVarChar, 255).Value = (object?)request.SfObjetoContrato ?? DBNull.Value;

                        // 3. Documentos
                        command.Parameters.Add("@sfd_solicitud", SqlDbType.VarBinary, -1).Value = (object?)request.SfdSolicitud ?? DBNull.Value;
                        command.Parameters.Add("@sfd_convenio", SqlDbType.VarBinary, -1).Value = (object?)request.SfdConvenio ?? DBNull.Value;
                        command.Parameters.Add("@sfd_pagare", SqlDbType.VarBinary, -1).Value = (object?)request.SfdPagare ?? DBNull.Value;
                        command.Parameters.Add("@sfd_prenda", SqlDbType.VarBinary, -1).Value = (object?)request.SfdPrenda ?? DBNull.Value;
                        command.Parameters.Add("@sfd_fecha_subida", SqlDbType.DateTime).Value = request.SfdFechaSubida ?? (object)DBNull.Value;
                        command.Parameters.Add("@sfd_fecha_vencimiento", SqlDbType.DateTime).Value = request.SfdFechaVencimiento ?? (object)DBNull.Value;
                        command.Parameters.Add("@sfd_poliza", SqlDbType.NVarChar, 255).Value = (object?)request.SfdPoliza ?? DBNull.Value;

                        // 4. Prendas (TVP)
                        var tablePrendas = CrearDataTablePrendas(request.Prendas);
                        var prendasParam = command.Parameters.AddWithValue("@Prendas", tablePrendas);
                        prendasParam.SqlDbType = SqlDbType.Structured;
                        prendasParam.TypeName = "TVP_PRENDA";

                        // 5. Usuario que crea (AUDITORIA)
                        command.Parameters.Add("@sf_usuario_id", SqlDbType.Int).Value = request.SfUsuarioId;

                        // 6. OUTPUT
                        var sfIdOutParam = new SqlParameter("@sf_id_out", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(sfIdOutParam);

                        // Ejecutar el SP
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
                Console.WriteLine($"SQL EXCEPTION: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
                throw;
            }
        }

        public async Task<SolicitudFianzaDetalleResponse> ObtenerSolicitudPorIdAsync(int solicitudId)
        {
            var solicitudDetalle = new SolicitudFianzaDetalleResponse();

            using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("sp_obtener_solicitud_fianza_por_id", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@sf_id", solicitudId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        // Primera tabla: solicitud y documentos
                        if (await reader.ReadAsync())
                        {
                            solicitudDetalle.SfId = reader.GetInt32(reader.GetOrdinal("sf_id"));
                            solicitudDetalle.SfEmpId = reader.GetInt32(reader.GetOrdinal("sf_emp_id"));
                            solicitudDetalle.EmpresaNombre = reader["emp_nombre"].ToString();
                            solicitudDetalle.EmpUbicacion = reader["emp_ubicacion"].ToString();
                            solicitudDetalle.EmpRuc = reader["emp_RUC"].ToString();
                            solicitudDetalle.EmpEmail = reader["emp_email"].ToString();
                            solicitudDetalle.EmpTelefono = reader["emp_telefono"].ToString();
                            solicitudDetalle.SfTposId = reader.GetInt32(reader.GetOrdinal("sf_tpos_id"));
                            solicitudDetalle.TipoSolicitudNombre = reader["tpos_nombre"].ToString();
                            solicitudDetalle.SfEstfId = reader.GetInt32(reader.GetOrdinal("sf_estf_id"));
                            solicitudDetalle.EstadoFianzaNombre = reader["estf_nombre"].ToString();
                            solicitudDetalle.SfBenId = reader.GetInt32(reader.GetOrdinal("sf_ben_id"));
                            solicitudDetalle.BenNombre = reader["ben_nombre"].ToString();
                            solicitudDetalle.BenDireccion = reader["ben_direccion"].ToString();
                            solicitudDetalle.BenCiRuc = reader["ben_ci_ruc"].ToString();
                            solicitudDetalle.BenEmail = reader["ben_email"].ToString();
                            solicitudDetalle.BenTelefono = reader["ben_telefono"].ToString();
                            solicitudDetalle.SfFechaSolicitud = reader.GetDateTime(reader.GetOrdinal("sf_fecha_solicitud"));
                            solicitudDetalle.SfInicioVigencia = reader.GetDateTime(reader.GetOrdinal("sf_inicio_vigencia"));
                            solicitudDetalle.SfFinVigencia = reader.IsDBNull(reader.GetOrdinal("sf_fin_vigencia")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("sf_fin_vigencia"));
                            solicitudDetalle.SfPlazoGarantiaDias = reader.GetInt32(reader.GetOrdinal("sf_plazo_garantia_dias"));
                            solicitudDetalle.SfSectorFianza = reader["sf_sector_fianza"].ToString();
                            solicitudDetalle.SfMontoFianza = reader.GetDecimal(reader.GetOrdinal("sf_monto_fianza"));
                            solicitudDetalle.SfMontoContrato = reader.GetDecimal(reader.GetOrdinal("sf_monto_contrato"));
                            solicitudDetalle.SfObjetoContrato = reader["sf_objeto_contrato"].ToString();
                            solicitudDetalle.SfAprobacionLegal = reader.IsDBNull(reader.GetOrdinal("sf_aprobacion_legal"))
      ? (int?)null
      : reader.GetInt32(reader.GetOrdinal("sf_aprobacion_legal"));

                            solicitudDetalle.SfAprobacionTecnica = reader.IsDBNull(reader.GetOrdinal("sf_aprobacion_tecnica"))
                                ? (int?)null
                                : reader.GetInt32(reader.GetOrdinal("sf_aprobacion_tecnica"));

                            // Documentos
                            solicitudDetalle.SfdId = reader.GetInt32(reader.GetOrdinal("sfd_id"));
                            solicitudDetalle.SfdFechaSubida = reader.IsDBNull(reader.GetOrdinal("sfd_fecha_subida")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("sfd_fecha_subida"));
                            solicitudDetalle.SfdFechaVencimiento = reader.IsDBNull(reader.GetOrdinal("sfd_fecha_vencimiento")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("sfd_fecha_vencimiento"));
                            solicitudDetalle.SfdPoliza = reader["sfd_poliza"].ToString();
                        }

                        Console.WriteLine($"MontoContrato: {solicitudDetalle.SfMontoContrato}");
                        Console.WriteLine($"MontoFianza: {solicitudDetalle.SfMontoFianza}");

                        // Segunda tabla: prendas
                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var prenda = new PrendaResponse
                                {
                                    PrenId = reader.GetInt32(reader.GetOrdinal("pren_id")),
                                    PrenFechaCreacion = reader.IsDBNull(reader.GetOrdinal("pren_fecha_creacino"))
                        ? DateTime.MinValue
                        : reader.GetDateTime(reader.GetOrdinal("pren_fecha_creacino")),
                                    PrenTipo = reader["pren_tipo"].ToString(),
                                    PrenBien = reader["pren_bien"].ToString(),
                                    PrenDescripcion = reader["pren_descripcion"].ToString(),
                                    PrenValor = reader.GetDecimal(reader.GetOrdinal("pren_valor")),
                                    PrenUbicacion = reader["pren_ubicacion"].ToString(),
                                    PrenCustodio = reader["pren_custodio"].ToString(),
                                    PrenFechaConstatacion = reader.IsDBNull(reader.GetOrdinal("pren_fecha_constatacion"))
                        ? DateTime.MinValue
                        : reader.GetDateTime(reader.GetOrdinal("pren_fecha_constatacion")),
                                    PrenResponsableConstatacion = reader["pren_responsable_constatacion"].ToString()
                                };



                                solicitudDetalle.Prendas.Add(prenda);
                            }
                        }

                        // Tercera tabla: historial
                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var historial = new HistorialSolicitudResponse
                                {
                                    SfhId = reader.GetInt32(reader.GetOrdinal("sfh_id")),
                                    SfhSfId = reader.GetInt32(reader.GetOrdinal("sfh_sf_id")),
                                    SfhEsfId = reader.GetInt32(reader.GetOrdinal("sfh_esf_id")),
                                    EstadoNombre = reader["estado_nombre"].ToString(),
                                    SfhUsuarioId = reader.GetInt32(reader.GetOrdinal("sfh_usuario_id")),
                                    UsuarioNombre = reader["usuario_nombre"].ToString(),
                                    SfhFechaCambio = reader.GetDateTime(reader.GetOrdinal("sfh_fecha_cambio")),
                                    SfhObservacion = reader["sfh_observacion"].ToString()
                                };

                                solicitudDetalle.Historial.Add(historial);
                            }
                        }
                    }
                }
            }

            return solicitudDetalle;
        }
        public async Task<List<SolicitudCompletaResponse>> ListarSolicitudesCompletasAsync()
        {
            var solicitudes = new List<SolicitudCompletaResponse>();

            using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_listar_solicitudes_completas", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        // 🚀 Debug: lista de columnas para validar
                        Console.WriteLine("🔍 Columnas devueltas por el SP:");
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine($"   {i}: {reader.GetName(i)}");
                        }

                        int idxSfhObservacion = -1;
                        try
                        {
                            idxSfhObservacion = reader.GetOrdinal("sfh_observacion");
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Console.WriteLine("⚠️  La columna 'sfh_observacion' no se encuentra en el resultado del SP.");
                            // Si no está, puedes manejarlo como prefieras, por ejemplo:
                            // return solicitudes;
                            // o simplemente asignar un valor default más adelante.
                        }

                        while (await reader.ReadAsync())
                        {
                            var solicitud = new SolicitudCompletaResponse
                            {
                                SfId = reader.GetInt32(reader.GetOrdinal("sf_id")),
                                SfEmpId = reader.GetInt32(reader.GetOrdinal("sf_emp_id")),
                                EmpresaNombre = reader["emp_nombre"]?.ToString(),

                                SfTposId = reader.GetInt32(reader.GetOrdinal("sf_tpos_id")),
                                TipoSolicitudNombre = reader["tpos_nombre"]?.ToString(),

                                SfEstfId = reader.GetInt32(reader.GetOrdinal("sf_estf_id")),
                                EstadoFianzaNombre = reader["estf_nombre"]?.ToString(),

                                SfBenId = reader.IsDBNull(reader.GetOrdinal("sf_ben_id"))
                                          ? (int?)null
                                          : reader.GetInt32(reader.GetOrdinal("sf_ben_id")),

                                BeneficiarioNombre = reader["ben_nombre"]?.ToString(),

                                SfFechaSolicitud = reader.GetDateTime(reader.GetOrdinal("sf_fecha_solicitud")),
                                SfInicioVigencia = reader.GetDateTime(reader.GetOrdinal("sf_inicio_vigencia")),

                                SfFinVigencia = reader.IsDBNull(reader.GetOrdinal("sf_fin_vigencia"))
                                                ? (DateTime?)null
                                                : reader.GetDateTime(reader.GetOrdinal("sf_fin_vigencia")),

                                SfPlazoGarantiaDias = reader.GetInt32(reader.GetOrdinal("sf_plazo_garantia_dias")),

                                SfSectorFianza = reader["sf_sector_fianza"]?.ToString(),
                                SfMontoFianza = reader.GetDecimal(reader.GetOrdinal("sf_monto_fianza")),
                                SfMontoContrato = reader.GetDecimal(reader.GetOrdinal("sf_monto_contrato")),
                                SfObjetoContrato = reader["sf_objeto_contrato"]?.ToString(),

                                SfAprobacionLegal = reader.IsDBNull(reader.GetOrdinal("sf_aprobacion_legal"))
                                                    ? (int?)null
                                                    : reader.GetInt32(reader.GetOrdinal("sf_aprobacion_legal")),

                                SfAprobacionTecnica = reader.IsDBNull(reader.GetOrdinal("sf_aprobacion_tecnica"))
                                                      ? (int?)null
                                                      : reader.GetInt32(reader.GetOrdinal("sf_aprobacion_tecnica")),

                                // Documentos
                                SfdId = reader.IsDBNull(reader.GetOrdinal("sfd_id"))
                                        ? (int?)null
                                        : reader.GetInt32(reader.GetOrdinal("sfd_id")),

                                SfdFechaSubida = reader.IsDBNull(reader.GetOrdinal("sfd_fecha_subida"))
                                                 ? (DateTime?)null
                                                 : reader.GetDateTime(reader.GetOrdinal("sfd_fecha_subida")),

                                SfdFechaVencimiento = reader.IsDBNull(reader.GetOrdinal("sfd_fecha_vencimiento"))
                                                      ? (DateTime?)null
                                                      : reader.GetDateTime(reader.GetOrdinal("sfd_fecha_vencimiento")),

                                SfdPoliza = reader["sfd_poliza"]?.ToString()
                            };

                            // ✅ Asignación segura de la observación
                            if (idxSfhObservacion >= 0)
                            {
                                solicitud.SfhObservacion = reader.IsDBNull(idxSfhObservacion)
                                    ? "Sin observación"  // Valor default si el campo está null
                                    : reader.GetString(idxSfhObservacion);
                            }
                            else
                            {
                                solicitud.SfhObservacion = "Observación no disponible";  // Valor si no se encontró la columna
                            }

                            // 🔥 Debug info para validar que la observación llegó bien
                            Console.WriteLine($"Solicitud ID: {solicitud.SfId} | Observación: {solicitud.SfhObservacion}");

                            solicitudes.Add(solicitud);
                        }
                    }
                }
            }

            return solicitudes;
        }

        public async Task<AuthResponse> AprobarSolicitudFianzaAsync(AprobarSolicitudRequest request)
        {
            using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_aprobar_solicitud_fianza", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@sf_id", request.SfId);
                    command.Parameters.AddWithValue("@tipo_aprobacion", request.TipoAprobacion?.ToUpper());
                    command.Parameters.AddWithValue("@aprobado", request.Aprobado ? 1 : 0);
                    command.Parameters.AddWithValue("@sfh_usuario_id", request.UsuarioId);
                    command.Parameters.AddWithValue("@sfh_observacion", string.IsNullOrWhiteSpace(request.Observacion) ? (object)DBNull.Value : request.Observacion);

                    try
                    {
                        await command.ExecuteNonQueryAsync();

                        return new AuthResponse
                        {
                            Estado = "Exito",
                            Mensaje = $"Solicitud {request.TipoAprobacion} procesada correctamente."
                        };
                    }
                    catch (SqlException ex)
                    {
                        return new AuthResponse
                        {
                            Estado = "Error",
                            Mensaje = $"Error al aprobar la solicitud: {ex.Message}"
                        };
                    }
                }
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
