using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class GeneroUpdateDto
    {
        [Required]
        [StringLength(40)]
        public string Nombre { get; set; }
    }
}
