using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.ReferenceSources;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces
{
    internal interface ICharacterDefinitionReferenceSourceInitializer
    {
        Task<InMemoryCharacterDefinitionReferenceSource> InitializeAsync(CancellationToken cancellationToken = default);
    }
}