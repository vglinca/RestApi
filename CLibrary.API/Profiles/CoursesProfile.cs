using AutoMapper;
using CLibrary.API.Models;
using CourseLibrary.API.Entities;

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
