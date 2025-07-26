using AutoFixture;
using AutoFixture.Kernel;
using AutoMapper;
using NSubstitute;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Interfaces;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos;
using ThunderbirdsBoardGameEngine.GameData.Api.Services;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.UnitTests.Services
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
    }
}
