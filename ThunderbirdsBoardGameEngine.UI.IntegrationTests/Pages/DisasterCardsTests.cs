using Bunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Client;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Catalog.WireMock;
using ThunderbirdsBoardGameEngine.TestUtils;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using ThunderbirdsBoardGameEngine.UI.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Pages;
using ThunderbirdsBoardGameEngine.UI.Services;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.IntegrationTests.Pages
{
    public class DisasterCardsTests : TestContext, IAsyncLifetime
    {
        private WireMockHost _host;

        private readonly IReadOnlyList<DisasterCardDto> _cards = TestDataLoader.LoadJsonFromFile<IReadOnlyList<DisasterCardDto>>("disaster-card-dto-data.json", TestDataConstants.V1InputFolder);

        public Task InitializeAsync()
        {
            _host = new WireMockHost();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "GameDataClient:BaseAddress", _host.Url }
                })
                .Build();

            Services.AddGameDataClients(configuration);
            Services.AddSingleton<IDisasterCardService, DisasterCardService>();

            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await _host.DisposeAsync();
        }

        [Fact]
        public void Render_WhenCardsExist_CardsExist()
        {
            // Arrange      
            _host.Reset();

            _host.DisasterCardStub.RegisterMissingHeaderGuard();
            _host.DisasterCardStub.RegisterIncorrectHeaderGuard();
            _host.DisasterCardStub.RegisterGetAllSuccess(_cards);

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Await
            cut.WaitForElement("#disasterSelect");

            // Assert
            var result = cut
                .FindAll("#disasterSelect option")
                .Select(o => o.TextContent.Trim())
                .ToList();

            Assert.Equal(_cards.Count + 1, result.Count);
        }

        [Fact]
        public void Render_WhenNoCardExist_DisplaysEmptyState()
        {
            // Arrange
            _host.Reset();

            _host.DisasterCardStub.RegisterMissingHeaderGuard();
            _host.DisasterCardStub.RegisterIncorrectHeaderGuard();
            _host.DisasterCardStub.RegisterGetAllSuccess(new List<DisasterCardDto>());

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Await
            cut.WaitForElement("[data-testid=empty-state]");

            // Assert
            Assert.Empty(cut.FindAll("#disasterSelect"));
            Assert.DoesNotContain("Disaster Card Details", cut.Markup);

        }

        [Fact]
        public void Render_WhenErrorOccurs_DisplaysEmptyState()
        {
            // Arrange
            _host.Reset();

            _host.DisasterCardStub.RegisterMissingHeaderGuard();
            _host.DisasterCardStub.RegisterIncorrectHeaderGuard();
            _host.DisasterCardStub.RegisterGetAllError(HttpStatusCode.InternalServerError, "An error has occurred");

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Await
            cut.WaitForElement("[data-testid=empty-state]");

            // Assert
            Assert.Empty(cut.FindAll("#disasterSelect"));
            Assert.DoesNotContain("Disaster Card Details", cut.Markup);
        }
    }
}

// Must-have (ship these first)
//1) Load – Happy path

//Given /api/v1/disaster-cards → 200 with two cards (full fields).

//When component renders.

//Then placeholder + options appear; selecting a card shows details (name, difficulty, location, rescue type, lists for bonus/rewards).

//2) Load – Empty list

//Given 200 with [].

//When component renders.

//Then “No disaster cards available.” is shown; no <select>, no details.

//3) Load – Server error (soft-fail)

//Given 500.

//When component renders.

//Then UI behaves like empty (current contract): shows “No disaster cards available.”, no dropdown/details.

//4) Loading indicator

//Given delayed 200 (e.g., 400ms).

//When component renders.

//Then “Loading cards…” visible before response, disappears after options render.

//5) Selection – With/without conditional sections

//Given 200 with:

//Card A: has bonus+rewards.

//Card B: empty arrays (or nulls).

//When select A then B.

//Then details update; headers/lists appear for A, are hidden for B.

//6) No refetch on selection

//Given 200 with list.

//When select several options (including blank).

//Then WireMock shows exactly one GET for /api/v1/disaster-cards.

//7) Sorting – Case-insensitive

//Given 200 with names in unsorted order, mixed case (e.g., “beta”, “Alpha”, “Gamma”).

//When render.

//Then options (after placeholder) are A–Z ignoring case.

//Should-have (next pass)
//8) Null name handling

//Given 200 with some name=null.

//When render.

//Then no crash; define expected ordering (e.g., nulls sorted as empty string) and assert.

//9) Duplicate IDs – “keep first”

//Given 200 with two items sharing id=1 but different bodies.

//When render & select “1”.

//Then the UI shows the first one (matches your dictionary TryAdd behavior).

//10) Large content safety

//Given very long name/location strings (several hundred chars).

//When render & select.

//Then component still renders; no exceptions; strings present (you don’t need to assert CSS here).

//11) XSS safety (basic)

//Given name="<script>alert(1)</script>".

//When render & select.

//Then markup contains encoded text (&lt;script&gt;…) and not a <script> element.

//12) Timeout behavior (soft-fail as empty)

//Given delayed response longer than HttpClient.Timeout.

//When render.

//Then after timeout, behaves like empty state (today’s contract).

//Nice-to-have (edge/corner coverage)
//13) Unexpected JSON shape (malformed)

//Given 200 with invalid JSON or wrong casing (if your SDK isn’t tolerant).

//When render.

//Then soft-fail → empty. (Useful if the SDK maps deserialization failure to Success=false.)

//14) Partial nulls in nested collections

//Given a card with bonusConditions=[ null, { description: "…" } ] or items missing fields.

//When select.

//Then UI doesn’t throw; skips nulls; renders present descriptions.

//15) Re-render stability

//Given 200 with list.

//When force a re-render (e.g., set a parameter if applicable, or trigger StateHasChanged via a tiny test hook).

//Then no extra HTTP calls; UI state remains consistent.Must-have (ship these first)
//1) Load – Happy path

//Given /api/v1/disaster-cards → 200 with two cards (full fields).

//When component renders.

//Then placeholder + options appear; selecting a card shows details (name, difficulty, location, rescue type, lists for bonus/rewards).

//2) Load – Empty list

//Given 200 with [].

//When component renders.

//Then “No disaster cards available.” is shown; no <select>, no details.

//3) Load – Server error (soft-fail)

//Given 500.

//When component renders.

//Then UI behaves like empty (current contract): shows “No disaster cards available.”, no dropdown/details.

//4) Loading indicator

//Given delayed 200 (e.g., 400ms).

//When component renders.

//Then “Loading cards…” visible before response, disappears after options render.

//5) Selection – With/without conditional sections

//Given 200 with:

//Card A: has bonus+rewards.

//Card B: empty arrays (or nulls).

//When select A then B.

//Then details update; headers/lists appear for A, are hidden for B.

//6) No refetch on selection

//Given 200 with list.

//When select several options (including blank).

//Then WireMock shows exactly one GET for /api/v1/disaster-cards.

//7) Sorting – Case-insensitive

//Given 200 with names in unsorted order, mixed case (e.g., “beta”, “Alpha”, “Gamma”).

//When render.

//Then options (after placeholder) are A–Z ignoring case.

//Should-have (next pass)
//8) Null name handling

//Given 200 with some name=null.

//When render.

//Then no crash; define expected ordering (e.g., nulls sorted as empty string) and assert.

//9) Duplicate IDs – “keep first”

//Given 200 with two items sharing id=1 but different bodies.

//When render & select “1”.

//Then the UI shows the first one (matches your dictionary TryAdd behavior).

//10) Large content safety

//Given very long name/location strings (several hundred chars).

//When render & select.

//Then component still renders; no exceptions; strings present (you don’t need to assert CSS here).

//11) XSS safety (basic)

//Given name="<script>alert(1)</script>".

//When render & select.

//Then markup contains encoded text (&lt;script&gt;…) and not a <script> element.

//12) Timeout behavior (soft-fail as empty)

//Given delayed response longer than HttpClient.Timeout.

//When render.

//Then after timeout, behaves like empty state (today’s contract).

//Nice-to-have (edge/corner coverage)
//13) Unexpected JSON shape (malformed)

//Given 200 with invalid JSON or wrong casing (if your SDK isn’t tolerant).

//When render.

//Then soft-fail → empty. (Useful if the SDK maps deserialization failure to Success=false.)

//14) Partial nulls in nested collections

//Given a card with bonusConditions=[ null, { description: "…" } ] or items missing fields.

//When select.

//Then UI doesn’t throw; skips nulls; renders present descriptions.

//15) Re-render stability

//Given 200 with list.

//When force a re-render (e.g., set a parameter if applicable, or trigger StateHasChanged via a tiny test hook).

//Then no extra HTTP calls; UI state remains consistent.
