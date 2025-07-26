using AutoMapper;
using ThunderbirdsBoardGameEngine.GameData.Api.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Interfaces;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Services
{
    public class DisasterCardService
    {
        private readonly IDisasterCardRepository _disasterCardRepository;
        private readonly IMapper _mapper;

        public DisasterCardService(IDisasterCardRepository disasterCardRepository, IMapper mapper)
        {
            _disasterCardRepository = disasterCardRepository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<DisasterCardDto>> GetAllAsync()
        {
            var cards = await _disasterCardRepository.GetAllAsync();

            return _mapper.Map<IReadOnlyList<DisasterCardDto>>(cards);
        }
    }
}
