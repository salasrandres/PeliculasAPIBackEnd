using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;
using PeliculasAPI.PeliculasAPI.DTOs;
using PeliculasAPI.PeliculasAPI.Models;

namespace PeliculasAPI.PeliculasAPI.Utilities
{
    public class _AutoMapper : Profile
    {
        public _AutoMapper(GeometryFactory geometryFactory)
        {
            CreateMap<Genero, GeneroGet>().ReverseMap();
            CreateMap<GeneroPost, Genero>();


            CreateMap<Actor, ActorGet>().ReverseMap();
            CreateMap<ActorPost, Actor>();
            CreateMap<Actor, ActorPatch>().ReverseMap();

            CreateMap<Pelicula, PeliculaGet>().ReverseMap();
            CreateMap<PeliculaPost, Pelicula>()
                .ForMember(x => x.PeliculaGeneros, opc => opc.MapFrom(MapPeliculaGenero))
                .ForMember(x => x.PeliculaActores, opc => opc.MapFrom(MapPeliculaActor));
            CreateMap<Pelicula, PeliculaDetailDTO>()
                .ForMember(x => x.Actores, opc => opc.MapFrom(MapActoresPelicula))
                .ForMember(x => x.Generos, opc => opc.MapFrom(MapGenerosPelicula));

            CreateMap<SalaCine, SalaCineGet>()
                .ForMember(x => x.Longitud, x => x.MapFrom(y => y.Ubicacion.X))
                .ForMember(x => x.Latitud, x => x.MapFrom(y => y.Ubicacion.Y));
            CreateMap<SalaCineGet, SalaCine>()
                .ForMember(x => x.Ubicacion, x => x.MapFrom(y => geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))));
            CreateMap<SalaCinePost, SalaCine>()
                .ForMember(x => x.Ubicacion, x => x.MapFrom(y => geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))));

            CreateMap<IdentityUser, UserGet>();
        }

        private List<GeneroGet> MapGenerosPelicula(Pelicula pelicula, PeliculaDetailDTO dto)
        {
            var result = new List<GeneroGet>();
            if(pelicula.PeliculaGeneros == null) { return result; }
            foreach(var genero_pelicula in pelicula.PeliculaGeneros)
            {
                result.Add(new GeneroGet() { Id = genero_pelicula.PeliculaId, Nombre = genero_pelicula.Genero.Nombre });
            }
            return result;
        }

        private List<ActorPeliculaDTO> MapActoresPelicula(Pelicula pelicula, PeliculaDetailDTO dto)
        {
            var result = new List<ActorPeliculaDTO>();
            if(pelicula.PeliculaActores == null) { return result; }
            foreach(var peliculaActor in pelicula.PeliculaActores)
            {
                result.Add(new ActorPeliculaDTO() { ActorId = peliculaActor.ActorId, NombreActor = peliculaActor.Actor.Nombre, 
                    Personaje = peliculaActor.Personaje });
            }
            return result;
        }

        private List<PeliculaGenero> MapPeliculaGenero(PeliculaPost peliculaPost, Pelicula pelicula)
        {
            var result = new List<PeliculaGenero>();
            if(peliculaPost.GenerosIDs == null) { return result; }
            foreach (var id in peliculaPost.GenerosIDs)
            {
                result.Add(new PeliculaGenero() { GeneroId = id });
            }

            return result;
        }

        private List<PeliculaActor> MapPeliculaActor(PeliculaPost peliculaPost, Pelicula pelicula)
        {
            var result = new List<PeliculaActor>();
            if(peliculaPost.Actores == null) { return result; }
            foreach(var actor in peliculaPost.Actores)
            {
                result.Add(new PeliculaActor() { ActorId = actor.ActorId, Personaje = actor.Personaje });
            }
            return result;
        }
    }
}
