using FluentAssertions;
using System.Net;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos.V1;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.IntegrationTests.EndpointTests.V1
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
            cards.Should().NotBeNull();
            cards.Count.Should().Be(9);
            cards.Should().Contain(c => c.Name == "Target: Tiger One");
            
            foreach (var card in cards)
            {
                card.BonusConditions.Should().NotBeNullOrEmpty();
                card.Rewards.Should().NotBeNullOrEmpty();
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
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await response.Content.ReadAsStringAsync();
            
            var card = JsonSerializer.Deserialize<DisasterCardDto>(json, _jsonOptions);

            card.Should().NotBeNull();
            card.BonusConditions.Should().NotBeNullOrEmpty();
            card.Rewards.Should().NotBeNullOrEmpty();            
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
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Disaster card with ID 9999 not found.");
        }

        [Fact] 
        public async Task GetDisasterCards_MissingApiVersionHeader_ReturnsBadRequest()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/DisasterCard");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("An API version is required, but was not specified.");
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
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("does not support the API version '99999'");
        }

        [Fact]
        public async Task GetDisasterCards_ContainsCharacterBonusType()
        {
            // Arrange


            // Act
            var cards = await GetDisasterCardsAsync();
            
            // Assert
            var card = GetCard(cards, "Bolt from the Blue");
            var characterBonuses = card.BonusConditions.Where(b => b.Description.Contains("John") || b.Description.Contains("Gordon"));

            characterBonuses.Should().NotBeEmpty("Character bonuses should be present in this card");
            characterBonuses.Should().AllSatisfy(b => b.Description.Should().MatchRegex(@"\(\+\d+\)"), "Each character bonus should include a bonus value");
        }

        [Fact]
        public async Task GetDisasterCards_ContainsThunderbirdBonusType()
        {
            // Arrange


            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card = GetCard(cards, "Day of Disaster");
            var thunderbirdBonuses = card!.BonusConditions.Where(b => b.Description.Contains("Thunderbird") || b.Description.Contains("FAB 1"));

            thunderbirdBonuses.Should().NotBeEmpty("Thunderbird bonuses should be present in this card");
            thunderbirdBonuses.Should().AllSatisfy(b => b.Description.Should().MatchRegex(@"\(\+\d+\)"), "Each Thunderbird bonus should include a bonus value");
        }

        [Fact]
        public async Task GetDisasterCards_ContainsPodVehicleBonusType()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card = GetCard(cards, "They Call Him Mr. X");
                        
            var podBonuses = card!.BonusConditions.Where(b =>
                b.Description.Contains("Mobile Crane") ||
                b.Description.Contains("Thunderizer") ||
                b.Description.Contains("DOMO"));

            podBonuses.Should().NotBeEmpty("Pod vehicle bonuses should be present in this card");
            podBonuses.Should().AllSatisfy(b => b.Description.Should().MatchRegex(@"\(\+\d+\)"), "Each pod vehicle bonus should include a bonus value");
        }

        [Fact]
        public async Task GetDisasterCards_ContainsCardWithSingleBonusCondition()
        { 
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();
            
            // Assert
            var card = GetCard(cards, "Martian Invasion");
            card!.BonusConditions.Should().HaveCount(1, "This card should only have one bonus condition.");
            card.BonusConditions[0].Description.Should().MatchRegex(@"\(\+\d+\)", "Bonus description should include a bonus value.");
        }

            [Fact] 
        public async Task GetDisasterCards_ContainsLocationBasedBonusDescriptions() 
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card  = GetCard(cards, "Target: Tiger One");

            card.BonusConditions.Should().Contain(b =>
                b.Description.Contains("if in"));
        }

        [Fact]
        public async Task GetDisasterCards_WhenLocationIsGeoStationaryOrbit_DescriptionIncludesOnThunderbird5()
        {
            // Arrange

            //Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card = GetCard(cards, "Sunrise on Mars");
            
            card.BonusConditions.Should().Contain(b =>
                b.Description.Contains("on Thunderbird 5"));
        }

        [Fact] 
        public async Task GetDisasterCards_AllBonusDescriptionsContainBonusValues()
        {
            // Arrange


            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var allDescriptions = cards
                .SelectMany(c => c.BonusConditions)
                .Select(b => b.Description)
                .ToList();

            allDescriptions.Should().NotBeEmpty("every disaster card should have at least one bonus");

            allDescriptions.Should().AllSatisfy(description =>
            {
                description.Should().NotBeNullOrWhiteSpace("each bonus should have a description");
                description.Should().MatchRegex(@"\(\+\d+\)", "description should include a bonus value like (+2)");
            });
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

            // Check that we have all three types by looking for known labels
            descriptions.Should().Contain(d => d.Contains("Firefly"), "should contain a Pod Vehicle bonus");
            descriptions.Should().Contain(d => d.Contains("Virgil"), "should contain a Character bonus");
            descriptions.Should().Contain(d => d.Contains("Thunderbird"), "should contain a Thunderbird bonus");

            // Optional: check each description includes a bonus value
            descriptions.Should().AllSatisfy(d =>
                d.Should().MatchRegex(@"\(\+\d+\)"), "each bonus should include a value like (+2)");
        }

        [Fact]
        public async Task GetDisasterCards_ContainsSingleSpecifiedReward()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card = GetCard(cards, "The Longest Day");

            card!.Rewards.Should().HaveCount(1);
            card.Rewards[0].DisplayName.Should().Be("Logistics");
        }

        [Fact]
        public async Task GetDisasterCards_ContainsSinglePlayerChoiceReward()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card = GetCard(cards, "Bolt from the Blue");

            card!.Rewards.Should().HaveCount(1);
            card.Rewards[0].DisplayName.Should().Be("Player Choice");
        }

        [Fact]
        public async Task GetDisasterCards_ContainsMixedPlayerChoiceAndSpecifiedReward()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card = GetCard(cards, "Sunrise on Mars");

            card!.Rewards.Should().Contain(r => r.DisplayName == "Player Choice");
            card.Rewards.Should().Contain(r => r.DisplayName == "Technology");
        }

        [Fact]
        public async Task GetDisasterCards_ContainsTwoSpecifiedRewards()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var card = GetCard(cards, "Terror in New York City");

            card!.Rewards.Should().HaveCount(2);
            card.Rewards.Should().Contain(r => r.DisplayName == "Determination");
            card.Rewards.Should().Contain(r => r.DisplayName == "Teamwork");
        }

        [Fact]
        public async Task GetDisasterCards_AllRewardsHaveDisplayNames()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            var allRewards = cards.SelectMany(c => c.Rewards).ToList();

            allRewards.Should().NotBeEmpty("each card should have at least one reward");
            allRewards.Should().AllSatisfy(r =>
                r.DisplayName.Should().NotBeNullOrWhiteSpace("each reward should have a DisplayName"));
        }

        [Fact]
        public async Task GetDisasterCards_EachCardHasUniqueId()
        {
            // Arrange

            // Act
            var cards = await GetDisasterCardsAsync();

            // Assert
            cards.Select(c => c.Id).Should().OnlyHaveUniqueItems("each card should have a unique ID");
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
            card.Should().NotBeNull($"Card '{name}' should exist in dataset.");
            return card!;
        }

    }
}
