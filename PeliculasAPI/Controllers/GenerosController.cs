using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/generos")]
    public class GenerosController: CustomBaseController
    {
        //Para controladores que realizan crud simples, una clase base (CustomBaseController)
        //Es muy util porque nos permite reutilizar el código

        //private readonly ApplicationDbContext context;
        //private readonly IMapper mapper;

        public GenerosController(ApplicationDbContext context,
            IMapper mapper): base(context, mapper)
        {
            //this.context = context;
            //this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GeneroDto>>> Get()
        {
            //var entidades = await context.Generos.ToListAsync();
            //var dto =  mapper.Map<List<GeneroDto>>(entidades);
            //return dto;
            return await Get<Genero,GeneroDto>();
        }
        
        [HttpGet("{id:int}", Name = "obtenerGenero")]
        public async Task<ActionResult<GeneroDto>> Get(int id)
        {
            //var entidad = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            //if(entidad == null)
            //{
            //    return NotFound();
            //}

            //var dto = mapper.Map<GeneroDto>(entidad);
            //return dto;
            return await Get<Genero, GeneroDto>(id);
        }
        
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreateDto generoCreateDto)
        {
            //var entidad = mapper.Map<Genero>(generoCreateDto);
            //context.Add(entidad);
            //await context.SaveChangesAsync();

            //var generoDto = mapper.Map<GeneroDto>(entidad);
            //return new CreatedAtRouteResult("obtenerGenero", new {id = generoDto.Id}, generoDto);
            return await Post<GeneroCreateDto, Genero, GeneroDto>(generoCreateDto, "obtenerGenero");
        }
       
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GeneroUpdateDto generoUpdateDto)
        {
            //var entidad = mapper.Map<Genero>(generoUpdateDto);
            //entidad.Id = id;
            //context.Entry(entidad).State = EntityState.Modified;
            //await context.SaveChangesAsync();
            //return NoContent();

            return await Put<GeneroUpdateDto, Genero>(id, generoUpdateDto);

            //return new CreatedAtRouteResult("obtenerGenero", new { id = entidad.Id }, entidad);
        }
        
       [HttpDelete("{id:int}")]
       public async Task<ActionResult> Delete(int id)
       {
           //var existe = await context.Generos.AnyAsync(x => x.Id == id);

           //if (!existe)
           //{
           //    return NotFound();
           //}

           //context.Remove(new Genero { Id = id });
           //await context.SaveChangesAsync();
           //return NoContent();

            return await Delete<Genero>(id);
       }

    }
}
