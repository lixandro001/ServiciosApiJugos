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
    }
}
