using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace ThunderbirdsBoardGameEngine.TestUtils.Stubs
{
    /// <summary>
    /// Test double for <see cref="IHostEnvironment"/> with predictable, immutable state.
    /// Defaults to a <see cref="NullFileProvider"/> (no disk I/O).
    /// Use the static factories to choose provider semantics.
    /// </summary>
    public sealed class StubHostEnvironment : IHostEnvironment, IDisposable
    {
        /// <summary>Immutable application name for consumers.</summary>
        public string ApplicationName { get; }

        /// <summary>Immutable environment name (e.g., Development).</summary>
        public string EnvironmentName { get; }

        /// <summary>File provider for the content root.</summary>
        public IFileProvider ContentRootFileProvider { get; }

        /// <summary>Absolute content root path.</summary>
        public string ContentRootPath { get; }

        string IHostEnvironment.ApplicationName 
        { 
            get => ApplicationName; 
            set => throw new NotSupportedException("StubHostEnvironment is immutable. Use factory methods to create a new instance."); 
        }

        IFileProvider IHostEnvironment.ContentRootFileProvider 
        { 
            get => ContentRootFileProvider; 
            set => throw new NotSupportedException("StubHostEnvironment is immutable. Use factory methods to create a new instance."); 
        }

        string IHostEnvironment.ContentRootPath 
        { 
            get => ContentRootPath; 
            set => throw new NotSupportedException("StubHostEnvironment is immutable. Use factory methods to create a new instance."); 
        }

        string IHostEnvironment.EnvironmentName 
        {
            get => EnvironmentName; 
            set => throw new NotSupportedException("StubHostEnvironment is immutable. Use factory methods to create a new instance."); 
        }

        private readonly bool _disposeProvider;

        private StubHostEnvironment(
            string contentRootPath,
            IFileProvider fileProvider,
            bool disposeProvider,
            string environmentName,
            string applicationName)
        {
            if (string.IsNullOrWhiteSpace(contentRootPath))
                throw new ArgumentException("Content root path is required.", nameof(contentRootPath));

            ContentRootPath = Path.GetFullPath(contentRootPath);
            ContentRootFileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            _disposeProvider = disposeProvider;
            EnvironmentName = environmentName ?? "Development";
            ApplicationName = applicationName ?? "TestApp";
        }

        /// <summary>
        /// Create a stub with a <see cref="NullFileProvider"/> (no disk dependency).
        /// Ideal for pure unit tests (e.g., PostConfigure normalization).
        /// </summary>
        public static StubHostEnvironment WithNullProvider(
            string contentRootPath,
            string environmentName = "Development",
            string applicationName = "TestApp")
            => new(
                contentRootPath,
                new NullFileProvider(),
                disposeProvider: false,
                environmentName,
                applicationName);

        /// <summary>
        /// Create a stub with a <see cref="PhysicalFileProvider"/> rooted at the content root.
        /// The stub owns and disposes this provider.
        /// </summary>
        public static StubHostEnvironment WithPhysicalProvider(
            string contentRootPath,
            string environmentName = "Development",
            string applicationName = "TestApp")
            => new(
                contentRootPath,
                new PhysicalFileProvider(Path.GetFullPath(contentRootPath)),
                disposeProvider: true,
                environmentName,
                applicationName);

        /// <summary>
        /// Create a stub with a caller-supplied provider (DIP). Set <paramref name="disposeProvider"/> if you want the stub to dispose it.
        /// </summary>
        public static StubHostEnvironment WithProvider(
            string contentRootPath,
            IFileProvider provider,
            bool disposeProvider = false,
            string environmentName = "Development",
            string applicationName = "TestApp")
            => new(contentRootPath, provider, disposeProvider, environmentName, applicationName);

        public void Dispose()
        {
            if (_disposeProvider && ContentRootFileProvider is IDisposable d)
                d.Dispose();
        }
    }
}
