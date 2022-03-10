using FacilityMonitoring.Infrastructure.Data.MongoDB;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FacilityMonitoring.Infrastructure.Services {

    public class DataRecordInput {
        public string DatabaseName { get; set; }
        public List<AnalogReading> AnalogReadings { get; set; }
        public List<DiscreteReading> DiscreteReadings { get; set; }
        public List<OutputReading> OutputReadings { get; set; }
        public List<VirtualReading> VirtuaReadings { get; set; }
        public List<ActionReading> ActionReadings { get; set; }
        public List<AlertReading> AlertReadins { get; set; }
    }


    public interface IDataRecordService {
        Task<bool> Connect(string databaseName);
        Task Record(DataRecordInput input);
    }

    public class DataRecordService : IDataRecordService {
        private MongoClient _client;
        private IMongoDatabase _database;
        private bool _connected;

        public DataRecordService() {
            this._connected = false;
        }

        public async Task<bool> Connect(string databaseName) {
            this._client = new MongoClient("mongodb://172.20.3.30");
            using var cursor = await this._client.ListDatabaseNamesAsync();
            if (cursor.ToList().FirstOrDefault(databaseName) is null) {
                return await CreateDatabase(databaseName);
            } else {
                this._database = this._client.GetDatabase(databaseName);
                this._connected= this._database != null;
                return this._connected;
            }
        }

        public async Task Record(DataRecordInput input) {
            if (this._connected) {
                //await this._database.G
            }
        }

        private async Task<bool> CreateDatabase(string databaseName) {
            this._database = this._client.GetDatabase(databaseName);
            this._connected = this._database != null;
            if (!this._connected) {
                return false;
            }

            await this._database.CreateCollectionAsync("analog_readings", 
                new CreateCollectionOptions() { 
                    TimeSeriesOptions = new TimeSeriesOptions("timestamp", "channelid", granularity: TimeSeriesGranularity.Seconds) 
                });
            await this._database.CreateCollectionAsync("discrete_readings", 
                new CreateCollectionOptions(){ 
                    TimeSeriesOptions = new TimeSeriesOptions("timestamp", "channelid", granularity: TimeSeriesGranularity.Seconds)
                });
            await this._database.CreateCollectionAsync("output_readings", 
                new CreateCollectionOptions() { 
                    TimeSeriesOptions = new TimeSeriesOptions("timestamp", "channelid", granularity: TimeSeriesGranularity.Seconds) 
                });
            await this._database.CreateCollectionAsync("virtual_readings", 
                new CreateCollectionOptions() { 
                    TimeSeriesOptions = new TimeSeriesOptions("timestamp", "channelid", granularity: TimeSeriesGranularity.Seconds) 
                });
            await this._database.CreateCollectionAsync("alert_readings", 
                new CreateCollectionOptions() { 
                    TimeSeriesOptions = new TimeSeriesOptions("timestamp", "alertid", granularity: TimeSeriesGranularity.Seconds) 
                });
            await this._database.CreateCollectionAsync("action_readings", 
                new CreateCollectionOptions() { 
                    TimeSeriesOptions = new TimeSeriesOptions("timestamp", "actionid", granularity: TimeSeriesGranularity.Seconds) 
                });

            var areadings = this._database.GetCollection<AnalogReading>("analog_readings");
            var dreadings = this._database.GetCollection<DiscreteReading>("discrete_readings");
            var oreadings = this._database.GetCollection<OutputReading>("output_readings");
            var vreadings = this._database.GetCollection<VirtualReading>("virtual_readings");
            var alertReadings = this._database.GetCollection<AlertReading>("alert_readings");
            var actionReadings = this._database.GetCollection<ActionReading>("action_readings");

            areadings.Indexes.CreateOne(new CreateIndexModel<AnalogReading>(Builders<AnalogReading>.IndexKeys.Ascending(x => x.channelid),
                new CreateIndexOptions()),
                new CreateOneIndexOptions());

            dreadings.Indexes.CreateOne(new CreateIndexModel<DiscreteReading>(Builders<DiscreteReading>.IndexKeys.Ascending(x => x.channelid),
                new CreateIndexOptions()),
                new CreateOneIndexOptions());

            oreadings.Indexes.CreateOne(new CreateIndexModel<OutputReading>(Builders<OutputReading>.IndexKeys.Ascending(x => x.channelid),
                new CreateIndexOptions()),
                new CreateOneIndexOptions());

            vreadings.Indexes.CreateOne(new CreateIndexModel<VirtualReading>(Builders<VirtualReading>.IndexKeys.Ascending(x => x.channelid),
                new CreateIndexOptions()),
                new CreateOneIndexOptions());

            alertReadings.Indexes.CreateOne(new CreateIndexModel<AlertReading>(Builders<AlertReading>.IndexKeys.Ascending(x => x.alertid),
                new CreateIndexOptions()),
                new CreateOneIndexOptions());

            actionReadings.Indexes.CreateOne(new CreateIndexModel<ActionReading>(Builders<ActionReading>.IndexKeys.Ascending(x => x.actionid),
                new CreateIndexOptions()),
                new CreateOneIndexOptions());
            this._connected = true;
            return this._connected;
        }

        //public async Task Record(string deviceId, DataRecord data) {
        //    var dataDevice = await DB.Find<MonitoringDevice>().OneAsync(deviceId);
        //    if (dataDevice is null) return;

        //    await data.SaveAsync();
        //    await dataDevice.DeviceData.AddAsync(data);
        //    await dataDevice.SaveAsync();
        //}

        //public async Task<int> GetDataConfigIteration(string deviceId) {
        //    var dev=await DB.Find<MonitoringDevice>().OneAsync(deviceId);
        //    if (dev is null) return -1;
        //    return dev.DataConfigurations.Max(e => e.Iteration);
        //}
    }
}
