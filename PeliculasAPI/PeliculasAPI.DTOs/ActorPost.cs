using Microsoft.AspNetCore.Http;
using PeliculasAPI.PeliculasAPI.Enums;
using PeliculasAPI.PeliculasAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.PeliculasAPI.DTOs
{
    public class ActorPost : ActorPatch
    {
        
        [FileWeightValidation(4)]
        [FileExtensionValidation(group: FileGroup.Imagen)]
        public IFormFile Foto { get; set; }
    }
}
