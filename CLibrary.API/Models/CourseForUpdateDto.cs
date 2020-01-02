using System.ComponentModel.DataAnnotations;

namespace CLibrary.API.Models {

	public class CourseForUpdateDto : CourseForManipulationDto {

		[Required(ErrorMessage = "Description is required.")]
		public override string Description { get => base.Description; set => base.Description = value; }
	}
}
