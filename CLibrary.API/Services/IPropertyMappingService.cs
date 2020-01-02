using System.Collections.Generic;
using CLibrary.API.Helpers;

namespace CLibrary.API.Services{
    public interface IPropertyMappingService{
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        bool IsMappingValid<TSource, TDestination>(string fields);
    }
}