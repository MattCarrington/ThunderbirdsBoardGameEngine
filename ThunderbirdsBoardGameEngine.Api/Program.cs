using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using ThunderbirdsBoardGameEngine.Api.Healthcheck;
using ThunderbirdsBoardGameEngine.Api.Routing;
using ThunderbirdsBoardGameEngine.Api.Swagger;
using ThunderbirdsBoardGameEngine.Catalog.Application;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;

namespace ThunderbirdsBoardGameEngine.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<CardDataOptions>(
                builder.Configuration.GetSection("CardData"));

            // Add services to the container.
            builder.Services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
            });

            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = false;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;

                options.ApiVersionReader = new HeaderApiVersionReader("X-API-Version");
            });

            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV"; // e.g., v1, v2
                options.SubstituteApiVersionInUrl = true;
            });

            // Health Checks
            builder.Services.AddHealthChecks()
                .AddCheck<JsonDataHealthCheck>("json-data-check");

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
                c.OperationFilter<AddApiVersionHeaderParameter>();
            });

            var allowedOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Apply the CORS policy globally
            app.UseCors("CorsPolicy");

            app.MapControllers();

            app.MapHealthChecks("/health/ready");

            app.Run();
        }
    }
}