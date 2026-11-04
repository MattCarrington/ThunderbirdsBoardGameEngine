using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Api.ComponentTests.Factories;
using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.TransferCharacters;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints.GameState.V1
{
    public class TransferCharactersTests : IClassFixture<ApiComponentWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly IGameRepository _repository;

        private const int ApiVersion = 1;

        public TransferCharactersTests(ApiComponentWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
            _repository = factory.Repository;
        }

        [Fact]
        public async Task ReturnsSuccessWhenValidSingleTransfer()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var characterCode = KnownCharacterCodes.Alan;
            var thunderbirdCode = KnownThunderbirdCodes.Thunderbird1;

            var dto = new TransferCharactersRequestDto
            {
                Assignments =
                [
                    new CharacterTransferAssignmentDto
                    {
                        CharacterCode = characterCode.Value,
                        DestinationThunderbirdCode = thunderbirdCode.Value
                    }
                ]
            };

            var route = GameStateRoutes.TransferCharacters(gameId);

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GameStateResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.NotNull(result.ThunderbirdMachines);

            AssertCharacterIsInThunderbird(result, characterCode.Value, thunderbirdCode.Value);
        }

        [Fact]
        public async Task ReturnsSuccessWhenValidReciprocalSwap()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var firstCharacterCode = KnownCharacterCodes.Scott;
            var firstThunderbirdCode = KnownThunderbirdCodes.Thunderbird1;
            var secondCharacterCode = KnownCharacterCodes.Gordon;
            var secondThunderbirdCode = KnownThunderbirdCodes.Thunderbird4;

            var dto = new TransferCharactersRequestDto
            {
                Assignments =
                [
                    new CharacterTransferAssignmentDto
                    {
                        CharacterCode = firstCharacterCode.Value,
                        DestinationThunderbirdCode = secondThunderbirdCode.Value
                    },
                    new CharacterTransferAssignmentDto
                    {
                        CharacterCode = secondCharacterCode.Value,
                        DestinationThunderbirdCode = firstThunderbirdCode.Value
                    }
                ]
            };

            var route = GameStateRoutes.TransferCharacters(gameId);

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GameStateResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.NotNull(result.ThunderbirdMachines);

            AssertCharacterIsInThunderbird(result, firstCharacterCode.Value, secondThunderbirdCode.Value);
            AssertCharacterIsInThunderbird(result, secondCharacterCode.Value, firstThunderbirdCode.Value);
        }

        [Fact]
        public async Task ReturnsUnprocessableEntityWhenCharacterTransferRejected()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var dto = new TransferCharactersRequestDto
            {
                Assignments =
                [
                    new CharacterTransferAssignmentDto
                    {
                        CharacterCode = KnownCharacterCodes.John.Value,
                        DestinationThunderbirdCode = KnownThunderbirdCodes.Thunderbird1.Value
                    }
                ]
            };

            var route = GameStateRoutes.TransferCharacters(gameId);

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);

            var reason = Assert.IsType<JsonElement>(problem.Extensions["reason"]);
            Assert.Equal("machines-not-co-located", reason.GetString());
        }

        [Fact]
        public async Task ReturnsBadRequestWhenDuplicateCharacters()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var duplicateCharacterCode = KnownCharacterCodes.Scott;

            var dto = new TransferCharactersRequestDto
            {
                Assignments =
                [
                    new CharacterTransferAssignmentDto
                    {
                        CharacterCode = duplicateCharacterCode.Value,
                        DestinationThunderbirdCode = KnownThunderbirdCodes.Thunderbird1.Value
                    },
                    new CharacterTransferAssignmentDto
                    {
                        CharacterCode = duplicateCharacterCode.Value,
                        DestinationThunderbirdCode = KnownThunderbirdCodes.Thunderbird2.Value
                    }
                ]
            };

            var route = GameStateRoutes.TransferCharacters(gameId);

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Invalid character transfer request.");
        }

        [Fact]
        public async Task ReturnsBadRequestWhenNoAssignments()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var dto = new TransferCharactersRequestDto
            {
                Assignments = []
            };

            var route = GameStateRoutes.TransferCharacters(gameId);

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Request validation failed.");
        }

        [Fact]
        public async Task ReturnsBadRequestWhenTooManyAssignments()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var dto = new TransferCharactersRequestDto
            {
                Assignments =
                [
                    new CharacterTransferAssignmentDto
                    {
                        CharacterCode = KnownCharacterCodes.Virgil.Value,
                        DestinationThunderbirdCode = KnownThunderbirdCodes.Thunderbird1.Value
                    },
                    new CharacterTransferAssignmentDto
                    {
                        CharacterCode = KnownCharacterCodes.Gordon.Value,
                        DestinationThunderbirdCode = KnownThunderbirdCodes.Thunderbird2.Value
                    },
                    new CharacterTransferAssignmentDto
                    {
                        CharacterCode = KnownCharacterCodes.Alan.Value,
                        DestinationThunderbirdCode = KnownThunderbirdCodes.Thunderbird4.Value
                    },
                ]
            };

            var route = GameStateRoutes.TransferCharacters(gameId);

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Request validation failed.");
        }

        [Fact]
        public async Task ReturnsBadRequestWhenCharacterMissingFromAssignment()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var dto = new
            {
                assignments = new[]
                {
                    new
                    {
                        destinationThunderbirdCode = "thunderbird-1"
                    }
                }
            };

            var route = GameStateRoutes.TransferCharacters(gameId);

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Request validation failed.");
        }

        [Fact]
        public async Task ReturnsBadRequestWhenDestinationThunderbirdMissingFromAssignment()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var dto = new
            {
                assignments = new[]
                {
                    new
                    {
                        characterCode = "alan"
                    }
                }
            };

            var route = GameStateRoutes.TransferCharacters(gameId);

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Request validation failed.");
        }

        [Fact]
        public async Task ReturnsNotFoundWhenGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var dto = new TransferCharactersRequestDto
            {
                Assignments =
                [
                    new CharacterTransferAssignmentDto
                    {
                        CharacterCode = KnownCharacterCodes.Alan.Value,
                        DestinationThunderbirdCode = KnownThunderbirdCodes.Thunderbird1.Value
                    }
                ]
            };

            var route = GameStateRoutes.TransferCharacters(gameId);

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Game not found.");
        }

        [Fact]
        public async Task ReturnsNotFoundWhenThunderbirdMachineDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var dto = new TransferCharactersRequestDto
            {
                Assignments =
                [
                    new CharacterTransferAssignmentDto
                    {
                        CharacterCode = KnownCharacterCodes.Alan.Value,
                        DestinationThunderbirdCode = "non-existent-thunderbird"
                    }
                ]
            };

            var route = GameStateRoutes.TransferCharacters(gameId);

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Thunderbird Machine was not found.");
        }

        [Fact]
        public async Task ReturnsNotFoundWhenCharacterDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);
            var dto = new TransferCharactersRequestDto

            {
                Assignments =
                [
                    new CharacterTransferAssignmentDto
                    {
                        CharacterCode = "non-existent-character",
                        DestinationThunderbirdCode = KnownThunderbirdCodes.Thunderbird1.Value
                    }
                ]
            };

            var route = GameStateRoutes.TransferCharacters(gameId);

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Character was not found.");
        }

        private static void AssertCharacterIsInThunderbird(GameStateResponseDto gameState, string characterCode, string thunderbirdCode)
        {
            var thunderbird = gameState.ThunderbirdMachines.SingleOrDefault(tm => tm.ThunderbirdCode == thunderbirdCode);
            Assert.NotNull(thunderbird);
            Assert.Contains(thunderbird.OccupantCharacterCodes, c => c == characterCode);
        }
    }
}
