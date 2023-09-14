using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Entidades
{
    public class SalaDeCine: IId //interfaz que permite ser utilizada para el tema  de la clase CustomBaseController
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
        public List<PeliculasSalasDeCine> PeliculasSalasDeCine { get; set; }
    }
}
