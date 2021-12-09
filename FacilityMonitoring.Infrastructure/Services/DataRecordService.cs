using FacilityMonitoring.Infrastructure.Data.MongoDB;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FacilityMonitoring.Infrastructure.Services {
    public class DataRecordService {
        private readonly IMongoCollection<Device> _devices;
        private readonly DatabaseSettings _settings;

        public DataRecordService(IOptions<DatabaseSettings> settings) {
            this._settings = settings.Value;
            var client = new MongoClient(this._settings.ConnectionString);
            var database = client.GetDatabase(this._settings.DatabaseName);
            this._devices = database.GetCollection<Device>(this._settings.CollectionName);
        }

        public async Task<List<Device>> GetAllAsync() {
            return await this._devices.Find(e => true).ToListAsync();
        }

        public async Task<Device> GetDevice(string id) {
            return await this._devices.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(string id,DeviceData data) {

        }

        
    }
}
