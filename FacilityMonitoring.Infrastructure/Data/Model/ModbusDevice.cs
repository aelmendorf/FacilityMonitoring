using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FacilityMonitoring.Infrastructure.Data.Model {

    public enum DeviceState { OKAY,WARNING,ALARM,MAINTENANCE }

    [Owned]
    public class NetworkConfiguration {
        public string? IPAddress { get; set; }
        public string? DNS { get; set; }
        public string? MAC { get; set; }
        public string? Gateway { get; set; }
        public int Port { get; set; }
        public int SlaveAddress { get; set; }
    }

    public class ModbusDevice {
        public int Id { get; set; }
        public string? Identifier { get; set; }
        public string? DisplayName { get; set; }
        public NetworkConfiguration? NetworkConfiguration { get; set; }
        public int Port { get; set; }
        public int SlaveAddress { get; set; }
        public DeviceState State { get; set; }
        public string? Status { get; set; }
        public bool BypassAlarms { get; set; }
        public double ReadInterval { get; set; }
        public double SaveInterval { get; set; }
        ICollection<Channel>? Channels { get; set; }
    }

    public class MonitoringBox:ModbusDevice {
        public ICollection<Module>? Modules { get; set; }

    }
}
