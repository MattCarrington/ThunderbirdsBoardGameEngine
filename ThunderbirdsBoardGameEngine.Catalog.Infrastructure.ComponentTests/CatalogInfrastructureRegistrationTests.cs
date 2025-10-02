using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Repositories;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using ThunderbirdsBoardGameEngine.TestUtils.Stubs;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.ComponentTests
{
    public class CatalogInfrastructureRegistrationTests
    {
        [Theory]
        [MemberData(nameof(Repositories))]
        public void AddInfrastructure_Registers_Repository(RepositoryCase repositoryCase)
        {
            // Arrange: absolute path to a real test file for THIS repo
            var absolutePath = TestDataPathHelper.GetPath(repositoryCase.TestFileName);

            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { [repositoryCase.ConfigKey] = absolutePath })
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IHostEnvironment>(new StubHostEnvironment(Directory.GetCurrentDirectory()));
            services.AddInfrastructure(cfg);

            using var sp = services.BuildServiceProvider(validateScopes: true);

            // Trigger options pipeline (Bind + PostConfigure + Validator) for THIS repo’s options type
            TriggerOptionsValidation(sp, repositoryCase.OptionsType);

            // Resolve repo (use a scope if repos are Scoped)
            using var scope = sp.CreateScope();
            var resolved = scope.ServiceProvider.GetRequiredService(repositoryCase.ServiceType);

            Assert.Equal(repositoryCase.ImplType, resolved.GetType());
        }

        private static void TriggerOptionsValidation(IServiceProvider sp, Type optionsType)
        {
            var ioptionsType = typeof(IOptions<>).MakeGenericType(optionsType);
            var options = sp.GetRequiredService(ioptionsType);
            _ = options.GetType().GetProperty("Value")!.GetValue(options);
        }

        public sealed record RepositoryCase(
            Type ServiceType,   // e.g. typeof(IDisasterCardRepository)
            Type ImplType,      // e.g. typeof(JsonDisasterCardRepository)
            Type OptionsType,   // e.g. typeof(DisasterCardOptions)
            string ConfigKey,   // e.g. "Data:DisasterCards:FilePath"
            string TestFileName // e.g. "disaster-cards-test.json"
        );

        public static IEnumerable<object[]> Repositories()
        {
            yield return new object[] {
                new RepositoryCase(
                    typeof(IDisasterCardRepository),
                    typeof(DisasterCardJsonRepository),
                    typeof(DisasterCardJsonOptions),
                    "Catalog:DisasterCards:Json:Filepath",
                    "disaster-cards-test.json")
            };            
        }
    }
}
