using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace CLibrary.API.Helpers{
    public static class IEnumerableExtensions{
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(
            this IEnumerable<TSource> source, string fields){
            
            if (source == null){
                throw new ArgumentNullException(nameof(source));
            }
            var expandoList = new List<ExpandoObject>();
            var propInfoList = new List<PropertyInfo>();
            if (string.IsNullOrWhiteSpace(fields)){
                var propInfos = typeof(TSource)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance);
                propInfoList.AddRange(propInfos);
            }else{
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
                    propInfoList.Add(propertyInfo);
                }
            }

            foreach (var srcObj in source){
                var dataShapedObj = new ExpandoObject();
                foreach (var propertyInfo in propInfoList){
                    var propValue = propertyInfo.GetValue(srcObj);
                    ((IDictionary<string, object>) dataShapedObj).Add(propertyInfo.Name, propValue);
                }
                expandoList.Add(dataShapedObj);
            }
            return expandoList;
        }
    }
}