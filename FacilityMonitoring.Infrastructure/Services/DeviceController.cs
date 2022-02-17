using FacilityMonitoring.Infrastructure.Data.Model;
using FacilityMonitoring.Infrastructure.Data.MongoDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityMonitoring.Infrastructure.Services {
    public interface IDeviceController {
        Task Read();
        Task Load(string deviceId);

    }
    public class DeviceController : IDeviceController {

        private readonly ILogger _logger;
        private readonly FacilityContext _context;
        private readonly IDataRecordService _dataService;
        private readonly IConfiguration _configuration;

        private string _objectRef;
        private NetworkConfiguration _netConfig;
        private ModbusConfig _modbusConfig;
        private bool _dataLoaded=false;

        //public DeviceController(ILogger<IDeviceController> logger, IDataRecordService dataService, IMonitoringBoxRepo repository) {
        //    this._dataService = dataService;
        //}

        public DeviceController(IDataRecordService dataService,FacilityContext context,IConfiguration config) {
            this._dataService = dataService;
            this._context = context;
            this._configuration = config;
        }

        public async Task Read() {
            if (!this._dataLoaded) return;
            var result = await ModbusService.Read(this._netConfig.IPAddress,this._netConfig.Port,this._modbusConfig);
            if (!result._success) return;
            var channelMapping = this._modbusConfig.ChannelMapping;
            DataRecord record = new DataRecord();
            record.DataConfigIteration = 1;
            record.TimeStamp = DateTime.Now;
            record.DiscreteInputs = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.DiscreteStart, (channelMapping.DiscreteStop - channelMapping.DiscreteStart) + 1).ToArray();
            record.Outputs = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.OutputStart, (channelMapping.OutputStop - channelMapping.OutputStart) + 1).ToArray();
            record.Actions = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.ActionStart, (channelMapping.ActionStop - channelMapping.ActionStart) + 1).ToArray();
            record.AnalogInputs = new ArraySegment<ushort>(result.InputRegisters, channelMapping.AnalogStart, (channelMapping.AnalogStop - channelMapping.AnalogStart) + 1).ToArray();
            record.Alerts = new ArraySegment<ushort>(result.HoldingRegisters, channelMapping.AlertStart, (channelMapping.AlertStop - channelMapping.AlertStart) + 1).ToArray();
            record.VirtualInputs = result.Coils;
            record.DeviceState = result.HoldingRegisters[channelMapping.DeviceStart];
            await this._dataService.Record(this._objectRef,record);
        }

        public async Task Load(string deviceId) {
            var device = await this._context.Devices
                .OfType<MonitoringBox>()
                .FirstOrDefaultAsync(e => e.Identifier == deviceId);
            if (device is null) return;
            this._netConfig = device.NetworkConfiguration;
            this._modbusConfig = this._netConfig.ModbusConfig;
            this._objectRef = device.DataReference;
            this._dataLoaded = true;
        }
    }
}
