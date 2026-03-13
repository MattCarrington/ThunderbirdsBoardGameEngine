using System.Reflection;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Providers
{
    public class EmbeddedResourceSnapshotProvider : ISnapshotProvider
    {
        public Task<Stream> GetSnapshotStreamAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{assembly.GetName().Name}.snapshot.json";

            var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");

            return Task.FromResult(stream);
        }
    }
}