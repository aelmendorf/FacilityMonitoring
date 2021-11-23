using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityMonitoring.Infrastructure.Data.Model {
    public class ChannelReading {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public int ChannelId { get; set; }
        public Channel Channel { get; set; }
    }

    public class DiscreteReading { 
        public bool Value { get; set; }
    }

    public class AnalogReading {
        public double Value { get; set; }
    }
}
