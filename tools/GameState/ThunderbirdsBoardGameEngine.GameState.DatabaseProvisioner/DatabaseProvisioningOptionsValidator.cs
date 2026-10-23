using Microsoft.Extensions.Options;

namespace ThunderbirdsBoardGameEngine.GameState.DatabaseProvisioner
{
    internal sealed class DatabaseProvisioningOptionsValidator : IValidateOptions<DatabaseProvisioningOptions>
    {
        public ValidateOptionsResult Validate(string? name, DatabaseProvisioningOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.AdministratorConnectionString))
            {
                return ValidateOptionsResult.Fail("Administrator connection string is required.");
            }

            if (string.IsNullOrWhiteSpace(options.RuntimeUsername))
            {
                return ValidateOptionsResult.Fail("Runtime username is required.");
            }

            if (string.IsNullOrWhiteSpace(options.RuntimePassword))
            {
                return ValidateOptionsResult.Fail("Runtime password is required.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}