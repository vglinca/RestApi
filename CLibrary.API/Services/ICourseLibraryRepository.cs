using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLibrary.API.Entities;
using CLibrary.API.Helpers;
using CLibrary.API.ResourceParameters;

namespace CLibrary.API.Services
{
    public interface ICourseLibraryRepository
    {    
        Task<IEnumerable<Course>> GetCoursesAsync(Guid authorId);
        Task<Course> GetCourseAsync(Guid authorId, Guid courseId);
        void AddCourse(Guid authorId, Course course);
        void UpdateCourse(Course course);
        void DeleteCourse(Course course);
        IEnumerable<Author> GetAuthors();
        Task<Author> GetAuthorAsync(Guid authorId);
        PagedList<Author> GetAuthors(AuthorsResourceParams authorsResourceParameters);
        Task<IEnumerable<Author>> GetAuthorsAsync(IEnumerable<Guid> authorIds);
        void AddAuthor(Author author);
        void DeleteAuthor(Author author);
        void UpdateAuthor(Author author);
        Task<bool> AuthorExistsAsync(Guid authorId);
        Task<bool> SaveChangesAsync();
    }
}
