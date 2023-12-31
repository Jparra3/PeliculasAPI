﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Helpers
{
    public class AutoMapperProfiles: Profile
    {

        //el geometryFactory viene en la inyección de dependencias que están configuradas desde el startup
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            CreateMap<Genero, GeneroDto>().ReverseMap();

            CreateMap<GeneroCreateDto, Genero>();

            CreateMap<GeneroUpdateDto, Genero>();

            CreateMap<Review, ReviewDto>()
                .ForMember(x => x.NombreUsuario, x => x.MapFrom(y => y.Usuario.UserName));

            CreateMap<ReviewDto, Review>();
            CreateMap<ReviewCreateDto, Review>();

            CreateMap<IdentityUser, UsuarioDto>();

            //Actores

            CreateMap<Actor, ActorDto>().ReverseMap();

            CreateMap<ActorCreateDto, Actor>()
                .ForMember(x => x.Foto, options => options.Ignore());//lo que hace es no mapear el campo foto

            CreateMap<ActorUpdateDto, Actor>()
                .ForMember(x => x.Foto, options => options.Ignore());//lo que hace es no mapear el campo foto

            CreateMap<ActorPatchDto, Actor>().ReverseMap();

            //Peliculas

            CreateMap<Pelicula, PeliculaDto>().ReverseMap();

            CreateMap<PeliculaCreateDto, Pelicula>()
                .ForMember(x => x.Poster, options => options.Ignore())//lo que hace es no mapear el campo foto
                .ForMember(x => x.PeliculasGeneros, options => options.MapFrom(MapPeliculasGeneroCreate))
                .ForMember(x => x.PeliculasActores, options => options.MapFrom(MapPeliculasActoresCreate));

            CreateMap<PeliculaUpdateDto, Pelicula>()
                .ForMember(x => x.Poster, options => options.Ignore())//lo que hace es no mapear el campo foto
                .ForMember(x => x.PeliculasGeneros, options => options.MapFrom(MapPeliculasGeneroUpdate))
                .ForMember(x => x.PeliculasActores, options => options.MapFrom(MapPeliculasActoresUpdate));

            CreateMap<Pelicula, PeliculaReadDto>()
                .ForMember(x => x.Generos, options => options.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.Actores, options => options.MapFrom(MapPeliculasActores));


            CreateMap<SalaDeCine, SalaDeCineDto>()
                .ForMember(x => x.Latitud, x => x.MapFrom(y => y.Ubicacion.Y))
                .ForMember(x => x.Longitud, x => x.MapFrom(y => y.Ubicacion.X));

            CreateMap<SalaDeCineDto, SalaDeCine>()
                .ForMember(x => x.Ubicacion, x => x.MapFrom(y => 
                    geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))
                ));
            CreateMap<SalaDeCineCreateDto, SalaDeCine>()
                .ForMember(x => x.Ubicacion, x => x.MapFrom(y =>
                    geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))
                )); ;

        }

        private List<GeneroDto> MapPeliculasGeneros(Pelicula pelicula, PeliculaReadDto peliculaReadDto)
        {
            var resultado = new List<GeneroDto>();
            if(pelicula.PeliculasGeneros == null) { return resultado;  }
            foreach(var generoPelicula in pelicula.PeliculasGeneros)
            {
                resultado.Add(new GeneroDto() { Id = generoPelicula.GeneroId, Nombre = generoPelicula.Genero.Nombre });
            }

            return resultado;
        }

        private List<ActorPeliculaReadDto> MapPeliculasActores(Pelicula pelicula, PeliculaReadDto peliculaReadDto)
        {
            var resultado = new List<ActorPeliculaReadDto>();
            if(pelicula.PeliculasActores == null) { return resultado; }

            foreach(var actorPelicula in pelicula.PeliculasActores)
            {
                resultado.Add(new ActorPeliculaReadDto
                {
                    ActorId = actorPelicula.ActorId,
                    Personaje = actorPelicula.Personaje,
                    NombrePersona = actorPelicula.Actor.Nombre
                });
            }

            return resultado;

        }


        private List<PeliculasGeneros> MapPeliculasGeneroCreate(PeliculaCreateDto peliculaCreateDto, Pelicula pelicula)
        {
            var resultado = new List<PeliculasGeneros>();
            if (peliculaCreateDto.GenerosId == null){ return resultado; }
            foreach(var id in peliculaCreateDto.GenerosId)
            {
                resultado.Add(new PeliculasGeneros() { GeneroId = id });
            }
            return resultado;
        }

        private List<PeliculasActores> MapPeliculasActoresCreate(PeliculaCreateDto peliculaCreateDto, Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();
            if (peliculaCreateDto.Actores == null) { return resultado; }
            foreach (var actor in peliculaCreateDto.Actores)
            {
                resultado.Add(new PeliculasActores() { ActorId = actor.ActorId, Personaje = actor.Personaje});
            }
            return resultado;
        }
        private List<PeliculasGeneros> MapPeliculasGeneroUpdate(PeliculaUpdateDto peliculaUpdateDto, Pelicula pelicula)
        {
            var resultado = new List<PeliculasGeneros>();
            if (peliculaUpdateDto.GenerosId == null) { return resultado; }
            foreach (var id in peliculaUpdateDto.GenerosId)
            {
                resultado.Add(new PeliculasGeneros() { GeneroId = id });
            }
            return resultado;
        }

        private List<PeliculasActores> MapPeliculasActoresUpdate(PeliculaUpdateDto peliculaUpdateDto, Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();
            if (peliculaUpdateDto.Actores == null) { return resultado; }
            foreach (var actor in peliculaUpdateDto.Actores)
            {
                resultado.Add(new PeliculasActores() { ActorId = actor.ActorId, Personaje = actor.Personaje });
            }
            return resultado;
        }

    }
}
