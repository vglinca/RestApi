using System;
using System.Collections.Generic;
using System.Linq;
using CLibrary.API.DbContexts;
using CLibrary.API.Helpers;
using CLibrary.API.Models;
using CLibrary.API.ResourceParameters;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Services;

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
  
        public Course GetCourse(Guid authorId, Guid courseId){
            if (authorId == Guid.Empty){
                throw new ArgumentNullException(nameof(authorId));
            }

            if (courseId == Guid.Empty){
                throw new ArgumentNullException(nameof(courseId));
            }

            return mContext.Courses.FirstOrDefault(c => c.AuthorId == authorId && c.Id == courseId);
        }

        public IEnumerable<Course> GetCourses(Guid authorId){
            if (authorId == Guid.Empty){
                throw new ArgumentNullException(nameof(authorId));
            }

            return mContext.Courses
                        .Where(c => c.AuthorId == authorId)
                        .OrderBy(c => c.Title).ToList();
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

        public bool AuthorExists(Guid authorId){
            if (authorId == Guid.Empty){
                throw new ArgumentNullException(nameof(authorId));
            }

            return mContext.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author){
            if (author == null){
                throw new ArgumentNullException(nameof(author));
            }

            mContext.Authors.Remove(author);
        }
        
        public Author GetAuthor(Guid authorId){
            if (authorId == Guid.Empty){
                throw new ArgumentNullException(nameof(authorId));
            }

            return mContext.Authors.FirstOrDefault(a => a.Id == authorId);
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
            return mContext.Authors.ToList<Author>();
        }
         
        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds){
            if (authorIds == null){
                throw new ArgumentNullException(nameof(authorIds));
            }

            return mContext.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .ToList();
        }

        public void UpdateAuthor(Author author){
        }

        public bool Save(){
            return (mContext.SaveChanges() >= 0);
        }

        public void Dispose(){
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing){
            if (disposing){
                //throw new NotImplementedException("Operation is currently unavailable");
                // dispose resources when needed
            }
        }
    }
}
