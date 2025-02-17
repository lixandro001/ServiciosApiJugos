namespace ServicioJugosVentas.Models
{
    public class DetalleVenta
    {
        public int DetalleVentaId { get; set; }
        public int VentaId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        // El subtotal se calcula en la BD, pero si deseas replicarlo aquí:
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
