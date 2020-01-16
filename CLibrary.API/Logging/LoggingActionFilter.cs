using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CLibrary.API.Logging{
    public class LoggingActionFilter : AbstractLogger, ILoggingActionFilter{
        private readonly ILogger mLogger;

        public LoggingActionFilter(ILoggerFactory loggerFactory){
            mLogger = loggerFactory.CreateLogger("LoggingActionFilter");
        }

        public override void OnActionExecuting(ActionExecutingContext context){
            base.OnActionExecuting(context);
            var methodName = context.ActionDescriptor.DisplayName;
            var name = context.Controller.GetType().FullName;
            var path = context.HttpContext.Request.Path;
            var requestMethod = context.HttpContext.Request.Method;
            var headersBuilder = new StringBuilder();
            
            foreach (var (key, value) in context.HttpContext.Request.Headers){
                headersBuilder.Append($"    [{key}] : [{value}]" + nl);
            }
            var queryBuilder = new StringBuilder();
            foreach (var (key, value) in context.HttpContext.Request.Query){
                queryBuilder.Append($"    {key} : {value}" + nl);
            }
            mLogger.LogInformation(GenerateTitle(name) + "Enter action: " + methodName
                                   + nl + "Http Method: " + requestMethod + nl
                                   + "Path: " + path + nl
                                   + (queryBuilder.Length == 0 ? "" :
                                       "Query Parameters: {" + nl
                                   + queryBuilder + "    }") + nl
                                   + (headersBuilder.Length == 0 ? "" :
                                       "Request Headers: {" + nl
                                   + headersBuilder + "    }") + GenerateTitle(name));
        }

        public override void OnActionExecuted(ActionExecutedContext context){
            base.OnActionExecuted(context);
            var methodName = context.ActionDescriptor.DisplayName;
            var name = context.Controller.GetType().FullName;
            var path = context.HttpContext.Request.Path;
            var requestMethod = context.HttpContext.Request.Method;
            var statusCode = context.HttpContext.Response.StatusCode;
            var headersBuilder = new StringBuilder();
            
            foreach (var (key, value) in context.HttpContext.Response.Headers){
                headersBuilder.Append($"    [{key}] : [{value}]" + nl);
            }

            mLogger.LogInformation(GenerateTitle(name) + "Exit action: " + methodName
                                   + nl + "Http Method: " + requestMethod + nl
                                   + "Path: " + path + nl
                                   + "Status code: " + statusCode + nl 
                                   + (headersBuilder.Length == 0 ? "" : 
                                       "Response Headers: {" + nl
                                   + headersBuilder + "    }") + GenerateTitle(name));
        }
    }
}