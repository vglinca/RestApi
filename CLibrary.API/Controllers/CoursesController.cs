using AutoMapper;
using CLibrary.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLibrary.API.Entities;
using CLibrary.API.Logging;
using CLibrary.API.Services;
using Marvin.Cache.Headers;

namespace CLibrary.API.Controllers {

    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    [ServiceFilter(typeof(ILoggingActionFilter))]
    // [ResponseCache(CacheProfileName = "240secondsCacheProfile")]
    public class CoursesController : ControllerBase {
        private readonly ICourseLibraryRepository mRepository;
        private readonly IMapper mMapper;
        public CoursesController(ICourseLibraryRepository repository, IMapper mapper) {
            mRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            mMapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(Name = "GetCoursesForAnAuthor")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesForAnAuthor(Guid authorId) {
            if (! await mRepository.AuthorExistsAsync(authorId)) {
                return NotFound();
            }
            var courses = await mRepository.GetCoursesAsync(authorId);
            return Ok(mMapper.Map<IEnumerable<CourseDto>>(courses));
        }

        [HttpGet("{courseId}", Name = "GetCourseOfAnAuthor")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        // [ResponseCache(Duration = 120)]
        public async Task<ActionResult<CourseDto>> GetCourseOfAnAuthor(Guid authorId, Guid courseId) {
            if (! await mRepository.AuthorExistsAsync(authorId)) {
                return NotFound();
            }
            var course = await mRepository.GetCourseAsync(authorId, courseId);
            if (course == null) {
                return NotFound();
            }
            return Ok(mMapper.Map<CourseDto>(course));
        }

        [HttpOptions(Name = "GetCourseOptions")]
        public IActionResult GetCourseOptions() {
            Response.Headers.Add("Allowed", "{GET},{OPTIONS},{POST},{PUT},{PATCH}");
            return Ok();
        }

        [HttpPost(Name = "CreateCourseForAnAuthor")]
        public async Task<ActionResult<CourseDto>> CreateCourseForAnAuthor(Guid authorId, CourseForCreateDTO dto) {
            if (! await mRepository.AuthorExistsAsync(authorId)) {
                return NotFound();
            }
            var createdEntity = mMapper.Map<Course>(dto);
            mRepository.AddCourse(authorId, createdEntity);
            await mRepository.SaveChangesAsync();
            var createdDto = mMapper.Map<CourseDto>(createdEntity);
            return CreatedAtRoute("GetCourseOfAnAuthor", new { authorId, courseId = createdDto.Id }, 
                createdDto);
        }

        [HttpPut("{courseId}", Name = "UpdateCourse")] 
        public async Task<IActionResult> UpdateCourse(Guid authorId, Guid courseId, CourseForUpdateDto dto) {
            if (! await mRepository.AuthorExistsAsync(authorId)) {
                return NotFound();
            }
            var targetEntity = mRepository.GetCourseAsync(authorId, courseId);
            if(targetEntity == null) { //if updating course is null, we create it. So here PUT = POST
                var newCourse = mMapper.Map<Course>(dto);
                newCourse.Id = courseId;
                mRepository.AddCourse(authorId, newCourse);
                await mRepository.SaveChangesAsync();
                var createdDto = mMapper.Map<CourseDto>(newCourse);
                return CreatedAtRoute("GetCourseOfAnAuthor", new { authorId, courseId = createdDto.Id },
                    createdDto);
            }
            await mMapper.Map(dto, targetEntity);
            await mRepository.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Partially update course of an author
        /// </summary>
        /// <param name="authorId">The id of the author you want to get</param>
        /// <param name="courseId">The id of the course you want to partially update</param>
        /// <param name="patchDocument">The set of operation to apply to the course</param>
        /// <returns>An IActionResult</returns>
        /// <remarks>
        /// Sample request (This request updates **course title**)      
        ///         '''PATCH /authors/v{version}/authorId/courses/courseId    
        ///         [   
        ///             {   
        ///                 "op": "replace",   
        ///                 "path": "/title",   
        ///                 "value": "Updated title"   
        ///             }   
        ///         ] 
        /// </remarks>
        [HttpPatch("{courseId}", Name = "PatchCourse")]
        public async Task<IActionResult> PatchCourse(Guid authorId, Guid courseId, 
            JsonPatchDocument<CourseForUpdateDto> patchDocument) {
            if (! await mRepository.AuthorExistsAsync(authorId)) {
                return NotFound();
            }
            var targetEntity = await mRepository.GetCourseAsync(authorId, courseId);
            if(targetEntity == null) {
                var newCourseDto = new CourseForUpdateDto();
                patchDocument.ApplyTo(newCourseDto, ModelState);

                if (!TryValidateModel(newCourseDto)) {
                    return ValidationProblem(ModelState);
                }
                var courseToAdd = mMapper.Map<Course>(newCourseDto);
                courseToAdd.Id = courseId;
                mRepository.AddCourse(authorId, courseToAdd);
                await mRepository.SaveChangesAsync();
                var createdDto = mMapper.Map<CourseDto>(courseToAdd);
                return CreatedAtRoute("GetCourseOfAnAuthor",
                    new { authorId, courseId = createdDto.Id }, createdDto);
            }
            var courseToPatch = mMapper.Map<CourseForUpdateDto>(targetEntity);
            patchDocument.ApplyTo(courseToPatch, ModelState);

            if (!TryValidateModel(courseToPatch)) {
                return ValidationProblem(ModelState);
            }
            mMapper.Map(courseToPatch, targetEntity);
            await mRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{courseId}", Name = "DeleteCourse")]
        public async Task<ActionResult> DeleteCourse(Guid authorId, Guid courseId) {
            if (! await mRepository.AuthorExistsAsync(authorId)) {
                return NotFound();
            }
            var courseToDel = await mRepository.GetCourseAsync(authorId, courseId);
            if(courseToDel == null) {
                return NotFound();
            }
            mRepository.DeleteCourse(courseToDel);
            await mRepository.SaveChangesAsync();
            return NoContent();
        }

        [NonAction]
        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary) {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult) options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}
