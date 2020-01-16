using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CLibrary.API.Entities;
using CLibrary.API.Helpers;
using CLibrary.API.Models;

namespace CLibrary.API.Services{

    public class PropertyMappingService : IPropertyMappingService{
        
        private readonly Dictionary<string, PropertyMappingValue> mAuthorPropMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase){
                {"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"MainCategory", new PropertyMappingValue(new List<string>(){"MainCategory"})},
                {"Age", new PropertyMappingValue(new List<string>(){"DateOfBirth"}, true)},
                {"Name", new PropertyMappingValue(new List<string>(){"FirstName", "LastName"})}
            };
        private readonly List<IPropertyMapping> mPropertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService(){
            mPropertyMappings.Add(new PropertyMapping<AuthorDto, Author>(mAuthorPropMapping));
        }

        public bool IsMappingValid<TSource, TDestination>(string fields){
            var propMapping = GetPropertyMapping<TSource, TDestination>();
            if (string.IsNullOrWhiteSpace(fields)){
                return true;
            }
            var splitFields = fields.Split(',');
            return (from field in splitFields select field.Trim() 
                into trimmedField let firstSpaceIndex = trimmedField.IndexOf(" ", StringComparison.Ordinal) 
                select firstSpaceIndex == -1 ? trimmedField : trimmedField.Remove(firstSpaceIndex))
                .All(propertyName => propMapping.ContainsKey(propertyName));
        }
        
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>(){
            var matchingMapping = mPropertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();
            if (matchingMapping.Count() == 1){
                return matchingMapping.First().MappingDictionary;
            }
            throw new Exception($"Cannot find exact property for instance " +
                                $"for <{typeof(TSource)}, {typeof(TDestination)}");
        }
    }
}