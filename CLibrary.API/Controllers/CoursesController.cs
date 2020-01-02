using AutoMapper;
using CLibrary.API.Models;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using CLibrary.API.Logging;
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
            this.mRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mMapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(Name = "GetCoursesForAnAuthor")]
        public ActionResult<IEnumerable<CourseDto>> GetCoursesForAnAuthor(Guid authorId) {
            if (!mRepository.AuthorExists(authorId)) {
                return NotFound();
            }
            var courses = mRepository.GetCourses(authorId);
            return Ok(mMapper.Map<IEnumerable<CourseDto>>(courses));
        }

        [HttpGet("{courseId}", Name = "GetCourseOfAnAuthor")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        // [ResponseCache(Duration = 120)]
        public ActionResult<CourseDto> GetCourseOfAnAuthor(Guid authorId, Guid courseId) {
            if (!mRepository.AuthorExists(authorId)) {
                return NotFound();
            }
            var course = mRepository.GetCourse(authorId, courseId);
            if (course == null) {
                return NotFound();
            }
            return Ok(mMapper.Map<CourseDto>(course));
        }

        [HttpOptions]
        public IActionResult GetCourseOptions() {
            Response.Headers.Add("Allowed", "{GET},{OPTIONS},{POST},{PUT},{PATCH}");
            return Ok();
        }

        [HttpPost(Name = "CreateCourseForAnAuthor")]
        public ActionResult<CourseDto> CreateCourseForAnAuthor(Guid authorId, CourseForCreateDTO dto) {
            if (!mRepository.AuthorExists(authorId)) {
                return NotFound();
            }
            var createdEntity = mMapper.Map<Course>(dto);
            mRepository.AddCourse(authorId, createdEntity);
            mRepository.Save();
            var createdDto = mMapper.Map<CourseDto>(createdEntity);
            return CreatedAtRoute("GetCourseOfAnAuthor", new { authorId, courseId = createdDto.Id }, 
                createdDto);
        }

        [HttpPut("{courseId}")] 
        public IActionResult UpdateCourse(Guid authorId, Guid courseId, CourseForUpdateDto dto) {
            if (!mRepository.AuthorExists(authorId)) {
                return NotFound();
            }
            var targetEntity = mRepository.GetCourse(authorId, courseId);
            if(targetEntity == null) { //if updating course is null, we create it. So here PUT = POST
                var newCourse = mMapper.Map<Course>(dto);
                newCourse.Id = courseId;
                mRepository.AddCourse(authorId, newCourse);
                mRepository.Save();
                var createdDto = mMapper.Map<CourseDto>(newCourse);
                return CreatedAtRoute("GetCourseOfAnAuthor", new { authorId, courseId = createdDto.Id },
                    createdDto);
            }
            mMapper.Map(dto, targetEntity);
            mRepository.Save();
            return NoContent();
        }

        [HttpPatch("{courseId}")]
        public IActionResult PatchCourse(Guid authorId, Guid courseId, 
            JsonPatchDocument<CourseForUpdateDto> dto) {
            if (!mRepository.AuthorExists(authorId)) {
                return NotFound();
            }
            var targetEntity = mRepository.GetCourse(authorId, courseId);
            if(targetEntity == null) {
                var newCourseDto = new CourseForUpdateDto();
                dto.ApplyTo(newCourseDto, ModelState);

                if (!TryValidateModel(newCourseDto)) {
                    return ValidationProblem(ModelState);
                }
                var courseToAdd = mMapper.Map<Course>(newCourseDto);
                courseToAdd.Id = courseId;
                mRepository.AddCourse(authorId, courseToAdd);
                mRepository.Save();
                var createdDto = mMapper.Map<CourseDto>(courseToAdd);
                return CreatedAtRoute("GetCourseOfAnAuthor",
                    new { authorId, courseId = createdDto.Id }, createdDto);
            }
            var courseToPatch = mMapper.Map<CourseForUpdateDto>(targetEntity);
            dto.ApplyTo(courseToPatch, ModelState);

            if (!TryValidateModel(courseToPatch)) {
                return ValidationProblem(ModelState);
            }
            mMapper.Map(courseToPatch, targetEntity);
            mRepository.Save();
            return NoContent();
        }

        [HttpDelete("{courseId}")]
        public ActionResult DeleteCourse(Guid authorId, Guid courseId) {
            if (!mRepository.AuthorExists(authorId)) {
                return NotFound();
            }
            var courseToDel = mRepository.GetCourse(authorId, courseId);
            if(courseToDel == null) {
                return NotFound();
            }
            mRepository.DeleteCourse(courseToDel);
            mRepository.Save();
            return NoContent();
        }

        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary) {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult) options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}
