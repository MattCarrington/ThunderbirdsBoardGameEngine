using ThunderbirdsBoardGameEngine.PublishedLanguage.Characters;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces
{
    public interface ICharacterContributionLookup
    {
        CharacterContribution GetCharacterContribution(CharacterCode characterCode);
    }
}
