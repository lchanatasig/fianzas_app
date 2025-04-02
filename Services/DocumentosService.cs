using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using fianzas_app.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;

namespace fianzas_app.Services
{
    public class DocumentosService
    {
        private readonly ILogger<DocumentosService> _logger;
        private readonly AppFianzasContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DocumentosService(ILogger<DocumentosService> logger, AppFianzasContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> InsertSfdAsync(
            int sfd_sf_id,
            byte[] sfd_solicitud,
            byte[] sfd_convenio,
            byte[] sfd_pagare,
            byte[] sfd_prenda,
            DateTime sfd_fecha_subida,
            DateTime sfd_fecha_vencimiento,
            string sfd_poliza)
        {
            // ¡Preparando el viaje al futuro con precisión quirúrgica en cada inserción!
            try
            {
                using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
                using (SqlCommand command = new SqlCommand("sp_InsertSfd", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@sfd_sf_id", sfd_sf_id);
                    command.Parameters.AddWithValue("@sfd_solicitud", sfd_solicitud);
                    command.Parameters.AddWithValue("@sfd_convenio", sfd_convenio);
                    command.Parameters.AddWithValue("@sfd_pagare", sfd_pagare);
                    command.Parameters.AddWithValue("@sfd_prenda", sfd_prenda);
                    command.Parameters.AddWithValue("@sfd_fecha_subida", sfd_fecha_subida);
                    command.Parameters.AddWithValue("@sfd_fecha_vencimiento", sfd_fecha_vencimiento);
                    command.Parameters.AddWithValue("@sfd_poliza", sfd_poliza ?? (object)DBNull.Value);

                    await connection.OpenAsync();
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar SFD");
                throw;
            }
        }



        //public async Task<SolicitudFianzaDocumentoDto> ObtenerDocumentoAsync(int sfd_sf_id, string tipoDocumento)
        //{
        //    SolicitudFianzaDocumentoDto resultado = null;

        //    using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
        //    {
        //        await connection.OpenAsync();
        //        using (SqlCommand command = new SqlCommand("sp_ObtenerDocumento", connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.Parameters.AddWithValue("@sfd_sf_id", sfd_sf_id);
        //            command.Parameters.AddWithValue("@tipoDocumento", tipoDocumento);

        //            using (SqlDataReader reader = await command.ExecuteReaderAsync())
        //            {
        //                if (await reader.ReadAsync())
        //                {
        //                    resultado = new SolicitudFianzaDocumentoDto
        //                    {
        //                        Documento = reader["Documento"] != DBNull.Value ? reader["Documento"].ToString() : null,
        //                        FechaSubida = reader["sfd_fecha_subida"] != DBNull.Value ? (DateTime)reader["sfd_fecha_subida"] : (DateTime?)null,
        //                        FechaVencimiento = reader["sfd_fecha_vencimiento"] != DBNull.Value ? (DateTime)reader["sfd_fecha_vencimiento"] : (DateTime?)null,
        //                        Poliza = reader["sfd_poliza"] != DBNull.Value ? reader["sfd_poliza"].ToString() : null
        //                    };
        //                }
        //            }
        //        }
        //    }

        //    return resultado;
        //}
    }
}
