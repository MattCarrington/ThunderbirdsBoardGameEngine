using ThunderbirdsBoardGameEngine.PublishedLanguage.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceLocationDefinition(
        LocationCode Code,
        string DisplayName);
}
