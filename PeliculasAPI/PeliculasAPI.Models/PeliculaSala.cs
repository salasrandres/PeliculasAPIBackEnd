namespace PeliculasAPI.PeliculasAPI.Models
{
    public class PeliculaSala
    {
        public int SalaCineId { get; set; }
        public int PeliculaId { get; set; }
        public Pelicula Pelicula { get; set; }
        public SalaCine SalaCine { get; set; }
    }
}
