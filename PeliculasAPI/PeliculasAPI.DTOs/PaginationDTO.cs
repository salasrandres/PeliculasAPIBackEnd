namespace PeliculasAPI.PeliculasAPI.DTOs
{
    public class PaginationDTO
    {
        public int Pagina { get; set; } = 1;
        private int rowsxPagina = 10;
        private readonly int maxRows = 50;

        public int RowsxPagina
        {
            get => rowsxPagina;
            set
            {
                rowsxPagina = value > maxRows ? maxRows : value;
            }
        }
        public string Titulo { get; set; }
        public int GeneroId { get; set; }
        public bool EnCines { get; set; }
        public bool ProximosEstrenos { get; set; }

        public string CampoOrden { get; set; }
        public bool Ascendente { get; set; }

    }
}
