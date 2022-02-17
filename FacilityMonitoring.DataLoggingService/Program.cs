using FacilityMonitoring.DataLoggingService;
using FacilityMonitoring.Infrastructure.Data.Model;
using FacilityMonitoring.Infrastructure.Data.MongoDB;
using FacilityMonitoring.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Entities;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((hostContext, configuration) => {
    configuration.AddJsonFile("dbSettings.json", optional: true, reloadOnChange: true);
});

builder.ConfigureServices((hostContext,services) => {
    services.Configure<ServiceConfiguration>(hostContext.Configuration.GetSection(ServiceConfiguration.Section));
    ServiceConfiguration sConfig = new();
    hostContext.Configuration.GetSection(ServiceConfiguration.Section).Bind(sConfig);
    Task.Run(async () => {
        await DB.InitAsync(sConfig.Database, sConfig.DatabaseIP,sConfig.Port);
    }).GetAwaiter().GetResult();
    services.AddHostedService<Worker>();
    services.AddDbContext<FacilityContext>();
    services.AddTransient<IDataRecordService, DataRecordService>();
    services.AddTransient<IDeviceController, DeviceController>();
});

IHost host = builder.Build();

await host.RunAsync();



//await host.RunAsync();
