using System;
using FacilityMonitoring.Infrastructure.Data.Model;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace FacilityMonitoring.ConsoleTesting {
    public class Program {
        static void Main(string[] args) {
            Console.WriteLine("DiscreteInputs: ");
            ParseDiscreteInputs();
            Console.WriteLine();
            Console.WriteLine("Network Configuration:");
            var config = ParseNetworkConfiguration();
            var outputs = ParseOutputs(new ModbusDevice());
            var actions = ParseActions(outputs);
        }

        static void CreateDevices() {
            using var context = new FacilityContext();
            NetworkConfiguration networkConfig = new NetworkConfiguration();
            networkConfig.IPAddress = "172.20.5.201";
            networkConfig.Port = 502;
            networkConfig.SlaveAddress = 0;
            networkConfig.Gateway = "172.20.3.5";
            networkConfig.MAC = "6052D0607093";


            MonitoringBox epi2 = new MonitoringBox();
            epi2.Identifier = "EpiLab2";

        }

        static IList<AnalogInput> ParseAnalogInputs(IList<FacilityAction> actions, ModbusDevice modDevice) {
            var aInputs = JArray.Parse(File.ReadAllText(@"C:\MonitorFiles\ANALOG.TXT"));
            IList<AnalogInput> analogInputs = aInputs.Select(p => CreateAnalogInput(p, actions,modDevice)).ToList();
            return analogInputs;
        }

        static AnalogInput CreateAnalogInput(JToken token,IList<FacilityAction> actions,ModbusDevice modDevice) {
            AnalogInput aInput = new AnalogInput();
            aInput.SystemChannel = token["Input"].Value<int>();
            aInput.ChannelAddress = new ChannelAddress();
            aInput.ChannelAddress.Channel = token["Address"]["Slot"] != null ? token["Address"]["Channel"].Value<int>() : -1;
            aInput.ChannelAddress.ModuleSlot = token["Address"]["Slot"] != null ? token["Address"]["Slot"].Value<int>() : -1;
            aInput.ModbusAddress = new ModbusAddress();
            aInput.ModbusAddress.Address = token["Coil"].Value<int>();
            aInput.ModbusAddress.RegisterLength = 1;
            aInput.Connected = token["Connected"].Value<bool>();
            aInput.ModbusDevice = modDevice;
            aInput.AnalogAlerts = new List<AnalogAlert>();
            int aTypeInt = token["A1"]["ActionType"].Value<int>();
            FacilityAction action1 = actions.FirstOrDefault(e => e.Id == token["A1"]["Action"].Value<int>());


            AnalogAlert alert1 = new AnalogAlert() {
                FacilityAction = action1,
                SetPoint = token["A1"]["Setpoint"].Value<int>()

            };
            return null;
        }

        static IList<FacilityAction> ParseActions(IList<DiscreteOutput> outputs) {
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
            action.Id = p["ActionId"].Value<int>();
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

        static IList<DiscreteOutput> ParseOutputs(ModbusDevice modbusDevice) {
            var outArr = JArray.Parse(File.ReadAllText(@"C:\MonitorFiles\OUTPUT.TXT"));
            IList<DiscreteOutput> outputs = outArr.Select(p => new DiscreteOutput { 
                ChannelAddress=new ChannelAddress() { 
                    Channel=p["Addr"]["Channel"].Value<int>(),
                    ModuleSlot = p["Addr"]["Module Slot"].Value<int>()
                },
                ModbusAddress=new ModbusAddress() {
                    Address=p["Register"].Value<int>(),
                    RegisterLength=1
                },
                SystemChannel = p["Output"].Value<int>(),
                IsVirtual=false,
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

        static void ParseDiscreteInputs() {
            JArray channelArray = JArray.Parse(File.ReadAllText(@"C:\MonitorFiles\DIGITAL.TXT"));
            List<DiscreteInput> inputs = new List<DiscreteInput>();

            foreach(var elem in channelArray) {
                DiscreteInput input = new DiscreteInput();
                input.SystemChannel= elem["Input"].Value<int>();
                input.ChannelAddress = new ChannelAddress();
                input.ChannelAddress.Channel = elem["Address"]["Slot"] != null ? elem["Address"]["Channel"].Value<int>() : -1;
                input.ChannelAddress.ModuleSlot = elem["Address"]["Slot"]!=null ? elem["Address"]["Slot"].Value<int>():-1;
                input.ModbusAddress = new ModbusAddress();
                input.ModbusAddress.Address = elem["Coil"].Value<int>();
                input.ModbusAddress.RegisterLength = 1;
                input.Connected = elem["Connected"].Value<bool>();
                inputs.Add(input);
            }
            foreach(var input in inputs) {
                Console.WriteLine("Input: {0} AddrChannel: {1} AddrSlot: {2}",input.SystemChannel,input.ChannelAddress.Channel,input.ChannelAddress.ModuleSlot);
            }
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
