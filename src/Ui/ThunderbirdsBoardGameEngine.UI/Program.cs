using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ThunderbirdsBoardGameEngine.GameState.Client.Configuration;
using ThunderbirdsBoardGameEngine.GameState.Client.Extensions;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime;
using ThunderbirdsBoardGameEngine.Rules.Client.Configuration;
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

            ConfigureClientEndpoint(builder.Configuration, RulesClientOptions.SectionName, builder.HostEnvironment.BaseAddress);
            ConfigureClientEndpoint(builder.Configuration, GameStateClientOptions.SectionName, builder.HostEnvironment.BaseAddress);

            builder.Services.AddRulesClients(builder.Configuration);
            builder.Services.AddGameStateClients(builder.Configuration);

            // Register service layer
            builder.Services.AddUiServices();

            await builder.Build().RunAsync();
        }

        private static void ConfigureClientEndpoint(IConfiguration configuration, string sectionName, string hostBaseAddress)
        {
            var endpointMode = configuration[$"{sectionName}:EndpointMode"] ?? "CoHosted";

            if (endpointMode.Equals("CoHosted", StringComparison.OrdinalIgnoreCase))
            {
                configuration[$"{sectionName}:BaseAddress"] = hostBaseAddress;

                return;
            }

            if (endpointMode.Equals("External", StringComparison.OrdinalIgnoreCase))
            {
                var configuredBaseAddress = configuration[$"{sectionName}:BaseAddress"];

                if (string.IsNullOrWhiteSpace(configuredBaseAddress))
                {
                    throw new InvalidOperationException(
                        $"{sectionName}:BaseAddress must be configured " +
                        $"when {sectionName}:EndpointMode is External.");
                }

                return;
            }

            throw new InvalidOperationException(
                $"Unsupported {sectionName}:EndpointMode " +
                $"'{endpointMode}'.");
        }
    }
}
