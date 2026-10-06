using Bunit;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Components;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.GameDashboard.Components
{
    public class ThunderbirdMachinesSegmentTests : BunitContext
    {
        [Fact]
        public void DisplaysMachinesLocationsAndOccupants()
        {
            var machines = new[]
            {
                new ThunderbirdMachinesDashboardViewModel(
                    ThunderbirdCode: "thunderbird-1",
                    ThunderbirdDisplayName: "Thunderbird 1",
                    LocationCode: "south-pacific",
                    LocationDisplayName: "South Pacific",
                    Occupants:
                    [
                        new(
                            CharacterCode: "scott",
                            CharacterDisplayName: "Scott"),
                        new(
                            CharacterCode: "alan",
                            CharacterDisplayName: "Alan")
                    ]),
                new ThunderbirdMachinesDashboardViewModel(
                    ThunderbirdCode: "thunderbird-2",
                    ThunderbirdDisplayName: "Thunderbird 2",
                    LocationCode: "europe",
                    LocationDisplayName: "Europe",
                    Occupants: [])
            };

            var cut = Render<ThunderbirdMachinesSegment>(
                parameters => parameters
                    .Add(component => component.Machines, machines));

            var rows = cut.FindAll("tbody tr");

            Assert.Collection(
                rows,
                first =>
                {
                    Assert.Contains("Thunderbird 1", first.TextContent);
                    Assert.Contains("South Pacific", first.TextContent);
                    Assert.Contains("Scott, Alan", first.TextContent);
                },
                second =>
                {
                    Assert.Contains("Thunderbird 2", second.TextContent);
                    Assert.Contains("Europe", second.TextContent);
                    Assert.Contains("Empty", second.TextContent);
                });
        }
    }
}
