using MediatR;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.DetermineFabCardBonus
{
    public record DetermineFabCardBonusQuery(CardCode DisasterCardCode) : IRequest<DetermineFabCardBonusResponse>
    {
    }
}
