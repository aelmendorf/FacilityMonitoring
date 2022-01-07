using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Entities;

namespace FacilityMonitoring.Infrastructure.Data.MongoDB { 
    public class Device:Entity {
        public string DeviceName { get; set; }
        public Many<DeviceData> DeviceData { get; set; }
        public Device() {
            this.InitOneToMany(() => DeviceData);
        }
    }
    public class DeviceData:Entity {
        public DateTime TimeStamp { get; set; }
        public One<Device> Device { get; set; }
        public Many<AnalogData> AnalogData { get; set; }
        public Many<DiscreteData> DiscreteData { get; set; } 
        public Many<VirtualData> CoilData { get; set; } 
        public Many<OutputData> OutputData { get; set; } 
        public Many<ActionData> ActionData { get; set; } 
        
        public DeviceData() {
            this.InitOneToMany(() => AnalogData);
            this.InitOneToMany(() => DiscreteData);
            this.InitOneToMany(() => CoilData);
            this.InitOneToMany(() => OutputData);
            this.InitOneToMany(() => ActionData);
        }

    }

    public abstract class RegisterData:Entity {
        public string Name { get; set; }
    }

    public class AnalogData:RegisterData {
        public double Value { get; set; }
        public One<DeviceData> DeviceData { get; set; }
    }

    public class DiscreteData:RegisterData {
        public bool Value { get; set; }
        public One<DeviceData> DeviceData { get; set; }
    }
    public class VirtualData:RegisterData {
        public bool Value { get; set; }
        public One<DeviceData> DeviceData { get; set; }
    }

    public class OutputData : RegisterData {
        public bool Value { get; set; }
        public One<DeviceData> DeviceData { get; set; }
    }

    public class ActionData : RegisterData {
        public bool Value { get; set; }
        public One<DeviceData> DeviceData { get; set; }
    }

    public class AlertData {
        public string Channel { get; set; }
        public string Type { get; set; }
    }


}
