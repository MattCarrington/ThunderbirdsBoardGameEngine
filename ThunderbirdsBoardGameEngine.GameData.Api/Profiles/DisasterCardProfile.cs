using AutoMapper; 
using ThunderbirdsBoardGameEngine.GameData.Api.Entities;
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

                    if (reward.IsUserChoice)
                    {
                        dto.DisplayName = "User Choice";
                    }
                    else
                    {
                        var token = reward.BonusTokenChoices.FirstOrDefault();
                        dto.DisplayName = token.ToString();
                    }

                    return dto;
                });
        }
    }
}
