using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLibrary.API.ResourceParameters
{
    public class AuthorsResourceParams
    {
        private const int MaxPageSize = 20;
        public string MainCategory { get; set; }
        public string SearchQuery { get; set; }
        public int PageNumber{ get; set; } = 1;
        private int mPageSize = 10;
        public int PageSize{
            get => mPageSize;
            set => mPageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public string OrderBy{ get; set; } = "Name";
        public string Fields{ get; set; }
    }
}
