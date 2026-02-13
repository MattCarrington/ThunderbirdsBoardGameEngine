using NSubstitute;
using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Client.Infrastructure;
using ThunderbirdsBoardGameEngine.UI.Services;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.UnitTests.Services
{
    public class CharactersServiceTests
    {
        [Fact]
        public async Task GetAllAsync_WhenResponseIsSuccessful_ReturnsListOfCharactersAsync()
        {
            // Arrange
            var dto = new List<CharacterDto>
            {
                new() { Key = "scott", DisplayName = "Scott" },
                new() { Key = "lady-penelope", DisplayName = "Lady Penelope" }
            };

            var response = ApiResult<IReadOnlyList<CharacterDto>>.SuccessResult(dto, HttpStatusCode.OK);

            var service = CreateService(response);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.NotNull(result);

            var characters = Assert.IsType<List<CharacterDto>>(result);
            Assert.Same(dto, characters);
        }

        [Fact]
        public async Task GetAllAsync_WhenClientReturnsNullData_ReturnsEmptyList()
        {
            // Arrange
            var response = ApiResult<IReadOnlyList<CharacterDto>>.SuccessResult(null, HttpStatusCode.OK);

            var service = CreateService(response);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_WhenResponseIsFailure_ReturnsEmptyList()
        {
            // Arrange
            var response = ApiResult<IReadOnlyList<CharacterDto>>.Failure("Error occurred", HttpStatusCode.InternalServerError);

            var service = CreateService(response);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        private static CharactersService CreateService(ApiResult<IReadOnlyList<CharacterDto>> response)
        {
            var client = Substitute.For<ICharactersClient>();
            client.GetAllAsync().Returns(response);

            return new CharactersService(client);
        }
    }
}
