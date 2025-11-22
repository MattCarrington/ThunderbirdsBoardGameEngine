using Microsoft.AspNetCore.Mvc.ApplicationModels;
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

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApiServices(builder.Configuration);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
                c.OperationFilter<AddApiVersionHeaderParameter>();
            });

            var app = builder.Build();

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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseApiCors();
            
            app.UseAuthorization();

            app.MapControllers();
            app.MapApiHealthChecks();

            app.Run();
        }
    }
}