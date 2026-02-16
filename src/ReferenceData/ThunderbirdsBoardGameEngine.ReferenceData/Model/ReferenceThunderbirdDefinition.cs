using ThunderbirdsBoardGameEngine.PublishedLanguage.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceThunderbirdDefinition(
         ThunderbirdCode Code,
         string DisplayName
     );
}