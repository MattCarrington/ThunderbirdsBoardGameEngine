using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{
    /// <summary>
    /// Represents a reward option granted by a disaster card.
    /// </summary>
    /// <remarks>
    /// A reward may either allow the player to choose a bonus token or
    /// specify a concrete token directly.
    /// </remarks>
    public sealed class RewardOption
    {
        public bool IsUserChoice { get; }

        public BonusToken? Token { get; }

        public RewardOption(bool isUserChoice, BonusToken? token)
        {
            if (!isUserChoice && token is null)
            {
                throw new ArgumentException("Token must be set when IsUserChoice is false.", nameof(token));
            }

            IsUserChoice = isUserChoice;
            Token = token;
        }

        public static RewardOption PlayerChoice()
        {
            return new RewardOption(isUserChoice: true, token: null);
        }

        public static RewardOption SpecifiedToken(BonusToken token)
        {
            return new RewardOption(isUserChoice: false, token: token);
        }
    }
}
