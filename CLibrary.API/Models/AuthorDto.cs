using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLibrary.API.Models
{
    /// <summary>
    /// An author with Id, Name, Age and MainCategory fields
    /// </summary>
    public class AuthorDto
    {
        /// <summary>
        /// The Id of an author
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// The Name of an author
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The Age of an author
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// The MainCategory of an author
        /// </summary>
        public string MainCategory { get; set; }
    }
}
