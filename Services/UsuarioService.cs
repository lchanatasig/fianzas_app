using fianzas_app.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace fianzas_app.Services
{
    public class UsuarioService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UsuarioService> _logger;
        private readonly AppFianzasContext _dbContext;

        public UsuarioService(IHttpContextAccessor httpContextAccessor, ILogger<UsuarioService> logger, AppFianzasContext dbContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Servicio que consume la lista de usuario
        /// </summary>
        /// <returns></returns>
        public async Task<List<AuthResponse>> ListarUsuariosAsync()
        {
            var usuarios = new List<AuthResponse>();

            using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("sp_listar_usuarios", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add(new AuthResponse
                            {
                                UsuarioId = reader["usuario_id"] as int?,
                                UsuarioNombres = reader["usuario_nombres"]?.ToString(),
                                UsuarioDireccion = reader["usuario_direccion"]?.ToString(),
                                UsuarioCI = reader["usuario_ci"]?.ToString(),
                                UsuarioEstado = reader["usuario_estado"] as int?,
                                PerfilId = reader["perfil_id"] as int?,
                                PerfilNombre = reader["perfil_nombre"]?.ToString()
                            });
                        }
                    }
                }
            }

            return usuarios;
        }

        /// <summary>
        /// Metodo para crear un nuevo usuario
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<AuthResponse> InsertarUsuarioAsync(Usuario request)
        {
            using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("sp_insertar_usuario", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@usuario_perfil_id", request.UsuarioPerfilId);
                    command.Parameters.AddWithValue("@usuario_nombres", request.UsuarioNombres);
                    command.Parameters.AddWithValue("@usuario_direccion", request.UsuarioDireccion);
                    command.Parameters.AddWithValue("@usuario_password", request.UsuarioPassword);
                    command.Parameters.AddWithValue("@usuario_ci", request.UsuarioCi);
                    command.Parameters.AddWithValue("@usuario_estado", request.UsuarioEstado ?? 1);

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
                Mensaje = "No se pudo registrar el usuario."
            };
        }


        public async Task<AuthResponse> ObtenerUsuarioPorIdAsync(int usuarioId)
        {
            using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("sp_obtener_usuario", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@usuario_id", usuarioId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            return new AuthResponse
                            {
                                UsuarioId = reader["usuario_id"] as int?,
                                UsuarioNombres = reader["usuario_nombres"]?.ToString(),
                                UsuarioDireccion = reader["usuario_direccion"]?.ToString(),
                                UsuarioCI = reader["usuario_ci"]?.ToString(),
                                UsuarioEstado = reader["usuario_estado"] as int?,
                                PerfilId = reader["perfil_id"] as int?,
                                PerfilNombre = reader["perfil_nombre"]?.ToString()
                            };
                        }
                    }
                }
            }

            return null; // Retorna null si no se encuentra el usuario
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<AuthResponse> ActualizarUsuarioAsync(Usuario request)
        {
            using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("sp_actualizar_usuario", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@usuario_id", request.UsuarioId);
                    command.Parameters.AddWithValue("@usuario_perfil_id", request.UsuarioPerfilId);
                    command.Parameters.AddWithValue("@usuario_nombres", request.UsuarioNombres);
                    command.Parameters.AddWithValue("@usuario_direccion", request.UsuarioDireccion);
                    command.Parameters.AddWithValue("@usuario_ci", request.UsuarioCi);
                    command.Parameters.AddWithValue("@usuario_estado", request.UsuarioEstado ?? 1);

                    if (string.IsNullOrEmpty(request.UsuarioPassword))
                        command.Parameters.AddWithValue("@usuario_password", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@usuario_password", request.UsuarioPassword);

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
                Mensaje = "No se pudo actualizar el usuario."
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        public async Task<AuthResponse> CambiarEstadoUsuarioAsync(int usuarioId)
        {
            using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("sp_cambiar_estado_usuario", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@usuario_id", usuarioId);

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
                Mensaje = "No se pudo cambiar el estado del usuario."
            };
        }
    
    
    }
}
