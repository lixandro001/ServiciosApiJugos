using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ServicioJugosVentas.Data;
using ServicioJugosVentas.Models;
using System.Data;

namespace ServicioJugosVentas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CategoriasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("ListarCategorias")]
        public async Task<IActionResult> GetCategorias()
        {
            var categorias = await _context.Categorias.FromSqlRaw("EXEC sp_GetCategorias").ToListAsync();
            return Ok(categorias);
        }

        [HttpPost("GuardarCategorias")]
        public async Task<IActionResult> CreateCategoria(Categoria categoria)
        {
            using (var conn = _context.Database.GetDbConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_InsertCategoria";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@Nombre", categoria.Nombre));
                    var result = await cmd.ExecuteScalarAsync();
                    return Ok(new { CategoriaId = result });
                }
            }
        }

        //Servicio para Actulizar categorias
        [HttpPost("ActualizarCategoria")]
        public async Task<IActionResult> ActualizarCategoria(Categoria categoria)
        {
            using (var conn = _context.Database.GetDbConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_UpdateCategoria";  // Nombre del Stored Procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros del Stored Procedure
                    cmd.Parameters.Add(new SqlParameter("@CategoriaId", categoria.CategoriaId));
                    cmd.Parameters.Add(new SqlParameter("@Nombre", categoria.Nombre));

                    // Usamos ExecuteNonQueryAsync para obtener el número de filas afectadas
                    var filasAfectadas = await cmd.ExecuteNonQueryAsync();

                    if (filasAfectadas > 0)
                    {
                        return Ok(new { mensaje = "Categoría actualizada con éxito" });
                    }
                    else
                    {
                        return NotFound(new { mensaje = "No se encontró la categoría para actualizar" });
                    }
                     
                }
            }
        }

        //obtner categoria por id
        [HttpGet("ObtenerCategoria/{id}")]
        public async Task<IActionResult> ObtenerCategoriaPorId(int id)
        {
            using (var conn = _context.Database.GetDbConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_GetCategoriaxId";  // Nombre del Stored Procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetro del Stored Procedure
                    cmd.Parameters.Add(new SqlParameter("@IdCategoria", id));

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var categoria = new Categoria
                            {
                                CategoriaId = reader.GetInt32(reader.GetOrdinal("CategoriaId")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                            };

                            return Ok(categoria);
                        }
                        else
                        {
                            return NotFound(new { mensaje = "Categoría no encontrada" });
                        }
                    }
                }
            }
        }



        //eliminar categoria
        [HttpDelete("EliminarCategoria/{id}")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            using (var conn = _context.Database.GetDbConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_DeleteCategoria";  // Nombre del Stored Procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetro del Stored Procedure
                    cmd.Parameters.Add(new SqlParameter("@CategoriaId", id));

                    var filasAfectadas = await cmd.ExecuteNonQueryAsync();
                    if (filasAfectadas > 0)
                    {
                        return Ok(new { mensaje = "Categoría eliminada con éxito" });
                    }
                    else
                    {
                        return NotFound(new { mensaje = "No se encontró la categoría a eliminar" });
                    }
                }
            }
        }






    }
}
