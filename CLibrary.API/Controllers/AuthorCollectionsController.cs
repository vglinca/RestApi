using AutoMapper;
using CLibrary.API.Helpers;
using CLibrary.API.Models;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using CLibrary.API.Logging;

namespace CLibrary.API.Controllers{

    [ApiController]
    [Route("api/authorcollections")]
    [ServiceFilter(typeof(ILoggingActionFilter))]
    public class AuthorCollectionsController : ControllerBase{

        private readonly ICourseLibraryRepository mRepository;
        private readonly IMapper mMapper;
        public AuthorCollectionsController(ICourseLibraryRepository repository, IMapper mapper){
            this.mRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mMapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("({ids})", Name = "GetAuthorCollection")]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthorCollection(
        [FromRoute]
        [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids){
            if(ids == null){
                return BadRequest();
            }
            var authorEntities = mRepository.GetAuthors(ids);

            if(ids.Count() != authorEntities.Count()){
                return NotFound();
            }
            var authorDtos = mMapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            return Ok(authorDtos);
        }

        [HttpPost]
        public ActionResult<IEnumerable<AuthorDto>> CreateAuthotCollection(
            IEnumerable<AuthorForCreationDto> authors){

            var authorEntities = mMapper.Map<IEnumerable<Author>>(authors);

            foreach(var author in authorEntities){
                mRepository.AddAuthor(author);
            }
            mRepository.Save();
            var authorDtosToReturn = mMapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            var idsAsString = string.Join(",", authorDtosToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetAuthorCollection", new { ids = idsAsString },
                authorDtosToReturn);
        }
    }
}
