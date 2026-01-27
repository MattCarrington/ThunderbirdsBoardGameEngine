using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces
{
    internal interface IGeneratedContentValidator
    {
        void Validate<TItem>(Payload<GeneratedCatalogManifest> payload);
    }
}