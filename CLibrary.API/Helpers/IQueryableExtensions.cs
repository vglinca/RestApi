using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq;

namespace CLibrary.API.Helpers {
    public static class IQueryableExtensions{
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy,
            Dictionary<string, PropertyMappingValue> mappingDictionary){
            if (source == null){
                throw new ArgumentNullException(nameof(source));
            }
            if (mappingDictionary == null){
                throw new ArgumentNullException(nameof(mappingDictionary));
            }
            if (string.IsNullOrWhiteSpace(orderBy)){
                return source;
            }

            var orderByString = "";
            var orderBySplit = orderBy.Split(',');
            foreach (var orderByClause in orderBySplit){
                var trimmedOrderByClause = orderByClause.Trim();
                var orderDesc = trimmedOrderByClause.EndsWith(" desc");
                var firstSpaceIndex = trimmedOrderByClause.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = firstSpaceIndex == -1
                    ? trimmedOrderByClause
                    : trimmedOrderByClause.Remove(firstSpaceIndex);

                if (!mappingDictionary.ContainsKey(propertyName)){
                    throw new ArgumentException($"Key mapping for {propertyName} is missing");
                }

                var propertyMatchingValue = mappingDictionary[propertyName];
                if (propertyMatchingValue == null){
                    throw new ArgumentNullException(nameof(propertyMatchingValue));
                }

                foreach (var destinationProperty in propertyMatchingValue.DestinationProps.Reverse()){
                    if (propertyMatchingValue.Revert){
                        orderDesc = !orderDesc;
                    }
                    orderByString += (!string.IsNullOrWhiteSpace(orderByString) ? "," : "") + destinationProperty +
                                 (orderDesc ? " descending" : " ascending");
                }
            }
            return source.OrderBy(orderByString);
        }
    }
}