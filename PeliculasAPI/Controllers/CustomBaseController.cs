using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Helpers;

namespace PeliculasAPI.Controllers
{
    public class CustomBaseController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CustomBaseController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TDto>> Get<TEntidad, TDto>() where TEntidad : class
        {
            var entidades = await context.Set<TEntidad>().ToListAsync();
            var dtos = mapper.Map<List<TDto>>(entidades);
            return dtos;
        }

        protected async Task<List<TDto>> Get<TEntidad, TDto>(PaginacionDto paginacionDto)
            where TEntidad: class
        {
            var queryable = context.Set<TEntidad>().AsQueryable();
            await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDto.CantidadRegistrosPorPagina);

            var entidades = await queryable.Paginar(paginacionDto).ToListAsync();
            var dto = mapper.Map<List<TDto>>(entidades);
            return dto;
        }
        protected async Task<ActionResult<TDto>> Get<TEntidad, TDto>(int id) where TEntidad : class, IId
        {
            var entidad = await context.Set<TEntidad>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entidad == null)
            {
                return NotFound();
            }

            return mapper.Map<TDto>(entidad);
        }

        protected async Task<ActionResult> Post<TCreacion, TEntidad, TLectura>
            (TCreacion creacionDto, string nombreRuta) where TEntidad : class, IId
        {
            var entidad = mapper.Map<TEntidad>(creacionDto);
            context.Add(entidad);
            await context.SaveChangesAsync();

            var dtoLectura = mapper.Map<TLectura>(entidad);
            return new CreatedAtRouteResult(nombreRuta, new { id = entidad.Id }, dtoLectura);
        }

        protected async Task<ActionResult> Put<TUpdate, TEntidad>
            (int id, TUpdate updateDto) where TEntidad: class, IId
        {
            var entidad = mapper.Map<TEntidad>(updateDto);
            entidad.Id = id;
            context.Entry(entidad).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        protected async Task<ActionResult> Patch<TEntidad, TDto>(int id, JsonPatchDocument<TDto> patchDocument)
            where TDto : class
            where TEntidad : class, IId
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entidadDB = await context.Set<TEntidad>().FirstOrDefaultAsync(x => x.Id == id);
            if (entidadDB == null)
            {
                return NotFound();
            }

            var entidadDTO = mapper.Map<TDto>(entidadDB);

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

        protected async Task<ActionResult> Delete<TEntidad>(int id) where TEntidad : class, IId, new()
        {
            var existe = await context.Set<TEntidad>().AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new TEntidad() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
