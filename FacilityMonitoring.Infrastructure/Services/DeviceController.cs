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
        Task Read();
        Task LoadDeviceAsync();
        Task Print();

    }

    internal class DeviceController : IDeviceController {

        private readonly ILogger _logger;
        private readonly IModbusService _modbusService;
        private readonly IDataRecordService _dataService;
        private readonly IFacilityRepository _repo;
        private ModbusDevice _device;

        public DeviceController(ILogger logger,IModbusService modbusService, IDataRecordService dataService, IFacilityRepository repository) {
            this._logger = logger;
            this._modbusService = modbusService;
            this._dataService = dataService;
            this._repo = repository;
        }

        public async Task LoadDevice() {
            this._device=await this._repo.GetDeviceAsync("Epi2");
            if (this._device != null) {


            } else {
                this._logger.LogError("Device return null");
            }
        }

        public void Print() {
            throw new NotImplementedException();
        }

        public async Task Read() {
            var networkConfig = this._device.NetworkConfiguration;
            this._modbusService.IpAddress = networkConfig.IPAddress;
            this._modbusService.Port = networkConfig.Port;
            this._modbusService.SlaveAddress = (byte)networkConfig.SlaveAddress;
            var rawAnalog = await this._modbusService.ReadHoldingRegistersAsync(0, networkConfig.ModbusConfig.InputRegisters);
            var analogChannels = this._device.Channels.OfType<AnalogInput>().OrderBy(e => e.SystemChannel).ToArray();
            if (rawAnalog != null && analogChannels != null) {
                if (analogChannels.Length == rawAnalog.Length) {

                    DeviceData data = new DeviceData();
                    data.TimeStamp = DateTime.Now;
                    for (int i = 0; i < rawAnalog.Length; i++) {
                        var channel = analogChannels[i - 1];
                        if (channel.DisplayName != "Not Set") {
                            data.AnalogData.Add(new AnalogData { Name = channel.DisplayName, Value = rawAnalog[i] / 1000 });
                        } else {

                        }
                    }
                }
            }
        }
    }
}
