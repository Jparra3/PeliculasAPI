namespace PeliculasAPI.DTOs
{
    public class PaginacionDto
    {
        public int Pagina { get; set; } = 1;
        private int cantidadRegistrosPorPagina { get; set; } = 10;
        private int cantidadMaximaRegistrosPorPagina { get; set; } = 50;
        public int CantidadRegistrosPorPagina {
            get => cantidadRegistrosPorPagina;
            set
            {
                cantidadRegistrosPorPagina = (value > cantidadMaximaRegistrosPorPagina) ? cantidadMaximaRegistrosPorPagina : value;
            }
        }
    }
}
