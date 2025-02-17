namespace ServicioJugosVentas.Models
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int CategoriaId { get; set; }
        // Propiedad adicional para recibir el nombre de la categoría (desde el SP)
        public string CategoriaNombre { get; set; }
    }
}
