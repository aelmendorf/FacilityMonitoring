using FacilityMonitoring.Infrastructure.Data.MongoDB;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FacilityMonitoring.Infrastructure.Services {
    public interface IDataRecordService {
        Task<List<DeviceData>> GetAllDataAsync(string id);
        Task<Device> GetDevice(string id);
        Task<bool> UpdateAsync(string id, DeviceData data);
    }

    public class DataRecordService : IDataRecordService {
        private readonly IMongoCollection<Device> _devices;
        private readonly DatabaseSettings _settings;

        public DataRecordService(IOptions<DatabaseSettings> settings) {
            this._settings = settings.Value;
            var client = new MongoClient(this._settings.ConnectionString);
            var database = client.GetDatabase(this._settings.DatabaseName);
            this._devices = database.GetCollection<Device>(this._settings.CollectionName);
        }

        public async Task<List<DeviceData>> GetAllDataAsync(string id) {
            var device = await this._devices.Find(e => e.Id == id).FirstOrDefaultAsync();
            return device.DeviceData.ToList();
        }

        public async Task<Device> GetDevice(string id) {
            return await this._devices.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(string id, DeviceData data) {
            var device = Builders<Device>.Filter.Eq(e => e.Id, id);
            var push = Builders<Device>.Update.Push(d => d.DeviceData, data);
            var result = await this._devices.UpdateOneAsync(device, push);
            return result.IsAcknowledged;
        }
    }
}
