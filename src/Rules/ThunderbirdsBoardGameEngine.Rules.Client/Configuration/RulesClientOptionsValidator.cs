using Microsoft.Extensions.Options;

namespace ThunderbirdsBoardGameEngine.Rules.Client.Configuration
{
    internal sealed class RulesClientOptionsValidator : IValidateOptions<RulesClientOptions>
    {
        public ValidateOptionsResult Validate(string? name, RulesClientOptions options)
        {
            if (options is null)
            {
                return ValidateOptionsResult.Fail("RulesClientOptions is required.");
            }

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(options.BaseAddress))
            {
                return ValidateOptionsResult.Fail("BaseAddress is required.");
            }

            if (!Uri.TryCreate(options.BaseAddress, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))  // Tighten to https-only in production once deployment story is in place.
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
