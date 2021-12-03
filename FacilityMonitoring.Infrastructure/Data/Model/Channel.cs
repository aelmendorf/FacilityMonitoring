using Microsoft.EntityFrameworkCore;

namespace FacilityMonitoring.Infrastructure.Data.Model {
    public enum DiscreteState {
        High=1,Low=0
    }

    [Owned]
    public class ChannelAddress {
        public int Channel { get; set; }
        public int ModuleSlot { get; set; }
    }

    [Owned]
    public class ModbusAddress {
        public int Address { get; set; }
        public int RegisterLength { get; set; }
    }

    public abstract class Channel {
        public int Id { get; set; }
        public int SystemChannel { get; set; }
        public bool IsVirtual { get; set; }
        public bool Connected { get; set; }
        public string? Identifier { get; set; }
        public string? DisplayName { get; set; }
        public ChannelAddress? ChannelAddress { get; set; }
        public ModbusAddress? ModbusAddress { get; set; }
        public int ModbusDeviceId { get; set; }
        public ModbusDevice? ModbusDevice { get; set; }
        public ICollection<FacilityZone> Zones { get; set; } = new List<FacilityZone>();
    }


    public class DiscreteInput : Channel {
        public DiscreteAlert? DiscreteAlert { get; set; }
    }
    
    public class AnalogInput : Channel {
        public int SensorId { get; set; }
        public Sensor? Sensor { get; set; }
        public ICollection<AnalogAlert> AnalogAlerts { get; set; } = new List<AnalogAlert>();
    }

    public class DiscreteOutput:Channel {
        public DiscreteState ChannelState { get; set; } 
        public DiscreteState StartState { get; set; }
    }
}
