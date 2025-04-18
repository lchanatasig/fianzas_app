﻿using fianzas_app.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace fianzas_app.Services
{
    public class ListaService
    {
        private readonly ILogger<ListaService> _logger;
        private readonly AppFianzasContext _dbContext;

        public ListaService( ILogger<ListaService> logger, AppFianzasContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Ejecuta el SP sp_listar_perfiles y retorna la lista de perfiles.
        /// </summary>
        /// <returns>Una lista de objetos Perfil.</returns>
        public async Task<List<Perfil>> ListarPerfilesAsync()
        {
            var perfiles = new List<Perfil>();

            using (var connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_listar_perfiles", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var perfil = new Perfil
                            {
                                PerfilId = reader["perfil_id"] != DBNull.Value ? Convert.ToInt32(reader["perfil_id"]) : 0,
                                PerfilNombre = reader["perfil_nombre"].ToString(),
                            };

                            perfiles.Add(perfil);
                        }
                    }
                }
            }

            return perfiles;
        }
        /// <summary>
        /// Ejecuta el SP sp_listar_tipos_empresas y retorna la lista de tipos de empresas.
        /// </summary>
        /// <returns></returns>
        public async Task<List<TipoEmpresa>> ListarTipoEmpresaAsync()
        {
            var tipoempresas = new List<TipoEmpresa>();

            using (var connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_listar_tipos_empresas", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var empresa = new TipoEmpresa
                            {
                                TempId = reader["temp_id"] != DBNull.Value ? Convert.ToInt32(reader["temp_id"]) : 0,
                                TempNombre = reader["temp_nombre"].ToString(),
                                TempEstado = reader["temp_estado"] != DBNull.Value ? Convert.ToByte(reader["temp_estado"]) : (byte)0
                            };

                            tipoempresas.Add(empresa);
                        }
                    }
                }
            }

            return tipoempresas;
        }

        public async Task<List<Corredor>> ListarCorredorAsync()
        {
            var corredors = new List<Corredor>();

            using (var connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ListarCorredor", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var empresa = new Corredor
                            {
                                CorredorId = reader["corredor_id"] != DBNull.Value ? Convert.ToInt32(reader["corredor_id"]) : 0,
                                CorredorNombre = reader["corredor_nombre"].ToString(),
                                CorredorEstado = reader["corredor_estado"] != DBNull.Value ? Convert.ToByte(reader["corredor_estado"]) : (byte)0
                            };

                            corredors.Add(empresa);
                        }
                    }
                }
            }

            return corredors;
        }


        /// <summary>
        /// Ejecuta el SP sp_listar_tipos_fianza y retorna la lista de tipos de fianza.
        /// </summary>
        /// <returns></returns>
        public async Task<List<TipoSolicitud>> ListarTipoFianzasAsync()
        {
            var tipofianzas = new List<TipoSolicitud>();

            using (var connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_listar_tipos_fianza", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var tipoFianza = new TipoSolicitud
                            {
                                TposId = reader["tpos_id"] != DBNull.Value ? Convert.ToInt32(reader["tpos_id"]) : 0,
                                TposSiglas = reader["tpos_siglas"].ToString(),
                                TposNombre = reader["tpos_nombre"].ToString(),
                                TposEstado = reader["tpos_estado"] != DBNull.Value ? Convert.ToByte(reader["tpos_estado"]) : (byte)0
                            };


                            tipofianzas.Add(tipoFianza);
                        }
                    }
                }
            }

            return tipofianzas;
        }

    }
}
