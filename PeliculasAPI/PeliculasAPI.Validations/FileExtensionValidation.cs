using PeliculasAPI.PeliculasAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PeliculasAPI.PeliculasAPI.Validations
{
    public class FileExtensionValidation : ValidationAttribute
    {
        private readonly string[] extensions;

        public FileExtensionValidation(string[] extensions)
        {
            this.extensions = extensions;
        }

        public FileExtensionValidation(FileGroup group)
        {
            if (group == FileGroup.Imagen)
            {
                extensions = new string[] { "image/jpeg", "image/png", "image/gif" };
            }
        }
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value == null) { return ValidationResult.Success; }
            IFormFile file = value as IFormFile;
            if(file == null) { return ValidationResult.Success; }
            if (!extensions.Contains(file.ContentType)) { return new ValidationResult($"Solo se aceptar archivos de tipo {String.Join(", ", extensions)}"); }
            return ValidationResult.Success;
        }
    }
}
