using Microsoft.Extensions.Options;

namespace ThunderbirdsBoardGameEngine.Client.Core.Configuration
{
    /// <summary>
    /// Validates the <see cref="IApiClientOptions"/> implementation to ensure that required properties are set and valid.
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public class ApiClientOptionsValidator<TOptions> : IValidateOptions<TOptions> where TOptions : class, IApiClientOptions
    {
        /// <summary>
        /// Validates the specified options instance.
        /// </summary>
        /// <param name="name">The name of the options instance.</param>
        /// <param name="options">The options instance to validate.</param>
        /// <returns>The result of the validation.</returns>
        public ValidateOptionsResult Validate(string? name, TOptions options)
        {
            if (options == null)
            {
                return ValidateOptionsResult.Fail($"{typeof(TOptions).Name} is required.");
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
