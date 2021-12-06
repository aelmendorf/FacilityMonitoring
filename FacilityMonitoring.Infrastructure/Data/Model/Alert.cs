namespace FacilityMonitoring.Infrastructure.Data.Model {

    public class Alert {
        public int Id { get; set; }
        public bool Bypass { get; set; }
        public bool Enabled { get; set; }
        public int? FacilityActionId { get; set; }
        public Channel Channel { get; set; }
        public FacilityAction FacilityAction { get; set; }
    }

    public class AnalogAlert:Alert {
        public double SetPoint { get; set; }
    }

    public class DiscreteAlert : Alert {
        public DiscreteState TriggerOn { get; set; }
    }
}
