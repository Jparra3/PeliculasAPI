using AutoMapper;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genero, GeneroDto>().ReverseMap();

            CreateMap<GeneroCreateDto, Genero>();

            CreateMap<GeneroUpdateDto, Genero>();

            CreateMap<Actor, ActorDto>().ReverseMap();

            CreateMap<ActorCreateDto, Actor>()
                .ForMember(x => x.Foto, options => options.Ignore());//lo que hace es no mapear el campo foto

            CreateMap<ActorUpdateDto, Actor>()
                .ForMember(x => x.Foto, options => options.Ignore());//lo que hace es no mapear el campo foto

        }
    }
}
