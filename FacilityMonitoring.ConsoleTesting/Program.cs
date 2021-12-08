using System;
using FacilityMonitoring.Infrastructure.Data.Model;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FacilityMonitoring.ConsoleTesting {
    public class Program {
        static async Task Main(string[] args) {
            //CreateModbusDevices();
            //await CreateOutputs();
            //await CreateFacilityActions();
            //await CreateDiscreteInputs();
            //await CreateAnalogInputs();
            //await CreateVirtualInputs();
            //TestMongoInsert();
            await ReadCollection();

        }

        static async Task ReadCollection() {
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase("monitoring");
            var collection = database.GetCollection<BsonDocument>("data");
            var filter = Builders<BsonDocument>.Filter.Eq("_id",new BsonObjectId(new ObjectId("61b104432d4cacaf4d3e4164")));
            var data = await collection.Find(filter).FirstOrDefaultAsync();
            Console.WriteLine(data.ToString());
        }

        static void TestMongoInsert() {
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase("monitoring");
            var collectiion = database.GetCollection<BsonDocument>("data");
            var document = new BsonDocument {
                {"DeviceName","Epi2Monitoring" },
                {"data",new BsonDocument{
                    {"TimeStamp",DateTime.Now },
                    {"AnalogInputs",new BsonArray {
                        new BsonDocument{{"Name","Channel1"},{"Value",42.9}},
                        new BsonDocument{{"Name","Channel2"},{"Value",78.5}},
                        new BsonDocument{{"Name","Channel3"},{"Value",24.98}},
                        new BsonDocument{{"Name","Channel4"},{"Value",45.77}}
                    }},
                    {"DiscreteInputs",new BsonArray {
                        new BsonDocument{{"Name","Channel1"},{"Value",true}},
                        new BsonDocument{{"Name","Channel2"},{"Value",false}},
                        new BsonDocument{{"Name","Channel3"},{"Value",false}},
                        new BsonDocument{{"Name","Channel4"},{"Value",true}}
                    }},
                    {"Coils",new BsonArray {
                        new BsonDocument{{"Name","Channel1"},{"Value",false}},
                        new BsonDocument{{"Name","Channel2"},{"Value",true}},
                        new BsonDocument{{"Name","Channel3"},{"Value",false}},
                        new BsonDocument{{"Name","Channel4"},{"Value",true}}
                    }},
                  }
                }
            };
            collectiion.InsertOne(document);
        }
        static async Task CreateAnalogInputs() {
            Console.WriteLine("Creating EpiLab2 AnalogInputs");
            var context = new FacilityContext();
            var monitoringBox = context.ModbusDevices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .Include(e => e.Modules)
                .AsTracking()
                .FirstOrDefault();

            var facilityActions = context.FacilityActions
                .Include(e => e.ActionOutputs)
                    .ThenInclude(e => e.Output)
                .AsTracking()
                .ToList();

            if (monitoringBox != null && facilityActions.Count > 0) {
                Console.WriteLine("Found Monitoring Box: {0}", monitoringBox.DisplayName);
                var dInputs = await ParseAnalogInputs(facilityActions, monitoringBox);
                await context.AddRangeAsync(dInputs);
                var ret = await context.SaveChangesAsync();
                if (ret > 0) {
                    Console.WriteLine("DiscreteInputs should be added");
                } else {
                    Console.WriteLine("Error adding DiscreteInputs");
                }

            } else {
                Console.WriteLine("Error: Could not find monitoring box");
            }
        }
        static async Task CreateDiscreteInputs() {
            Console.WriteLine("Creating EpiLab2 DiscreteInputs");
            var context = new FacilityContext();
            var monitoringBox = context.ModbusDevices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .Include(e => e.Modules)
                .AsTracking()
                .FirstOrDefault();

            var actions = context.FacilityActions.AsTracking().ToList();

            if (monitoringBox != null) {
                Console.WriteLine("Found Monitoring Box: {0}", monitoringBox.DisplayName);
                var dInputs = await ParseDiscreteInputs(monitoringBox,actions);
                await context.AddRangeAsync(dInputs);
                var ret = await context.SaveChangesAsync();
                if (ret > 0) {
                    Console.WriteLine("DiscreteInputs should be added");
                } else {
                    Console.WriteLine("Error creating DiscreteInputs");
                }

            } else {
                Console.WriteLine("Error: Could not find monitoring box");
            }
        }
        static async Task CreateVirtualInputs() {
            Console.WriteLine("Creating EpiLab2 VirtualInputs");
            var context = new FacilityContext();
            var monitoringBox = context.ModbusDevices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .Include(e => e.Modules)
                .AsTracking()
                .FirstOrDefault();

            var actions = context.FacilityActions.AsTracking().ToList();

            if (monitoringBox != null) {
                Console.WriteLine("Found Monitoring Box: {0}", monitoringBox.DisplayName);
                var dInputs = await ParseVirtualChannels(monitoringBox, actions);
                await context.AddRangeAsync(dInputs);
                var ret = await context.SaveChangesAsync();
                if (ret > 0) {
                    Console.WriteLine("VirtualInputs should be added");
                } else {
                    Console.WriteLine("Error creating VirtualInputs");
                }

            } else {
                Console.WriteLine("Error: Could not find monitoring box");
            }
        }
        static async Task CreateOutputs() {
            Console.WriteLine("Creating EpiLab2 Outputs");
            var context = new FacilityContext();
            var monitoringBox = context.ModbusDevices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .Include(e => e.Modules)
                .AsTracking()
                .FirstOrDefault();

            if (monitoringBox != null) {
                Console.WriteLine("Found Monitoring Box: {0}", monitoringBox.DisplayName);
                var outputs = await ParseOutputs(monitoringBox);
                await context.AddRangeAsync(outputs);
                var ret = await context.SaveChangesAsync();
                if (ret > 0) {
                    Console.WriteLine("Outputs should be added");
                } else {
                    Console.WriteLine("Error adding channel outputs");
                }

            } else {
                Console.WriteLine("Error: Could not fing monitoring box");
            }
        }
        static void CreateModbusDevices() {
            using var context = new FacilityContext();
            var modules = context.Modules.ToList();
            MonitoringBox box = new MonitoringBox();
            NetworkConfiguration netConfig = new NetworkConfiguration();
            netConfig.IPAddress = "172.20.5.201";
            netConfig.DNS = "172.20.3.5";
            netConfig.Gateway = "172.20.5.1";
            netConfig.SlaveAddress = 0;
            netConfig.Port = 502;
            netConfig.MAC = "6052D0607093";
            ModbusConfig modbusConfig = new ModbusConfig();
            modbusConfig.DiscreteInputs = 54;
            modbusConfig.InputRegisters = 16;
            modbusConfig.Coils = 4;
            netConfig.ModbusConfig = modbusConfig;
            box.NetworkConfiguration = netConfig;
            box.Identifier = "Epi2";
            box.DisplayName = "EpiLab2";
            box.State = DeviceState.OKAY;
            box.Status = "Normal";
            box.BypassAlarms = false;
            box.ReadInterval = 5;
            box.SaveInterval = 10;
            box.Modules = context.Modules.ToList();
            context.ModbusDevices.Add(box);
            var ret=context.SaveChanges();
            if (ret > 0) {
                Console.WriteLine("Monitoring Boxx added");
            } else {
                Console.WriteLine("Failed to add Monitoring Box");
            }
        }
        static async Task CreateFacilityActions() {
            Console.WriteLine("Creating Monitoring Box EpiLab2");
            var context = new FacilityContext();
            var outputs = context.Channels.OfType<DiscreteOutput>().AsTracking().ToList();

            if (outputs.Count>0) {
                var actions = await ParseActions(outputs);
                await context.AddRangeAsync(actions);
                var ret = await context.SaveChangesAsync();
                if (ret > 0) {
                    Console.WriteLine("FacilityActions should be created");
                } else {
                    Console.WriteLine("Error creating FacilityActions");
                }
            } else {
                Console.WriteLine("Error: Not outputs found");
            }
        }
        static NetworkConfiguration ParseNetworkConfiguration() {
            var arr = JArray.Parse(File.ReadAllText(@"C:\MonitorFiles\NETWORK.TXT"));
            NetworkConfiguration netConfig = new NetworkConfiguration();
            netConfig.ModbusConfig = new ModbusConfig();
            var config = arr[0];
            netConfig.IPAddress = config["IP"].Value<string>();
            netConfig.DNS = config["DNS"].Value<string>();
            netConfig.MAC = config["Mac"].Value<string>();
            netConfig.Gateway = config["Gateway"].Value<string>();
            netConfig.ModbusConfig.DiscreteInputs = config["DiscreteInputs"].Value<int>();
            netConfig.ModbusConfig.InputRegisters = config["InputRegsters"].Value<int>();
            netConfig.ModbusConfig.Coils = config["Coils"].Value<int>();

            Console.WriteLine("IP: {0} DNS: {1}", netConfig.IPAddress, netConfig.DNS);
            return netConfig;
        }
        static async Task<IList<DiscreteInput>> ParseDiscreteInputs(ModbusDevice modbusDevice,IList<FacilityAction> actions) {
            JArray channelArray = JArray.Parse(File.ReadAllText(@"C:\MonitorFiles\DIGITAL.TXT"));
            List<DiscreteInput> inputs = new List<DiscreteInput>();
            foreach(var elem in channelArray) {
                DiscreteInput input = new DiscreteInput();
                input.SystemChannel= elem["Input"].Value<int>();
                input.ChannelAddress = new ChannelAddress();
                input.ChannelAddress.Channel = elem["Address"]["Slot"].Value<int>();
                input.ChannelAddress.ModuleSlot = elem["Address"]["Slot"].Value<int>();
                input.ModbusAddress = new ModbusAddress();
                input.ModbusAddress.Address = elem["Coil"].Value<int>();
                input.ModbusAddress.RegisterLength = 1;
                input.Connected = elem["Connected"].Value<bool>();
                input.ModbusDevice = modbusDevice;
                input.ModbusDeviceId = modbusDevice.Id;
                input.DiscreteAlert = new DiscreteAlert();
                input.DiscreteAlert.TriggerOn = elem["Alert"]["TriggerOn"].Value<int>() == 1 ? DiscreteState.High : DiscreteState.Low;
                int actionId = elem["Alert"]["Action"].Value<int>()+1;
                if (actionId >= 1) {
                    input.DiscreteAlert.FacilityActionId = actionId;
                }
                input.DiscreteAlert.Bypass = elem["Alert"]["Bypass"].Value<bool>();
                input.DiscreteAlert.Enabled = elem["Alert"]["Enabled"].Value<bool>();
                
                inputs.Add(input);
            }
            foreach(var input in inputs) {
                Console.WriteLine("Input: {0} AddrChannel: {1} AddrSlot: {2}",input.SystemChannel,input.ChannelAddress.Channel,input.ChannelAddress.ModuleSlot);
            }
            return inputs;
        }
        static async Task<IList<FacilityAction>> ParseActions(IList<DiscreteOutput> outputs) {
            var actionArr = JArray.Parse(File.ReadAllText(@"C:\MonitorFiles\ACTIONS.TXT"));
            IList<FacilityAction> actions = actionArr.Select(p => CreateFacilityAction(p,outputs)).ToList();
            foreach(var action in actions) {
                Console.WriteLine("ActionName: {0}, ActionType: {1}  ActionOutputs",action.ActionName,action.ActionType);
                foreach(var aoutput in action.ActionOutputs) {
                    if (aoutput.Output is null) {
                        Console.WriteLine("Output: {0} No Output",aoutput.OffLevel);
                    } else {
                        Console.WriteLine("Output: {0} AddrChannel: {1} AddrSlot: {2}",
                            aoutput.Output.SystemChannel, aoutput.Output.ChannelAddress.Channel, aoutput.Output.ChannelAddress.ModuleSlot);
                    }
                }
            }
            return actions;
        }
        static FacilityAction CreateFacilityAction(JToken p,IList<DiscreteOutput> outputs) {
            FacilityAction action = new FacilityAction();
            action.Id = p["ActionId"].Value<int>()+1;
            action.ActionName = ToActionType(p["ActionType"].Value<int>()).ToString();
            action.ActionOutputs = new List<ActionOutput>();

            ChannelAddress addr1 = new ChannelAddress() {
                Channel = p["O1"]["Address"]["Channel"].Value<int>(),
                ModuleSlot = p["O1"]["Address"]["Slot"].Value<int>()
            };
            ChannelAddress addr2 = new ChannelAddress() {
                Channel = p["O2"]["Address"]["Channel"].Value<int>(),
                ModuleSlot = p["O2"]["Address"]["Slot"].Value<int>()
            };
            ChannelAddress addr3 = new ChannelAddress() {
                Channel = p["O3"]["Address"]["Channel"].Value<int>(),
                ModuleSlot = p["O3"]["Address"]["Slot"].Value<int>()
            };

            var out1 = outputs.FirstOrDefault(e => e.ChannelAddress.Channel == addr1.Channel && e.ChannelAddress.ModuleSlot == addr1.ModuleSlot);
            var out2 = outputs.FirstOrDefault(e => e.ChannelAddress.Channel == addr2.Channel && e.ChannelAddress.ModuleSlot == addr2.ModuleSlot);
            var out3 = outputs.FirstOrDefault(e => e.ChannelAddress.Channel == addr3.Channel && e.ChannelAddress.ModuleSlot == addr3.ModuleSlot);

            ActionOutput aout1 = new ActionOutput() {
                Output = out1,
                OnLevel = p["O1"]["OnLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
                OffLevel = p["O1"]["OffLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
            };
            ActionOutput aout2 = new ActionOutput() {
                Output = out2,
                OnLevel = p["O2"]["OnLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
                OffLevel = p["O2"]["OffLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
            };
            ActionOutput aout3 = new ActionOutput() {
                Output = out3,
                OnLevel = p["O3"]["OnLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
                OffLevel = p["O3"]["OffLevel"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High,
            };
            action.ActionOutputs.Add(aout1);
            action.ActionOutputs.Add(aout2);
            action.ActionOutputs.Add(aout3);
            action.ActionType = ToActionType(p["ActionType"].Value<int>());
            return action;
        }        
        static async Task<IList<AnalogInput>> ParseAnalogInputs(IList<FacilityAction> actions, ModbusDevice modDevice) {
            using var context = new FacilityContext();
            var sensor = context.Sensors.FirstOrDefault(e => e.Name == "H2 Detector-PPM");
            if (sensor != null) {
                Console.WriteLine("Sensor Found!");
                Console.WriteLine("Name: {0} Slope: {1} Offset: {2}", sensor.Name, sensor.Slope, sensor.Offset);
            }
            var aInputs = JArray.Parse(File.ReadAllText(@"C:\MonitorFiles\ANALOG.TXT"));
            IList<AnalogInput> analogInputs = aInputs.Select(p => CreateAnalogInput(p, actions,modDevice,sensor)).ToList();
            Console.WriteLine("AnalogInputs: ");
            int count = 0;
            foreach(var aInput in analogInputs) {
                if (aInput.Sensor != null) {
                    Console.WriteLine("Input: {0} Addr: {1} Slot: {2} Sensor: {3} Slope: {4} ", count, aInput.ChannelAddress.Channel, aInput.ChannelAddress.ModuleSlot,aInput.Sensor.Name, aInput.Sensor.Slope);
                } else {
                    Console.WriteLine("Input: {0} Addr: {1} Slot: {2} Sensor: No Sensor ", count, aInput.ChannelAddress.Channel, aInput.ChannelAddress.ModuleSlot);
                }

                count++;
                foreach (var alert in aInput.AnalogAlerts) {
                    if (alert.FacilityAction != null) {
                        Console.WriteLine(" ActionId: {0} ActionType {1}", alert.FacilityActionId, alert.FacilityAction.ActionType);
                    } else {
                        Console.WriteLine("Alert Bypassed: {0}", alert.Bypass);
                    }
                }
            }
            return analogInputs;
        }
        static AnalogInput CreateAnalogInput(JToken token,IList<FacilityAction> actions,ModbusDevice modDevice,Sensor sensor) {
            AnalogInput aInput = new AnalogInput();
            aInput.SystemChannel = token["Input"].Value<int>();
            aInput.ChannelAddress = new ChannelAddress();
            aInput.ChannelAddress.Channel = token["Address"]["Channel"].Value<int>();
            aInput.ChannelAddress.ModuleSlot = token["Address"]["Slot"].Value<int>();
            aInput.Identifier = "System Channel: " + aInput.SystemChannel;
            aInput.DisplayName = "Not Set";
            aInput.ModbusAddress = new ModbusAddress();
            aInput.ModbusAddress.Address = token["Register"].Value<int>();
            aInput.ModbusAddress.RegisterLength = 1;
            aInput.Connected = token["Connected"].Value<bool>();
            aInput.ModbusDevice = modDevice;
            aInput.AnalogAlerts = new List<AnalogAlert>();

            int a1 = token["A1"]["Action"].Value<int>()+1;
            int a2 = token["A1"]["Action"].Value<int>()+1;
            int a3 = token["A1"]["Action"].Value<int>()+1;

            FacilityAction action1 = actions.FirstOrDefault(e => e.Id ==a1);
            FacilityAction action2 = actions.FirstOrDefault(e => e.Id == a2);
            FacilityAction action3 = actions.FirstOrDefault(e => e.Id == a3);

            AnalogAlert alert1 = new AnalogAlert() {
                FacilityAction = action1,
                SetPoint = token["A1"]["Setpoint"].Value<int>(),
                Bypass= token["A1"]["Bypass"].Value<bool>(),
                Enabled = token["A1"]["Enabled"].Value<bool>(),
            };

            AnalogAlert alert2 = new AnalogAlert() {
                FacilityAction = action2,
                SetPoint = token["A2"]["Setpoint"].Value<int>(),
                Bypass = token["A2"]["Bypass"].Value<bool>(),
                Enabled = token["A2"]["Enabled"].Value<bool>(),
            };

            AnalogAlert alert3 = new AnalogAlert() {
                FacilityAction = action3,
                SetPoint = token["A3"]["Setpoint"].Value<int>(),
                Bypass = token["A3"]["Bypass"].Value<bool>(),
                Enabled = token["A3"]["Enabled"].Value<bool>(),
            };

            aInput.AnalogAlerts.Add(alert1);
            aInput.AnalogAlerts.Add(alert2);
            aInput.AnalogAlerts.Add(alert3);
            if (sensor != null) {
                if (aInput.Connected) {
                    aInput.SensorId = sensor.Id;
                }
            }
            return aInput;
        }
        static async Task<IList<DiscreteOutput>> ParseOutputs(ModbusDevice modbusDevice) {
            var outArr = JArray.Parse(File.ReadAllText(@"C:\MonitorFiles\OUTPUT.TXT"));
            IList<DiscreteOutput> outputs = outArr.Select(p => new DiscreteOutput { 
                ModbusDevice=modbusDevice,
                ModbusDeviceId=modbusDevice.Id,
                ChannelAddress=new ChannelAddress() { 
                    Channel=p["Addr"]["Channel"].Value<int>(),
                    ModuleSlot = p["Addr"]["Module Slot"].Value<int>()
                },
                ModbusAddress=new ModbusAddress() {
                    Address=p["Register"].Value<int>(),
                    RegisterLength=1
                },
                SystemChannel = p["Output"].Value<int>(),
                Connected=p["Connected"].Value<bool>(),
                Identifier="Channel " + p["Output"].Value<int>().ToString(),
                DisplayName="Not Set",
                StartState=p["Start State"].Value<int>()==0 ? DiscreteState.Low:DiscreteState.High,
                ChannelState= p["Start State"].Value<int>() == 0 ? DiscreteState.Low : DiscreteState.High
            }).ToList();
            Console.WriteLine("DiscreteOutputs: ");
            foreach (var output in outputs) {
                Console.WriteLine("Input: {0} AddrChannel: {1} AddrSlot: {2}", output.SystemChannel, output.ChannelAddress.Channel, output.ChannelAddress.ModuleSlot);
            }
            return outputs;
        }
        static async Task<IList<VirtualInput>> ParseVirtualChannels(ModbusDevice modbusDevice, IList<FacilityAction> actions) {
            var vInputs = JArray.Parse(File.ReadAllText(@"C:\MonitorFiles\VIRTUAL.TXT"));
            IList<VirtualInput> virtualInputs = vInputs.Select(e => CreateVirtualChannel(e, modbusDevice, actions)).ToList();
            foreach(var input in virtualInputs) {
                Console.WriteLine("VirtualInput: {0} ActionId: {1}",input.SystemChannel,input.DiscreteAlert.FacilityActionId);
            }
            return virtualInputs;
        }

        static VirtualInput CreateVirtualChannel(JToken token,ModbusDevice modbusDevice, IList<FacilityAction> actions) {
            VirtualInput vInput = new VirtualInput();
            vInput.SystemChannel = token["Input"].Value<int>();
            vInput.ChannelAddress = new ChannelAddress();
            vInput.ChannelAddress.Channel = 0;
            vInput.ChannelAddress.ModuleSlot = 0;
            vInput.ModbusAddress = new ModbusAddress();
            vInput.ModbusAddress.Address = token["Coil"].Value<int>();
            vInput.ModbusAddress.RegisterLength = 1;
            vInput.Connected = token["Connected"].Value<bool>();
            vInput.ModbusDevice = modbusDevice;
            vInput.ModbusDeviceId = modbusDevice.Id;
            vInput.DiscreteAlert = new DiscreteAlert();
            vInput.DiscreteAlert.TriggerOn = token["Alert"]["TriggerOn"].Value<int>() == 1 ? DiscreteState.High : DiscreteState.Low;
            int actionId = token["Alert"]["Action"].Value<int>() + 1;
            if (actionId >= 1) {
                vInput.DiscreteAlert.FacilityActionId = actionId;
            }
            vInput.DiscreteAlert.Bypass = token["Alert"]["Bypass"].Value<bool>();
            vInput.DiscreteAlert.Enabled = token["Alert"]["Enabled"].Value<bool>();
            return vInput;
        }
        static ActionType ToActionType(int i) {
            switch(i) {
                case 1: {
                    return ActionType.Custom;
                }
                case 2: {
                    return ActionType.Maintenance;
                }
                case 3: {
                    return ActionType.SoftWarn;
                }
                case 4: {
                    return ActionType.Warning;
                }
                case 5: {
                    return ActionType.Alarm;
                }
                case 6: {
                    return ActionType.Okay;
                }
                default: {
                    return ActionType.Okay;
                }
            }
        }
    }
}
