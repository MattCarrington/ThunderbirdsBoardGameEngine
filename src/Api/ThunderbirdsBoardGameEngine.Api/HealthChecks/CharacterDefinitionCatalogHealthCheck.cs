using Microsoft.Extensions.Diagnostics.HealthChecks;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Api.HealthChecks
{
    public class CharacterDefinitionCatalogHealthCheck : IHealthCheck
    {
        private readonly ICharacterDefinitionReferenceSourceProbe _characterDefinitionProbe;        

        public CharacterDefinitionCatalogHealthCheck(ICharacterDefinitionReferenceSourceProbe characterDefinitionProbe)
        {
             _characterDefinitionProbe = characterDefinitionProbe ?? throw new ArgumentNullException(nameof(characterDefinitionProbe));
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var definedCharacters = _characterDefinitionProbe.Keys.ToHashSet();
                var expectedCharacters = Enum.GetValues<Character>();

                var missing = expectedCharacters
                    .Where(c => !definedCharacters.Contains(c))
                    .ToArray();

                var data = new Dictionary<string, object>
                {
                    ["version"] = _characterDefinitionProbe.Version,
                    ["definedCount"] = definedCharacters.Count,
                    ["expectedCount"] = expectedCharacters.Length
                };

                if (definedCharacters.Count == 0)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy(
                        "Character catalog contains no data.",
                        data: data));
                }

                if (missing.Length > 0)
                {
                    data["missing"] = missing.Select(m => m.ToString()).ToArray();

                    return Task.FromResult(HealthCheckResult.Unhealthy(
                        "Character catalog is incomplete.",
                        data: data));
                }

                return Task.FromResult(
                    HealthCheckResult.Healthy(
                        "Character catalog is complete.",
                        data: data));
            }
            catch (OperationCanceledException)
            {
                // Respect shutdown / timeouts: let the framework handle cancellation semantics.
                throw;
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Character catalog health check failed.", ex));
            }
        }
    }
}
