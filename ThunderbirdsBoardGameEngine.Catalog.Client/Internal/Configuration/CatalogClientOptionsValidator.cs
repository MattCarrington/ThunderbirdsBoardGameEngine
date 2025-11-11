using Microsoft.Extensions.Options;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Configuration
{
    internal sealed class CatalogClientOptionsValidator : IValidateOptions<CatalogClientOptions>
    {
        public ValidateOptionsResult Validate(string? name, CatalogClientOptions options)
        {
            if (options is null)
            {
                return ValidateOptionsResult.Fail("CatalogClientOptions is required.");
            }

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(options.BaseAddress))
            {
                errors.Add("BaseAddress is required.");
            }
            else if (!Uri.TryCreate(options.BaseAddress, UriKind.Absolute, out var uri))
            {
                errors.Add("BaseAddress must be a valid absolute URI.");
            }
            else if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                errors.Add("BaseAddress must use http or https scheme.");
            }
            else
            {
                if (!string.IsNullOrEmpty(uri.Query))
                {
                    errors.Add("BaseAddress must not contain query strings.");
                }

                if (!string.IsNullOrEmpty(uri.Fragment))
                {
                    errors.Add("BaseAddress must not contain a fragment.");
                }
            }

                return errors.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errors);
        }
    }
}
