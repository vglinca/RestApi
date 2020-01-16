using System;
using System.ComponentModel.DataAnnotations;

namespace CLibrary.API.Models{
    public class AuthorForCreationWithDateOfDeathDto {
        
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Birth date is required.")]
        //[RegularExpression("^[0-9]{4}-[0-9]{2}-[0-9]{2}$",
        //ErrorMessage = "Date of birth should have \"yyyy-MM-dd\" format.")]
        public DateTimeOffset DateOfBirth { get; set; }
        
        [Required(ErrorMessage = "Category is required.")]
        public string MainCategory { get; set; }
        public DateTimeOffset? DateOfDeath{ get; set; }
    }
}