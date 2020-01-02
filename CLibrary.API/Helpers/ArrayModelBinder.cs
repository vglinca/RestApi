using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CLibrary.API.Helpers{
    public class ArrayModelBinder : IModelBinder{
        public Task BindModelAsync(ModelBindingContext bindingContext){

            if (!bindingContext.ModelMetadata.IsEnumerableType){
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();

            if (string.IsNullOrWhiteSpace(value)){
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }
            //get type of elements in IEnumerable
            var elementType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];

            //creating converter for this type
            var converter = TypeDescriptor.GetConverter(elementType);

            //convert each item in values list to the IEnumerable type
            var values = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => converter.ConvertFromString(x.Trim()))
                .ToArray();

            //creating an array of this type using converter
            var resultValues = Array.CreateInstance(elementType, values.Length);
            values.CopyTo(resultValues, 0);
            bindingContext.Model = resultValues;

            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;
        }
    }
}
