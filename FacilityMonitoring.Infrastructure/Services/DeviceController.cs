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
        Task<DeviceData> Read();
        Task LoadDeviceAsync();
    }
    public class DeviceController : IDeviceController {

        //private readonly ILogger _logger;
        private readonly IModbusService _modbusService;
        private readonly IDataRecordService _dataService;
        private readonly IFacilityRepository _repo;
        private ModbusDevice? _device;
        private NetworkConfiguration? _netConfig;
        private bool _dataLoaded=false;

        public DeviceController(ILogger<IDeviceController> logger,IModbusService modbusService, IDataRecordService dataService, IFacilityRepository repository) {
            this._modbusService = modbusService;
            this._dataService = dataService;
            this._repo = repository;
        }

        public DeviceController(IModbusService modbusService, IDataRecordService dataService, IFacilityRepository repository) {
            //this._logger = logger;
            this._modbusService = modbusService;
            this._dataService = dataService;
            this._repo = repository;
        }

        public async Task LoadDeviceAsync() {
            this._device = await this._repo.GetDeviceAsync("Epi2");
            if (this._device != null) {
                this._netConfig = this._device.NetworkConfiguration;
                this._dataLoaded = this._netConfig.IPAddress != null;
            } else {
                this._dataLoaded = false;
                Console.WriteLine("Error: Could not find device");
            }
        }

        public async Task<DeviceData?> Read() {
            if (!this._dataLoaded) {
                return null;
            }
            if (this._modbusService.Connect(this._netConfig.IPAddress, this._netConfig.Port)) {
                var rawAnalog = await this._modbusService.ReadHoldingRegistersAsync((byte)this._netConfig.SlaveAddress,0,(ushort)this._netConfig.ModbusConfig.HoldingRegisters);
                var rawDiscrete = await this._modbusService.ReadDiscreteInputsAsync((byte)this._netConfig.SlaveAddress,0, (ushort)this._netConfig.ModbusConfig.DiscreteInputs);
                var rawInput = await this._modbusService.ReadInputRegistersAsync((byte)this._netConfig.SlaveAddress, 0, (ushort)this._netConfig.ModbusConfig.InputRegisters);
                var rawCoils = await this._modbusService.ReadCoilsAsync((byte)this._netConfig.SlaveAddress,0, (ushort)this._netConfig.ModbusConfig.Coils);
                if (rawAnalog != null && rawDiscrete != null && rawCoils != null && rawInput!=null) {
                    DeviceData data = new DeviceData();
                    data.TimeStamp = DateTime.Now;
                    //var analogData = this.ParseAnalogData(rawAnalog);
                    //var discrete = this.ParseDiscreteData(rawDiscrete);
                    //var coilData = this.ParseVirtualData(rawCoils);
                    this._modbusService.Disconnect();
                    return data;
                } else {
                    this._modbusService.Disconnect();
                    return null;
                }
            } else {
                return null;
            }

        }

       // private AnalogData ParseAnalogData(ushort[] rawAnalog) {
       //     AnalogData aData = new AnalogData();
       //     aData.Readings = new List<AnalogReading>();
       //     for (int i = 0; i < rawAnalog.Length; i++) {
       //         var channel = this._analogInputs[i];
       //         if (channel.DisplayName != "Not Set") {
       //             aData.Readings.Add(new AnalogReading { Name = channel.DisplayName, Value = rawAnalog[i] });
       //         } else {
       //             aData.Readings.Add(new AnalogReading { Name = channel.Identifier, Value = rawAnalog[i] });
       //         }
       //     }
       //     return aData;
       // }

       // private DiscreteData ParseDiscreteChannels(int start, int stop,ushort[] raw) {
       //     DiscreteData data = new DiscreteData();
       //     for(int i = 0; i < stop; i++) {
       //         data.Readings.Add(new DiscreteReading { Name = "", Value = raw[i] });
       //     }
       // }

       //private Tuple<List<DiscreteData>,List<OutputData>,List<ActionData>> ParseDiscreteData(bool[] rawDiscrete) {
       //     int inputMaxReg = this._discreteInputs.Max(e => e.ModbusAddress.Address);
       //     int outputMaxReg = this._discreteOutputs.Max(e => e.ModbusAddress.Address);
       //     List<DiscreteData> discreteData = new List<DiscreteData>();
       //     List<OutputData> outputData = new List<OutputData>();
       //     List<ActionData> actionData = new List<ActionData>();

       //     for (int i = 0; i < rawDiscrete.Length; i++) {
       //         if (i < inputMaxReg) {
       //             var channel = this._discreteInputs[i];
       //             if (channel.DisplayName != "Not Set") {
       //                 discreteData.Add(new DiscreteData { Name = channel.DisplayName, Value = rawDiscrete[i] });
       //             } else {
       //                 discreteData.Add(new DiscreteData { Name = channel.Identifier, Value = rawDiscrete[i] });
       //             }
       //         } else if(i>=inputMaxReg && i<outputMaxReg) {
       //             var channel = this._discreteOutputs[i - inputMaxReg];
       //             if (channel.DisplayName != "Not Set") {
       //                 outputData.Add(new OutputData { Name = channel.DisplayName, Value = rawDiscrete[i] });
       //             } else {
       //                 outputData.Add(new OutputData { Name = channel.Identifier, Value = rawDiscrete[i] });
       //             }
       //         } else {
       //             var faction = this._facilityActions[i - outputMaxReg];
       //             actionData.Add(new ActionData { Name = faction.ActionName, Value = rawDiscrete[i] });
       //         }
       //     }
       //     return new Tuple<List<DiscreteData>, List<OutputData>, List<ActionData>>(discreteData, outputData, actionData);
       // }

       //private List<VirtualData> ParseVirtualData(bool[] rawCoilData) {
       //     List<VirtualData> virtualData = new List<VirtualData>();
       //     for (int i = 0; i < rawCoilData.Length; i++) {
       //         var channel = this._virtualInputs[i];
       //         if (channel.DisplayName != "Not Set") {
       //             virtualData.Add(new VirtualData { Name = channel.DisplayName, Value = rawCoilData[i] });
       //         } else {
       //             virtualData.Add(new VirtualData { Name = channel.Identifier, Value = rawCoilData[i] });
       //         }
       //     }
       //     return virtualData;
       // }
    }
}
