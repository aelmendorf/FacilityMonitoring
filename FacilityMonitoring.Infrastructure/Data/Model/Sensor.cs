using System.Collections.ObjectModel;

namespace FacilityMonitoring.Infrastructure.Data.Model {
    public class Sensor {
        public int Id { get; set; }
        public double ZeroValue { get; set; }
        public double Slope { get; set; }
        public double Factor { get; set; }
        public string? Units { get; set; }
        public ICollection<AnalogInput> AnalogInputs { get; set; } = new List<AnalogInput>();
    }
}
