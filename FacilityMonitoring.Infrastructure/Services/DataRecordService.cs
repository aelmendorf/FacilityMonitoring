using FacilityMonitoring.Infrastructure.Data.MongoDB;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FacilityMonitoring.Infrastructure.Services {
    public interface IDataRecordService {
        Task Record(string deviceId, DataRecord data);
        Task<int> GetDataConfigIteration(string deviceId);
    }

    public class DataRecordService : IDataRecordService {
        public async Task Record(string deviceId, DataRecord data) {
            var dataDevice = await DB.Find<MonitoringDevice>().OneAsync(deviceId);
            if (dataDevice is null) return;

            await data.SaveAsync();
            await dataDevice.DeviceData.AddAsync(data);
            await dataDevice.SaveAsync();
        }

        public async Task<int> GetDataConfigIteration(string deviceId) {
            var dev=await DB.Find<MonitoringDevice>().OneAsync(deviceId);
            if (dev is null) return -1;
            return dev.DataConfigurations.Max(e => e.Iteration);
        }
    }
}
