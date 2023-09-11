using Microsoft.AspNetCore.Mvc;
using PeliculasAPI.Helpers;
using PeliculasAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class PeliculaUpdateDto
    {
        [Required]
        [StringLength(300)]
        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
        [PesoArchivoValidacion(pesoMaximoEnMB: 4)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile Poster { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))] //model binder personalizado que se encuentra en helpers -> TypeBinder.cs
        public List<int> GenerosId { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorPeliculasUpdateDto>>))] //model binder personalizado que se encuentra en helpers -> TypeBinder.cs
        public List<ActorPeliculasUpdateDto> Actores { get; set; }

    }
}
