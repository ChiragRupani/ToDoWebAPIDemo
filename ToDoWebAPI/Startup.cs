using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ToDoWebAPI.DBContext;
using ToDoWebAPI.Repository;

namespace ToDoWebAPI
{
    public class Startup
    {
        private const string SPACorsURLS = "AllowSPAApps";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ToDoContext>(options => options.UseInMemoryDatabase("ToDoList"));

            services.AddScoped<ToDoRepository>();

            services.AddCors(options =>
            {
                options.AddPolicy(SPACorsURLS, builder => builder.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8080")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                );
            });

            services.AddControllers()
                .AddJsonOptions(x => x.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDoWebAPI", Version = "v1" });
                c.CustomSchemaIds(x => x.FullName);

                // Set the comments path for the Swagger JSON and UI.
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error-local-development");
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDoWebAPI v1"));
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseCors(SPACorsURLS);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
