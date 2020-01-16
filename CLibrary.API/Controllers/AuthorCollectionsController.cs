using AutoMapper;
using CLibrary.API.Helpers;
using CLibrary.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLibrary.API.Entities;
using CLibrary.API.Logging;
using CLibrary.API.Services;

namespace CLibrary.API.Controllers{

    [ApiController]
    [Route("api/authorcollections")]
    [ServiceFilter(typeof(ILoggingActionFilter))]
    public class AuthorCollectionsController : ControllerBase{

        private readonly ICourseLibraryRepository mRepository;
        private readonly IMapper mMapper;
        public AuthorCollectionsController(ICourseLibraryRepository repository, IMapper mapper){
            mRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            mMapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("({ids})", Name = "GetAuthorCollection")]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthorCollection(
        [FromRoute]
        [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids){
            if(ids == null){
                return BadRequest();
            }
            var authorEntities = await mRepository.GetAuthorsAsync(ids);

            if(ids.Count() != authorEntities.Count()){
                return NotFound();
            }
            var authorDtos = mMapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            return Ok(authorDtos);
        }

        [HttpPost(Name = "CreateAuthorsCollection")]
        public async Task<CreatedAtRouteResult> CreateAuthorsCollection(
            IEnumerable<AuthorForCreationDto> authors){

            var authorEntities = mMapper.Map<IEnumerable<Author>>(authors);

            foreach(var author in authorEntities){
                mRepository.AddAuthor(author);
            }
            await mRepository.SaveChangesAsync();
            var authorDtosToReturn = mMapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            var idsAsString = string.Join(",", authorDtosToReturn.Select(a => a.Id));

            return (CreatedAtRoute("GetAuthorCollection", new{ids = idsAsString},
                authorDtosToReturn));
        }
    }
}
