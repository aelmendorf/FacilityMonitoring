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

    }

    public class DeviceController : IDeviceController {

        //private readonly ILogger _logger;
        private readonly IModbusService _modbusService;
        private readonly IDataRecordService _dataService;
        private readonly IFacilityRepository _repo;
        private ModbusDevice _device;

        private IList<AnalogInput> _analogInputs = new List<AnalogInput>();
        private IList<DiscreteInput> _discreteInputs = new List<DiscreteInput>();
        private IList<VirtualInput> _virtualInputs = new List<VirtualInput>();
        private IList<DiscreteOutput> _discreteOutputs = new List<DiscreteOutput>();
        private IList<FacilityAction> _facilityActions = new List<FacilityAction>();
        private NetworkConfiguration _networkConfiguration;
        private bool _dataLoaded=false;

        public DeviceController(ILogger<IDeviceController> logger,IModbusService modbusService, IDataRecordService dataService, IFacilityRepository repository) {
            //this._logger = logger;
            this._modbusService = modbusService;
            this._dataService = dataService;
            this._repo = repository;
        }
        //public DeviceController(IModbusService modbusService, IDataRecordService dataService, IFacilityRepository repository) {
        //    //this._logger = logger;
        //    this._modbusService = modbusService;
        //    this._dataService = dataService;
        //    this._repo = repository;
        //}

        public async Task LoadDeviceAsync() {
            this._device = await this._repo.GetDeviceAsync("Epi2");
            if (this._device != null) {
                this._networkConfiguration = this._device.NetworkConfiguration;
                this._analogInputs = await this._repo.GetAnalogInputsAsync("Epi2");
                this._discreteInputs = await this._repo.GetDiscreteInputsAsync("Epi2");
                this._virtualInputs = await this._repo.GetVirtualInputsAsync("Epi2");
                this._discreteOutputs = await this._repo.GetDiscreteOutputsAsync("Epi2");
                this._facilityActions = await this._repo.GetFacilityActions();
                if (this._networkConfiguration != null && this._analogInputs.Count > 0 && this._discreteInputs.Count > 0 && this._virtualInputs.Count > 0) {
                    this._dataLoaded = this._networkConfiguration.IPAddress != null;
                    this._modbusService.IpAddress = this._networkConfiguration.IPAddress;
                    this._modbusService.Port = this._networkConfiguration.Port;
                    this._modbusService.SlaveAddress = (byte)this._networkConfiguration.SlaveAddress;
                }
            } else {
                this._dataLoaded = false;
                Console.WriteLine("Error: Could not find device");
               // this._logger.LogError("Device return null");
            }
        }

        public async Task Read() {
            if (!this._dataLoaded) {
                return;
            }
            var rawAnalog = await this._modbusService.ReadHoldingRegistersAsync(0, this._networkConfiguration.ModbusConfig.InputRegisters);
            var rawDiscrete = await this._modbusService.ReadDiscreteInputsAsync(0, this._networkConfiguration.ModbusConfig.DiscreteInputs-1);
            var rawCoils = await this._modbusService.ReadCoilsAsync(0, this._networkConfiguration.ModbusConfig.Coils);
            if (rawAnalog != null && rawDiscrete!=null && rawCoils!=null) {
                DeviceData data = new DeviceData();
                data.TimeStamp = DateTime.Now;
                this.ParseAnalogData(rawAnalog, ref data);
                this.ParseDiscreteData(rawDiscrete, ref data);
                this.ParseVirtualData(rawCoils, ref data);
                await this._dataService.UpdateAsync(this._device.DataReference, data);
            }
        }

        private void ParseAnalogData(ushort[] rawAnalog,ref DeviceData data) {
            for (int i = 0; i < rawAnalog.Length; i++) {
                var channel = this._analogInputs[i];
                if (channel.DisplayName != "Not Set") {
                    data.AnalogData.Add(new AnalogData { Name = channel.DisplayName, Value = rawAnalog[i] });
                } else {
                    data.AnalogData.Add(new AnalogData { Name = channel.Identifier, Value = rawAnalog[i] });
                }
            }
        }

       private void ParseDiscreteData(bool[] rawDiscrete, ref DeviceData data) {
            int inputMaxReg = this._discreteInputs.Max(e => e.ModbusAddress.Address);
            int outputMaxReg = this._discreteOutputs.Max(e => e.ModbusAddress.Address);

            for (int i = 0; i < rawDiscrete.Length; i++) {
                if (i < inputMaxReg) {
                    var channel = this._discreteInputs[i];
                    if (channel.DisplayName != "Not Set") {
                        data.DiscreteData.Add(new DiscreteData { Name = channel.DisplayName, Value = rawDiscrete[i] });
                    } else {
                        data.DiscreteData.Add(new DiscreteData { Name = channel.Identifier, Value = rawDiscrete[i] });
                    }
                } else if(i>=inputMaxReg && i<outputMaxReg) {
                    var channel = this._discreteOutputs[i - inputMaxReg];
                    if (channel.DisplayName != "Not Set") {
                        data.OutputData.Add(new OutputData { Name = channel.DisplayName, Value = rawDiscrete[i] });
                    } else {
                        data.OutputData.Add(new OutputData { Name = channel.Identifier, Value = rawDiscrete[i] });
                    }
                } else {
                    var faction = this._facilityActions[i - outputMaxReg];
                    data.ActionData.Add(new ActionData { Name = faction.ActionName, Value = rawDiscrete[i] });
                }
            }
        }

       private void ParseVirtualData(bool[] rawCoilData,ref DeviceData data) {
            for (int i = 0; i < rawCoilData.Length; i++) {
                var channel = this._virtualInputs[i];
                if (channel.DisplayName != "Not Set") {
                    data.CoilData.Add(new VirtualData { Name = channel.DisplayName, Value = rawCoilData[i] });
                } else {
                    data.CoilData.Add(new VirtualData { Name = channel.Identifier, Value = rawCoilData[i] });
                }
            }
        }
    }
}
