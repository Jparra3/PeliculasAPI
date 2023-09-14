using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Helpers;
using PeliculasAPI.Servicicos;
using System.Data.Common;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/actores")] 
    public class ActoresController: CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenarArchivos almacenarArchivos;
        private readonly string contenedor = "actores";

        public ActoresController(ApplicationDbContext context, 
            IMapper mapper, IAlmacenarArchivos almacenarArchivos): base (context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenarArchivos = almacenarArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDto>>> Get([FromQuery] PaginacionDto paginacionDto)
        {
            //var queryable = context.Actores.AsQueryable();
            //await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDto.CantidadRegistrosPorPagina);

            //var entidades = await queryable.Paginar(paginacionDto).ToListAsync();
            //var dto = mapper.Map<List<ActorDto>>(entidades);
            //return dto;
            return await Get<Actor, ActorDto>(paginacionDto);
        }

        [HttpGet("{id:int}", Name = "obtenerActor")]
        public async Task<ActionResult<ActorDto>> Get(int id)
        {
            //var entidad = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            //if (entidad == null)
            //{
            //    return NotFound();
            //}

            //return mapper.Map<ActorDto>(entidad);            
            return await Get<Actor, ActorDto>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreateDto actorCreateDto)
        {
            var entidad = mapper.Map<Actor>(actorCreateDto);

            if(actorCreateDto.Foto != null)
            {
                using (var memorystream = new MemoryStream())
                {
                    await actorCreateDto.Foto.CopyToAsync(memorystream);
                    var contenido = memorystream.ToArray(); //es un arreglo de bytes
                    var extension = Path.GetExtension(actorCreateDto.Foto.FileName);
                    entidad.Foto = await almacenarArchivos.GuardarArchivo(contenido, extension, contenedor, 
                        actorCreateDto.Foto.ContentType);
                }
            }

            context.Add(entidad);
            await context.SaveChangesAsync();

            var actorDto = mapper.Map<ActorDto>(entidad);
            return new CreatedAtRouteResult("obtenerActor", new { id = actorDto.Id }, actorDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorUpdateDto actorUpdateDto)
        {
            //var entidad = mapper.Map<Actor>(actorUpdateDto);
            //entidad.Id = id;
            //context.Entry(entidad).State = EntityState.Modified;

            //↓↓↓ actualizar solo los campos que se modifiquen ↓↓↓
            var actorDB = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if(actorDB == null)
            {
                return NotFound();
            }

            // ↓↓↓ Va a tomar los campos  actorUpdateDto  y los va a mapear hacia actorDB ↓↓↓
            // por tanto los campos que son distintos, van a ser actualizados
            actorDB = mapper.Map(actorUpdateDto, actorDB);

            if (actorUpdateDto.Foto != null)
            {
                using (var memorystream = new MemoryStream())
                {
                    await actorUpdateDto.Foto.CopyToAsync(memorystream);
                    var contenido = memorystream.ToArray(); //es un arreglo de bytes
                    var extension = Path.GetExtension(actorUpdateDto.Foto.FileName);
                    actorDB.Foto = await almacenarArchivos.EditarArchivo(contenido, extension, contenedor,
                        actorDB.Foto,
                        actorUpdateDto.Foto.ContentType);
                }
            }


            await context.SaveChangesAsync();
            return NoContent();
            //return new CreatedAtRouteResult("obtenerGenero", new { id = entidad.Id }, entidad);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDto> patchDocument)
        {
            if(patchDocument == null)
            {
                return BadRequest();
            }

            var entidadDB = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if (entidadDB == null)
            {
                return NotFound();
            }

            var entidadDTO = mapper.Map<ActorPatchDto>(entidadDB);

            patchDocument.ApplyTo(entidadDTO, ModelState);

            var esValido = TryValidateModel(entidadDTO);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(entidadDTO, entidadDB);

            await context.SaveChangesAsync();

            return NoContent();
        }
       
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {

            //var existe = await context.Actores.AnyAsync(x => x.Id == id);

            //if (!existe)
            //{
            //    return NotFound();
            //}
            //context.Remove(new Actor { Id = id });

            var actorDB = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if (actorDB == null)
            {
                return NotFound();
            }
            if (actorDB.Foto != null)
            {
                await almacenarArchivos.BorrarArchivo(actorDB.Foto, contenedor);
            }

            context.Remove(actorDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
