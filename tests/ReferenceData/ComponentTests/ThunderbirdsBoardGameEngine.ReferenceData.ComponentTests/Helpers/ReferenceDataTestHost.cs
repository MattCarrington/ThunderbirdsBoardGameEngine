using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers
{
    public static class ReferenceDataTestHost
    {
        public static ServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddReferenceData();
            return serviceCollection.BuildServiceProvider();
        }
    }
}
