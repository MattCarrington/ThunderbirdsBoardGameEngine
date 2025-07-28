using AutoMapper;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos;
using ThunderbirdsBoardGameEngine.Serialization.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Profiles
{
    public class DisasterCardProfile : Profile
    {
        public DisasterCardProfile()
        {
            CreateMap<DisasterCard, DisasterCardDto>()
                .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => src.RewardOptions))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => EnumDisplayHelper.GetDisplayName(src.Location)));
                
            CreateMap<Bonus, BonusDto>()
                .ConvertUsing((bonus, context) =>
                {
                    var displayName = bonus switch
                    {
                        CharacterBonus cb => EnumDisplayHelper.GetDisplayName(cb.Character),
                        ThunderbirdBonus tb => EnumDisplayHelper.GetDisplayName(tb.Thunderbird),
                        PodVehicleBonus pvb => EnumDisplayHelper.GetDisplayName(pvb.PodVehicle),
                        _ => throw new InvalidOperationException("Unknown bonus type")
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


                    var dto = new BonusDto
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
                            ? "User Choice"
                            : reward.SpecifiedToken!.Value.ToString() // Safe because of guard
                    };
                });
        }
    }
}
