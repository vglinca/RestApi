using System;
using System.Linq;
using AutoMapper;
using CLibrary.API.DbContexts;
using CLibrary.API.Logging;
using CLibrary.API.Services;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

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
                setupActions.ReturnHttpNotAcceptable = true;
                setupActions.CacheProfiles.Add("240secondsCacheProfile", 
                    new CacheProfile(){
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

            services.Configure<MvcOptions>(config => {
                var newtonsoftJsonOutFormatter = config
                    .OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
