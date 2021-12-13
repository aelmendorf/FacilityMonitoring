using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace FacilityMonitoring.Infrastructure.Data.MongoDB { 
    public class Device {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string DeviceName { get; set; }
        // [BsonRepresentation(BsonType.Array)]
        public IList<DeviceData> DeviceData { get; set; } = new List<DeviceData>();
    }
    public class DeviceData {
        public DateTime TimeStamp { get; set; }

        //[BsonRepresentation(BsonType.Array)]
        public IList<AnalogData> AnalogData { get; set; } = new List<AnalogData>();

        // [BsonRepresentation(BsonType.Array)]
        public IList<DiscreteData> DiscreteData { get; set; } = new List<DiscreteData>();

        //[BsonRepresentation(BsonType.Array)]
        public IList<VirtualData> CoilData { get; set; } = new List<VirtualData>();
        public IList<OutputData> OutputData { get; set; } = new List<OutputData>();
        public IList<ActionData> ActionData { get; set; } = new List<ActionData>();
    }

    public abstract class RegisterData {
        public string Name { get; set; }
    }

    public class AnalogData:RegisterData {
        public double Value { get; set; }
    }

    public class DiscreteData:RegisterData {
        public bool Value { get; set; }
    }
    public class VirtualData:RegisterData {
        public bool Value { get; set; }
    }

    public class OutputData : RegisterData {
        public bool Value { get; set; }
    }

    public class ActionData : RegisterData {
        public bool Value { get; set; }
    }

    public class AlertData {
        public string Channel { get; set; }
        public string Type { get; set; }
    }


}
