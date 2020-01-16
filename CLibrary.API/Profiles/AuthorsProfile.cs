using AutoMapper;
using CLibrary.API.Entities;
using CLibrary.API.Helpers;
using CLibrary.API.Models;

namespace CLibrary.API.Profiles {
    public class AuthorsProfile : Profile {
        public AuthorsProfile() {
            CreateMap<Author, AuthorDto>()
                .ForMember(
                    dest => dest.Name,
                    opt => 
                        opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(
                    dest => dest.Age,
                    opt => 
                        opt.MapFrom(src => src.DateOfBirth.GetCurrentAge(src.DateOdDeath)));
            CreateMap<AuthorForCreationDto, Author>();
            CreateMap<AuthorForUpdateDTO, Author>().ReverseMap();
            CreateMap<AuthorForCreationWithDateOfDeathDto, Author>();
            //CreateMap<Author, AuthorForUpdateDTO>();
            CreateMap<Author, AuthorFullDto>();
        }
    }
}
