using CLibrary.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using CLibrary.API.ActionConstraints;
using CLibrary.API.Entities;
using CLibrary.API.Helpers;
using CLibrary.API.Logging;
using CLibrary.API.ResourceParameters;
using CLibrary.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace CLibrary.API.Controllers{

    [ApiController]
    [Route("api/authors")]
    [ServiceFilter(typeof(ILoggingActionFilter))]
    [Produces("application/json", 
        "application/vnd.marvin.hateoas+json",
        "application/vnd.marvin.author.full+json",
        "application/vnd.marvin.author.full.hateoas+json",
        "application/vnd.marvin.author.friendly+json",
        "application/vnd.marvin.author.friendly.hateoas+json")]
    public class AuthorsController : ControllerBase {

        private readonly ICourseLibraryRepository mRepository;
        private readonly IMapper mMapper;
        private readonly IPropertyMappingService mMappingService;
        private readonly IPropertyCheckerService mCheckerService;
        public AuthorsController(ICourseLibraryRepository repository, IMapper mapper, 
            IPropertyMappingService mappingService, IPropertyCheckerService checkerService){
            
            mRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            mMapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            mMappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));
            mCheckerService = checkerService ?? throw new ArgumentNullException(nameof(checkerService));
        }
        
        /// <summary>
        /// Get all list of authors
        /// </summary>
        /// <param name="resourceParams">Additional parameters you may want to apply to authors list</param>
        /// <param name="mediaType">Accepted media type</param>
        /// <returns>List of authors</returns>
        [HttpGet(Name = "GetAuthors")]
        [HttpHead]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto))]
        public IActionResult GetAuthors(
            [FromQuery]AuthorsResourceParams resourceParams,
            [FromHeader(Name = "Accept")] string mediaType){

            if (!MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType)){
                return BadRequest();
            }
            
            if (!mMappingService.IsMappingValid<AuthorDto, Author>(resourceParams.OrderBy)){
                return BadRequest();
            }
            
            var authors = mRepository.GetAuthors(resourceParams);
            // var prevPageLink = authors.HasPrevious
            //     ? CreateAuthorsResourceUri(resourceParams,
            //         ResourceUriType.PREVIOUS_PAGE)
            //     : null;
            // var nextPageLink = authors.HasNext
            //     ? CreateAuthorsResourceUri(resourceParams,
            //         ResourceUriType.NEXT_PAGE)
            //     : null;
            
            // var includeLinksPerAuthor = parsedMediaType.SubTypeWithoutSuffix
            //     .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
            // IEnumerable<IDictionary<string, object>> authorResourcesToReturn = new List<IDictionary<string, object>>();
            // if (includeLinksPerAuthor){
            //     var primaryMediaType = parsedMediaType.SubTypeWithoutSuffix
            //         .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8);
            //     
            //     foreach (var author in authors){
            //         IEnumerable<LinkDto> linksPerAuthor = new List<LinkDto>();
            //         linksPerAuthor = CreateLinksForAuthor(author.Id, resourceParams.Fields);
            //
            //         switch (primaryMediaType){
            //             case "vnd.marvin.author.full":{
            //                 var fullResourceToReturn = mMapper.Map<AuthorFullDto>(author)
            //                     .ShapeData(resourceParams.Fields) as IDictionary<string, object>;
            //                 fullResourceToReturn.Add("links", linksPerAuthor);
            //                 authorResourcesToReturn.Append(fullResourceToReturn);
            //                 break;
            //             }
            //             case "vnd.marvin.author.friendly":{
            //                 var fullResourceToReturn = mMapper.Map<AuthorDto>(author)
            //                     .ShapeData(resourceParams.Fields) as IDictionary<string, object>;
            //                 fullResourceToReturn.Add("links", linksPerAuthor);
            //                 authorResourcesToReturn.Append(fullResourceToReturn);
            //                 break;
            //             }
            //         }
            //     }
            // }
            
            var pagesMetaData = new {
                totalCount  = authors.TotalCount,
                pageSize    = authors.PageSize,
                currentPage = authors.CurrentPage,
                totalPages  = authors.TotalPages,
                // prevPageLink,
                // nextPageLink
            };
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagesMetaData));

            if (parsedMediaType.MediaType != "application/vnd.marvin.hateoas+json")
                return Ok(mMapper.Map<IEnumerable<AuthorDto>>(authors));
            
            var links = CreateLinksForAuthors(resourceParams, 
                authors.HasNext, authors.HasPrevious);
            var shapedAuthorsData = mMapper.Map<IEnumerable<AuthorDto>>(authors)
                .ShapeData(resourceParams.Fields);
            var shapedAuthorsDataWithLinks = shapedAuthorsData
                .Select(author => {
                    var authorAsDictionary = author as IDictionary<string, object>;
                    var authorLinks = CreateLinksForAuthor((Guid) authorAsDictionary["Id"], null);
                    authorAsDictionary.Add("links", authorLinks);
                    return authorAsDictionary;
                });
            
            return Ok(new { authors = shapedAuthorsDataWithLinks, links });
        }

        /// <summary>
        /// Get an author by his/her id
        /// </summary>
        /// <param name="authorId">The id of the author you want to get</param>
        /// <param name="fields">Fields you want to see in response body</param>
        /// <param name="mediaType">Media type which is being accepted</param>
        /// <returns>An author with id, firstname, lastname fields</returns>
        /// <response code="200">Returns the requested author</response>
        [HttpGet("{authorId}", Name = "GetAuthor")]
        [HttpHead("{authorId}", Name = "Head")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto))]
        public async Task<IActionResult> GetAuthor(Guid authorId, string fields,
            [FromHeader(Name = "Accept")] string mediaType){

            if (!MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType)){
                return BadRequest();
            }
            if (!mCheckerService.CheckIfValid<AuthorDto>(fields)){
                return BadRequest();
            }
            var author = await mRepository.GetAuthorAsync(authorId);
            if (author == null){
                return NotFound();
            }
            //for mediatype, that contains ...hateoas+json
            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
            IEnumerable<LinkDto> links = new List<LinkDto>();
            if (includeLinks){
                links = CreateLinksForAuthor(authorId, fields);
            }

            var primaryMediaType = includeLinks
                ? parsedMediaType.SubTypeWithoutSuffix
                    .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
                : parsedMediaType.SubTypeWithoutSuffix;
            
            //full author dto
            if (primaryMediaType == "vnd.marvin.author.full"){
                var fullResourceToReturn = mMapper.Map<AuthorFullDto>(author)
                    .ShapeData(fields) as IDictionary<string, object>;
                if (includeLinks){
                    fullResourceToReturn.Add("links", links);
                }
                return Ok(fullResourceToReturn);
            }
            
            //friendly author dto
            var friendlyResourceToReturn = mMapper.Map<AuthorDto>(author)
                .ShapeData(fields) as IDictionary<string, object>;
            if (includeLinks){
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);
        }

        /// <summary>
        /// Create a new author
        /// </summary>
        /// <param name="authorForCreation">Author model</param>
        /// <returns>Created author body</returns>
        [HttpPost(Name = "CreateAuthor")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.authorforcreation+json")]
        [Consumes("application/json",
            "application/vnd.marvin.authorforcreation+json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult CreateAuthor(AuthorForCreationDto authorForCreation){
            var createdEntity = mMapper.Map<Author>(authorForCreation);
            mRepository.AddAuthor(createdEntity);
            mRepository.SaveChangesAsync();
            var createdDtoToReturn = mMapper.Map<AuthorDto>(createdEntity);
            var linkedResource = createdDtoToReturn.ShapeData(null) as IDictionary<string, object>;
            linkedResource.Add("links", CreateLinksForAuthor(createdEntity.Id, null));
            
            return CreatedAtRoute("GetAuthor", 
                new { authorId = linkedResource["Id"] }, linkedResource);
        }
        
        // [HttpPost(Name = "CreateAuthorWithDateOfDeath")]
        // [RequestHeaderMatchesMediaType("Content-Type",
        //     "application/vnd.marvin.authorforcreationwithdateofdeath+json")]
        // [Consumes("application/vnd.marvin.authorforcreationwithdateofdeath+json")]
        // [ApiExplorerSettings(IgnoreApi = true)]
        // public IActionResult CreateAuthorWithDateOfDeath(AuthorForCreationDto authorForCreation){
        //     var createdEntity = mMapper.Map<Author>(authorForCreation);
        //     mRepository.AddAuthor(createdEntity);
        //     mRepository.Save();
        //     var createdDtoToReturn = mMapper.Map<AuthorDto>(createdEntity);
        //     var linkedResource = createdDtoToReturn.ShapeData(null) as IDictionary<string, object>;
        //     linkedResource.Add("links", CreateLinksForAuthor(createdEntity.Id, null));
        //     
        //     return CreatedAtRoute("GetAuthor", 
        //         new { authorId = linkedResource["Id"] }, linkedResource);
        // }

        [HttpOptions(Name = "GetAuthorsOptions")]
        public IActionResult GetAuthorsOptions() {
            Response.Headers.Add("Allowed", "{GET},{POST},{OPTIONS},{HEAD},{DELETE},{PUT},{PATCH}");
            return Ok();
        }

        /// <summary>
        /// Update author with given authorId
        /// </summary>
        /// <param name="authorId">Id of an author you want to update</param>
        /// <param name="dto">Author model</param>
        [HttpPut("{authorId}", Name = "UpdateAuthor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateAuthor(Guid authorId, AuthorForUpdateDTO dto) {
            var entityToUpd = mRepository.GetAuthorAsync(authorId);
            if (entityToUpd == null) {
                return NotFound();
            }
            mMapper.Map(dto, entityToUpd);
            mRepository.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Partially update author with given authorId
        /// </summary>
        /// <param name="authorId">Id of an author you want to update</param>
        /// <param name="patchDto"></param>
        [HttpPatch("{authorId}", Name = "PatchAuthor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult PatchAuthor(Guid authorId, JsonPatchDocument<AuthorForUpdateDTO> patchDto) {
            var targetEntity = mRepository.GetAuthorAsync(authorId);
            if (targetEntity == null) {
                return NotFound();
            }
            var authorToPatch = mMapper.Map<AuthorForUpdateDTO>(targetEntity);
            patchDto.ApplyTo(authorToPatch, ModelState);
            if(!TryValidateModel(authorToPatch)) {
                return ValidationProblem(ModelState);
            }
            mMapper.Map(authorToPatch, targetEntity);
            mRepository.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Delete author with given authorId
        /// </summary>
        /// <param name="authorId">Id of the author</param>
        [HttpDelete("{authorId}", Name = "DeleteAuthor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete(Guid authorId) {
            var author = await mRepository.GetAuthorAsync(authorId);
            if (author == null) {
                return NotFound();
            }
            mRepository.DeleteAuthor(author);
            await mRepository.SaveChangesAsync();
            return NoContent();
        }

        [NonAction]
        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary) {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult) options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

        private string CreateAuthorsResourceUri(AuthorsResourceParams authorsResourceParams,
            ResourceUriType type){
            return type switch{
                ResourceUriType.PREVIOUS_PAGE => Url.Link("GetAuthors",
                    new{
                        fields = authorsResourceParams.Fields,
                        pageNumber = authorsResourceParams.PageNumber - 1,
                        pageSize = authorsResourceParams.PageSize,
                        mainCategory = authorsResourceParams.MainCategory,
                        searchQuery = authorsResourceParams.SearchQuery,
                        orderBy = authorsResourceParams.OrderBy
                    }),
                ResourceUriType.NEXT_PAGE => Url.Link("GetAuthors",
                    new{
                        fields = authorsResourceParams.Fields,
                        pageNumber = authorsResourceParams.PageNumber + 1,
                        pageSize = authorsResourceParams.PageSize,
                        mainCategory = authorsResourceParams.MainCategory,
                        searchQuery = authorsResourceParams.SearchQuery,
                        orderBy = authorsResourceParams.OrderBy
                    }),
                // ResourceUriType.CURRENT => 
                _ => Url.Link("GetAuthors",
                    new{
                        fields = authorsResourceParams.Fields,
                        pageNumber = authorsResourceParams.PageNumber,
                        pageSize = authorsResourceParams.PageSize,
                        mainCategory = authorsResourceParams.MainCategory,
                        searchQuery = authorsResourceParams.SearchQuery,
                        orderBy = authorsResourceParams.OrderBy
                    })
            };
        }

        private IEnumerable<LinkDto> CreateLinksForAuthor(Guid authorId, string fields){
            var links = new List<LinkDto>();
            if (string.IsNullOrWhiteSpace(fields)){
                links.Add(new LinkDto(Url.Link("GetAuthor", new{authorId}),
                    "self", "GET"));
            }else{
                links.Add(new LinkDto(Url.Link("GetAuthor", new{authorId, fields}),
                    "self", "GET"));
            }
            links.Add(new LinkDto(Url.Link("DeleteAuthor", new{authorId}),
                "delete_author", "DELETE"));
            links.Add(new LinkDto(Url.Link("CreateCourseForAnAuthor", new {authorId}), 
                "create_course_for_author", "POST"));
            links.Add(new LinkDto(Url.Link("GetCoursesForAnAuthor", new {authorId}), 
                "courses", "GET"));
            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForAuthors(AuthorsResourceParams resourceParams,
            bool hasNext, bool hasPrev){
            var links = new List<LinkDto>();
            links.Add(new LinkDto(CreateAuthorsResourceUri(resourceParams, ResourceUriType.CURRENT),
                "self", "GET"));
            if (hasNext){
                links.Add(new LinkDto(CreateAuthorsResourceUri(resourceParams, ResourceUriType.NEXT_PAGE),
                    "next_page", "GET"));
            }
            if (hasPrev){
                links.Add(new LinkDto(CreateAuthorsResourceUri(resourceParams, ResourceUriType.PREVIOUS_PAGE),
                    "previous_page", "GET"));
            }
            return links;
        }
    }
}
