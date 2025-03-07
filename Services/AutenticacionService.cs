using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fianzas_app.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace fianzas_app.Services
{
    public class AutenticacionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AutenticacionService> _logger;
        private readonly AppFianzasContext _dbContext;

        public AutenticacionService(IHttpContextAccessor httpContextAccessor, ILogger<AutenticacionService> logger, AppFianzasContext dbContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<AuthResponse> ValidarCredencialesAsync(string usuarioCI, string usuarioPassword)
        {
            using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("sp_validar_credenciales", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@usuario_ci", usuarioCI);
                    command.Parameters.AddWithValue("@usuario_password", usuarioPassword);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            string estado = reader["Estado"].ToString();
                            string mensaje = reader["Mensaje"].ToString();

                            if (estado == "Error")
                            {
                                return new AuthResponse
                                {
                                    Estado = estado,
                                    Mensaje = mensaje
                                };
                            }

                            var usuario = new AuthResponse
                            {
                                Estado = estado,
                                Mensaje = mensaje,
                                UsuarioId = reader["usuario_id"] as int?,
                                UsuarioNombres = reader["usuario_nombres"]?.ToString(),
                                UsuarioDireccion = reader["usuario_direccion"]?.ToString(),
                                UsuarioEstado = reader["usuario_estado"] as int?,
                                PerfilId = reader["perfil_id"] as int?,
                                PerfilNombre = reader["perfil_nombre"]?.ToString(),
                                PerfilEstado = reader["perfil_estado"] as int?
                            };

                            // Almacenar en la sesión si el usuario es válido
                            var httpContext = _httpContextAccessor.HttpContext;
                            if (httpContext != null)
                            {
                                httpContext.Session.SetInt32("UsuarioId", usuario.UsuarioId ?? 0);
                                httpContext.Session.SetInt32("PerfilId", usuario.PerfilId ?? 0);
                                httpContext.Session.SetString("UsuarioNombre", usuario.UsuarioNombres ?? "");
                                httpContext.Session.SetString("UsuarioPerfil", usuario.PerfilNombre ?? "");
                            }

                            return usuario;
                        }
                    }
                }
            }

            return new AuthResponse
            {
                Estado = "Error",
                Mensaje = "Error inesperado en la autenticación."
            };
        }
    }
}