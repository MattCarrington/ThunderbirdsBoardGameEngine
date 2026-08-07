using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Migrations;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddGameStatePersistence(builder.Configuration);
builder.Services.AddGameStateDatabaseMigrator();

using var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();

var migrator = scope.ServiceProvider.GetRequiredService<IGameStateDatabaseMigrator>();

await migrator.Migrate();