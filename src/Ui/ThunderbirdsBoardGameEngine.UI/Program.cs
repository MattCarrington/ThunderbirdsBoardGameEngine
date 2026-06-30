using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ThunderbirdsBoardGameEngine.GameState.Client;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime;
using ThunderbirdsBoardGameEngine.Rules.Client.Extensions;

namespace ThunderbirdsBoardGameEngine.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddReferenceData();

            var endpointMode = builder.Configuration["RulesClient:EndpointMode"] ?? "CoHosted";
            if (endpointMode.Equals("CoHosted", StringComparison.OrdinalIgnoreCase))
            {
                // Co-hosted mode: UI and API share the same runtime base URI, so use the host base address (origin + any path base).
                builder.Configuration["RulesClient:BaseAddress"] = builder.HostEnvironment.BaseAddress;
                builder.Configuration["GameStateClient:BaseAddress"] = builder.HostEnvironment.BaseAddress;
            }
            else if (endpointMode.Equals("External", StringComparison.OrdinalIgnoreCase))
            {
                var configuredBaseAddress = builder.Configuration["RulesClient:BaseAddress"];
                if (string.IsNullOrWhiteSpace(configuredBaseAddress))
                {
                    throw new InvalidOperationException(
                        "RulesClient:BaseAddress must be configured when RulesClient:EndpointMode is set to External.");
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unsupported RulesClient:EndpointMode '{endpointMode}'. Supported values are CoHosted and External.");
            }

            builder.Services.AddRulesClients(builder.Configuration);
            builder.Services.AddGameStateClients(builder.Configuration);

            // Register service layer
            builder.Services.AddUiServices();

            await builder.Build().RunAsync();
        }
    }
}
