using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CLibrary.API.Models {
	/// <summary>
	/// An author for update with FirstName, LastName, DateOfBirth and MainCategory fields
	/// </summary>
	public class AuthorForUpdateDTO {
		
		/// <summary>
		/// The First Name of an author
		/// </summary>
		[Required(ErrorMessage = "First Name is required.")]
		public string FirstName { get; set; } 
        
		/// <summary>
		/// The Last Name of an author
		/// </summary>
		[Required(ErrorMessage = "Last Name is required.")]
		public string LastName { get; set; }
        
		/// <summary>
		/// The Date of Birth of an author
		/// </summary>
		[Required(ErrorMessage = "Birth date is required.")]
		public DateTimeOffset DateOfBirth { get; set; }
        
		/// <summary>
		/// The Main Category of an author
		/// </summary>
		[Required(ErrorMessage = "Category is required.")]
		public string MainCategory { get; set; }
	}
}
