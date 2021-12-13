using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace FacilityMonitoring.Infrastructure.Data.Model {

    public enum ActionType {
        Okay = 6,
        Alarm = 5,
        Warning = 4,
        SoftWarn = 3,
        Maintenance = 2,
        Custom = 1
    }

    [Owned]
    public class ActionOutput {
        public DiscreteOutput? Output { get; set; }
        public DiscreteState OnLevel { get; set; }
        public DiscreteState OffLevel{ get; set; }
    }

    public class FacilityAction {
        public int Id { get; set; }
        public string? ActionName { get; set; }
        public ModbusAddress? ModbusAddress { get; set; }
        public ActionType ActionType { get; set; }
        public ICollection<ActionOutput> ActionOutputs { get; set; } = new List<ActionOutput>();
        public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
    }
}
