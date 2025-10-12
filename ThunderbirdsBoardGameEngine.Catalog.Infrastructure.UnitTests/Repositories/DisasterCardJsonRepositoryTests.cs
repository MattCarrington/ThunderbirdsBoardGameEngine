using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Repositories;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Builders;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Fakes;
using ThunderbirdsBoardGameEngine.TestUtils;
using ThunderbirdsBoardGameEngine.TestUtils.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Repositories
{
    public class DisasterCardJsonRepositoryTests
    {
        private const string TestPath = "/cards.json";
        
        [Fact]
        public async Task GetAllAsync_WhenValidData_ReturnsDisasterCardsAsync()
        {
            // Arrange
            var disasterCards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(1).WithName("Test Disaster 1").WithLocation(BoardLocation.NorthAtlantic).WithRescueType(RescueType.Land).WithDifficulty(5).Build(),
                new DisasterCardBuilder().WithId(2).WithName("Test Disaster 2").WithLocation(BoardLocation.SouthPacific).WithRescueType(RescueType.Space).WithDifficulty(7).Build(),
                new DisasterCardBuilder().WithId(3).WithName("Test Disaster 3").WithLocation(BoardLocation.Africa).WithRescueType(RescueType.Air).WithDifficulty(9).Build()
            };

            var jsonText = SerializeDisasterCardData(disasterCards);

            var repository = new DisasterCardJsonRepositoryBuilder().WithJson(jsonText).Build();

            // Act
            var result = await repository.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(disasterCards.Count, result.Count);
        }

        [Theory]
        [InlineData("[]")]
        [InlineData("null")]
        public async Task GetAllAsync_WhenNoData_ReturnsEmptyListAsync(string data)
        {
            // Arrange
            var logger = Substitute.For<ILogger<DisasterCardJsonRepository>>();

            var repository = new DisasterCardJsonRepositoryBuilder().WithJson(data).WithLogger(logger).WithFilePath(TestPath).Build();

            // Act
            var result = await repository.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            logger.Received(1).Log(
               LogLevel.Warning,
               Arg.Any<EventId>(),
               Arg.Is<object>(s => HasLogState(
                   s,
                   "No Disaster Cards found in the file {Path}. Returning empty list.",
                   new ValueTuple<string, string>("Path", TestPath))),
               Arg.Any<Exception>(),
               Arg.Any<Func<object, Exception, string>>());
        }

        [Fact]
        public async Task GetAllAsync_WhenEmptyString_ThrowsJsonException()
        {
            // Arrange
            var repository = new DisasterCardJsonRepositoryBuilder().WithJson(string.Empty).Build();

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => repository.GetAllAsync(CancellationToken.None));
        }

        [Theory]
        [InlineData("{ invalid json }")]
        [InlineData("   \r\n\t  ")]
        [InlineData("This is not JSON")]
        [InlineData("")]
        [InlineData("    ")]
        public async Task GetAllAsync_WhenInvalidJson_ThrowsJsonException(string invalidJson)
        {
            // Arrange
            var repository = new DisasterCardJsonRepositoryBuilder().WithJson(invalidJson).Build();

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => repository.GetAllAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAsync_WhenBonusTypeMissing_ThrowsJsonException()
        {
            // Arrange
            var card = new List<DisasterCard> 
            { 
                new DisasterCardBuilder().WithId(1).Build(),
                new DisasterCardBuilder().WithId(2).Build()
            };

            var json = SerializeDisasterCardData(card);

            var missingType = json.Replace("\"type\":", "\"typ\":", StringComparison.Ordinal); // valid JSON, wrong key

            var repo = new DisasterCardJsonRepositoryBuilder().WithJson(missingType).Build();

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => repo.GetAllAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAsync_WhenInvalidDisasterCards_ThrowsDisasterCardValidationException()
        {
            // Arrange
            var disasterCards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(1).WithNullBonusConditions().WithNullRewards().Build(),
                new DisasterCardBuilder().WithId(2).WithName(string.Empty).WithLocation(BoardLocation.SouthPacific).WithRescueType(RescueType.Space).WithDifficulty(7).Build(), // Invalid: Name is empty
                new DisasterCardBuilder().WithId(3).WithName("Another Valid Disaster").WithLocation(BoardLocation.Africa).WithRescueType(RescueType.Air).WithDifficulty(-9).Build()
            };

            var jsonText = SerializeDisasterCardData(disasterCards);

            var repository = new DisasterCardJsonRepositoryBuilder().WithJson(jsonText).Build();

            // Act & Assert
            await Assert.ThrowsAsync<DisasterCardValidationException>(() => repository.GetAllAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAsync_WhenCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var files = new FakeFileReader().AddCanceled(TestPath);

            var repository = new DisasterCardJsonRepositoryBuilder().WithFileReader(files).WithFilePath(TestPath).Build();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => repository.GetAllAsync(new CancellationToken(canceled: true)));
        }

        private static bool HasLogState(object state, string template, params (string Key, string Value)[] props)
        {
            if (state is IEnumerable<KeyValuePair<string, object>> kvps)
            {
                var dict = kvps.ToDictionary(k => k.Key, k => k.Value?.ToString() ?? "");
                if (!dict.TryGetValue("{OriginalFormat}", out var fmt) || fmt != template) return false;
                return props.All(p => dict.TryGetValue(p.Key, out var v) && v == p.Value);
            }
            return false;
        }

        private static string SerializeDisasterCardData(IList<DisasterCard> disasterCards)
        {
            return JsonSerializer.Serialize(disasterCards, JsonDefaults.DisasterCards);
        }
    }    
}
