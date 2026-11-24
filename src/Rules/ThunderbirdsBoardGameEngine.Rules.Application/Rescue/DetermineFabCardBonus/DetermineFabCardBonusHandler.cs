using MediatR;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Registries;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.DetermineFabCardBonus
{
    internal class DetermineFabCardBonusHandler : IRequestHandler<DetermineFabCardBonusQuery, DetermineFabCardBonusResponse>
    {
        private readonly IDisasterCatalogLookup _disasterCatalogLookup;
        private readonly IFabCardRescueModifierSourceRegistry _fabCardRegistry;

        public DetermineFabCardBonusHandler(IDisasterCatalogLookup disasterCatalogLookup, IFabCardRescueModifierSourceRegistry fabCardRegistry)
        {
            _disasterCatalogLookup = disasterCatalogLookup;
            _fabCardRegistry = fabCardRegistry;
        }

        public Task<DetermineFabCardBonusResponse> Handle(DetermineFabCardBonusQuery request, CancellationToken cancellationToken)
        {
            var disaster = _disasterCatalogLookup.GetDisasterRescueContribution(request.DisasterCardCode);

            if (_fabCardRegistry.TryGetCard(disaster.RescueType, out var fabCardSource))
            {
                return Task.FromResult(new DetermineFabCardBonusResponse(fabCardSource.CardCode, fabCardSource.BonusValue));
            }

            throw new InvalidOperationException($"No FAB card found for disaster card '{request.DisasterCardCode}' with rescue type '{disaster.RescueType}'.");
        }
    }
}
