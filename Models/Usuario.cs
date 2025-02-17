namespace ServicioJugosVentas.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}
