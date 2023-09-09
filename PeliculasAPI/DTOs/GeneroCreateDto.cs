using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class GeneroCreateDto
    {
        [Required]
        [StringLength(40)]
        public string Nombre { get; set; }
    }
}
