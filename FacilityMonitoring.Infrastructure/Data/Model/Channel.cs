using Microsoft.EntityFrameworkCore;

namespace FacilityMonitoring.Infrastructure.Data.Model {
    public enum DiscreteState {
        High=1,Low=0
    }

    public enum ModbusRegister {
        DiscreteInput,
        Input,
        Holding,
        Coil
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
        public ModbusRegister RegisterType { get; set; }
    }

    public abstract class Channel {
        public int Id { get; set; }
        public int SystemChannel { get; set; }
        public string Identifier { get; set; }
        public string DisplayName { get; set; }
        public bool Connected { get; set; }
        public bool Bypass { get; set; }
        public bool Display { get; set; }
        public ModbusAddress ModbusAddress { get; set; }
        public ChannelAddress ChannelAddress { get; set; }
        public int ModbusDeviceId { get; set; }
        public ModbusDevice ModbusDevice { get; set; }
        public ICollection<FacilityZone> Zones { get; set; } = new List<FacilityZone>();
    }

    public class InputChannel:Channel {
        public Alert Alert { get; set; }
    }

    public class OutputChannel:Channel {

    }

    public class DiscreteInput : InputChannel {
        //public DiscreteAlert DiscreteAlert { get; set; }
    }
    
    public class AnalogInput : InputChannel {
        //public AnalogAlert AnalogAlert { get; set; }
        public int? SensorId { get; set; }
        public Sensor Sensor { get; set; }
    }

    public class DiscreteOutput: OutputChannel {
        public DiscreteState StartState { get; set; }
    }

    public class VirtualInput : InputChannel {
       // public DiscreteAlert VirtualAlert { get; set; }
    }
}
