using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Api.Composition;
using ThunderbirdsBoardGameEngine.Api.Routing;
using ThunderbirdsBoardGameEngine.Api.Swagger;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure;

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
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.AddReferenceData();
            builder.Services.AddRules();
            builder.Services.AddApiServices(builder.Configuration);

            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
                c.OperationFilter<AddApiVersionHeaderParameter>();
            });

            var app = builder.Build();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation(
                "Thunderbirds API started in {Environment} mode",
                app.Environment.EnvironmentName);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseApiExceptionHandling();
            app.UseApiProblemDetails();

            app.UseHttpsRedirection();

            // Static files: add .dat mapping so ICU files are served
            app.UseCustomStaticFiles();

            app.UseRouting();

            app.UseApiCors();

            app.UseAuthorization();

            app.MapMetaEndpoint();

            app.MapControllers();
            app.MapApiHealthChecks();

            // SPA fallback: anything not matched goes to index.html
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}
