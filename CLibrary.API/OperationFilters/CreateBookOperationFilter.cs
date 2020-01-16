using CLibrary.API.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CLibrary.API.OperationFilters{
    public abstract class CreateBookOperationFilter : IOperationFilter{
        public void Apply(OpenApiOperation operation, OperationFilterContext context){
            if (context.MethodInfo.Name != "CreateAuthorWithDateOfDeath"){
                return;
            }
            operation.RequestBody.Content.Add(
                "application/vnd.marvin.authorforcreationwithdateofdeath+json",
                new OpenApiMediaType(){
                    Schema = context.SchemaRepository.GetOrAdd(typeof(AuthorForCreationWithDateOfDeathDto),
                        "AuthorForCreationWithDateOfDeathDto", () => new OpenApiSchema())
                });
        }
    }
}