using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Helpers;
using PeliculasAPI.Migrations;
using PeliculasAPI.Servicicos;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculasController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenarArchivos almacenarArchivos;
        private readonly string contenedor = "peliculas";

        public PeliculasController(ApplicationDbContext context, IMapper mapper, IAlmacenarArchivos almacenarArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenarArchivos = almacenarArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<PeliculasIndexDto>> Get() {
            var top = 5;
            var hoy = DateTime.Today;

            var proximosEstrenos = await context.Peliculas.Where(x => x.FechaEstreno > hoy)
                .OrderBy(x => x.FechaEstreno)
                .Take(top)
                .ToListAsync();

            var enCines = await context.Peliculas.Where(x => x.EnCines)
                .Take(top).ToListAsync();

            var resultado = new PeliculasIndexDto();
            resultado.FuturosEstrenos = mapper.Map<List<PeliculaDto>>(proximosEstrenos);
            resultado.EnCines = mapper.Map<List<PeliculaDto>>(enCines);

            return resultado;

            //var peliculas = await context.Peliculas.ToListAsync();
            //return mapper.Map<List<PeliculaDto>>(peliculas);
            
        }

        [HttpGet("filtro")]
        public async Task<ActionResult<List<PeliculaDto>>> Filtrar([FromQuery] FiltroPeliculaDto filtroPeliculaDto)
        {
            //se realizará ejecución diferida
            var peliculasQueryable = context.Peliculas.AsQueryable();

            if (!string.IsNullOrEmpty(filtroPeliculaDto.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(filtroPeliculaDto.Titulo));
            }

            if (filtroPeliculaDto.EnCines)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.EnCines);
            }

            if (filtroPeliculaDto.ProximosEstrenos)
            {
                var hoy = DateTime.Today;
                peliculasQueryable = peliculasQueryable.Where(x => x.FechaEstreno > hoy);
            }

            if (filtroPeliculaDto.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable
                    .Where(x => x.PeliculasGeneros.Select( y => y.GeneroId)
                    .Contains(filtroPeliculaDto.GeneroId)
                    );
            }

            await HttpContext.InsertarParametrosPaginacion(peliculasQueryable,
                filtroPeliculaDto.CantidadRegistrosPorPagina);

            var peliculas = await peliculasQueryable.Paginar(filtroPeliculaDto.Paginacion).ToListAsync();

            return mapper.Map<List<PeliculaDto>>(peliculas);

        }

        [HttpGet("{id}", Name ="obtenerPelicula")]
        public async Task<ActionResult<PeliculaDto>> Get(int id)
        {
            var pelicula = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);
            if (pelicula == null)
            {
                return NotFound();
            }

            return mapper.Map<PeliculaDto>(pelicula);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculaCreateDto peliculaCreateDto)
        {
            var pelicula = mapper.Map<Pelicula>(peliculaCreateDto);

            if (peliculaCreateDto.Poster != null)
            {
                using (var memorystream = new MemoryStream())
                {
                    await peliculaCreateDto.Poster.CopyToAsync(memorystream);
                    var contenido = memorystream.ToArray(); //es un arreglo de bytes
                    var extension = Path.GetExtension(peliculaCreateDto.Poster.FileName);
                    pelicula.Poster = await almacenarArchivos.GuardarArchivo(contenido, extension, contenedor,
                        peliculaCreateDto.Poster.ContentType);
                }
            }

            AsignarOrdenActoresPelicula(pelicula);

            context.Add(pelicula);
            await context.SaveChangesAsync();

            var peliculaDto = mapper.Map<PeliculaDto>(pelicula);
            return new CreatedAtRouteResult("obtenerPelicula", new { id = pelicula.Id }, peliculaDto);
        }

        private void AsignarOrdenActoresPelicula(Pelicula pelicula)
        {
            if(pelicula.PeliculasActores != null)
            {
                for(int i =0; i < pelicula.PeliculasActores.Count; i++)
                {
                    pelicula.PeliculasActores[i].Orden = i;
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaUpdateDto peliculaUpdateDto)
        {
            //↓↓↓ actualizar solo los campos que se modifiquen ↓↓↓
            var peliculaDB = await context.Peliculas
                .Include(x => x.PeliculasGeneros)
                .Include(x => x.PeliculasActores)
                .FirstOrDefaultAsync(x => x.Id == id);

            if(peliculaDB == null)
            {
                return NotFound();
            }

            // ↓↓↓ Va a tomar los campos  peliculaUpdateDto  y los va a mapear hacia peliculaDB ↓↓↓
            // por tanto los campos que son distintos, van a ser actualizados
            peliculaDB = mapper.Map(peliculaUpdateDto, peliculaDB);

            if (peliculaUpdateDto.Poster != null)
            {
                using (var memorystream = new MemoryStream())
                {
                    await peliculaUpdateDto.Poster.CopyToAsync(memorystream);
                    var contenido = memorystream.ToArray(); //es un arreglo de bytes
                    var extension = Path.GetExtension(peliculaUpdateDto.Poster.FileName);
                    peliculaDB.Poster = await almacenarArchivos.EditarArchivo(contenido, extension, contenedor,
                        peliculaDB.Poster,
                        peliculaUpdateDto.Poster.ContentType);
                }
            }

            AsignarOrdenActoresPelicula(peliculaDB);

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

            var peliculaDB = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);
            if (peliculaDB == null)
            {
                return NotFound();
            }
            if (peliculaDB.Poster!= null)
            {
                await almacenarArchivos.BorrarArchivo(peliculaDB.Poster, contenedor);
            }

            context.Remove(peliculaDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
