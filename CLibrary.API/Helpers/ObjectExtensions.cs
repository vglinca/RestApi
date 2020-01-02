using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace CLibrary.API.Helpers{
    public static class ObjectExtensions{
        
        public static ExpandoObject ShapeData<TSource>(
            this TSource source, string fields){
            if (source == null){
                throw new ArgumentNullException(nameof(source));
            }
            var dataShapedObj = new ExpandoObject();
            if (string.IsNullOrWhiteSpace(fields)){
                var propInfos = typeof(TSource)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var propInfo in propInfos){
                    var propValue = propInfo.GetValue(source);
                    ((IDictionary<string, object>) dataShapedObj).Add(propInfo.Name, propValue);
                }
                return dataShapedObj;
            }
            var splitFields = fields.Split(',');
            foreach (var field in splitFields){
                var property = field.Trim();
                var propertyInfo = typeof(TSource)
                    .GetProperty(property, BindingFlags.IgnoreCase | BindingFlags.Public |
                                           BindingFlags.Instance);
                if (propertyInfo == null){
                    throw new Exception($"Property {property} wasn't found on " +
                                        $"{typeof(TSource)}");
                }
                var propValue = propertyInfo.GetValue(source);
                ((IDictionary<string, object>) dataShapedObj).Add(propertyInfo.Name, propValue);
            }
            return dataShapedObj;
        }
    }
}