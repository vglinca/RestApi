﻿using CLibrary.API.ResourceParameters;
using CourseLibrary.API.Entities;
using System;
using System.Collections.Generic;
using CLibrary.API.Helpers;

namespace CourseLibrary.API.Services
{
    public interface ICourseLibraryRepository
    {    
        IEnumerable<Course> GetCourses(Guid authorId);
        Course GetCourse(Guid authorId, Guid courseId);
        void AddCourse(Guid authorId, Course course);
        void UpdateCourse(Course course);
        void DeleteCourse(Course course);
        IEnumerable<Author> GetAuthors();
        Author GetAuthor(Guid authorId);
        PagedList<Author> GetAuthors(AuthorsResourceParams authorsResourceParameters);
        IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds);
        void AddAuthor(Author author);
        void DeleteAuthor(Author author);
        void UpdateAuthor(Author author);
        bool AuthorExists(Guid authorId);
        bool Save();
    }
}
