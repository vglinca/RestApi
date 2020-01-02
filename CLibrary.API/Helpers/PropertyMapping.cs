﻿using System;
using System.Collections.Generic;

namespace CLibrary.API.Helpers{
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping {
        public Dictionary<string, PropertyMappingValue> MappingDictionary{ get; private set; }

        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary){
            MappingDictionary = mappingDictionary ?? throw new ArgumentNullException(nameof(mappingDictionary));
        }
    }
}