using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.PeliculasAPI.Common;
using PeliculasAPI.PeliculasAPI.DTOs;
using PeliculasAPI.PeliculasAPI.Models;
using PeliculasAPI.PeliculasAPI.Services;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActorController : _BaseController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IUploadFiles uploadFiles;
        private readonly string container = "actores";

        public ActorController(ApplicationDBContext context, IMapper mapper, IUploadFiles uploadFiles) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.uploadFiles = uploadFiles;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorGet>>> Get([FromQuery] PaginationDTO page)
        {
            return await Get<Actor, ActorGet>(page);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorPost x)
        {
            var lastActor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if (lastActor == null) { return NotFound(); }
            lastActor = mapper.Map(x, lastActor);
            if (x.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await x.Foto.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var ext = Path.GetExtension(x.Foto.FileName);
                    lastActor.Foto = await uploadFiles.UpdateFile(content, ext, container, lastActor.Foto, x.Foto.ContentType);
                }
            }
            context.Entry(lastActor).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorPost ap)
        {
            var actorDb = mapper.Map<Actor>(ap);
            if (ap.Foto != null)
            {
                using(var memoryStream = new MemoryStream())
                {
                    await ap.Foto.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var ext = Path.GetExtension(ap.Foto.FileName);
                    actorDb.Foto = await uploadFiles.SaveFile(content, ext, container, ap.Foto.ContentType);
                }
            }
            context.Actores.Add(actorDb);
            await context.SaveChangesAsync();
            var dto = mapper.Map<ActorGet>(actorDb);
            return new CreatedAtRouteResult("ObtenerActor", new { id = actorDb.Id }, dto);
        }

        [HttpGet("{id:int}", Name = "ObtenerActor")]
        public async Task<ActionResult<ActorGet>> GetById(int id)
        {
            return await Get<Actor, ActorGet>(id);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
         {
            return await Delete<Actor>(id);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<ActorPatch> patch)
        {
            return await Patch<Actor, ActorPatch>(id, patch);
        }
    }
}
