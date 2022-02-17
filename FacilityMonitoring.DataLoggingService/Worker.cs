using FacilityMonitoring.Infrastructure.Services;

namespace FacilityMonitoring.DataLoggingService {
    public class Worker : IHostedService, IDisposable {
        private readonly ILogger<Worker> _logger;
        private readonly IDeviceController _controller;
        private readonly IConfiguration _configuration;
        private ServiceConfiguration _serviceConfiguration;

        private Timer _timer = null;

        public Worker(ILogger<Worker> logger,IDeviceController controller,IConfiguration configuration) {
            this._logger = logger;
            this._controller = controller;
            this._configuration = configuration;
            this._serviceConfiguration = this._configuration.GetSection(ServiceConfiguration.Section).Get<ServiceConfiguration>();
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            this._logger.LogInformation("Starting Service");
            await this._controller.Load(this._serviceConfiguration.DeviceId);
            this._logger.LogInformation($"Controller Loaded with {this._serviceConfiguration.DeviceId} settings");
            this._timer = new Timer(this.DataLogHandler, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            this._logger.LogInformation("Logging Started");
        }

        private async void DataLogHandler(object state) {
            await this._controller.Read();
            this._logger.LogInformation("Logged Data");
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            this._logger.LogInformation("Logging Stopped");
            return Task.CompletedTask;
        }

        public void Dispose() {

        }
    }
}