using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace FacilityMonitoring.Infrastructure.Data.Model {

    public enum DeviceState { OKAY,WARNING,ALARM,MAINTENANCE }

    public class ModbusConfig {
        public int DiscreteInputs { get; set; }
        public int InputRegisters { get; set; }
        public int Coils { get; set; }
    }

    public class NetworkConfiguration {
        public string? IPAddress { get; set; }
        public string? DNS { get; set; }
        public string? MAC { get; set; }
        public string? Gateway { get; set; }
        public int Port { get; set; }
        public int SlaveAddress { get; set; }
        public ModbusConfig? ModbusConfig { get; set; }
    }

    public class ModbusDevice {
        public int Id { get; set; }
        public string? Identifier { get; set; }
        public string? DisplayName { get; set; }
        public NetworkConfiguration? NetworkConfiguration { get; set; }
        public DeviceState State { get; set; }
        public string? Status { get; set; }
        public bool BypassAlarms { get; set; }
        public double ReadInterval { get; set; }
        public double SaveInterval { get; set; }
        public ICollection<Channel> Channels { get; set; } = new List<Channel>();
        public ICollection<FacilityZone> Zones { get; set; } = new List<FacilityZone>();
    }

    public class MonitoringBox:ModbusDevice {
        public ICollection<Module> Modules { get; set; } = new List<Module>();

    }
}
