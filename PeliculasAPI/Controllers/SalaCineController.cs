using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using PeliculasAPI.PeliculasAPI.Common;
using PeliculasAPI.PeliculasAPI.DTOs;
using PeliculasAPI.PeliculasAPI.Models;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/Sala")]
    public class SalaCineController : _BaseController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly GeometryFactory geometryFactory;

        public SalaCineController(ApplicationDBContext context, IMapper mapper, GeometryFactory geometryFactory) : base(context, mapper) 
        {
            this.context = context;
            this.mapper = mapper;
            this.geometryFactory = geometryFactory;
        }

        [HttpGet]
        public async Task<ActionResult<List<SalaCineGet>>> Get([FromQuery] PaginationDTO page)
        {
            return await Get<SalaCine, SalaCineGet>(page);
        }

        [HttpGet("CercaDeTi")]
        public async Task<List<SalasNearDTO>> GetNear([FromQuery] SalaNearFilterDTO sala)
        {
            var location = geometryFactory.CreatePoint(new Coordinate(sala.Longitud, sala.Latitud));
            var cinemarks = await context.SalasCines.OrderBy(x => x.Ubicacion.Distance(location))
                .Where(x => x.Ubicacion.IsWithinDistance(location, sala.DistanciaKms * 1000))
                .Select(x => new SalasNearDTO
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Latitud = x.Ubicacion.Y,
                    Longitud = x.Ubicacion.X,
                    DistanciaEnMts = Math.Round(x.Ubicacion.Distance(location))
                }).ToListAsync();
            return cinemarks;
        }

        [HttpGet("{id:int}", Name = "ObtenerSala")]
        public async Task<ActionResult<SalaCineGet>> Get(int id)
        {
            return await Get<SalaCine, SalaCineGet>(id);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<SalaCine>(id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, SalaCinePost put)
        {
            return await Put<SalaCine, SalaCinePost>(id, put);
        }

        [HttpPost]
        public async Task<ActionResult> Post(SalaCinePost post)
        {
            return await Post<SalaCine, SalaCinePost, SalaCineGet>(post, "ObtenerSala");
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<SalaCinePost> patch)
        {
            return await Patch<SalaCine, SalaCinePost>(id, patch);
        }
    }
}
