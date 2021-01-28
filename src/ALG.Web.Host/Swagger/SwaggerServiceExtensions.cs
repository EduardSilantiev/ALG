using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

namespace ALG.Web.Host.Swagger
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddVersionedApiExplorer(options =>
            {
                //The format of the version added to the route URL (VV = <major>.<minor>) 
                options.GroupNameFormat = "'v'VV";

                //Order API explorer to change /api/v{version}/ to /api/v1/  
                options.SubstituteApiVersionInUrl = true;
            });

            // Get IApiVersionDescriptionProvider service
            IApiVersionDescriptionProvider provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

            services.AddSwaggerGen(options =>
            {
                //Create description for each discovered API version
                foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName,
                        new OpenApiInfo()
                        {
                            Title = $"ALG API {description.ApiVersion}",
                            Version = description.ApiVersion.ToString(),
                            Description = "<b>All APIs requere JWT authorization header using the Bearer scheme.</b>\n\n" +
                                    "For authentication, get JWT AccessToken calling API /Users/Login. Then add Authorization header for any request.\n\n" +
                                    "The value of the header should be “Bearer ” followed by the JWT AccessToken. For example:\n\n" +
                                    "<i>Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bml...</i>\n\n"
                        });
                }

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }});

                // [SwaggerRequestExample] & [SwaggerResponseExample]
                // version < 3.0 like this: c.OperationFilter<ExamplesOperationFilter>(); 
                // version 3.0 like this: c.AddSwaggerExamples(services.BuildServiceProvider());
                // version > 4.0 like this:
                options.ExampleFilters();

                //Get XML comments file path and include it to Swagger for the JSON documentation and UI.
                string xmlCommentsPath = Assembly.GetExecutingAssembly().Location.Replace("dll", "xml");
                options.IncludeXmlComments(xmlCommentsPath);
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app,
                                                                     IWebHostEnvironment env,
                                                                     IApiVersionDescriptionProvider provider,
                                                                     string documentTitle)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                //Build a swagger endpoint for each discovered API version  
                foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    options.RoutePrefix = "";
                    options.DocumentTitle = documentTitle;
                    options.DocExpansion(DocExpansion.None);
                }

                if (!env.IsDevelopment())
                {
                    options.ConfigObject.SupportedSubmitMethods = new SubmitMethod[0];
                }
            });

            return app;
        }
    }
}
