using ALG.Application.Application.Settings;
using ALG.Application.Helpers.Exceptions;
using ALG.Application.Services;
using ALG.Application.Services.Dto;
using ALG.Application.Settings;
using ALG.Application.Users;
using ALG.Core.Services;
using ALG.Core.Users;
using ALG.EntityFrameworkCore;
using ALG.EntityFrameworkCore.EntityFrameworkCore.Repositories;
using ALG.Web.Host.JWT;
using ALG.Web.Host.Swagger;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace ALG.Web.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AlgDbContext>(options =>
                     options.UseSqlServer(Configuration.GetConnectionString("Default"),
                            //change migration history table schema from dbo to alg (Ownership and User-Schema Separation)
                            x => x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "alg"))
                            .EnableSensitiveDataLogging(Environment.IsDevelopment()));

            services.AddOptions();

            services.AddAutoMapper(typeof(ServicesMapProfile).Assembly);

            services.Configure<PagingSettings>(Configuration.GetSection("PagingSettings"));
            services.Configure<RuntimeSettings>(Configuration.GetSection("RuntimeSettings"));
            
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IServicesRepository, ServicesRepository>();

            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IServicesService, ServicesService>();

            //configure CORS
            var corsSettingsSection = Configuration.GetSection("CORSSettings");
            services.Configure<CORSSettings>(corsSettingsSection);
            var corsSettings = corsSettingsSection.Get<CORSSettings>();

            services.AddCors(options =>
            {
                options.AddPolicy("Default", builder =>
                {
                    builder.WithOrigins(
                        //CORS Origins in appsettings.json can contain more than one address separated by comma.
                        corsSettings.Origins.Split(",", StringSplitOptions.RemoveEmptyEntries));
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
            });

            // configure jwt authentication
            var jwtSettingsSection = Configuration.GetSection("JWTSettings");
            services.Configure<JWTSettings>(jwtSettingsSection);
            var jwtSettings = jwtSettingsSection.Get<JWTSettings>();

            services.AddJWTSecurity(jwtSettings);

            //support API versioning
            services.AddApiVersioning(o => {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });

            //Add OpenAPI documentation
            services.AddSwaggerDocumentation();
            services.AddSwaggerExamplesFromAssemblyOf<UserLoginRequestExample>();

            //Prevent default MVC behaviour for API model validation - 
            //  implement unified error messaging approach
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
                            IApiVersionDescriptionProvider provider, AlgDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionsHandlingMiddleware>();

            app.UseCors("Default");

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwaggerDocumentation(env, provider, "ALG API documentation");

            //migrate and seed database
            new DbInitializer(context).InitDatabase();
        }
    }
}
