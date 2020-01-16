using CLibrary.API.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace CLibrary.API.Models {

    [CourseTitleDescriptionValidation(ErrorMessage = "Title must be different from description.")]
    public abstract class CourseForManipulationDto {
        /// <summary>
        /// The title of the course
        /// </summary>
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(100, ErrorMessage = "Title shouldn't have more that 100 characters.")]
        public string Title { get; set; }

        [MaxLength(1500, ErrorMessage = "Description shouldn't have more that 1500 characters.")]
        public virtual string Description { get; set; }
    }
}
