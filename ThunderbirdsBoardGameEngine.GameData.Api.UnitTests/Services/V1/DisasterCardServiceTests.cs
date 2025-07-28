using AutoFixture;
using AutoFixture.Kernel;
using AutoMapper;
using NSubstitute;
using System.Threading.Tasks;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Interfaces;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos;
using ThunderbirdsBoardGameEngine.GameData.Api.Services.V1;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.UnitTests.Services.V1
{
    public class DisasterCardServiceTests
    {
        private readonly Fixture _fixture = new();
        private readonly IDisasterCardRepository _repository = Substitute.For<IDisasterCardRepository>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly DisasterCardService _service;

        public DisasterCardServiceTests()
        {
            _service = new DisasterCardService(_repository, _mapper);
            _fixture.Customizations.Add(new TypeRelay(typeof(Bonus), typeof(CharacterBonus)));
        }

        [Fact]
        public async Task GetDisasterCards_WhenDisasterCardsExist_ReturnsDisasterCardDtosAsync()
        {
            // Arrange
            var disasterCards = _fixture.CreateMany<DisasterCard>(5).ToList();
            
            var expectedDtos = disasterCards.Select(c => new DisasterCardDto
            {
                Id = c.Id,
                Name = c.Name,
                DifficultyNumber = c.DifficultyNumber,
                Location = c.Location.ToString(),
                RescueType = c.RescueType.ToString(),
                Bonuses = [],
                Rewards = []
            }).ToList();

            _repository.GetAllAsync().Returns(disasterCards);

            _mapper.Map<IReadOnlyList<DisasterCardDto>>(disasterCards).Returns(expectedDtos);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDtos.Count, result.Count);
            Assert.All(result, dto => Assert.IsType<DisasterCardDto>(dto));

            await _repository.Received(1).GetAllAsync();
            _mapper.Received(1).Map<IReadOnlyList<DisasterCardDto>>(disasterCards);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoDisasterCardsExist_ReturnsEmptyList()
        {
            // Arrange
            _repository.GetAllAsync().Returns(Array.Empty<DisasterCard>());

            _mapper.Map<IReadOnlyList<DisasterCardDto>>(Arg.Any<IReadOnlyList<DisasterCard>>())
                  .Returns(Array.Empty<DisasterCardDto>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            await _repository.Received(1).GetAllAsync();
            _mapper.Received(1).Map<IReadOnlyList<DisasterCardDto>>(Arg.Any<IReadOnlyList<DisasterCard>>());
        }

        [Fact]
        public async Task GetByIdAsync_WhenCardExists_ReturnsDisasterCardDto()
        {
            // Arrange
            var disasterCard = _fixture.Create<DisasterCard>();

            var expectedDto = new DisasterCardDto
            {
                Id = disasterCard.Id,
                Name = disasterCard.Name,
                DifficultyNumber = disasterCard.DifficultyNumber,
                Location = disasterCard.Location.ToString(),
                RescueType = disasterCard.RescueType.ToString(),
                Bonuses = [],
                Rewards = []
            };

            _repository.GetByIdAsync(disasterCard.Id).Returns(disasterCard);

            _mapper.Map<DisasterCardDto>(disasterCard).Returns(expectedDto);

            // Act
            DisasterCardDto result = await _service.GetByIdAsync(disasterCard.Id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DisasterCardDto>(result);

            await _repository.Received(1).GetByIdAsync(disasterCard.Id);
            _mapper.Received(1).Map<DisasterCardDto>(disasterCard);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCardDoesNotExist_ReturnsNull()
        {
            // Arrange
            int nonExistentId = 999;

            _repository.GetByIdAsync(nonExistentId).Returns((DisasterCard)null);

            _mapper.Map<DisasterCardDto>(Arg.Any<DisasterCard>()).Returns((DisasterCardDto)null);

            // Act
            var result = await _service.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
            await _repository.Received(1).GetByIdAsync(nonExistentId);
        }
    }
}
