using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Entities;

namespace FacilityMonitoring.Infrastructure.Data.MongoDB { 
    public class Device:Entity {
        public string DeviceName { get; set; }
       //public Many<DeviceData> DeviceData { get; set; }
        public Device() {
            //this.InitOneToMany(() => DeviceData);
        }
    }

    public enum DeviceType {
        Modbus,
        BacNet,
        API
    }

    public class DataConfiguration:Entity {
        public string Version { get; set; }
        public string[] AnalogHeaders { get; set; }
        public string[] DiscreteInputHeaders { get; set; }
        public string[] OutputHeaders { get; set; }
        public string[] AlertHeaders { get; set; }
        public string[] VirtualHeaders { get; set; }
    }


    public class HoldingRegisters {

    }

    public class InputRegisters {

    }

    public class DiscreteInputs {

    }

    public class Coils {
        public string[] Headers { get; set; }
        public bool[] Data { get; set; }
    }






    //public class DeviceData:Entity {
    //    public One<Device> Device { get; set; }
    //    public DateTime TimeStamp { get; set; }
    //    public One<AnalogData> AnalogData { get; set; }
    //    public One<DiscreteData> DiscreteData { get; set; }
    //    public One<VirtualData> VirtualData { get; set; }
    //    public One<OutputData> OutputData { get; set; }
    //    public One<AlertData> AlertData { get; set; }
    //}
    //public class AnalogData:Entity {
    //    public List<AnalogReading> Readings { get; set; }
    //}
    //public class DiscreteData : Entity {
    //    public List<DiscreteReading> Readings { get; set; }
    //}
    //public class VirtualData : Entity {
    //    public List<VirtualReading> Readings { get; set; }
    //}
    //public class AlertData : Entity {
    //    public List<AlertReading> Readings { get; set; }
    //}
    //public class OutputData : Entity {
    //    public List<OutputReading> Readings { get; set; }
    //}
    //public abstract class RegisterData {
    //    public string Name { get; set; }
    //}
    //public class AnalogReading : RegisterData {
    //    public double Value { get; set; }
    //}
    //public class DiscreteReading : RegisterData {
    //    public bool Value { get; set; }
    //}
    //public class VirtualReading : RegisterData {
    //    public bool Value { get; set; }
    //}
    //public class OutputReading : RegisterData {
    //    public bool Value { get; set; }
    //}
    //public class AlertReading : RegisterData {
    //    public int Value { get; set; }
    //}
}
