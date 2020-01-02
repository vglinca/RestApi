using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CLibrary.API.Logging{
    public abstract class AbstractLogger : ActionFilterAttribute{
        protected static readonly string nl = Environment.NewLine;

        protected static string GenerateTitle(string title){
            return $"{nl}------------------------------{title}---------------------------------{nl}";
        }
    }
}