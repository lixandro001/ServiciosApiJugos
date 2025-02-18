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
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductos()
        {
            var productos = await _context.Productos.FromSqlRaw("EXEC sp_GetProductos").ToListAsync();
            return Ok(productos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProducto(Producto producto)
        {
            using (var conn = _context.Database.GetDbConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_InsertProducto";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@Nombre", producto.Nombre));
                    cmd.Parameters.Add(new SqlParameter("@Descripcion", (object)producto.Descripcion ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@Precio", producto.Precio));
                    cmd.Parameters.Add(new SqlParameter("@Stock", producto.Stock));
                    cmd.Parameters.Add(new SqlParameter("@CategoriaId", producto.CategoriaId));

                    var result = await cmd.ExecuteScalarAsync();
                    return Ok(new { ProductoId = result });
                }
            }
        }
        
        //Atualizar Producto
        [HttpPut("ActualizarProducto")]
        public async Task<IActionResult> ActualizarProducto(Producto producto)
        {
            using (var conn = _context.Database.GetDbConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_UpdateProducto";  // Nombre del Stored Procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros del Stored Procedure
                    cmd.Parameters.Add(new SqlParameter("@ProductoId", producto.ProductoId));
                    cmd.Parameters.Add(new SqlParameter("@Nombre", producto.Nombre));
                    cmd.Parameters.Add(new SqlParameter("@Descripcion", producto.Descripcion ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@Precio", producto.Precio));
                    cmd.Parameters.Add(new SqlParameter("@Stock", producto.Stock));
                    cmd.Parameters.Add(new SqlParameter("@CategoriaId", producto.CategoriaId));

                    var filasAfectadas = await cmd.ExecuteNonQueryAsync();
                    if (filasAfectadas > 0)
                    {
                        return Ok(new { mensaje = "Producto actualizado con éxito" });
                    }
                    else
                    {
                        return NotFound(new { mensaje = "No se encontró el producto a actualizar" });
                    }
                }
            }
        }

        //Obtener productos por Id
        [HttpGet("ObtenerProducto/{id}")]
        public async Task<IActionResult> ObtenerProductoPorId(int id)
        {
            using (var conn = _context.Database.GetDbConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_GetProductos_ID";  // Nombre del Stored Procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetro del Stored Procedure
                    cmd.Parameters.Add(new SqlParameter("@idProducto", id));

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var producto = new Producto
                            {
                                ProductoId = reader.GetInt32(reader.GetOrdinal("ProductoId")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion"))
                                              ? string.Empty
                                              : reader.GetString(reader.GetOrdinal("Descripcion")),
                                Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
                                Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                                CategoriaId = reader.GetInt32(reader.GetOrdinal("CategoriaId")),
                                CategoriaNombre = reader.GetString(reader.GetOrdinal("CategoriaNombre"))
                            };

                            return Ok(producto);
                        }
                        else
                        {
                            return NotFound(new { mensaje = "Producto no encontrado" });
                        }
                    }
                }
            }
        }


        //Eliminar productos
        [HttpDelete("EliminarProducto/{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            using (var conn = _context.Database.GetDbConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_DeleteProducto";  // Nombre del Stored Procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetro del Stored Procedure
                    cmd.Parameters.Add(new SqlParameter("@ProductoId", id));

                    var filasAfectadas = await cmd.ExecuteNonQueryAsync();
                    if (filasAfectadas > 0)
                    {
                        return Ok(new { mensaje = "Producto eliminado con éxito" });
                    }
                    else
                    {
                        return NotFound(new { mensaje = "No se encontró el producto a eliminar" });
                    }
                }
            }
        }
        







    }
}
