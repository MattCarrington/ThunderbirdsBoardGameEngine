using Microsoft.Extensions.Options;

namespace ThunderbirdsBoardGameEngine.Client.Core.Configuration
{
    /// <summary>
    /// Post-configures the <see cref="IApiClientOptions"/> implementation to ensure that the BaseAddress is properly formatted.
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public class ApiClientOptionsPostConfigure<TOptions> : IPostConfigureOptions<TOptions> where TOptions : class, IApiClientOptions
    {
        /// <summary>
        /// Post-configures the specified options instance.
        /// </summary>
        /// <param name="name">The name of the options instance.</param>
        /// <param name="options">The options instance to post-configure.</param>
        public void PostConfigure(string? name, TOptions options)
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
