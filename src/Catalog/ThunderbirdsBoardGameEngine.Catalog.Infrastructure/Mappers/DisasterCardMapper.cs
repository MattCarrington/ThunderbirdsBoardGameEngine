using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Helpers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Mappers
{
    internal sealed class DisasterCardMapper : IDisasterCardMapper
    {
        public DisasterCard Map(DisasterCardCatalogDto dto)
        {
            try
            {
                if (dto is null)
                {
                    throw DisasterCardValidationException.NullEntry();
                }

                var location = EnumParser.ParseEnum<BoardLocation>(dto.Location);
                var rescueType = EnumParser.ParseEnum<RescueType>(dto.RescueType);

                return new DisasterCard(
                    dto.Id,
                    dto.Name,
                    new CardCode(dto.Code),
                    dto.DifficultyNumber,
                    location,
                    rescueType,
                    dto.BonusConditions.Select(bc => MapBonus(bc, dto.Id, dto.Name)).ToList(),
                    dto.RewardOptions.Select(ro => MapReward(ro, dto.Id, dto.Name)).ToList()
                );
            }
            catch (ArgumentException ex)
            {
                throw DisasterCardValidationException.Unknown(dto?.Id, dto?.Name, ex);
            }
        }

        private static BonusCondition MapBonus(BonusConditionCatalogDto dto, int id, string name)
        {
            if (dto is null)
            {
                throw DisasterCardValidationException.NullBonusCondition(id, name);
            }

            var location = string.IsNullOrWhiteSpace(dto.Location)
                ? (BoardLocation?)null
                : EnumParser.ParseEnum<BoardLocation>(dto.Location);

            switch (dto)
            {
                case CharacterBonusCatalogDto characterBonus:
                    var character = EnumParser.ParseEnum<Character>(characterBonus.Character);
                    return new CharacterBonusCondition(character, characterBonus.BonusValue, location);
                case ThunderbirdBonusCatalogDto thunderbirdBonus:
                    var thunderbird = EnumParser.ParseEnum<ThunderbirdMachine>(thunderbirdBonus.Thunderbird);
                    return new ThunderbirdBonusCondition(thunderbird, thunderbirdBonus.BonusValue, location);
                case PodVehicleBonusCatalogDto podVehicleBonus:
                    var podVehicle = EnumParser.ParseEnum<PodVehicle>(podVehicleBonus.PodVehicle);
                    return new PodVehicleBonusCondition(podVehicle, podVehicleBonus.BonusValue, location);
                default:
                    throw DisasterCardValidationException.UnknownBonusCondition(id, name);
            }
        }

        private static RewardOption MapReward(RewardOptionCatalogDto dto, int id, string name)
        {
            if (dto is null)
            {
                throw DisasterCardValidationException.NullRewardOption(id, name);
            }

            switch (dto)
            {
                case PlayerChoiceRewardCatalogDto:
                    return RewardOption.PlayerChoice();
                case TokenRewardCatalogDto specifiedToken:
                    var token = EnumParser.ParseEnum<BonusToken>(specifiedToken.Token);
                    return RewardOption.SpecifiedToken(token);
                default:
                    throw DisasterCardValidationException.UnknownRewardOption(id, name);
            }
        }
    }
}
