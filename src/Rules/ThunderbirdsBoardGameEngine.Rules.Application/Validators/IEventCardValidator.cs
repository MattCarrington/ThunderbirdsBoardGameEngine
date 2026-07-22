using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Validators
{
    public interface IEventCardValidator
    {
        void Validate(IReadOnlyCollection<CardCode> eventCards);
    }
}