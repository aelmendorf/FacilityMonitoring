using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Entities;

namespace FacilityMonitoring.Infrastructure.Data.MongoDB {

    public enum DeviceType {
        Modbus,
        BacNet,
        API
    }
    public class MonitoringDevice:Entity {
        public string DeviceName { get; set; }

        public Many<DisplayConfig> DisplayConfig { get; set; }
        public Many<Data> DeviceData { get; set; }
        public MonitoringDevice() {
            this.InitOneToMany(() => DeviceData);
            this.InitOneToMany(() => DisplayConfig);
        }
    }

    public class DisplayConfig:Entity {
        public int Iteration { get; set; }
        public string[] AnalogHeaders { get; set; }
        public string[] AlertHeaders { get; set; }
        public string[] DiscreteHeaders { get; set; }
        public string[] VirtualHeaders { get; set; }
        public string[] OutputHeaders { get; set; }
        public string[] ActionHeaders { get; set; }
        public string DeviceHeader { get; set; }
    }

    public class Data:Entity {
        public int DisplayConfigIteration { get; set; }
        public DateTime TimeStamp { get; set; }
        public ushort[] AnalogInputs { get; set; }
        public ushort[] Alerts { get; set; }
        public bool[] DiscreteInputs { get; set; }
        public bool[] VirtualInputs { get; set; }
        public bool[] Outputs { get; set; }
        public bool[] Actions { get; set; }
        public int DeviceState { get; set; }
    }
}
