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
                    command.Parameters.AddWithValue("@emp_ciudad", request.EmpCiudad);
                    command.Parameters.AddWithValue("@emp_ubicacion", request.EmpUbicacion);
                    command.Parameters.AddWithValue("@emp_RUC", request.EmpRUC);
                    command.Parameters.AddWithValue("@emp_telefono", request.EmpTelefono);
                    command.Parameters.AddWithValue("@emp_email", request.EmpEmail);
                    command.Parameters.AddWithValue("@emp_estado", request.EmpEstado ?? true);

                    // Datos financieros
                    command.Parameters.AddWithValue("@empf_activo_corriente", request.EmpfActivoCorriente);
                    command.Parameters.AddWithValue("@empf_activo_fijo", request.EmpfActivoFijo);
                    command.Parameters.AddWithValue("@empf_activo_diferido", request.EmpfActivoDiferido ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_otros_activos", request.EmpfOtrosActivos ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_total_activos", request.EmpfTotalActivos ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_pasivo_corriente", request.EmpfPasivoCorriente ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_pasivo_largo_plazo", request.EmpfPasivoLargoPlazo ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_pasivo_diferido", request.EmpfPasivoDiferido ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_total_pasivo", request.EmpfTotalPasivo ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_capital", request.EmpfCapital);
                    command.Parameters.AddWithValue("@empf_reserva", request.EmpfReserva);
                    command.Parameters.AddWithValue("@empf_otras_cuentas_patrimonio", request.EmpfOtrasCuentasPatrimonio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_utilidades_acumuladas", request.EmpfUtilidadesAcumuladas ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_utilidad_ejercicio", request.EmpfUtilidadEjercicio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_perdida", request.EmpfPerdida);
                    command.Parameters.AddWithValue("@empf_otras_perdidas", request.EmpfOtrasPerdidas ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_patrimonio_neto", request.EmpfPatrimonioNeto ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@empf_pasivo_patrimonio", request.EmpfPasivoPatrimonio ?? (object)DBNull.Value);
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

                    // Datos de clasificación empresa
                    command.Parameters.AddWithValue("@clemp_cliente_c", request.ClempClienteC ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@clemp_clasificacion", request.ClempClasificacion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@clemp_rango", request.ClempRango ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@clemp_archivo_soporte", request.ClempArchivoSoporte ?? (object)DBNull.Value);

                    // Observación inicial
                    command.Parameters.AddWithValue("@hist_observacion", request.HistObservacion ?? (object)DBNull.Value);

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

    }
}
