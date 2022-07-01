using PeliculasAPI.PeliculasAPI.DTOs;

namespace PeliculasAPI.PeliculasAPI.Utilities
{
    public static class Pagination
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginationDTO page)
        {
            return queryable.Skip((page.Pagina - 1) * page.RowsxPagina).Take(page.RowsxPagina);
        }
    }
}
