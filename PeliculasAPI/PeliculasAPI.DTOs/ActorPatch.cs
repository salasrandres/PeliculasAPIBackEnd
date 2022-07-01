using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.PeliculasAPI.DTOs
{
    public class ActorPatch
    {
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
    }
}
