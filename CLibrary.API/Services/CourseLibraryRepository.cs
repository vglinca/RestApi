using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLibrary.API.DbContexts;
using CLibrary.API.Entities;
using CLibrary.API.Helpers;
using CLibrary.API.Models;
using CLibrary.API.ResourceParameters;
using Microsoft.EntityFrameworkCore;

namespace CLibrary.API.Services
{
    public sealed class CourseLibraryRepository : ICourseLibraryRepository, IDisposable{

        private readonly CourseLibraryContext mContext;
        private readonly IPropertyMappingService mPropertyMappingService;

        public CourseLibraryRepository(CourseLibraryContext context, 
            IPropertyMappingService propertyMappingService){
            this.mContext = context ?? throw new ArgumentNullException(nameof(context));
            this.mPropertyMappingService = propertyMappingService ??
                                           throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddCourse(Guid authorId, Course course){
            if (authorId == Guid.Empty){
                throw new ArgumentNullException(nameof(authorId));
            }

            if (course == null){
                throw new ArgumentNullException(nameof(course));
            }
            // always set the AuthorId to the passed-in authorId
            course.AuthorId = authorId;
            mContext.Courses.Add(course); 
        }         

        public void DeleteCourse(Course course){
            mContext.Courses.Remove(course);
        }
  
        public async Task<Course> GetCourseAsync(Guid authorId, Guid courseId){
            if (authorId == Guid.Empty){
                throw new ArgumentNullException(nameof(authorId));
            }

            if (courseId == Guid.Empty){
                throw new ArgumentNullException(nameof(courseId));
            }

            return await mContext.Courses
                .FirstOrDefaultAsync(c => c.AuthorId == authorId && c.Id == courseId);
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync(Guid authorId){
            if (authorId == Guid.Empty){
                throw new ArgumentNullException(nameof(authorId));
            }
            return await mContext.Courses
                        .Where(c => c.AuthorId == authorId)
                        .OrderBy(c => c.Title).ToListAsync();
        }

        public void UpdateCourse(Course course){
        }

        public void AddAuthor(Author author){
            if (author == null){
                throw new ArgumentNullException(nameof(author));
            }
            // the repository fills the id (instead of using identity columns)
            author.Id = Guid.NewGuid();

            foreach (var course in author.Courses){
                course.Id = Guid.NewGuid();
            }
            mContext.Authors.Add(author);
        }

        public async Task<bool> AuthorExistsAsync(Guid authorId){
            if (authorId == Guid.Empty){
                throw new ArgumentNullException(nameof(authorId));
            }

            return await mContext.Authors.AnyAsync(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author){
            if (author == null){
                throw new ArgumentNullException(nameof(author));
            }

            mContext.Authors.Remove(author);
        }
        
        public async Task<Author> GetAuthorAsync(Guid authorId){
            if (authorId == Guid.Empty){
                throw new ArgumentNullException(nameof(authorId));
            }

            return await mContext.Authors.FirstOrDefaultAsync(a => a.Id == authorId);
        }

        public PagedList<Author> GetAuthors(AuthorsResourceParams authorsResourceParameters){
            if(authorsResourceParameters == null){
                throw new ArgumentNullException(nameof(authorsResourceParameters));
            }

            var collection = mContext.Authors as IQueryable<Author>;
            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.MainCategory)){
                var mainCategory = authorsResourceParameters.MainCategory.Trim();
                collection = collection.Where(a => a.MainCategory == mainCategory);
            }
            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.SearchQuery)){
                var searchQuery = authorsResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.MainCategory.Contains(searchQuery)
                    || a.FirstName.Contains(searchQuery)
                    || a.LastName.Contains(searchQuery));
            }
            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.OrderBy)){
                var mappingDictionary = mPropertyMappingService.GetPropertyMapping<AuthorDto, Author>();
                collection = collection.ApplySort(authorsResourceParameters.OrderBy, mappingDictionary);
            }
            
            return PagedList<Author>.Create(collection, authorsResourceParameters.PageNumber,
                authorsResourceParameters.PageSize);
        }

        public IEnumerable<Author> GetAuthors(){
            return mContext.Authors.ToList();
        }
         
        public async Task<IEnumerable<Author>> GetAuthorsAsync(IEnumerable<Guid> authorIds){
            if (authorIds == null){
                throw new ArgumentNullException(nameof(authorIds));
            }

            return await mContext.Authors
                .Where(a => authorIds
                    .Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .ToListAsync();
        }

        public void UpdateAuthor(Author author){
        }

        public async Task<bool> SaveChangesAsync(){
            return await mContext.SaveChangesAsync() >= 0;
        }

        public void Dispose(){
            Dispose(true);
            GC.SuppressFinalize(obj: this);
        }

        private void Dispose(bool disposing){
            if (disposing){
                //throw new NotImplementedException("Operation is currently unavailable");
                // dispose resources when needed
            }
        }
    }
}
