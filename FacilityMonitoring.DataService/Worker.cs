using FacilityMonitoring.Infrastructure.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FacilityMonitoring.DataService {
    public class Worker : IHostedService,IDisposable {
        private readonly ILogger<Worker> _logger;
        private readonly IDeviceController _controller;
        private Timer _timer = null;

        public Worker(ILogger<Worker> logger,IDeviceController controller) {
            this._logger = logger;
            this._controller = controller;
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            this._logger.LogInformation("Starting Service");
            //await this._controller.LoadDeviceAsync();
            this._timer = new Timer(this.DataLogHandler, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
            this._logger.LogInformation("Logging Started");
        }

        private async void DataLogHandler(object state) {
            //await this._controller.Read();
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            this._logger.LogInformation("Logging Stopped");
            return Task.CompletedTask;
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}
