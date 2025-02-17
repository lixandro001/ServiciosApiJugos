using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicioJugosVentas.Data;

namespace ServicioJugosVentas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FormasPagoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public FormasPagoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetFormasPago()
        {
            var formas = await _context.FormasPago.FromSqlRaw("EXEC sp_GetFormasPago").ToListAsync();
            return Ok(formas);
        }
    }
}
