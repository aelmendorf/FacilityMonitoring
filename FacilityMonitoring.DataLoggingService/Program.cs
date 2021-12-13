using FacilityMonitoring.DataLoggingService;
using FacilityMonitoring.Infrastructure.Data.Model;
using FacilityMonitoring.Infrastructure.Data.MongoDB;
using FacilityMonitoring.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((hostContext, configuration) => {
    configuration.Sources.Clear();

    IHostEnvironment env = hostContext.HostingEnvironment;

    configuration
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

    IConfigurationRoot configurationRoot = configuration.Build();
    DatabaseSettings databaseSettings = new();
    configurationRoot.GetSection(nameof(DatabaseSettings)).Bind(databaseSettings);
    hostContext.Configuration.Get<DatabaseSettings>();

});

builder.ConfigureServices((hostContext,services) => {
    services.Configure<DatabaseSettings>(hostContext.Configuration.GetSection(nameof(DatabaseSettings)));
    services.AddHostedService<Worker>();
    services.AddDbContext<FacilityContext>();
    services.AddTransient<IFacilityRepository, FacilityRepository>();
    services.AddTransient<IModbusService, ModbusService>();
    services.AddTransient<IDataRecordService, DataRecordService>();
    services.AddTransient<IDeviceController, DeviceController>();
});

IHost host = builder.Build();

await host.RunAsync();



//await host.RunAsync();
