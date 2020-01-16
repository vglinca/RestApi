using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CLibrary.API.Models
{
    public class AuthorForCreationDto {
        
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Birth date is required.")]
        public DateTimeOffset DateOfBirth { get; set; }
        
        [Required(ErrorMessage = "Category is required.")]
        public string MainCategory { get; set; }
        public ICollection<CourseForCreateDTO> Courses { get; set; } = new List<CourseForCreateDTO>();
    }
}
