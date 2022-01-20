using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace FacilityMonitoring.Infrastructure.Data.Model {
    public enum DeviceState { OKAY,WARNING,ALARM,MAINTENANCE }
    public class ModbusConfig {
        public int DiscreteInputs { get; set; }
        public int InputRegisters { get; set; }
        public int HoldingRegisters { get; set; }
        public int Coils { get; set; }
    }

    public class NetworkConfiguration {
        public string IPAddress { get; set; }
        public string DNS { get; set; }
        public string MAC { get; set; }
        public string Gateway { get; set; }
        public int Port { get; set; }
        public int SlaveAddress { get; set; }
        public ModbusConfig ModbusConfig { get; set; }
    }

    public class ChannelRegisterMap {
        public ModbusRegister RegisterType { get; set; }
        public int Start { get; set; }
        public int Stop { get; set; }
    }

    public class ChannelRegisterMapping {
        ChannelRegisterMap DiscreteRegisterMap { get; set; }
        ChannelRegisterMap AnalogRegisterMap { get; set; }
        ChannelRegisterMap AlertRegisterMap { get; set; }
        ChannelRegisterMap VirtualRegisterMap { get; set; }
    }

    public class ModbusDevice:Device {
        public NetworkConfiguration NetworkConfiguration { get; set; }
        public ChannelRegisterMapping ChannelMapping  { get; set; }
        public ModbusAddress ModbusAddress { get;set; }
        public ICollection<Channel> Channels { get; set; } = new List<Channel>();

    }

    public class MonitoringBox:ModbusDevice {
        public ICollection<Module> Modules { get; set; } = new List<Module>();
    }
}
