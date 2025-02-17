using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ServicioJugosVentas.Data;
using ServicioJugosVentas.Models;
using System.Data;
using System.Security.Claims;

namespace ServicioJugosVentas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VentasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DTO para el request de venta
        public class VentaRequest
        {
            public int? ClienteId { get; set; }
            public int FormaPagoId { get; set; }
            public List<DetalleVentaDto> DetalleVentas { get; set; }
        }

        public class DetalleVentaDto
        {
            public int ProductoId { get; set; }
            public int Cantidad { get; set; }
            public decimal PrecioUnitario { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetVentas()
        {
            // Para simplificar, se retorna una consulta simple
            var ventas = await _context.Ventas.FromSqlRaw("SELECT * FROM Ventas").ToListAsync();
            return Ok(ventas);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVenta(VentaRequest request)
        {
            // Obtener el usuario autenticado
            var userClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userClaim == null)
                return Unauthorized();
            int usuarioId = int.Parse(userClaim.Value);

            // Calcular el total de la venta
            decimal total = request.DetalleVentas.Sum(d => d.Cantidad * d.PrecioUnitario);

            // Preparar DataTable para TVP
            DataTable dt = new DataTable();
            dt.Columns.Add("ProductoId", typeof(int));
            dt.Columns.Add("Cantidad", typeof(int));
            dt.Columns.Add("PrecioUnitario", typeof(decimal));
            foreach (var d in request.DetalleVentas)
            {
                dt.Rows.Add(d.ProductoId, d.Cantidad, d.PrecioUnitario);
            }

            using (var conn = _context.Database.GetDbConnection() as SqlConnection)
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_InsertVenta";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@UsuarioId", usuarioId));
                    cmd.Parameters.Add(new SqlParameter("@ClienteId", request.ClienteId.HasValue ? (object)request.ClienteId.Value : DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@FormaPagoId", request.FormaPagoId));
                    cmd.Parameters.Add(new SqlParameter("@Total", total));

                    var tvp = new SqlParameter("@DetalleVenta", dt)
                    {
                        SqlDbType = SqlDbType.Structured,
                        TypeName = "dbo.DetalleVentaType"
                    };
                    cmd.Parameters.Add(tvp);

                    await cmd.ExecuteNonQueryAsync();
                    return Ok("Venta registrada correctamente.");
                }
            }
        }
    }
}
