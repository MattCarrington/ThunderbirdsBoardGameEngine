using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.GameState.DatabaseProvisioner;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddOptions<DatabaseProvisioningOptions>()
    .Bind(builder.Configuration.GetSection(DatabaseProvisioningOptions.SectionName))
    .ValidateOnStart();

builder.Services.AddSingleton<IValidateOptions<DatabaseProvisioningOptions>, DatabaseProvisioningOptionsValidator>();
builder.Services.AddTransient<DatabaseProvisioner>();

using var host = builder.Build();

await host.StartAsync();

var provisioner = host.Services.GetRequiredService<DatabaseProvisioner>();
await provisioner.ProvisionAsync();

await host.StopAsync();