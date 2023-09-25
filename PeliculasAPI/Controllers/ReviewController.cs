using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using PeliculasAPI.Helpers;

namespace PeliculasAPI.Controllers
{

    // api/pelicula/2/review
    [ApiController]
    [Route("api/peliculas/{peliculaId:int}/review")]
    [ServiceFilter(typeof(PeliculaExisteAttribute))]
    public class ReviewController: CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ReviewController(
            ApplicationDbContext context,
            IMapper mapper): base (context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReviewDto>>> Get(int peliculaId, [FromQuery] PaginacionDto paginacion)
        {
            //var existe = await context.Peliculas.AnyAsync(x => x.Id == peliculaId);
            //if (!existe)
            //{
            //    return NotFound();
            //}

            var queryable = context.Reviews.Include(x => x.Usuario).AsQueryable();
            queryable = queryable.Where(x => x.PeliculaId == peliculaId);
            return await Get<Review, ReviewDto>(paginacion, queryable);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int peliculaId, [FromBody] ReviewCreateDto reviewCreateDto)
        {
            //var existe = await context.Peliculas.AnyAsync(x => x.Id == peliculaId);
            //if (!existe)
            //{
            //    return NotFound();
            //}

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var reviewExiste = await context.Reviews
                .AnyAsync(x => x.PeliculaId == peliculaId && x.UsuarioId == usuarioId);

            if (reviewExiste)
            {
                return BadRequest("El usuario ya ha escrito un review de esta pelicula");
            }

            var review = mapper.Map<Review>(reviewCreateDto);
            review.PeliculaId = peliculaId;
            review.UsuarioId = usuarioId;

            context.Add(review);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(int peliculaId, int reviewId, [FromBody] ReviewCreateDto reviewCreateDto)
        {
            //var existe = await context.Peliculas.AnyAsync(x => x.Id == peliculaId);
            //if (!existe)
            //{
            //    return NotFound();
            //}

            var reviewDb = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
            if (reviewDb == null) { return NotFound(); }

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(X => X.Type == ClaimTypes.NameIdentifier).Value;

            if (reviewDb.UsuarioId != usuarioId) {
                return BadRequest("No tiene permisos de editar este review");
                //return Forbid();
            }


            reviewDb = mapper.Map(reviewCreateDto, reviewDb);
            
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete(int reviewId)
        {
            var reviewDb = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
            if (reviewDb == null) { return NotFound(); }

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(X => X.Type == ClaimTypes.NameIdentifier).Value;

            if (reviewDb.UsuarioId != usuarioId) { return Forbid(); }

            context.Remove(reviewDb);
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
