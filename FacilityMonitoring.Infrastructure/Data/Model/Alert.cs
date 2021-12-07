namespace FacilityMonitoring.Infrastructure.Data.Model {

    public class Alert {
        public int Id { get; set; }
        public bool Bypass { get; set; }
        public bool Enabled { get; set; }
        public int? FacilityActionId { get; set; }
        public FacilityAction FacilityAction { get; set; }
    }

    public class AnalogAlert:Alert {
        public double SetPoint { get; set; }
        public int? AnalogInputId { get; set; }
        public AnalogInput AnalogInput { get; set; }
    }

    public class DiscreteAlert : Alert {
        public DiscreteState TriggerOn { get; set; }
        public int? DiscreteInputId { get; set; }
        public DiscreteInput DiscreteInput { get; set; }

    }
}
