using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ThunderbirdsBoardGameEngine.GameData.Api.Client;

namespace ThunderbirdsBoardGameEngine.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddGameDataClients(builder.Configuration);

            await builder.Build().RunAsync();
        }
    }
}
