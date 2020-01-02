using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CLibrary.API.Services{
    public class PropertyCheckerService : IPropertyCheckerService {
        public bool CheckIfValid<T>(string fields){
            if (string.IsNullOrWhiteSpace(fields)){
                return true;
            }

            var splitFields = fields.Split(',');
            /*foreach (var field in splitFields){
                var propertyName = field.Trim();
                var propertyInfo = typeof(T)
                    .GetProperty(propertyName, BindingFlags.IgnoreCase |
                                               BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null){
                    return false;
                }
            }
            return true;
            */

            return splitFields.Select(field => field.Trim())
                .Select(propertyName => typeof(T)
                    .GetProperty(propertyName, BindingFlags.IgnoreCase | 
                                               BindingFlags.Public | BindingFlags.Instance))
                .All(propertyInfo => propertyInfo != null);
        }
    }
}