using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicioJugosVentas.Data;

namespace ServicioJugosVentas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MovimientosInventarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public MovimientosInventarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovimientos()
        {
            var movimientos = await _context.MovimientosInventario.FromSqlRaw("EXEC sp_GetMovimientosInventario").ToListAsync();
            return Ok(movimientos);
        }
    }
}
