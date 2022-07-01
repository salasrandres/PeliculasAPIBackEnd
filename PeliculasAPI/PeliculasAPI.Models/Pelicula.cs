using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.PeliculasAPI.Models
{
    public class Pelicula : IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(300)]
        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
        public string Poster { get; set; }
        public List<PeliculaActor> PeliculaActores { get; set; }
        public List<PeliculaGenero> PeliculaGeneros { get; set; }
        public List<PeliculaSala> PeliculasSalas { get; set; }
    }
}
