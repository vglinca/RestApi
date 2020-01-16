using AutoMapper;
using CLibrary.API.Entities;
using CLibrary.API.Models;

namespace CLibrary.API.Profiles {
    public class CoursesProfile : Profile {
        public CoursesProfile() {
            CreateMap<Course, CourseDto>();
            CreateMap<CourseForCreateDTO, Course>();
            CreateMap<CourseForUpdateDto, Course>();
            CreateMap<Course, CourseForUpdateDto>();
        }
    }
}
