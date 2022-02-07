using FacilityMonitoring.Infrastructure.Data.Model;
using FacilityMonitoring.Infrastructure.Data.MongoDB;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityMonitoring.Infrastructure.Services {
    public interface IDeviceController {
        //Task<MonitoringDevice> Read();
        //Task LoadDeviceAsync();
    }
    public class DeviceController : IDeviceController {

        //private readonly ILogger _logger;
        private readonly IModbusService _modbusService;
        private readonly IDataRecordService _dataService;
        private readonly IMonitoringBoxRepo _repo;
        private ModbusDevice _device;
        private NetworkConfiguration _netConfig;
        private bool _dataLoaded=false;

        public DeviceController(ILogger<IDeviceController> logger,IModbusService modbusService, IDataRecordService dataService, IMonitoringBoxRepo repository) {
            this._modbusService = modbusService;
            this._dataService = dataService;
            this._repo = repository;
        }

        public DeviceController(IModbusService modbusService, IDataRecordService dataService, IMonitoringBoxRepo repository) {
            //this._logger = logger;
            this._modbusService = modbusService;
            this._dataService = dataService;
            this._repo = repository;
        }
    }
}
