﻿using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.PeliculasAPI.Models
{
    public class Genero : IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(40)]
        public string Nombre { get; set; }
        public List<PeliculaGenero> PeliculaGeneros { get; set; }
    }
}
