using CLibrary.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CLibrary.API.ValidationAttributes {
	public class CourseTitleDescriptionValidation : ValidationAttribute {

		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
			var course = (CourseForManipulationDto) validationContext.ObjectInstance;

			return course.Title == course.Description ? 
				new ValidationResult(ErrorMessage, new[] { nameof(CourseForManipulationDto) }) :
				ValidationResult.Success;
		}
	}
}
