namespace PeliculasAPI.DTOs
{
    public class PeliculaReadDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
        public string Poster { get; set; }

        public List<GeneroDto> Generos { get; set; }
        public List<ActorPeliculaReadDto> Actores { get; set; }
    }
}
