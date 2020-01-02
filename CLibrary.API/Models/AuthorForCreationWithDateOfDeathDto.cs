using System;

namespace CLibrary.API.Models{
    public class AuthorForCreationWithDateOfDeathDto : AuthorForCreationDto{
        public DateTimeOffset? DateOfDeath{ get; set; }
    }
}