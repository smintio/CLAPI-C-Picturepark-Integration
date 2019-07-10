﻿using Client.Jobs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Services
{
    public class TimedSynchronizerService : IHostedService, IDisposable
    {
        private readonly ISyncJob _syncJob;

        private Timer _timer;

        private readonly ILogger _logger;

        public TimedSynchronizerService(
            ISyncJob syncJob,
            ILogger<TimedSynchronizerService> logger)
        {
            _syncJob = syncJob;

            _logger = logger;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting timed synchronizer service...");

            _timer = new Timer(DoWorkAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));

            _logger.LogInformation("Started timed synchronizer service");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping timed synchronizer service...");

            _timer?.Change(Timeout.Infinite, 0);

            _logger.LogInformation("Stopped timed synchronizer service");

            return Task.CompletedTask;
        }

        private async void DoWorkAsync(object state)
        {
            await _syncJob.SynchronizeAssetsAsync();
        }
    }
}
