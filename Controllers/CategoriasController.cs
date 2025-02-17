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
    }
}
