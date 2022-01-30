using FacilityMonitoring.Infrastructure.Data.MongoDB;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Entities;


namespace FacilityMonitoring.Infrastructure.Services {
    public interface IDataRecordService {
    //    Task<List<DeviceData>> GetAllDataAsync(string id);
    //    Task<Device> GetDevice(string id);
    //    Task UpdateAsync(string id, 
    //        DeviceData data, 
    //        AnalogData aData,
    //        DiscreteData dData,
    //        OutputData oData, 
    //        AlertData actionData, 
    //        VirtualData vData);
    }

    public class DataRecordService : IDataRecordService {
    //    public Task<List<DeviceData>> GetAllDataAsync(string id) {
    //        return DB.Find<DeviceData>()
    //            .Match(e => e.Device.ID == id)
    //            .ExecuteAsync();
    //    }
    //    public Task<Device> GetDevice(string id) {
    //        return DB.Find<Device>().Match(e => e.ID == id).ExecuteFirstAsync();
    //    }
    //    public async Task UpdateAsync(string id, DeviceData data,
    //        AnalogData aData,
    //        DiscreteData dData,
    //        OutputData oData,
    //        AlertData alertData,
    //        VirtualData vData) {
    //        var device = await this.GetDevice(id);
    //        data.Device = device;
    //        await data.SaveAsync();
    //        await aData.SaveAsync();
    //        await dData.SaveAsync();
    //        await oData.SaveAsync();
    //        await alertData.SaveAsync();
    //        await vData.SaveAsync();

    //        data.AnalogData = aData;
    //        data.DiscreteData = dData;
    //        data.VirtualData = vData;
    //        data.AlertData = alertData;
    //        data.OutputData = oData;
    //        await data.SaveAsync();
    //    }
    //}

    //public class DataRecordService : IDataRecordService {
    //   // private readonly IMongoCollection<Device> _devices;
    //    //private readonly DatabaseSettings _settings;

    //    //public DataRecordService(IOptions<DatabaseSettings> settings) {

    //    //    this._settings = settings.Value;
    //    //    var client = new MongoClient("mongodb://172.20.3.30");
    //    //    var database = client.GetDatabase("monitoring");
    //    //    this._devices = database.GetCollection<Device>("data");
    //    //}

    //    public DataRecordService() {

    //    }

    //    public DataRecordService(string conString,string db,string collection) {
    //        var client = new MongoClient(conString);
    //        var database = client.GetDatabase(db);
    //        this._devices = database.GetCollection<Device>(collection);
    //    }

    //    public async Task<List<DeviceData>> GetAllDataAsync(string id) {
    //        var device = await this._devices.Find(e => e.ID == id).FirstOrDefaultAsync();
    //        return device.DeviceData.ToList();
    //    }

    //    public async Task<Device> GetDevice(string id) {
    //        return await this._devices.Find(e => e.ID == id).FirstOrDefaultAsync();
    //    }

    //    public async Task<bool> UpdateAsync(string id, DeviceData data) {
    //        var device = await this.GetDevice(id);

    //    }

    //    //public async Task<bool> UpdateAsync(string id, DeviceData data) {
    //    //    var device = Builders<Device>.Filter.Eq(e => e.Id, id);
    //    //    var push = Builders<Device>.Update.Push(d => d.DeviceData, data);
    //    //    var result = await this._devices.UpdateOneAsync(device, push);
    //    //    return result.IsAcknowledged;
    //    //}
    }
}
