using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using CLibrary.API.Auth;
using CLibrary.API.DbContexts;
using CLibrary.API.Logging;
using CLibrary.API.OperationFilters;
using CLibrary.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace CLibrary.API {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services){
            services.AddHttpCacheHeaders(expirationOptions => {
                expirationOptions.MaxAge = 60;
                expirationOptions.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
            }, validationOptions => { validationOptions.MustRevalidate = true; });
            services.AddResponseCaching();
            services.AddControllers(setupActions => {
                    setupActions.Filters.Add(new ProducesResponseTypeAttribute(
                        StatusCodes.Status400BadRequest));
                    setupActions.Filters.Add(new ProducesResponseTypeAttribute(
                        StatusCodes.Status401Unauthorized));
                    setupActions.Filters.Add(new ProducesResponseTypeAttribute(
                        StatusCodes.Status404NotFound));
                    setupActions.Filters.Add(new ProducesResponseTypeAttribute(
                        StatusCodes.Status406NotAcceptable));
                    setupActions.Filters.Add(new ProducesResponseTypeAttribute(
                        StatusCodes.Status422UnprocessableEntity));
                    setupActions.Filters.Add(new ProducesResponseTypeAttribute(
                        StatusCodes.Status500InternalServerError));
                    //setupActions.Filters.Add(new AuthorizeFilter());
                setupActions.ReturnHttpNotAcceptable = true;
                setupActions.CacheProfiles.Add("240secondsCacheProfile", 
                    new CacheProfile{
                        Duration = 240
                    });
            })
            .AddNewtonsoftJson(action => {
                action.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(action => {
                action.InvalidModelStateResponseFactory = context => {
                    var probDetails = new ValidationProblemDetails(context.ModelState) {
                        Type = "https://courselibrary.com/modelvalidationproblem",
                        Title = "One or more model validation errors has occured.",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail = "See the errors property for more details",
                        Instance = context.HttpContext.Request.Path
                    };
                    probDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                    return new UnprocessableEntityObjectResult(probDetails) {
                        ContentTypes = {"application/problem+json"}
                    };
                };
            });
            
            services.AddVersionedApiExplorer(setupAction => {
                setupAction.GroupNameFormat = "'v'VV";
            });

            // services.AddAuthentication("Basic")
            //     .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("Basic", null);
            
            services.AddApiVersioning(setupAction => {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
                setupAction.ReportApiVersions = true;
            });
            
            var apiVersionDescriptionProvider =
                services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();
            
            services.AddSwaggerGen(action => {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions){
                    action.SwaggerDoc(
                        $"LibraryOpenApiSpecification{description.GroupName}", new OpenApiInfo{
                        Title = "Library API",
                        Version = description.ApiVersion.ToString(),
                        Description = "Through this API you can access authors and courses.",
                        Contact = new OpenApiContact{
                            Email = "vitalii.glinka@yahoo.com",
                            Name = "Vitaly GLinca",
                            Url = new Uri("https://vk.com/gainsayer")
                        },
                        License = new OpenApiLicense(){
                            Name = "MIT License",
                            Url = new Uri("https://opensource.org/licenses/MIT")
                        }
                    });
                }
                
                // action.AddSecurityDefinition("basicAuth", new OpenApiSecurityScheme(){
                //     Type = SecuritySchemeType.Http,
                //     Scheme = "basic",
                //     Description = "Input your username and password to access this API"
                // });
                //
                // action.AddSecurityRequirement(new OpenApiSecurityRequirement(){
                //     {
                //         new OpenApiSecurityScheme{
                //             Reference = new OpenApiReference{
                //                 Type = ReferenceType.SecurityScheme,
                //                 Id = "basicAuth"
                //             }
                //         }, new List<string>()
                //     }
                //});
                
                action.DocInclusionPredicate((docName, apiDescription) => {
                    var actionApiVersionModel = apiDescription.ActionDescriptor
                        .GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);
                    if (actionApiVersionModel == null){
                        return true;
                    }
                
                    if (actionApiVersionModel.DeclaredApiVersions.Any()){
                        return actionApiVersionModel.DeclaredApiVersions.Any(v =>
                            $"LibraryOpenApiSpecificationv{v.ToString()}" == docName);
                    }
                
                    return actionApiVersionModel.ImplementedApiVersions.Any(v =>
                        $"LibraryOpenApiSpecificationv{v.ToString()}" == docName);
                });
                
                // action.OperationFilter<CreateBookOperationFilter>();
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlFileFullPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                action.IncludeXmlComments(xmlFileFullPath);
            });

            services.Configure<MvcOptions>(config => {
                var newtonsoftJsonOutFormatter = config
                    .OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()
                    .FirstOrDefault();
                newtonsoftJsonOutFormatter?.SupportedMediaTypes.Add("application/vnd.marvin.hateoas+json");
            });
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();
            services.AddScoped<ILoggingActionFilter, LoggingActionFilter>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();
            services.AddDbContext<CourseLibraryContext>(options => {
                options.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=CourseLibraryDB;Trusted_Connection=True;");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env
            ,IApiVersionDescriptionProvider apiVersionDescriptionProvider) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }else {
                app.UseExceptionHandler(appBuilder => {
                    appBuilder.Run(async context => {
                        context.Response.StatusCode = 500;
                        await context.Response
                        .WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });
            }

            // app.UseResponseCaching();

            app.UseHttpCacheHeaders();
            
            app.UseRouting();
            
            //app.UseAuthorization();
            
            app.UseSwagger();
            
            app.UseSwaggerUI(options => {
                    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions){
                        options.SwaggerEndpoint(
                            $"../swagger/LibraryOpenApiSpecification{description.GroupName}/swagger.json",
                            "Library API");
                    }
                    options.RoutePrefix = "";
                    
                    options.DefaultModelExpandDepth(2);
                    options.DefaultModelRendering(ModelRendering.Model);
                    options.DocExpansion(DocExpansion.None);
                    options.EnableDeepLinking();
                    options.DisplayOperationId();
                });

            //app.UseAuthentication();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
