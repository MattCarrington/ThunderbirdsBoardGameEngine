using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime;
using ThunderbirdsBoardGameEngine.Rules.Client.Extensions;
using ThunderbirdsBoardGameEngine.UI.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Services;

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
            builder.Services.AddRulesClients(builder.Configuration);

            // Register service layer
            builder.Services.AddScoped<IDisasterCardService, DisasterCardService>();
            builder.Services.AddScoped<ICharacterService, CharacterService>();
            builder.Services.AddScoped<IRescueService, RescueService>();

            await builder.Build().RunAsync();
        }
    }
}
