using System.Globalization;
using System.Net;
using System.Reflection;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ToDoWebAPI.DBContext;
using ToDoWebAPI.Repository;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]

namespace ToDoWebAPI;

public class Startup
{
    private const string SPACorsURLS = "AllowSPAApps";
    private const string RateLimiterPolicyName = "Fixed";

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ToDoContext>(options => options.UseInMemoryDatabase("ToDoList"));

        services.AddDatabaseDeveloperPageExceptionFilter();

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

        services.AddOpenApi();

        //services.AddSwaggerGen(c =>
        //{
        //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDoWebAPI", Version = "v1" });
        //    c.CustomSchemaIds(x => x.FullName);

        //    // Set the comments path for the Swagger JSON and UI.
        //    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        //    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        //    c.IncludeXmlComments(xmlPath);
        //});

        services.AddHealthChecks();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                return new ValueTask();
            };

            FixedWindowRateLimiterOptions fixedWindowRateLimiterOptions = new() {
                QueueLimit = 50,
                PermitLimit = 500,
                Window = TimeSpan.FromSeconds(60),               
            };

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(
                context =>
                {
                    IPAddress remoteIpAddress = context.Connection.RemoteIpAddress ?? IPAddress.IPv6Loopback;
                    
                    return RateLimitPartition.GetFixedWindowLimiter(remoteIpAddress, _ => fixedWindowRateLimiterOptions);
                });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseExceptionHandler("/error-local-development");
            // app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/openapi/v1.json", "ToDoWebAPI v1"));
        }
        else
        {
            app.UseExceptionHandler("/error");
        }

        app.UseCors(SPACorsURLS);

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseRateLimiter();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapOpenApi().CacheOutput();
            endpoints.MapControllers(); //.RequireRateLimiting(RateLimiterPolicyName);
            endpoints.MapHealthChecks("/health"); //.RequireRateLimiting(RateLimiterPolicyName);
        });
    }
}
