using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{
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
