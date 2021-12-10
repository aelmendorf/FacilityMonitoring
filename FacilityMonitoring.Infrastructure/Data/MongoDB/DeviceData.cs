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
        public IList<CoilData> CoilData { get; set; } = new List<CoilData>();
       
    }

    public class AnalogData {
        public string Name { get; set; }
        public double Value { get; set; }
    }

    public class DiscreteData {
        public string Name { get; set; }
        public bool Value { get; set; }
    }
    public class CoilData {
        public string Name { get; set; }
        public bool Value { get; set; }
    }
}
