using AutoMapper;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Profiles
{
    public class DisasterCardProfile : Profile
    {
        public DisasterCardProfile()
        {
            CreateMap<DisasterCard, DisasterCardDto>()
                .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => src.RewardOptions));

            CreateMap<Bonus, BonusDto>()
                .ConvertUsing((bonus, context) =>
                {
                    var dto = new BonusDto
                    {
                        BonusValue = bonus.BonusValue,
                        Location = bonus.Location?.ToString(),
                        DisplayName = bonus switch
                        {
                            CharacterBonus cb => cb.Character.ToString(),
                            ThunderbirdBonus tb => tb.Thunderbird.ToString(),
                            PodVehicleBonus pvb => pvb.PodVehicle.ToString(),
                            _ => throw new InvalidOperationException("Unknown bonus type")
                        }
                    };

                    return dto;
                });

            CreateMap<RewardOption, RewardDto>()
                .ConvertUsing((reward, context) =>
                {
                    var dto = new RewardDto();

                    dto.DisplayName = reward.IsUserChoice
                        ? "User Choice"
                        : reward.SpecifiedToken?.ToString() ?? throw new InvalidOperationException("SpecifiedToken must be set for non-user-choice rewards");

                    return dto;
                });
        }
    }
}
