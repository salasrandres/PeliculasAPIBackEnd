using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.PeliculasAPI.Common;
using PeliculasAPI.PeliculasAPI.DTOs;
using PeliculasAPI.PeliculasAPI.Models;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/genero")]
    public class GenerosController : _BaseController
    {

        public GenerosController(ApplicationDBContext context, IMapper mapper) : base(context, mapper)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<GeneroGet>>> Get()
        {
            return await Get<Genero, GeneroGet>();
        }

        [HttpGet("{id:int}", Name = "ObtenerGenero")]
        public async Task<ActionResult<GeneroGet>> GetById(int id)
        {
            return await Get<Genero, GeneroGet>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]GeneroPost data)
        {
            return await Post<Genero, GeneroPost, GeneroGet>(data, "ObtenerGenero");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GeneroPost gp)
        {
            return await Put<Genero, GeneroPost>(id, gp);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Genero>(id);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<GeneroPost> patch)
        {
            return await Patch<Genero, GeneroPost>(id, patch);
        }
    }
}
