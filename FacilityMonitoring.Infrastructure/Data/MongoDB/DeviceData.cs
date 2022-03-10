using MongoDB.Entities;

namespace FacilityMonitoring.Infrastructure.Data.MongoDB {

    //public enum DeviceType {
    //    Modbus,
    //    BacNet,
    //    API
    //}
    //public class MonitoringDevice:Entity {
    //    public string DeviceName { get; set; }
    //    public Many<DataConfiguration> DataConfigurations { get; set; }
    //    public Many<DataRecord> DeviceData { get; set; }
    //    public MonitoringDevice() {
    //        this.InitOneToMany(() => DeviceData);
    //        this.InitOneToMany(() => DataConfigurations);
    //    }
    //}

    //public class DataConfiguration:Entity {
    //    public Many<DataRecord> DataRecords { get; set; }
    //    public One<MonitoringDevice> MonitoringDevice { get; set; }
    //    public int Iteration { get; set; }
    //    public List<DataConfig> AnalogConfig { get; set; }
    //    public List<DataConfig> DiscreteConfig { get; set; }
    //    public List<DataConfig> VirtualConfig { get; set; }
    //    public List<DataConfig> OutputConfig { get; set; }
    //    public List<DataConfig> AlertConfig { get; set; }
    //    public List<DataConfig> ActionConfig { get; set; }
    //    public DataConfig DeviceConfig { get; set; }
    //    public DataConfiguration() {
    //        this.InitOneToMany(() => DataRecords);
    //    }
    //}

    //public class DataConfig {
    //    public string Name { get; set; }
    //    public int AlertIndex { get; set; }
    //    public int DataIndex { get; set; }
    //    public bool Display { get; set; }
    //    public bool Enabled { get; set; }
    //    public bool Bypass { get; set; }
    //}

    //public class DataRecord : Entity {
    //    public One<DataConfiguration> DataConfig { get; set; }
    //    public int DataConfigIteration { get; set; }
    //    public DateTime TimeStamp { get; set; }
    //    public ushort[] AnalogInputs { get; set; }
    //    public ushort[] Alerts { get; set; }
    //    public bool[] DiscreteInputs { get; set; }
    //    public bool[] VirtualInputs { get; set; }
    //    public bool[] Outputs { get; set; }
    //    public bool[] Actions { get; set; }
    //    public int DeviceState { get; set; }
    //}
}
