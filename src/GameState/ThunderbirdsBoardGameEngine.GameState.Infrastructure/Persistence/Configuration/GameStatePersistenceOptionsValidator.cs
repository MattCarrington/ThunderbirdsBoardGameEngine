using Microsoft.Extensions.Options;
using Npgsql;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Configuration
{
    internal class GameStatePersistenceOptionsValidator : IValidateOptions<GameStatePersistenceOptions>
    {
        private const string ConnectionStringKey = "GameState:Persistence:ConnectionString";

        public ValidateOptionsResult Validate(string? name, GameStatePersistenceOptions options)
        {
            if (options is null)
            {
                return ValidateOptionsResult.Fail("GameState persistence options are required.");
            }

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                return ValidateOptionsResult.Fail($"{ConnectionStringKey} is required.");
            }

            try
            {
                var connectionString = new NpgsqlConnectionStringBuilder(options.ConnectionString);

                if (string.IsNullOrWhiteSpace(connectionString.Host))
                {
                    return ValidateOptionsResult.Fail($"{ConnectionStringKey} must specify a valid host.");
                }

                if (string.IsNullOrWhiteSpace(connectionString.Database))
                {
                    return ValidateOptionsResult.Fail($"{ConnectionStringKey} must specify a valid database name.");
                }
            }
            catch (ArgumentException)
            {
                return ValidateOptionsResult.Fail($"{ConnectionStringKey} is invalid.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
