using System.Text;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Mappers
{
    internal sealed class DisasterCardMapper : IDisasterCardMapper
    {
        public DisasterCard Map(DisasterCardCatalogDto dto)
        {
            if (dto is null)
            {
                throw new JsonException("Root list is null");
            }

            var code = string.IsNullOrWhiteSpace(dto.Code)
                ? Slug(dto.Name)
                : dto.Code.Trim().ToLowerInvariant();   // TEMPORARY: relax requirement for 'code' to allow legacy data to work

            // if (string.IsNullOrWhiteSpace(dto.Code))
            // throw new JsonException($"Card Id={dto.Id}, Name='{dto.Name}': 'code' is required.");


            var location = ParseEnum<BoardLocation>(dto.Location);
            var rescueType = ParseEnum<RescueType>(dto.RescueType);

            return new DisasterCard(
                dto.Id,
                dto.Name,
                code,
                dto.DifficultyNumber,
                location,
                rescueType,
                dto.BonusConditions.Select(MapBonus).ToList(),
                dto.RewardOptions.Select(MapReward).ToList()
            );
        }

        private static BonusCondition MapBonus(BonusConditionCatalogDto dto)
        {
            if (dto is null)
            {
                throw new JsonException("Bonus condition is null");
            }

            var location = string.IsNullOrWhiteSpace(dto.Location) 
                ? (BoardLocation?)null 
                : ParseEnum<BoardLocation>(dto.Location);

            switch (dto)
            {
                case CharacterBonusCatalogDto characterBonus:
                    var character = ParseEnum<Character>(characterBonus.Character);
                    return new CharacterBonusCondition(character, characterBonus.BonusValue, location);
                case ThunderbirdBonusCatalogDto thunderbirdBonus:
                    var thunderbird = ParseEnum<ThunderbirdMachine>(thunderbirdBonus.Thunderbird);
                    return new ThunderbirdBonusCondition(thunderbird, thunderbirdBonus.BonusValue, location);
                case PodVehicleBonusCatalogDto podVehicleBonus:
                    var podVehicle = ParseEnum<PodVehicle>(podVehicleBonus.PodVehicle);
                    return new PodVehicleBonusCondition(podVehicle, podVehicleBonus.BonusValue, location);
                default:
                    throw new JsonException("Unknown bonus condition type");
            }
        }

        private static RewardOption MapReward(RewardOptionCatalogDto dto)
        {
            if (dto is null)
            {
                throw new JsonException("Reward option is null");
            }

            switch (dto)
            {
                case PlayerChoiceRewardCatalogDto:
                    return RewardOption.PlayerChoice();
                case TokenRewardCatalogDto specifiedToken:
                    var token = ParseEnum<BonusToken>(specifiedToken.Token);
                    return RewardOption.SpecifiedToken(token);
                default:
                    throw new JsonException("Unknown reward option type");
            }
        }

        private static TEnum ParseEnum<TEnum>(string value) where TEnum : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new JsonException("Enum value cannot be null or empty");
            }

            if (Enum.TryParse<TEnum>(value, ignoreCase: true,out var result))
            {
                return result;
            }

            throw new JsonException($"Invalid enum value: {value}");
        }

        private string Slug(string input)
        {
            var cleaned = input.Trim().ToLowerInvariant();

            var stringBuilder = new StringBuilder(cleaned.Length);

            bool dash = false;

            foreach (var ch in cleaned)
            {
                if (char.IsLetterOrDigit(ch)) 
                { 
                    stringBuilder.Append(ch); dash = false; 
                }
                else if (char.IsWhiteSpace(ch) || ch is '-' or '_' or '/' or '.') 
                { 
                    if (!dash) 
                    { 
                        stringBuilder.Append('-'); dash = true; } }
            }
            return stringBuilder.ToString().Trim('-');
        }
    }
}
