using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ThunderbirdsBoardGameEngine.GameState.DatabaseProvisioner
{
    internal sealed class DatabaseProvisioner
    {
        private readonly DatabaseProvisioningOptions _options;
        private readonly ILogger<DatabaseProvisioner> _logger;

        public DatabaseProvisioner(IOptions<DatabaseProvisioningOptions> options, ILogger<DatabaseProvisioner> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task ProvisionAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting database account provisioning");

            await using var connection = new NpgsqlConnection(_options.AdministratorConnectionString);

            await connection.OpenAsync(cancellationToken);

            var roleExists = await RuntimeRoleExistsAsync(connection, cancellationToken);

            if (!roleExists)
            {
                await CreateRuntimeRoleAsync(connection, cancellationToken);
                _logger.LogInformation("Runtime database role created");
            }
            else
            {
                _logger.LogInformation("Runtime database role already exists; applying configuration");
            }

            await ConfigureRuntimeRoleAsync(connection, cancellationToken);
            await GrantDatabaseAccessAsync(connection, cancellationToken);

            _logger.LogInformation("Database account provisioning completed");
        }

        private async Task<bool> RuntimeRoleExistsAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
        {
            const string sql = """
                SELECT EXISTS (
                    SELECT 1
                    FROM pg_catalog.pg_roles
                    WHERE rolname = @username
                );
                """;

            await using var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("username", _options.RuntimeUsername);

            var result = await command.ExecuteScalarAsync(cancellationToken);

            return result is true;
        }

        private async Task CreateRuntimeRoleAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
        {
            using var commandBuilder = new NpgsqlCommandBuilder();

            var roleName = commandBuilder.QuoteIdentifier(_options.RuntimeUsername);

            var sql = $"""
                CREATE ROLE {roleName}
                WITH NOLOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE;
                """;

            await using var command = new NpgsqlCommand(sql, connection);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private async Task ConfigureRuntimeRoleAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
        {
            const string sql = """
                SELECT format(
                    'ALTER ROLE %I WITH LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE PASSWORD %L',
                    @username,
                    @password
                );
                """;

            await using var formatCommand = new NpgsqlCommand(sql, connection);

            formatCommand.Parameters.AddWithValue("username", _options.RuntimeUsername);

            formatCommand.Parameters.AddWithValue("password", _options.RuntimePassword);

            var commandText = (string?)await formatCommand.ExecuteScalarAsync(cancellationToken)
                ?? throw new InvalidOperationException("Could not build runtime role configuration.");

            await using var command = new NpgsqlCommand(commandText, connection);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private async Task GrantDatabaseAccessAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
        {
            using var commandBuilder = new NpgsqlCommandBuilder();

            var roleName = commandBuilder.QuoteIdentifier(_options.RuntimeUsername);

            var databaseName = commandBuilder.QuoteIdentifier(connection.Database);

            var sql = $"""
                CREATE SCHEMA IF NOT EXISTS game_state;

                GRANT CONNECT ON DATABASE {databaseName} TO {roleName};

                GRANT USAGE ON SCHEMA game_state TO {roleName};

                GRANT SELECT, INSERT, UPDATE
                ON ALL TABLES IN SCHEMA game_state
                TO {roleName};

                ALTER DEFAULT PRIVILEGES IN SCHEMA game_state
                GRANT SELECT, INSERT, UPDATE ON TABLES TO {roleName};

                REVOKE CONNECT ON DATABASE postgres FROM PUBLIC;
                REVOKE TEMPORARY ON DATABASE {databaseName} FROM PUBLIC;
                """;

            await using var command = new NpgsqlCommand(sql, connection);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}