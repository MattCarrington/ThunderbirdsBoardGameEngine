using System.Reflection;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Providers
{
    public class EmbeddedResourceSnapshotProvider : ISnapshotProvider
    {
        private readonly Assembly _assembly;
        private readonly string _resourceName;

        public EmbeddedResourceSnapshotProvider()
            : this(
                Assembly.GetExecutingAssembly(),
                $"{Assembly.GetExecutingAssembly().GetName().Name}.snapshot.json")
        {
        }

        internal EmbeddedResourceSnapshotProvider(Assembly assembly, string resourceName)
        {
            _assembly = assembly;
            _resourceName = resourceName;
        }

        public Task<Stream> GetSnapshotStreamAsync()
        {
            var stream = _assembly.GetManifestResourceStream(_resourceName)
                ?? throw new InvalidOperationException($"Embedded resource '{_resourceName}' not found.");

            return Task.FromResult(stream);
        }
    }
}