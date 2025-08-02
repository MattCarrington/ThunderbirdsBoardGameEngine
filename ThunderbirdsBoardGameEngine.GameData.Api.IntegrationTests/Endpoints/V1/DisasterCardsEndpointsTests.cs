using System.Net;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos.V1;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.IntegrationTests.Endpoints.V1
{
    public class DisasterCardsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private const int ApiVersion = 1;

        public DisasterCardsEndpointsTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact] 
        public async Task GetAllDisasterCards_ReturnsOkAndValidResponse() 
        {
            // Arrange
            
            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            Assert.NotNull(cards);
            Assert.Equal(9, cards.Count);
            Assert.Contains(cards, c => c.Name == "Target: Tiger One");

            foreach (var card in cards)
            {
                Assert.NotNull(card.BonusConditions);
                Assert.NotEmpty(card.BonusConditions);
                Assert.NotNull(card.Rewards);
                Assert.NotEmpty(card.Rewards);
            }            
        }

        [Fact] 
        public async Task GetDisasterCardById_ValidId_ReturnsOkAndExpectedData() 
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/DisasterCard/4");
            request.Headers.Add("X-API-Version", ApiVersion.ToString());

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            
            var card = JsonSerializer.Deserialize<DisasterCardDto>(json, _jsonOptions);

            Assert.NotNull(card);
            Assert.Equal(4, card.Id);
            Assert.NotNull(card.BonusConditions);
            Assert.NotEmpty(card.BonusConditions);
            Assert.NotNull(card.Rewards);
            Assert.NotEmpty(card.Rewards);
        }

        [Fact] 
        public async Task GetDisasterCardById_InvalidId_ReturnsNotFound()
        { 
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/DisasterCard/9999");
            request.Headers.Add("X-API-Version", "1");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            
            Assert.Contains("Disaster card with ID 9999 not found.", content);
        }

        [Fact] 
        public async Task GetDisasterCards_MissingApiVersionHeader_ReturnsBadRequest()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/DisasterCard");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();

            Assert.Contains("An API version is required, but was not specified.", content);
        }

        [Fact] 
        public async Task GetDisasterCards_InvalidApiVersionHeader_ReturnsBadRequest()
        { 
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/DisasterCard");
            request.Headers.Add("X-API-Version", "99999");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            
            Assert.Contains("does not support the API version '99999'", content);
        }

        [Fact]
        public async Task GetDisasterCards_ContainsCharacterBonusType()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card = GetCard(cards, "Bolt from the Blue");
            var characterBonuses = card.BonusConditions
                .Where(b => b.Description.Contains("John") || b.Description.Contains("Gordon"))
                .ToList();

            Assert.NotEmpty(characterBonuses); // At least one expected

            foreach (var bonus in characterBonuses)
            {
                Assert.False(string.IsNullOrWhiteSpace(bonus.Description));
                Assert.Matches(@"\(\+\d+\)", bonus.Description); // e.g., "(+2)"
            }
        }

        [Fact]
        public async Task GetDisasterCards_ContainsThunderbirdBonusType()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card = GetCard(cards, "Day of Disaster");
            var thunderbirdBonuses = card.BonusConditions
                .Where(b => b.Description.Contains("Thunderbird") || b.Description.Contains("FAB 1"))
                .ToList();

            foreach (var bonus in thunderbirdBonuses)
            {
                Assert.False(string.IsNullOrWhiteSpace(bonus.Description));
                Assert.Matches(@"\(\+\d+\)", bonus.Description); // e.g., "(+2)"
            }
        }

        [Fact]
        public async Task GetDisasterCards_ContainsPodVehicleBonusType()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card = GetCard(cards, "They Call Him Mr. X");
                        
            var podBonuses = card!.BonusConditions
                .Where(b => b.Description.Contains("Mobile Crane") || b.Description.Contains("Thunderizer") || b.Description.Contains("DOMO"))
                .ToList();

            foreach (var bonus in podBonuses)
            {
                Assert.False(string.IsNullOrWhiteSpace(bonus.Description));
                Assert.Matches(@"\(\+\d+\)", bonus.Description); // e.g., "(+2)"
            }
        }

        [Fact]
        public async Task GetDisasterCards_ContainsCardWithSingleBonusCondition()
        { 
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();
            
            // Assert
            var card = GetCard(cards, "Martian Invasion");
            
            Assert.NotNull(card.BonusConditions);
            
            var bonus = Assert.Single(card.BonusConditions);
            Assert.Matches(@"\(\+\d+\)", bonus.Description);
        }

            [Fact] 
        public async Task GetDisasterCards_ContainsLocationBasedBonusDescriptions() 
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card  = GetCard(cards, "Target: Tiger One");

            Assert.Contains(card.BonusConditions, b => b.Description.Contains("if in"));

        }

        [Fact]
        public async Task GetDisasterCards_WhenLocationIsGeoStationaryOrbit_DescriptionIncludesOnThunderbird5()
        {
            // Arrange

            //Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card = GetCard(cards, "Sunrise on Mars");

            Assert.Contains(card.BonusConditions, b => b.Description.Contains("if on Thunderbird 5"));

        }

        [Fact] 
        public async Task GetDisasterCards_AllBonusDescriptionsContainBonusValues()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            foreach (var card in cards)
            {
                Assert.NotNull(card.BonusConditions);
                Assert.NotEmpty(card.BonusConditions);
                
                foreach (var bonus in card.BonusConditions)
                {
                    Assert.False(string.IsNullOrWhiteSpace(bonus.Description), "Bonus description should not be null or empty");
                    Assert.Matches(@"\(\+\d+\)", bonus.Description); // e.g., "(+2)"
                }
            }
        }

        [Fact]
        public async Task GetDisasterCards_ContainsMultipleBonusTypesInOneCard()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card = GetCard(cards, "Terror in New York City");
            var descriptions = card!.BonusConditions.Select(b => b.Description).ToList();

            // Assert that it contains known labels
            Assert.Contains(descriptions, d => d.Contains("Firefly"));       // Pod Vehicle
            Assert.Contains(descriptions, d => d.Contains("Virgil"));        // Character
            Assert.Contains(descriptions, d => d.Contains("Thunderbird"));   // Thunderbird

            // Assert all have a bonus value format (+X)
            foreach (var desc in descriptions)
            {
                Assert.False(string.IsNullOrWhiteSpace(desc), "Bonus description should not be null or empty");
                Assert.Matches(@"\(\+\d+\)", desc); // e.g., "(+2)"
            }
        }

        [Fact]
        public async Task GetDisasterCards_ContainsSingleSpecifiedReward()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card = GetCard(cards, "The Longest Day");

            var reward = Assert.Single(card.Rewards);
            Assert.Equal("Logistics", reward.DisplayName);
        }

        [Fact]
        public async Task GetDisasterCards_ContainsSinglePlayerChoiceReward()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card = GetCard(cards, "Bolt from the Blue");

            var reward = Assert.Single(card.Rewards);
            Assert.Equal("Player Choice", reward.DisplayName);
        }

        [Fact]
        public async Task GetDisasterCards_ContainsMixedPlayerChoiceAndSpecifiedReward()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card = GetCard(cards, "Sunrise on Mars");

            Assert.NotNull(card.Rewards);
            Assert.Equal(2, card.Rewards.Count);
            Assert.Contains(card.Rewards, r => r.DisplayName == "Player Choice");
            Assert.Contains(card.Rewards, r => r.DisplayName == "Technology");
        }

        [Fact]
        public async Task GetDisasterCards_ContainsTwoSpecifiedRewards()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card = GetCard(cards, "Terror in New York City");

            Assert.NotNull(card.Rewards);
            Assert.Equal(2, card.Rewards.Count);
            Assert.Contains(card.Rewards, r => r.DisplayName == "Determination");
            Assert.Contains(card.Rewards, r => r.DisplayName == "Teamwork");
        }

        [Fact]
        public async Task GetDisasterCards_AllRewardsHaveDisplayNames()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            
            foreach (var card in cards)
            {
                Assert.NotNull(card.Rewards);
                Assert.NotEmpty(card.Rewards);

                foreach (var reward in card.Rewards)
                {
                    Assert.False(string.IsNullOrWhiteSpace(reward.DisplayName), "Reward display name should not be null or empty");
                }
            }
        }

        [Fact]
        public async Task GetDisasterCards_EachCardHasUniqueId()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            Assert.Distinct(cards.Select(c => c.Id));
        }

        private async Task<List<DisasterCardDto>> GetDisasterCardsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/DisasterCard");
            request.Headers.Add("X-API-Version", ApiVersion.ToString());

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<DisasterCardDto>>(json, _jsonOptions)!;
        }

        private static DisasterCardDto GetCard(List<DisasterCardDto> cards, string name)
        {
            var card = cards.FirstOrDefault(c => c.Name == name);
            
            Assert.NotNull(card);

            return card;
        }

    }
}
