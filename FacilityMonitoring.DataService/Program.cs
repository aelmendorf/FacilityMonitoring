using FacilityMonitoring.Infrastructure.Data.Model;
using FacilityMonitoring.Infrastructure.Services;
using FacilityMonitoring.Infrastructure.Data.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace FacilityMonitoring.DataService {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, configuration) => {
                    var keys=configuration.Build().AsEnumerable();
                    foreach((string key,string value) in keys) {
                        Console.WriteLine($"{key}={value}");
                    }
                })
            .ConfigureServices((hostContext, services) => {
                services.AddHostedService<Worker>()
                .AddDbContext<FacilityContext>()
                .AddTransient<IFacilityRepository, FacilityRepository>()
                .AddTransient<IDataRecordService, DataRecordService>()
                .AddTransient<IDeviceController, DeviceController>()
                .AddLogging();
            });
    }
}
