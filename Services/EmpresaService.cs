using fianzas_app.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace fianzas_app.Services
{
    public class EmpresaService
    {
        private readonly ILogger<EmpresaService> _logger;
        private readonly AppFianzasContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmpresaService(ILogger<EmpresaService> logger, AppFianzasContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<EmpresaResponse>> ListarEmpresasAsync()
        {
            var empresas = new List<EmpresaResponse>();

            using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_listar_empresas", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var empresa = new EmpresaResponse
                            {
                                EmpId = reader.GetInt32(reader.GetOrdinal("emp_id")),
                                EmpTipoEmpresaId = reader.GetInt32(reader.GetOrdinal("emp_tipo_empresa_id")),
                                TipoEmpresaNombre = reader.GetString(reader.GetOrdinal("tipo_empresa_nombre")),
                                EmpNombre = reader.GetString(reader.GetOrdinal("emp_nombre")),
                                EmpCiudad = reader["emp_ciudad"]?.ToString(),
                                EmpUbicacion = reader["emp_ubicacion"]?.ToString(),
                                EmpRUC = reader["emp_RUC"]?.ToString(),
                                EmpTelefono = reader["emp_telefono"]?.ToString(),
                                EmpEmail = reader["emp_email"]?.ToString(),
                                EmpEstado = reader.GetBoolean(reader.GetOrdinal("emp_estado")),
                                EmpFechaCreacion = reader.GetDateTime(reader.GetOrdinal("emp_fecha_creacion")),
                                EmpFechaActualizacion = reader["emp_fecha_actualizacion"] as DateTime?,

                                Finanzas = new EmpresaFinanzasDto
                                {
                                    EmpfId = reader.GetInt32(reader.GetOrdinal("empf_id")),
                                    ActivoCorriente = reader.GetDecimal(reader.GetOrdinal("empf_activo_corriente")),
                                    ActivoFijo = reader.GetDecimal(reader.GetOrdinal("empf_activo_fijo")),
                                    ActivoDiferido = reader["empf_activo_diferido"] as decimal?,
                                    OtrosActivos = reader["empf_otros_activos"] as decimal?,
                                    TotalActivos = reader["empf_total_activos"] as decimal?,
                                    PasivoCorriente = reader["empf_pasivo_corriente"] as decimal?,
                                    PasivoLargoPlazo = reader["empf_pasivo_largo_plazo"] as decimal?,
                                    PasivoDiferido = reader["empf_pasivo_diferido"] as decimal?,
                                    TotalPasivo = reader["empf_total_pasivo"] as decimal?,
                                    Capital = reader.GetDecimal(reader.GetOrdinal("empf_capital")),
                                    Reserva = reader.GetDecimal(reader.GetOrdinal("empf_reserva")),
                                    OtrasCuentasPatrimonio = reader["empf_otras_cuentas_patrimonio"] as decimal?,
                                    UtilidadesAcumuladas = reader["empf_utilidades_acumuladas"] as decimal?,
                                    UtilidadEjercicio = reader["empf_utilidad_ejercicio"] as decimal?,
                                    Perdida = reader.GetDecimal(reader.GetOrdinal("empf_perdida")),
                                    OtrasPerdidas = reader["empf_otras_perdidas"] as decimal?,
                                    PatrimonioNeto = reader["empf_patrimonio_neto"] as decimal?,
                                    PasivoPatrimonio = reader["empf_pasivo_patrimonio"] as decimal?,
                                    Ventas = reader.GetDecimal(reader.GetOrdinal("empf_ventas")),
                                    Utilidad = reader.GetDecimal(reader.GetOrdinal("empf_utilidad")),
                                    CupoAsignado = reader.GetDecimal(reader.GetOrdinal("empf_cupo_asignado"))
                                },

                                AnalisisFinanciero = new AnalisisFinancieroDto
                                {
                                    Liquidez = reader["anf_liquidez"] as decimal?,
                                    Solvencia = reader["anf_solvencia"] as decimal?,
                                    CapCobertura = reader["anf_cap_cobertura"] as decimal?,
                                    Endeudamiento = reader["anf_endeudamiento"] as decimal?,
                                    CapitalPropio = reader["anf_capital_propio"] as decimal?,
                                    ROA = reader["anf_ROA"] as decimal?,
                                    ROE = reader["anf_ROE"] as decimal?,
                                    AnalisisSf = reader["anf_analisis_sf"]?.ToString()
                                },

                                Clasificacion = new ClasificacionEmpresaDto
                                {
                                    ClienteC = reader["clemp_cliente_c"]?.ToString(),
                                    Clasificacion = reader["clemp_clasificacion"]?.ToString(),
                                    Rango = reader["clemp_rango"]?.ToString()
                                },

                                Historial = new HistorialEmpresaDto
                                {
                                    CupoRestante = reader["hist_cupo_restante"] as decimal?,
                                    HistFechaActualizacion = reader["hist_fecha_actualizacion"] as DateTime?
                                }
                            };

                            empresas.Add(empresa);
                        }
                    }
                }
            }

            return empresas;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        public async Task<AuthResponse> InsertarEmpresaAsync(EmpresaRequest request)
        {
            using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("sp_insertar_empresa", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Datos generales de la empresa
                    command.Parameters.AddWithValue("@emp_tipo_empresa_id", request.EmpTipoEmpresaId);
                    command.Parameters.AddWithValue("@emp_nombre", request.EmpNombre);
                    command.Parameters.AddWithValue("@emp_ciudad", request.EmpCiudad ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@emp_ubicacion", request.EmpUbicacion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@emp_RUC", request.EmpRUC);
                    command.Parameters.AddWithValue("@emp_telefono", request.EmpTelefono);
                    command.Parameters.AddWithValue("@emp_email", request.EmpEmail);
                    command.Parameters.AddWithValue("@emp_estado", request.EmpEstado ?? true);
                    command.Parameters.AddWithValue("@emp_corredor", request.EmpCorredorId );

                    // Datos financieros - Activos
                    command.Parameters.AddWithValue("@empf_activo_corriente", request.EmpfActivoCorriente);
                    command.Parameters.AddWithValue("@empf_activo_fijo", request.EmpfActivoFijo);
                    command.Parameters.AddWithValue("@empf_activo_diferido", request.EmpfActivoDiferido ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_otros_activos", request.EmpfOtrosActivos ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_total_activos", request.EmpfTotalActivos ?? (object)DBNull.Value);

                    // Pasivos
                    command.Parameters.AddWithValue("@empf_pasivo_corriente", request.EmpfPasivoCorriente ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_pasivo_largo_plazo", request.EmpfPasivoLargoPlazo ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_pasivo_diferido", request.EmpfPasivoDiferido ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_total_pasivo", request.EmpfTotalPasivo ?? (object)DBNull.Value);

                    // Patrimonio
                    command.Parameters.AddWithValue("@empf_capital", request.EmpfCapital);
                    command.Parameters.AddWithValue("@empf_reserva", request.EmpfReserva);
                    command.Parameters.AddWithValue("@empf_otras_cuentas_patrimonio", request.EmpfOtrasCuentasPatrimonio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_utilidades_acumuladas", request.EmpfUtilidadesAcumuladas ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_utilidad_ejercicio", request.EmpfUtilidadEjercicio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_perdida", request.EmpfPerdida);
                    command.Parameters.AddWithValue("@empf_otras_perdidas", request.EmpfOtrasPerdidas ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_patrimonio_neto", request.EmpfPatrimonioNeto ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_pasivo_patrimonio", request.EmpfPasivoPatrimonio ?? (object)DBNull.Value);

                    // Resultados
                    command.Parameters.AddWithValue("@empf_ventas", request.EmpfVentas);
                    command.Parameters.AddWithValue("@empf_utilidad", request.EmpfUtilidad);
                    command.Parameters.AddWithValue("@empf_cupo_asignado", request.EmpfCupoAsignado);

                    // Datos de análisis financiero
                    command.Parameters.AddWithValue("@anf_liquidez", request.AnfLiquidez ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@anf_solvencia", request.AnfSolvencia ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@anf_cap_cobertura", request.AnfCapCobertura ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@anf_endeudamiento", request.AnfEndeudamiento ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@anf_capital_propio", request.AnfCapitalPropio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@anf_ROA", request.AnfROA ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@anf_ROE", request.AnfROE ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@anf_analisis_sf", request.AnfAnalisisSf ?? (object)DBNull.Value);

                    // Clasificación empresa
                    command.Parameters.AddWithValue("@clemp_cliente_c", request.ClempClienteC ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@clemp_clasificacion", request.ClempClasificacion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@clemp_rango", request.ClempRango ?? (object)DBNull.Value);
                    var archivoBytes = ConvertIFormFileToByteArray(request.ClempArchivoSoporte);
                    command.Parameters.AddWithValue("@clemp_archivo_soporte", archivoBytes ?? (object)DBNull.Value);


                    // Observación inicial
                    command.Parameters.AddWithValue("@hist_observacion", request.HistObservacion ?? (object)DBNull.Value);

                    // Ejecutar y leer la respuesta
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            return new AuthResponse
                            {
                                Estado = reader["Estado"].ToString(),
                                Mensaje = reader["Mensaje"].ToString()
                            };
                        }
                    }
                }
            }

            return new AuthResponse
            {
                Estado = "Error",
                Mensaje = "No se pudo registrar la empresa."
            };
        }

        private byte[] ConvertIFormFileToByteArray(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            using (var ms = new MemoryStream())
            {
                file.OpenReadStream().CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public async Task<EmpresaRequest> ObtenerEmpresaPorIdAsync(int empId)
        {
            EmpresaRequest empresa = null;

            using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_obtener_empresa_por_id", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@emp_id", empId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            empresa = new EmpresaRequest
                            {
                                EmpresaId = reader.GetInt32(reader.GetOrdinal("emp_id")),
                                EmpTipoEmpresaId = reader.GetInt32(reader.GetOrdinal("emp_tipo_empresa_id")),
                                EmpNombre = reader.GetString(reader.GetOrdinal("emp_nombre")),
                                EmpCiudad = reader["emp_ciudad"]?.ToString(),
                                EmpUbicacion = reader["emp_ubicacion"]?.ToString(),
                                EmpRUC = reader.GetString(reader.GetOrdinal("emp_RUC")),
                                EmpTelefono = reader.GetString(reader.GetOrdinal("emp_telefono")),
                                EmpEmail = reader.GetString(reader.GetOrdinal("emp_email")),
                                EmpEstado = reader.GetBoolean(reader.GetOrdinal("emp_estado")),
                                EmpCorredorId = reader.GetInt32(reader.GetOrdinal("empresa_corredor_id")),


                                EmpfActivoCorriente = reader.GetDecimal(reader.GetOrdinal("empf_activo_corriente")),
                                EmpfActivoFijo = reader.GetDecimal(reader.GetOrdinal("empf_activo_fijo")),
                                EmpfActivoDiferido = reader["empf_activo_diferido"] as decimal?,
                                EmpfOtrosActivos = reader["empf_otros_activos"] as decimal?,
                                EmpfTotalActivos = reader["empf_total_activos"] as decimal?,
                                EmpfPasivoCorriente = reader["empf_pasivo_corriente"] as decimal?,
                                EmpfPasivoLargoPlazo = reader["empf_pasivo_largo_plazo"] as decimal?,
                                EmpfPasivoDiferido = reader["empf_pasivo_diferido"] as decimal?,
                                EmpfTotalPasivo = reader["empf_total_pasivo"] as decimal?,
                                EmpfCapital = reader.GetDecimal(reader.GetOrdinal("empf_capital")),
                                EmpfReserva = reader.GetDecimal(reader.GetOrdinal("empf_reserva")),
                                EmpfOtrasCuentasPatrimonio = reader["empf_otras_cuentas_patrimonio"] as decimal?,
                                EmpfUtilidadesAcumuladas = reader["empf_utilidades_acumuladas"] as decimal?,
                                EmpfUtilidadEjercicio = reader["empf_utilidad_ejercicio"] as decimal?,
                                EmpfPerdida = reader.GetDecimal(reader.GetOrdinal("empf_perdida")),
                                EmpfOtrasPerdidas = reader["empf_otras_perdidas"] as decimal?,
                                EmpfPatrimonioNeto = reader["empf_patrimonio_neto"] as decimal?,
                                EmpfPasivoPatrimonio = reader["empf_pasivo_patrimonio"] as decimal?,
                                EmpfVentas = reader.GetDecimal(reader.GetOrdinal("empf_ventas")),
                                EmpfUtilidad = reader.GetDecimal(reader.GetOrdinal("empf_utilidad")),
                                EmpfCupoAsignado = reader.GetDecimal(reader.GetOrdinal("empf_cupo_asignado")),



                                AnfLiquidez = reader["anf_liquidez"] as decimal?,
                                AnfSolvencia = reader["anf_solvencia"] as decimal?,
                                AnfCapCobertura = reader["anf_cap_cobertura"] as decimal?,
                                AnfEndeudamiento = reader["anf_endeudamiento"] as decimal?,
                                AnfCapitalPropio = reader["anf_capital_propio"] as decimal?,
                                AnfROA = reader["anf_ROA"] as decimal?,
                                AnfROE = reader["anf_ROE"] as decimal?,
                                AnfAnalisisSf = reader["anf_analisis_sf"]?.ToString(),

                                ClempClienteC = reader["clemp_cliente_c"]?.ToString(),
                                ClempClasificacion = reader["clemp_clasificacion"]?.ToString(),
                                ClempRango = reader["clemp_rango"]?.ToString(),



                                CupoRestante = reader["hist_cupo_restante"] as decimal?,
                                HistFechaActualizacion = reader["hist_fecha_actualizacion"] as DateTime?

                            };
                        }
                    }
                }
            }

            return empresa;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<AuthResponse> ActualizarEmpresaAsync(EmpresaRequest request)
        {
            using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_actualizar_empresa", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros de la Empresa
                    command.Parameters.AddWithValue("@emp_id", request.EmpresaId);
                    command.Parameters.AddWithValue("@emp_tipo_empresa_id", request.EmpTipoEmpresaId);
                    command.Parameters.AddWithValue("@emp_nombre", request.EmpNombre);
                    command.Parameters.AddWithValue("@emp_ciudad", request.EmpCiudad ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@emp_ubicacion", request.EmpUbicacion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@emp_RUC", request.EmpRUC);
                    command.Parameters.AddWithValue("@emp_telefono", request.EmpTelefono);
                    command.Parameters.AddWithValue("@emp_email", request.EmpEmail);
                    command.Parameters.AddWithValue("@emp_estado", request.EmpEstado ?? true);
                    command.Parameters.AddWithValue("@emp_corredor", request.EmpCorredorId);

                    // Datos financieros - Activos
                    command.Parameters.AddWithValue("@empf_activo_corriente", request.EmpfActivoCorriente);
                    command.Parameters.AddWithValue("@empf_activo_fijo", request.EmpfActivoFijo);
                    command.Parameters.AddWithValue("@empf_activo_diferido", request.EmpfActivoDiferido ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_otros_activos", request.EmpfOtrosActivos ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_total_activos", request.EmpfTotalActivos ?? (object)DBNull.Value);

                    // Pasivos
                    command.Parameters.AddWithValue("@empf_pasivo_corriente", request.EmpfPasivoCorriente ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_pasivo_largo_plazo", request.EmpfPasivoLargoPlazo ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_pasivo_diferido", request.EmpfPasivoDiferido ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_total_pasivo", request.EmpfTotalPasivo ?? (object)DBNull.Value);

                    // Patrimonio
                    command.Parameters.AddWithValue("@empf_capital", request.EmpfCapital);
                    command.Parameters.AddWithValue("@empf_reserva", request.EmpfReserva);
                    command.Parameters.AddWithValue("@empf_otras_cuentas_patrimonio", request.EmpfOtrasCuentasPatrimonio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_utilidades_acumuladas", request.EmpfUtilidadesAcumuladas ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_utilidad_ejercicio", request.EmpfUtilidadEjercicio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_perdida", request.EmpfPerdida);
                    command.Parameters.AddWithValue("@empf_otras_perdidas", request.EmpfOtrasPerdidas ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_patrimonio_neto", request.EmpfPatrimonioNeto ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_pasivo_patrimonio", request.EmpfPasivoPatrimonio ?? (object)DBNull.Value);

                    // Resultados
                    command.Parameters.AddWithValue("@empf_ventas", request.EmpfVentas);
                    command.Parameters.AddWithValue("@empf_utilidad", request.EmpfUtilidad);
                    command.Parameters.AddWithValue("@empf_cupo_asignado", request.EmpfCupoAsignado);

                    // Datos de análisis financiero
                    command.Parameters.AddWithValue("@anf_liquidez", request.AnfLiquidez ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@anf_solvencia", request.AnfSolvencia ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@anf_cap_cobertura", request.AnfCapCobertura ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@anf_endeudamiento", request.AnfEndeudamiento ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@anf_capital_propio", request.AnfCapitalPropio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@anf_ROA", request.AnfROA ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@anf_ROE", request.AnfROE ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@anf_analisis_sf", request.AnfAnalisisSf ?? (object)DBNull.Value);

                    // Clasificación empresa
                    command.Parameters.AddWithValue("@clemp_cliente_c", request.ClempClienteC ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@clemp_clasificacion", request.ClempClasificacion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@clemp_rango", request.ClempRango ?? (object)DBNull.Value);
                    var archivoBytes = ConvertIFormFileToByteArray(request.ClempArchivoSoporte);
                    command.Parameters.AddWithValue("@clemp_archivo_soporte", archivoBytes ?? (object)DBNull.Value);
                    // Observación para el historial
                    command.Parameters.AddWithValue("@hist_observacion", request.HistObservacion ?? (object)DBNull.Value);

                    // Ejecutar y leer la respuesta
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            return new AuthResponse
                            {
                                Estado = reader["Estado"].ToString(),
                                Mensaje = reader["Mensaje"].ToString()
                            };
                        }
                    }
                }
            }

            return new AuthResponse
            {
                Estado = "Error",
                Mensaje = "No se pudo actualizar la empresa."
            };
        }


        public async Task<List<HistorialEmpresa>> ObtenerHistorialEmpresaAsync(int empresaId, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            var historial = new List<HistorialEmpresa>();

            using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_historial_empresa", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros del procedimiento
                    command.Parameters.AddWithValue("@empresa_id", empresaId);

                    // Fecha inicio
                    if (fechaInicio.HasValue)
                        command.Parameters.AddWithValue("@fecha_inicio", fechaInicio.Value);
                    else
                        command.Parameters.AddWithValue("@fecha_inicio", DBNull.Value);

                    // Fecha fin
                    if (fechaFin.HasValue)
                        command.Parameters.AddWithValue("@fecha_fin", fechaFin.Value);
                    else
                        command.Parameters.AddWithValue("@fecha_fin", DBNull.Value);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var item = new HistorialEmpresa
                            {
                                HistId = reader.GetInt32(reader.GetOrdinal("hist_id")),
                                HistEmpresaId = reader.GetInt32(reader.GetOrdinal("hist_empresa_id")),
                                HistUsuarioId = reader.GetInt32(reader.GetOrdinal("hist_usuario_id")),
                                HistFechaActualizacion = reader.GetDateTime(reader.GetOrdinal("hist_fecha_actualizacion")),

                                HistActivoC = reader["hist_activoC"] as decimal?,
                                HistActivoF = reader["hist_activoF"] as decimal?,
                                HistActivoD = reader["hist_activoD"] as decimal?,
                                HistActivoO = reader["hist_activoO"] as decimal?,
                                HistActivoT = reader["hist_activoT"] as decimal?,

                                HistPasivoC = reader["hist_pasivoC"] as decimal?,
                                HistPasivoLp = reader["hist_pasivoLP"] as decimal?,
                                HistPasivoD = reader["hist_pasivoD"] as decimal?,
                                HistPasivoT = reader["hist_pasivoT"] as decimal?,

                                HistCapital = reader["hist_capital"] as decimal?,
                                HistReserva = reader["hist_reserva"] as decimal?,
                                HistOtrasCp = reader["hist_otrasCP"] as decimal?,
                                HistUtilidadesA = reader["hist_utilidadesA"] as decimal?,
                                HistUtilidadE = reader["hist_utilidadE"] as decimal?,
                                HistPerdidas = reader["hist_perdidas"] as decimal?,
                                HistOtrasPer = reader["hist_otrasPer"] as decimal?,
                                HistPatrimonioT = reader["hist_patrimonioT"] as decimal?,
                                HistPatrimonioPasivo = reader["hist_patrimonio_pasivo"] as decimal?,

                                HistVentas = reader["hist_ventas"] as decimal?,
                                HistUtilidad = reader["hist_utilidad"] as decimal?,
                                HistCupoRestante = reader["hist_cupo_restante"] as decimal?,
                                HistObservacion = reader["hist_observacion"]?.ToString(),

                                HistAnfLiquidez = reader["hist_anf_liquidez"] as decimal?,
                                HistAnfSolvencia = reader["hist_anf_solvencia"] as decimal?,
                                HistAnfCapCobertura = reader["hist_anf_cap_cobertura"] as decimal?,
                                HistAnfEndeudamiento = reader["hist_anf_endeudamiento"] as decimal?,
                                HistAnfCapitalPropio = reader["hist_anf_capital_propio"] as decimal?,
                                HistAnfRoa = reader["hist_anf_ROA"] as decimal?,
                                HistAnfRoe = reader["hist_anf_ROE"] as decimal?,
                                HistAnfAnalisisSf = reader["hist_anf_analisis_sf"]?.ToString(),

                                HistClempClienteC = reader["hist_clemp_cliente_c"]?.ToString(),
                                HistClempClasificacion = reader["hist_clemp_clasificacion"]?.ToString(),
                                HistClempRango = reader["hist_clemp_rango"]?.ToString()
                            };

                            historial.Add(item);
                        }
                    }
                }
            }

            return historial;
        }

    }
}
