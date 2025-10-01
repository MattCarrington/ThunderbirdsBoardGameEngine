using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace ThunderbirdsBoardGameEngine.TestUtils.Stubs
{
    public class StubHostEnvironment : IHostEnvironment
    {
        public StubHostEnvironment(string contentRootPath)
        {
            ContentRootPath = Path.GetFullPath(contentRootPath);
            ContentRootFileProvider = new PhysicalFileProvider(ContentRootPath);
        }

        public string ApplicationName { get; set; } = "TestApp";

        public string EnvironmentName { get; set; } = "Development";

        public IFileProvider ContentRootFileProvider { get; set; }

        public string ContentRootPath { get; set; }
    }
}
