namespace PeliculasAPI.PeliculasAPI.DTOs
{
    public class PeliculaDetailDTO : PeliculaGet
    {
        public List<GeneroGet> Generos { get; set; }
        public List<ActorPeliculaDTO> Actores { get; set; }
    }
}
