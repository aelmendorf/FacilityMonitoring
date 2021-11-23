using Microsoft.EntityFrameworkCore;

namespace FacilityMonitoring.Infrastructure.Data.Model {
    public enum DiscreteState {
        High=1,Low=0
    }

    [Owned]
    public class ChannelAddress {
        public int Channel { get; set; }
        public int ModbusSlot { get; set; }
    }

    [Owned]
    public class ModbusAddress {
        public int Address { get; set; }
        public int RegisterLength { get; set; }
    }

    public abstract class Channel {
        public int Id { get; set; }
        public int SystemChannel { get; set; }
        public string? Identifier { get; set; }
        public string? DisplayName { get; set; }
        public bool IsVirtual { get; set; }
        public bool Connected { get; set; }
        public ChannelAddress? ChannelAddress { get; set; }
        public ModbusAddress? ModbusAddress { get; set; }
        public int ModbusDeviceId { get; set; }
        public ModbusDevice? ModbusDevice { get; set; }
        public ICollection<ChannelReading> Readings { get; set; }
    }

    public class DiscreteInput : Channel {
        public DiscreteState DiscreteState { get; set; }
        public DiscreteAlert? Alert { get; set; }
    }

    public class AnalogInput : Channel {
        public double CurrentValue { get; set; }
        public int SensorId { get; set; }
        public Sensor? Sensor { get; set; }
        public ICollection<AnalogAlert>? Alerts { get; set; }
    }

    public class DiscreteOutput:Channel {
        public DiscreteState StartState { get; set; }
        public DiscreteState ChannelState { get; set; }
    }
}
