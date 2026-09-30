using Microsoft.Extensions.Options;
using System.Diagnostics;
using ThunderbirdsBoardGameEngine.GameState.Client.Configuration;

namespace ThunderbirdsBoardGameEngine.Api.UserJourneys.Support
{
    public class DockerApplicationRestarter : IApplicationRestarter
    {
        private const string DefaultContainerName = "thunderbirds-api";

        private static readonly TimeSpan ReadinessTimeout = TimeSpan.FromSeconds(30);

        private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(500);

        private readonly HttpClient _httpClient;
        private readonly string _containerName;

        public DockerApplicationRestarter(IOptions<GameStateClientOptions> options)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(options.Value.BaseAddress)
            };

            _containerName = Environment.GetEnvironmentVariable("GAME_STATE_API_CONTAINER_NAME")
                ?? DefaultContainerName;
        }

        public async Task RestartAsync(CancellationToken cancellationToken = default)
        {
            await RestartContainer(cancellationToken);
            await WaitUntilReady(cancellationToken);
        }

        private async Task RestartContainer(CancellationToken cancellationToken)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "docker",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            startInfo.ArgumentList.Add("restart");
            startInfo.ArgumentList.Add(_containerName);

            using var process = new Process
            {
                StartInfo = startInfo
            };

            if (!process.Start())
            {
                throw new InvalidOperationException("Docker could not be started.");
            }

            var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);

            var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);

            await process.WaitForExitAsync(cancellationToken);

            var output = await outputTask;
            var error = await errorTask;

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"Docker failed to restart the API container. " +
                    $"Exit code: {process.ExitCode}. Error: {error}");
            }
        }

        private async Task WaitUntilReady(CancellationToken cancellationToken)
        {
            using var timeout = new CancellationTokenSource(ReadinessTimeout);

            using var linked = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeout.Token);

            while (!linked.IsCancellationRequested)
            {
                try
                {
                    using var response = await _httpClient.GetAsync("health/ready", linked.Token);

                    if (response.IsSuccessStatusCode)
                    {
                        return;
                    }
                }
                catch (HttpRequestException)
                {
                    // Expected while the API is restarting.
                }
                catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    // The readiness attempt or overall timeout elapsed.
                }

                try
                {
                    await Task.Delay(PollInterval, linked.Token);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            throw new TimeoutException(
                $"The API did not become ready within " +
                $"{ReadinessTimeout.TotalSeconds} seconds.");
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
