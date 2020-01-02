using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CLibrary.API.Helpers{
    public class PagedList<T> : List<T> {
        public int CurrentPage{ get; private set; }
        public int TotalPages{ get; private set; }
        public int PageSize{ get; private set; }
        public int TotalCount{ get; private set; }
        public bool HasPrevious => (CurrentPage > 1);
        public bool HasNext => (CurrentPage < TotalPages);

        public PagedList(List<T> items, int totalCount, int currentPage, int pageSize){
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int) Math.Ceiling(totalCount / (double) pageSize);
            AddRange(items);
        }

        public static PagedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize){
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}