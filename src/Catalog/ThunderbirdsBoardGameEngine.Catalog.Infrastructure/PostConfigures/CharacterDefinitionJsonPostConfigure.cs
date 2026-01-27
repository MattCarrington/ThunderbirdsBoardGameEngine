using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.PostConfigures
{
    internal sealed class CharacterDefinitionJsonPostConfigure : IPostConfigureOptions<CharacterDefinitionJsonOptions>
    {
        private readonly IHostEnvironment _env;

        public CharacterDefinitionJsonPostConfigure(IHostEnvironment env)
        {
            _env = env;
        }

        public void PostConfigure(string? name, CharacterDefinitionJsonOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.FilePath))
            {
                return; // return early so validation can catch the error
            }

            options.FilePath = JsonFilePathNormalizer.Normalize(options.FilePath, _env.ContentRootPath);
        }
    }
}
