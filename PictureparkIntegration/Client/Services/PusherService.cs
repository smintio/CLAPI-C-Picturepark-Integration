using Client.Auth;
using Client.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PusherClient;
using System.Threading;
using System.Threading.Tasks;
using Client.Jobs;
using Client.Providers;

namespace Client.Services
{
    public class PusherService : IHostedService
    {
        private readonly SmintIoAppOptions _options;

        private readonly AuthResult _authData;

        private Pusher _pusher;
        private Channel _channel;

        private readonly ISyncJob _syncJob;

        private readonly ILogger _logger;

        public PusherService(
            IOptionsMonitor<SmintIoAppOptions> optionsMonitor,
            IAuthDataProvider authDataProvider,
            ISyncJob syncJob,
            ILogger<PusherService> logger)
        {
            _options = optionsMonitor.CurrentValue;
            _authData = authDataProvider.SmintIo;
            _syncJob = syncJob;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Pusher service...");

            StartPusher();

            _logger.LogInformation("Pusher service started");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Pusher service...");

            StopPusher();

            _logger.LogInformation("Pusher service stopped");

            return Task.CompletedTask;
        }

        private void StartPusher()
        {
            var pusherAuthEndpoint = $"https://{_options.TenantId}-clapi.smint.io/consumer/v1/notifications/pusher/auth";

            var authorizer = new SmintIo.CLAPI.Consumer.Client.Pusher.HttpAuthorizer(pusherAuthEndpoint, _authData.AccessToken);

            _pusher = new Pusher("32f31c26a83e09dc401b", new PusherOptions()
            {
                Cluster = "eu",
                Authorizer = authorizer
            });

            _pusher.ConnectionStateChanged += ConnectionStateChanged;
            _pusher.Error += PusherError;

            var connectionState = _pusher.ConnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            if (connectionState == ConnectionState.Connected)
            {
                SubscribeToPusherChannel();
            }
        }

        private void StopPusher()
        {
            _channel?.UnbindAll();
            _channel?.Unsubscribe();

            _pusher.DisconnectAsync().ConfigureAwait(false);

            _pusher.ConnectionStateChanged -= ConnectionStateChanged;
            _pusher.Error -= PusherError;
        }

        private void SubscribeToPusherChannel()
        {
            _channel = _pusher.SubscribeAsync($"private-2-{_options.ChannelId}").ConfigureAwait(false).GetAwaiter().GetResult();

            _channel.Bind("global-transaction-history-update", async (payload) =>
            {
                _logger.LogInformation("Received Pusher event");

                // do not sync generic metadata, because we need to be fast here

                await _syncJob.SynchronizeAsync(synchronizeGenericMetadata: false);
            });
        }

        private void ConnectionStateChanged(object sender, ConnectionState state)
        {
            _logger.LogInformation($"Pusher connection state changed to {state}");
        }

        private void PusherError(object sender, PusherException pusherException)
        {
            _logger.LogError(pusherException, "An pusher exception occured");
        }
    }
}
