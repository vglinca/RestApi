using System.ComponentModel.DataAnnotations;

namespace CLibrary.API.Models {

	/// <summary>
	/// A course for update with Title, Description fields
	/// </summary>
	public class CourseForUpdateDto : CourseForManipulationDto {

		/// <summary>
		/// The Description of the course
		/// </summary>
		[Required(ErrorMessage = "Description is required.")]
		public override string Description { get => base.Description; set => base.Description = value; }
	}
}
