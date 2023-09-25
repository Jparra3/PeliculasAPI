using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class ReviewCreateDto
    {
        public string Comentario { get; set; }
        [Range(1, 5)]
        public int Puntuacion { get; set; }
    }
}
