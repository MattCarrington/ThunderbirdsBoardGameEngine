using Microsoft.Extensions.Options;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Configuration
{
    internal sealed class CatalogClientOptionsPostConfigure : IPostConfigureOptions<CatalogClientOptions>
    {
        public void PostConfigure(string? name, CatalogClientOptions options)
        {
            var baseAddress = options.BaseAddress;

            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                return; // Return early so validation can catch the error
            }

            baseAddress = baseAddress.Trim();

            if (baseAddress.Length >= 2)
            {
                var first = baseAddress[0];
                var last = baseAddress[^1];

                if ((first == '"' && last == '"') || (first == '\'' && last == '\''))
                {
                    baseAddress = baseAddress[1..^1];
                }
            }

            if (Uri.TryCreate(baseAddress, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                var uriBuilder = new UriBuilder(uri);

                var path = string.IsNullOrEmpty(uriBuilder.Path) ? "/" : uriBuilder.Path;

                if (!path.EndsWith('/'))
                {
                    uriBuilder.Path += '/';
                }

                options.BaseAddress = uriBuilder.Uri.ToString();
            }
            else
            {
                options.BaseAddress = baseAddress;
            }
        }
    }
}
