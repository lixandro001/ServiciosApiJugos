namespace ServicioJugosVentas.Models
{
    public class Venta
    {
        public int VentaId { get; set; }
        public DateTime FechaVenta { get; set; }
        public int UsuarioId { get; set; }
        public int? ClienteId { get; set; }
        public int FormaPagoId { get; set; }
        public decimal Total { get; set; }
    }
}
