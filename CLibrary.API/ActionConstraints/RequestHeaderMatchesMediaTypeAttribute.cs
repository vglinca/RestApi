using System;
using System.IO.Enumeration;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace CLibrary.API.ActionConstraints{
    
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class RequestHeaderMatchesMediaTypeAttribute : Attribute, IActionConstraint{
        
        private readonly MediaTypeCollection mMediaTypes = new MediaTypeCollection();
        private readonly string mRequestHeaderToMatch;

        public RequestHeaderMatchesMediaTypeAttribute(string requestHeaderToMatch, string mediaType,
            params string[] otherMediaTypes){
            mRequestHeaderToMatch =
                requestHeaderToMatch ?? throw new ArgumentNullException(nameof(requestHeaderToMatch));

            if (MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType)){
                mMediaTypes.Add(parsedMediaType);
            }else{
                throw new ArgumentException(nameof(mediaType));
            }

            foreach (var otherMediaType in otherMediaTypes){
                if (MediaTypeHeaderValue.TryParse(otherMediaType, out var parsedOtherMediaType)){
                    mMediaTypes.Add(parsedOtherMediaType);
                }else{
                    throw new ArgumentException(nameof(otherMediaType));
                }
            }
        }
        
        public bool Accept(ActionConstraintContext context){
            var headers = context.RouteContext.HttpContext.Request.Headers;
            if (!headers.ContainsKey(mRequestHeaderToMatch)){
                return false;
            }
            var parsedRequestMediaType = new MediaType(headers[mRequestHeaderToMatch]);
            return mMediaTypes.Select(mediaType => new MediaType(mediaType)).Contains(parsedRequestMediaType);
        }

        public int Order => 0;
    }
}