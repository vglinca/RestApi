
namespace CLibrary.API.Models{

    public class CourseForCreateDTO : CourseForManipulationDto/*: IValidatableObject*/ {

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
        //    if(Title == Description) {
        //        yield return new ValidationResult(
        //            "The provided description should be different from the title.",
        //            new[] { "CreatedCourseDTO" });
        //    }
        //}
    }
}
