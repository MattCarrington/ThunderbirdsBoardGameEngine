using ThunderbirdsBoardGameEngine.PublishedLanguage.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceLocationDefinition(
        LocationCode Code,
        string DisplayName);
}
