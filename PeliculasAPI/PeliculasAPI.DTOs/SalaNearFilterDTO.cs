using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.PeliculasAPI.DTOs
{
    public class SalaNearFilterDTO
    {
        [Range(-90, 90)]
        public double Latitud { get; set; }
        [Range(-180, 180)]
        public double Longitud { get; set; }
        private int distanciaKms = 10;
        private int distanciaMax = 50;
        public int DistanciaKms
        {
            get { return distanciaKms; }
            set
            {
                distanciaKms = value > distanciaMax ? distanciaMax : value;
            }
        }
    }
}
