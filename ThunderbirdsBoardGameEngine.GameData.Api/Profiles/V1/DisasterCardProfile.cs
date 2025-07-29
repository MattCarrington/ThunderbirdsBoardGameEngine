using AutoMapper;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos;
using ThunderbirdsBoardGameEngine.Serialization.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Profiles.V1
{
    public class DisasterCardProfile : Profile
    {
        public DisasterCardProfile()
        {
            CreateMap<DisasterCard, DisasterCardDto>()
                .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => src.RewardOptions))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => EnumDisplayHelper.GetDisplayName(src.Location)));
                
            CreateMap<BonusCondition, BonusConditionDto>()
                .ConvertUsing((bonus, context) =>
                {
                    var displayName = bonus switch
                    {
                        CharacterBonusCondition cb => EnumDisplayHelper.GetDisplayName(cb.Character),
                        ThunderbirdBonusCondition tb => EnumDisplayHelper.GetDisplayName(tb.Thunderbird),
                        PodVehicleBonusCondition pvb => EnumDisplayHelper.GetDisplayName(pvb.PodVehicle),
                        _ => throw new InvalidOperationException("Unknown bonus condition")
                    };

                    var locationText = bonus.Location switch
                    {
                        BoardLocation.GeoStationaryOrbit => "on Thunderbird 5",
                        BoardLocation loc => $"in {EnumDisplayHelper.GetDisplayName(loc)}",
                        _ => null
                    };

                    var description = locationText == null
                        ? $"{displayName} (+{bonus.BonusValue})"
                        : $"{displayName} (+{bonus.BonusValue}) (if {locationText})";


                    var dto = new BonusConditionDto
                    {
                        Description = description
                    };

                    return dto;
                });

            CreateMap<RewardOption, RewardDto>()
                .ConvertUsing((reward, context) =>
                {
                    if (!reward.IsUserChoice && reward.SpecifiedToken == null)
                    {
                        throw new InvalidOperationException("SpecifiedToken must be set for non-user-choice rewards");
                    }

                    return new RewardDto
                    {
                        DisplayName = reward.IsUserChoice
                            ? "Player Choice"
                            : reward.SpecifiedToken!.Value.ToString() // Safe because of guard
                    };
                });
        }
    }
}
