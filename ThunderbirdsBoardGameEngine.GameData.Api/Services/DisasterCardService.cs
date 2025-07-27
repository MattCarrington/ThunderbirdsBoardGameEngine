using AutoMapper;
using ThunderbirdsBoardGameEngine.GameData.Api.Interfaces;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Services
{
    public class DisasterCardService : IDisasterCardService
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

        public async Task<DisasterCardDto> GetByIdAsync(int id)
        {
            var card = await _disasterCardRepository.GetByIdAsync(id);

            return _mapper.Map<DisasterCardDto>(card);
        }
    }
}
