using System;
using Reqnroll;
using ThunderbirdsBoardGameEngine.Api.UserJourneys.Support;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UserJourneys.StepDefinitions
{
    [Binding]
    public class GameConnectivityStepDefinitions
    {
        private readonly IApplicationRestarter _restarter;

        public GameConnectivityStepDefinitions(IApplicationRestarter restarter)
        {
            _restarter = restarter;
        }

        [When("the application is restarted")]
        public async Task WhenTheApplicationIsRestarted()
        {
            await _restarter.RestartAsync(TestContext.Current.CancellationToken);
        }
    }
}
