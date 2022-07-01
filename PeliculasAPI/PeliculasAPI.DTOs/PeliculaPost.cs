using Microsoft.AspNetCore.Mvc;
using PeliculasAPI.PeliculasAPI.Enums;
using PeliculasAPI.PeliculasAPI.Utilities;
using PeliculasAPI.PeliculasAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.PeliculasAPI.DTOs
{
    public class PeliculaPost
    {
        [Required]
        [StringLength(300)]
        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
        [FileWeightValidation(4)]
        [FileExtensionValidation(group: FileGroup.Imagen)]
        public IFormFile Poster { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GenerosIDs { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<PeliculaActorDTO>>))]
        public List<PeliculaActorDTO> Actores { get; set; }
    }
}
