using FacilityMonitoring.Infrastructure.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacilityMonitoring.Infrastructure.Services;

namespace FacilityMonitoring.ConsoleTesting {
    public class Parsing {
        static async Task Main(string[] args) {
            //ParseConfiguation();
            TestModbus();
        }

        static async Task FixDisplayNames() {
            Console.WriteLine("Fixing Channel Names, Please Wait....");
            using var context = new FacilityContext();
            var monitoring = context.Devices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .FirstOrDefault();
            var dInputs = monitoring.Channels.OfType<OutputChannel>().OrderBy(e => e.SystemChannel).ToList();
            foreach (var dIn in dInputs) {
                dIn.Identifier = "Output " + dIn.SystemChannel;
                if (string.IsNullOrEmpty(dIn.DisplayName)) {
                    dIn.DisplayName = dIn.Identifier;
                } else if (dIn.DisplayName == "Not Set") {
                    dIn.DisplayName = dIn.Identifier;
                }
            }
            context.UpdateRange(dInputs);
            var ret = await context.SaveChangesAsync();
            if (ret > 0) {
                Console.WriteLine("Changes saved");
            } else {
                Console.WriteLine("Save Failed");
            }
            Console.WriteLine();
            Console.ReadKey();
        }

        static async Task TestModbus() {
            using var context = new FacilityContext();
            var monitoring = context.Devices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .FirstOrDefault(e=>e.Identifier=="Epi1");
            if (monitoring != null) {
                Console.WriteLine("Monitoring device found, performing read");
                var networkConfig = monitoring.NetworkConfiguration;
                networkConfig.ModbusConfig.SlaveAddress = 1;
                var result = await ModbusService.Read(networkConfig.IPAddress, 502, networkConfig.ModbusConfig);
                var channelMapping = networkConfig.ModbusConfig.ChannelMapping;
                ArraySegment<bool> discreteInputs = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.DiscreteStart, channelMapping.DiscreteStop - channelMapping.DiscreteStart);
                ArraySegment<bool> outputs = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.OutputStart, channelMapping.OutputStop - channelMapping.OutputStart);
                ArraySegment<bool> actions = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.ActionStart, channelMapping.ActionStop - channelMapping.ActionStart);
                ArraySegment<ushort> analogInputs = new ArraySegment<ushort>(result.InputRegisters, channelMapping.AnalogStart, channelMapping.AnalogStop - channelMapping.AnalogStart);
                ArraySegment<ushort> alerts = new ArraySegment<ushort>(result.HoldingRegisters, channelMapping.AlertStart, channelMapping.AlertStop - channelMapping.AlertStart);

                Console.WriteLine("DiscreteInputs:");
                for (int i = 0; i < discreteInputs.Count; i++) {
                    Console.WriteLine($"D[{i}]: {discreteInputs[i]}");
                }

                Console.WriteLine("Outputs:");
                for (int i = 0; i < outputs.Count; i++) {
                    Console.WriteLine($"Out[{i}]: {outputs[i]}");
                }

                Console.WriteLine("Actions:");
                for (int i = 0; i < actions.Count; i++) {
                    Console.WriteLine($"Action[{i}]: {actions[i]}");
                }

                Console.WriteLine("AnalogInputs:");
                for (int i = 0; i < analogInputs.Count; i++) {
                    Console.WriteLine($"A[{i}]: {analogInputs[i]}");
                }

                Console.WriteLine("VirtualInputs:");
                for (int i = 0; i < result.Coils.Length; i++) {
                    Console.WriteLine($"C[{i}]: {result.Coils[i]}");
                }

                Console.WriteLine("Alerts:");
                for (int i = 0; i < alerts.Count; i++) {
                    Console.WriteLine($"Alert[{i}]: {(ActionType)alerts[i]}");
                }
            } else {
                Console.WriteLine("Error: Could not find monitoring device");
            }
            Console.ReadKey();
        }

        static void ParseConfiguation() {
            Console.WriteLine("Creating ModbusDevice,Please wait..");
            //FacilityParser.CreateModbusDevices();
            //FacilityParser.CreateOutputs();
            //FacilityParser.CreateFacilityActions();
            //FacilityParser.CreateDiscreteInputs();
            //FacilityParser.CreateAnalogInputs();
            //FacilityParser.CreateVirtualInputs();
            Console.WriteLine("Press any key..");
            Console.ReadKey();
        }
    }
}
