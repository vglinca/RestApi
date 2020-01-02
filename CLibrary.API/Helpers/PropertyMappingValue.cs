using System;
using System.Collections.Generic;

namespace CLibrary.API.Helpers{
    public class PropertyMappingValue{
        public IEnumerable<string> DestinationProps{ get; set; }
        public bool Revert{ get; private set; }

        public PropertyMappingValue(IEnumerable<string> destinationProps, bool revert = false){
            DestinationProps = destinationProps ?? throw new ArgumentNullException(nameof(destinationProps));
            Revert = revert;
        }
    }
}