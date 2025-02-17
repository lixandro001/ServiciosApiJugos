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
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetClientes()
        {
            var clientes = await _context.Clientes.FromSqlRaw("EXEC sp_GetClientes").ToListAsync();
            return Ok(clientes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCliente(Cliente cliente)
        {
            using (var conn = _context.Database.GetDbConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_InsertCliente";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@Nombre", cliente.Nombre));
                    cmd.Parameters.Add(new SqlParameter("@Direccion", (object)cliente.Direccion ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@Telefono", (object)cliente.Telefono ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@Email", (object)cliente.Email ?? DBNull.Value));

                    var result = await cmd.ExecuteScalarAsync();
                    return Ok(new { ClienteId = result });
                }
            }
        }
    }
}
