using System.Collections.Generic;
using CLibrary.API.Logging;
using CLibrary.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CLibrary.API.Controllers{
    
    [ApiController]
    [Route("api")]
    [ServiceFilter(typeof(ILoggingActionFilter))]
    public class RootController : ControllerBase{
        
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot(){
            var links = new List<LinkDto>();
            links.Add(new LinkDto(Url.Link("GetRoot", new{}),
                "self", "GET"));
            links.Add(new LinkDto(Url.Link("GetAuthors", new{}),
                "authors", "GET"));
            links.Add(new LinkDto(Url.Link("CreateAuthor", new{}),
                "create_author", "POST"));
            return Ok(links);
        }
    }
}