using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.PeliculasAPI.DTOs;
using PeliculasAPI.PeliculasAPI.Utilities;

namespace PeliculasAPI.PeliculasAPI.Common
{
    public class _BaseController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public _BaseController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntity, TDTO>() where TEntity : class
        {
            var entities = await context.Set<TEntity>().AsNoTracking().ToListAsync();
            return mapper.Map<List<TDTO>>(entities);
        }

        protected async Task<ActionResult<TDTO>> Get<TEntity, TDTO>(int id) where TEntity : class, IId
        {
            var entity = await context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if(entity == null) { return BadRequest(); }
            return mapper.Map<TDTO>(entity);
        }

        protected async Task<ActionResult> Post<TEntity, TDTO, TRead>(TDTO created, string route) where TEntity: class, IId
        {
            var entity = mapper.Map<TEntity>(created);
            context.Add(entity);
            await context.SaveChangesAsync();
            var dto = mapper.Map<TRead>(entity);
            return new CreatedAtRouteResult(route, new { id = entity.Id }, dto);
        }

        protected async Task<ActionResult<List<TDTO>>> Get<TEntity, TDTO>(PaginationDTO page) where TEntity : class
        {
            var queryable = context.Set<TEntity>().AsQueryable();
            await HttpContext.InsertParamsPagination(queryable, page.RowsxPagina);
            var result = await queryable.Paginar(page).ToListAsync();
            return mapper.Map<List<TDTO>>(result);
        }

        protected async Task<ActionResult> Put<TEntity, TDTO>(int id, TDTO update) where TEntity : class, IId
        {
            var entity = await context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return BadRequest($"No existe una entidad con el id {id}");
            entity = mapper.Map(update, entity);
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        protected async Task<ActionResult> Delete<TEntity>(int id) where TEntity : class, IId, new()
        {
            var entity = await context.Set<TEntity>().AsNoTracking().AnyAsync(x => x.Id == id);
            if(!entity)
                return BadRequest($"No existe la entidad con el id {id}");
            context.Remove(new TEntity() { Id = id});
            await context.SaveChangesAsync();
            return NoContent();
        }

        protected async Task<ActionResult> Patch<TEntity, TDTO>(int id, JsonPatchDocument<TDTO> patch) where TDTO : class where TEntity : class, IId
        {
            if (patch == null) { return BadRequest(); }
            var entity = await context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) { return NotFound(); }
            var entityDTO = mapper.Map<TDTO>(entity);
            patch.ApplyTo(entityDTO, ModelState);

            var isValid = TryValidateModel(entityDTO);

            if (!isValid) { return BadRequest(ModelState); }
            mapper.Map(entityDTO, entity);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
