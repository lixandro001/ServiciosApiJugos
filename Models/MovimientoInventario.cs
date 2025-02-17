namespace ServicioJugosVentas.Models
{
    public class MovimientoInventario
    {
        public int MovimientoInventarioId { get; set; }
        public int ProductoId { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string TipoMovimiento { get; set; }
        public int Cantidad { get; set; }
        public string Observaciones { get; set; }
        // Propiedad adicional para el nombre del producto
        public string ProductoNombre { get; set; }
    }
}
