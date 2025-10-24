using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using ThunderbirdsBoardGameEngine.Api.Composition;
using ThunderbirdsBoardGameEngine.Api.Routing;
using ThunderbirdsBoardGameEngine.Api.Swagger;
using ThunderbirdsBoardGameEngine.Catalog.Application;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure;

namespace ThunderbirdsBoardGameEngine.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));

            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = false;
            });

            builder.Services.AddHeaderApiVersioning();

            builder.Services.AddHealthChecks();

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddCatalogWarmupServices(builder.Configuration);

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

            builder.Services.AddApiExceptionHandling();
            builder.Services.AddProblemDetails();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseApiExceptionHandling();
            app.UseApiProblemDetails();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Apply the CORS policy globally
            app.UseCors("CorsPolicy");

            app.MapControllers();

            // liveness: dependency-free
            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false
            }).AllowAnonymous();

            // readiness: only checks tagged "readiness" (none yet)
            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("readiness")
            }).AllowAnonymous();

            app.Run();
        }
    }
}