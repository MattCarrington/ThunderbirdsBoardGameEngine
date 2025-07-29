using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using ThunderbirdsBoardGameEngine.GameData.Api.Interfaces;
using ThunderbirdsBoardGameEngine.GameData.Api.Profiles.V1;
using ThunderbirdsBoardGameEngine.GameData.Api.Repositories;
using ThunderbirdsBoardGameEngine.GameData.Api.Services.V1;

namespace ThunderbirdsBoardGameEngine.GameData.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

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

            builder.Services.AddAutoMapper(cfg => { }, typeof(DisasterCardProfile));
            builder.Services.AddScoped<IDisasterCardRepository, DisasterCardRepository>();
            builder.Services.AddScoped<IDisasterCardService, DisasterCardService>();

            builder.Services.Configure<CardDataOptions>(
                builder.Configuration.GetSection("CardData"));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}