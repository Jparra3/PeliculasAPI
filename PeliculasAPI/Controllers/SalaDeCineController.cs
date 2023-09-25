using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Controllers
{
    [Route("api/salasdecine")]
    [ApiController]
    public class SalaDeCineController: CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly GeometryFactory geometryFactory;

        public SalaDeCineController(ApplicationDbContext context, 
            IMapper mapper,
            GeometryFactory geometryFactory): base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.geometryFactory = geometryFactory;
        }

        [HttpGet]
        public async Task<ActionResult<List<SalaDeCineDto>>> Get()
        {
            return await Get<SalaDeCine, SalaDeCineDto>();
        }

        [HttpGet("{id}", Name = "obtenerSalaDeCine")]
        public async Task<ActionResult<SalaDeCineDto>> Get(int id)
        {
            return await Get<SalaDeCine, SalaDeCineDto>(id);
        }

        [HttpGet("cercanos")]
        public async Task<ActionResult<List<SalaDeCineCercanoDto>>> Cercanos(
            [FromQuery] SalaDeCineCercanoFiltroDto salaDeCineCercanoFiltro)
        {
            var ubicacionUsuario = geometryFactory.CreatePoint(new Coordinate(salaDeCineCercanoFiltro.Longitud, salaDeCineCercanoFiltro.Latitud));
            var salasDeCine = await context.SalaDeCines
                .OrderBy(x => x.Ubicacion.Distance(ubicacionUsuario))
                .Where(x => x.Ubicacion.IsWithinDistance(ubicacionUsuario, salaDeCineCercanoFiltro.DistanciaEnKms * 1000))
                .Select(x => new SalaDeCineCercanoDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Latitud = x.Ubicacion.Y,
                    Longitud = x.Ubicacion.X,
                    DistanciaEnMetros = Math.Round(x.Ubicacion.Distance(ubicacionUsuario))
                }).ToListAsync();

            return salasDeCine;
        }


        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SalaDeCineCreateDto salaDeCineCreateDto)
        {
            return await Post<SalaDeCineCreateDto, SalaDeCine, SalaDeCineDto>(salaDeCineCreateDto, "obtenerSalaDeCine");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] SalaDeCineCreateDto salaDeCineCreateDto)
        {
            return await Put<SalaDeCineCreateDto, SalaDeCine>(id, salaDeCineCreateDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<SalaDeCine>(id);
        }
    }
}
