using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.PeliculasAPI.Common;
using PeliculasAPI.PeliculasAPI.DTOs;
using PeliculasAPI.PeliculasAPI.Models;
using PeliculasAPI.PeliculasAPI.Services;
using PeliculasAPI.PeliculasAPI.Utilities;
using System.Linq.Dynamic.Core;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/pelicula")]
    public class PeliculaController : _BaseController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IUploadFiles uploadFile;
        private readonly string container = "peliculas";

        public PeliculaController(ApplicationDBContext context, IMapper mapper, IUploadFiles uploadFile) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.uploadFile = uploadFile;
        }

        [HttpGet]
        public async Task<ActionResult<PeliculaIndexDTO>> Get()
        {
            var top = 5;
            var hoy = DateTime.Today;
            var proximos = await context.Peliculas.Where(x => x.FechaEstreno > hoy).OrderBy(x => x.FechaEstreno)
                .Take(top).ToListAsync();
            var enCine = await context.Peliculas.Where(x => x.EnCines == true).OrderBy(x => x.FechaEstreno)
                .Take(top).ToListAsync();
            var result = new PeliculaIndexDTO();
            result.ProximosEstrenos = mapper.Map<List<PeliculaGet>>(proximos);
            result.EnCines = mapper.Map<List<PeliculaGet>>(enCine);

            return result;
        }

        [HttpGet("{id:int}", Name = "ObtenerPelicula")]
        public async Task<ActionResult<PeliculaDetailDTO>> GetById(int id)
        {
            var pelicula = await context.Peliculas.Include(x => x.PeliculaActores).ThenInclude(x => x.Actor)
                .Include(x => x.PeliculaGeneros).ThenInclude(x => x.Genero).FirstOrDefaultAsync(x => x.Id == id);
            if(pelicula == null) { return NotFound(); }
            return mapper.Map<PeliculaDetailDTO>(pelicula);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<PeliculaGet>>> Filter([FromQuery] PaginationDTO page)
        {
            var peliculasQueryable = context.Peliculas.AsQueryable();
            if (!string.IsNullOrEmpty(page.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(page.Titulo));
            }
            if (page.EnCines) { peliculasQueryable = peliculasQueryable.Where(x => x.EnCines); }
            if (page.ProximosEstrenos)
            {
                var today = DateTime.Now;
                peliculasQueryable = peliculasQueryable.Where(x => x.FechaEstreno > today);
            }
            if(page.GeneroId != 0) 
            {
                peliculasQueryable = peliculasQueryable.Where
                    (x => x.PeliculaGeneros.Select(y => y.GeneroId).Contains(page.GeneroId));
            }
            if (!string.IsNullOrEmpty(page.CampoOrden))
            {
                try
                {
                    var asc = page.Ascendente ? "ascending" : "descending";
                    peliculasQueryable = peliculasQueryable.OrderBy($"{page.CampoOrden} {asc}");
                }
                catch
                {
                    return BadRequest("El campo de orden no existe");
                }
            }
            await HttpContext.InsertParamsPagination(peliculasQueryable, page.RowsxPagina);
            var result = await peliculasQueryable.Paginar(page).ToListAsync();
            return mapper.Map<List<PeliculaGet>>(result);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculaPost peli)
        {
            var pelicula = mapper.Map<Pelicula>(peli);
            if (peli.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peli.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var ext = Path.GetExtension(peli.Poster.FileName);
                    pelicula.Poster = await uploadFile.SaveFile(content, ext, container, peli.Poster.ContentType);
                }
            }
            context.Peliculas.Add(pelicula);
            await context.SaveChangesAsync();
            var dto = mapper.Map<PeliculaGet>(pelicula);
            return new CreatedAtRouteResult("ObtenerPelicula", new { id = pelicula.Id }, dto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<PeliculaGet>> Put(int id, [FromForm] PeliculaPost update)
        {
            var pelicula = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);
            if (pelicula == null)
                return NotFound();
            pelicula = mapper.Map(update, pelicula);
            if (update.Poster != null)
            {
                using(var memoryStream = new MemoryStream())
                {
                    await update.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var ext = Path.GetExtension(update.Poster.FileName);
                    pelicula.Poster = await uploadFile.UpdateFile(content, ext, container, pelicula.Poster, update.Poster.ContentType);
                }
            }
            context.Entry(pelicula).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Pelicula>(id);
        }
    }
}
