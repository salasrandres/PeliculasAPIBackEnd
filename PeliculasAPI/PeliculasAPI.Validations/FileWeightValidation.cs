using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.PeliculasAPI.Validations
{
    public class FileWeightValidation : ValidationAttribute
    {
        private readonly int max;

        public FileWeightValidation(int max)
        {
            this.max = max;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) { return ValidationResult.Success; }

            IFormFile file = value as IFormFile;

            if (file == null) { return ValidationResult.Success; }

            if(file.Length > max * 1024 * 1024) 
            { return new ValidationResult($"El peso del archivo debe ser menor a {max} mb"); }

            return ValidationResult.Success;

        }
    }
}
