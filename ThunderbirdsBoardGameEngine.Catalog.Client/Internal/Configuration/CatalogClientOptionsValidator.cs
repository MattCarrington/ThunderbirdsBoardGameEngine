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
                return ValidateOptionsResult.Fail("BaseAddress is required.");
            }

            if (!Uri.TryCreate(options.BaseAddress, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return ValidateOptionsResult.Fail("BaseAddress must be a valid absolute http(s) URI.");
            }

            if (!string.IsNullOrEmpty(uri.Query))
            {
                errors.Add("BaseAddress must not contain query strings.");
            }

            if (!string.IsNullOrEmpty(uri.Fragment))
            {
                errors.Add("BaseAddress must not contain a fragment.");
            }

            return errors.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errors);
        }
    }
}
