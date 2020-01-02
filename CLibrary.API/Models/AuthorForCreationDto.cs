using System.Collections.Generic;

namespace CLibrary.API.Models
{
    public class AuthorForCreationDto : AuthorForManipulationDto {
        public ICollection<CourseForCreateDTO> Courses { get; set; } = new List<CourseForCreateDTO>();
    }
}
