using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CLibrary.API.Models {
	public abstract class AuthorForManipulationDto {
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
    }
}
