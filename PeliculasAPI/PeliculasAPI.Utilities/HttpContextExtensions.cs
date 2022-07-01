using Microsoft.EntityFrameworkCore;

namespace PeliculasAPI.PeliculasAPI.Utilities
{
    public static class HttpContextExtensions
    {
        public async static Task InsertParamsPagination<T>(this HttpContext httpContext,
            IQueryable<T> queryable, int cantidad)
        {
            double rows = await queryable.CountAsync();
            double pages = Math.Ceiling(rows / cantidad);
            httpContext.Response.Headers.Add("paginas", pages.ToString());
        }
    }
}
