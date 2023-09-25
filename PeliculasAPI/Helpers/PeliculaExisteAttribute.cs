﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace PeliculasAPI.Helpers
{
    public class PeliculaExisteAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly ApplicationDbContext dbContext;

        public PeliculaExisteAttribute(ApplicationDbContext context)
        {
            dbContext = context;
        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var peliculaIdObject = context.HttpContext.Request.RouteValues["peliculaId"];

            if (peliculaIdObject == null)
            {
                return;
            }

            var peliculaId = int.Parse(peliculaIdObject.ToString());
            var existe = await dbContext.Peliculas.AnyAsync(x => x.Id == peliculaId);
            if (!existe)
            {
                context.Result = new NotFoundResult();
            }
            else
            {
                await next();
            }



        }
    }
}
