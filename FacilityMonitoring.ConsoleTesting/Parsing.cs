using FacilityMonitoring.Infrastructure.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacilityMonitoring.Infrastructure.Services;
using MongoDB.Entities;
using FacilityMonitoring.Infrastructure.Data.MongoDB;
using MongoDB.Driver.Linq;

namespace FacilityMonitoring.ConsoleTesting {
    public class Parsing {
        static async Task Main(string[] args) {
            //ParseConfiguation();

            //await TestModbus();
            //await FixDiscreteNames("Epi1");
            //await FixAnalogNames("Epi1");
            //await FixOutputNames("Epi1");
            //await SetAlertNames();
            await TestModbusWithNames();
            //await CreateDisplayConfig();
            //await GetDataRecordsTest();
            //await LogDataTest();
            //await AlertEmailTest();
            //await TestDeviceController();
            //await TestingMongo();
            //await DB.InitAsync("monitoring_v2", "172.20.3.30", 27017);
            //await DB.DeleteAsync<DisplayConfig>("61fd79a9b37b0a184a9f2372");
            //Console.WriteLine("Check the database");
            //await CreateDisplayConfig();
            //await LogDataTest();
            //var context = new FacilityContext();
            //IMonitoringBoxRepo repo = new FacilityRepository(context);
            //var e1RegCounts=repo.UpdateChannelMapping("Epi1");
            //var e2RegCounts = repo.UpdateChannelMapping("Epi2");
            //var e1Mapping = repo.UpdateChannelRegisterMap("Epi1");
            //var e2Mapping = repo.UpdateChannelRegisterMap("Epi2");

            //Console.WriteLine("Epi 1 Register Counts:");
            //Console.WriteLine($"Holding {e1RegCounts.holdingCount} Inputs: {e1RegCounts.inputCount} Discrete: {e1RegCounts.discreteCount} Coil: {e1RegCounts.coilCount}");
            //Console.WriteLine();
            //Console.WriteLine("Epi 2 Register Counts:");
            //Console.WriteLine($"Holding {e2RegCounts.holdingCount} Inputs: {e2RegCounts.inputCount} Discrete: {e2RegCounts.discreteCount} Coil: {e2RegCounts.coilCount}");
            //Console.WriteLine(); Console.WriteLine();
            //Console.WriteLine("Epi 1 Channel Mapping");
            //Console.WriteLine($"AnalogStart {e1Mapping.AnalogStart} AnalogStop: {e1Mapping.AnalogStop}");
            //Console.WriteLine($"DiscreteStart {e1Mapping.DiscreteStart} DiscreteStop: {e1Mapping.DiscreteStop}");
            //Console.WriteLine($"OutputStart {e1Mapping.OutputStart} OutputStop: {e1Mapping.OutputStop}");
            //Console.WriteLine($"VirtualStart {e1Mapping.VirtualStart} VirtualStop: {e1Mapping.VirtualStop}");
            //Console.WriteLine($"AlertStart {e1Mapping.AlertStart} AlertStop: {e1Mapping.AlertStop}");
            //Console.WriteLine($"ActionStart {e1Mapping.ActionStart} ActionStop: {e1Mapping.AlertStop}");

            //Console.WriteLine(); Console.WriteLine();
            //Console.WriteLine("Epi 2 Channel Mapping");
            //Console.WriteLine($"AnalogStart {e2Mapping.AnalogStart} AnalogStop: {e2Mapping.AnalogStop}");
            //Console.WriteLine($"DiscreteStart {e2Mapping.DiscreteStart} DiscreteStop: {e2Mapping.DiscreteStop}");
            //Console.WriteLine($"OutputStart {e2Mapping.OutputStart} OutputStop: {e2Mapping.OutputStop}");
            //Console.WriteLine($"VirtualStart {e2Mapping.VirtualStart} VirtualStop: {e2Mapping.VirtualStop}");
            //Console.WriteLine($"AlertStart {e2Mapping.AlertStart} AlertStop: {e2Mapping.AlertStop}");
            //Console.WriteLine($"ActionStart {e2Mapping.ActionStart} ActionStop: {e2Mapping.AlertStop}");

            //Console.ReadKey();

            //using var context = new FacilityContext();
            //var device = context.Devices.OfType<MonitoringBox>().FirstOrDefault(e => e.Identifier == "Epi2");

            //ChannelRegisterMapping channelMapping = new ChannelRegisterMapping();

            //channelMapping.AnalogRegisterType = ModbusRegister.Input;
            //channelMapping.AnalogStart = 0;
            //channelMapping.AnalogStop = 15;

            //channelMapping.DiscreteRegisterType = ModbusRegister.DiscreteInput;
            //channelMapping.DiscreteStart = 0;
            //channelMapping.DiscreteStop = 39;

            //channelMapping.OutputRegisterType = ModbusRegister.DiscreteInput;
            //channelMapping.OutputStart = 40;
            //channelMapping.OutputStop = 47;

            //channelMapping.ActionRegisterType = ModbusRegister.DiscreteInput;
            //channelMapping.ActionStart = 48;
            //channelMapping.ActionStop = 53;

            //channelMapping.VirtualRegisterType = ModbusRegister.Coil;
            //channelMapping.VirtualStart = 0;
            //channelMapping.VirtualStop = 3;

            //channelMapping.AlertRegisterType = ModbusRegister.Holding;
            //channelMapping.AlertStart = 0;
            //channelMapping.AlertStop = 59;

            //channelMapping.DeviceRegisterType = ModbusRegister.Holding;
            //channelMapping.DeviceStart = 60;
            //channelMapping.DeviceStop = 60;

            //device.NetworkConfiguration.ModbusConfig.ChannelMapping=channelMapping;
            //context.Update(device);
            //context.SaveChanges();
            //Console.WriteLine("Check Database");
            //Console.ReadKey();

        }

        static async Task LogDataTest() {
            await DB.InitAsync("monitoring_debug", "172.20.3.30", 27017);
            using var context = new FacilityContext();
            var monitoring = context.Devices.OfType<MonitoringBox>()
                .FirstOrDefault(e => e.Identifier == "Epi1");
            if (monitoring != null) {
                Console.WriteLine($"Epi1 Box found, DataRef: {monitoring.DataReference}");
                var dataDevice = await DB.Find<MonitoringDevice>().OneAsync(monitoring.DataReference);
                var networkConfig = monitoring.NetworkConfiguration;
                networkConfig.ModbusConfig.SlaveAddress = 1;
                var channelMapping = networkConfig.ModbusConfig.ChannelMapping;
                if (dataDevice != null) {
                    Console.WriteLine($"Data device found,logging started, Id: {dataDevice.ID}");
                    var dataConfig = dataDevice.DataConfigurations.FirstOrDefault(e => e.Iteration == 1);
                    do {
                        var result = await ModbusService.Read(networkConfig.IPAddress, networkConfig.Port, networkConfig.ModbusConfig);
                        var discreteInputs = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.DiscreteStart, (channelMapping.DiscreteStop - channelMapping.DiscreteStart) + 1).ToArray();
                        var outputs = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.OutputStart, (channelMapping.OutputStop - channelMapping.OutputStart) + 1).ToArray();
                        var actions = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.ActionStart, (channelMapping.ActionStop - channelMapping.ActionStart) + 1).ToArray();
                        var analogInputs = new ArraySegment<ushort>(result.InputRegisters, channelMapping.AnalogStart, (channelMapping.AnalogStop - channelMapping.AnalogStart) + 1).ToArray();
                        var alerts = new ArraySegment<ushort>(result.HoldingRegisters, channelMapping.AlertStart, (channelMapping.AlertStop - channelMapping.AlertStart) + 1).ToArray();
                        var devState = result.HoldingRegisters[channelMapping.DeviceStart];
                        DataRecord data = new DataRecord();
                        data.DiscreteInputs = discreteInputs;
                        data.Outputs = outputs;
                        data.Actions = actions;
                        data.Alerts = alerts;
                        data.AnalogInputs = analogInputs;
                        data.DeviceState = devState;
                        data.VirtualInputs = result.Coils;
                        data.DataConfigIteration = dataConfig!=null ? dataConfig.Iteration:0;
                        data.TimeStamp = DateTime.Now;
                        await data.SaveAsync();
                        await dataDevice.DeviceData.AddAsync(data);
                        await dataDevice.SaveAsync();
                        Console.WriteLine("Press Q to quit, any other key to continue");
                    } while (Console.ReadKey().Key != ConsoleKey.Q);
                    Console.WriteLine("Completed");
                } else {
                    Console.WriteLine("Error: Could not find MongoDB device");
                }
            } else {
                Console.WriteLine("Error: Could not find monitoring device");
            }
        }

        static async Task TestDeviceController() {
            //using var context = new FacilityContext();
            //await DB.InitAsync("monitoring", "172.20.3.30", 27017);
            //IDataRecordService dataService = new DataRecordService();
            //IDeviceController controller = new DeviceController(dataService, context);
            //await controller.Load("Epi1");
            //await controller.Read();
            //Console.WriteLine("Check database");
            //Console.ReadKey();
        }

        static async Task GetDataRecordsTest() {
            await DB.InitAsync("monitoring_debug", "172.20.3.30", 27017);
            using var context = new FacilityContext();
            var monitoring = context.Devices.OfType<MonitoringBox>()
                .FirstOrDefault(e => e.Identifier == "Epi2");
            if (monitoring != null) {
                //var dev = await DB.Find<MonitoringDevice>().OneAsync(monitoring.DataReference);
                var dev = await (from a in DB.Queryable<MonitoringDevice>()
                                 where a.ID == monitoring.DataReference
                                 select a).FirstOrDefaultAsync();
                if (dev != null) {
                    var devData = (from a in dev.DeviceData.ChildrenQueryable()
                                  select a).ToList();
                    foreach(var data in devData) {
                        Console.Write(data.TimeStamp.ToString()+" :");
                        if (data.AnalogInputs != null) {
                            foreach (var ain in data.AnalogInputs) {
                                Console.Write($" {ain},");
                            }
                        } else {
                            Console.Write(" null");
                        }

                        Console.WriteLine();
                    }
                } else {
                    Console.WriteLine("Error Finding ");
                }


            } else {
                Console.WriteLine("Error: Monitoring Box not found");
            }
        }
        static async Task AlertEmailTest() {
            await DB.InitAsync("monitoring_debug", "172.20.3.30", 27017);
            using var context = new FacilityContext();
            var monitoring = context.Devices.OfType<MonitoringBox>()
                .FirstOrDefault(e => e.Identifier == "Epi1");
            if (monitoring != null) {
                Console.WriteLine($"Epi1 Box found, DataRef: {monitoring.DataReference}");
                var dataDevice = await DB.Find<MonitoringDevice>().OneAsync(monitoring.DataReference);
                var networkConfig = monitoring.NetworkConfiguration;
                networkConfig.ModbusConfig.SlaveAddress = 1;
                var channelMapping = networkConfig.ModbusConfig.ChannelMapping;
                if (dataDevice != null) {
                    Console.WriteLine($"Data device found,logging started, Id: {dataDevice.ID}");
                    var dataConfig = dataDevice.DataConfigurations.FirstOrDefault(e => e.Iteration == 1);
                    var result = await ModbusService.Read(networkConfig.IPAddress, networkConfig.Port, networkConfig.ModbusConfig);
                    var discreteInputs = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.DiscreteStart, (channelMapping.DiscreteStop - channelMapping.DiscreteStart) + 1).ToArray();
                    var outputs = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.OutputStart, (channelMapping.OutputStop - channelMapping.OutputStart) + 1).ToArray();
                    var actions = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.ActionStart, (channelMapping.ActionStop - channelMapping.ActionStart) + 1).ToArray();
                    var analogInputs = new ArraySegment<ushort>(result.InputRegisters, channelMapping.AnalogStart, (channelMapping.AnalogStop - channelMapping.AnalogStart) + 1).ToArray();
                    var alerts = new ArraySegment<ushort>(result.HoldingRegisters, channelMapping.AlertStart, (channelMapping.AlertStop - channelMapping.AlertStart) + 1).ToArray();
                    var devState = result.HoldingRegisters[channelMapping.DeviceStart];
                    DataRecord data = new DataRecord();
                    data.DiscreteInputs = discreteInputs;
                    data.Outputs = outputs;
                    data.Actions = actions;
                    data.Alerts = alerts;
                    data.AnalogInputs = analogInputs;
                    data.DeviceState = devState;
                    data.VirtualInputs = result.Coils;
                    data.DataConfigIteration = dataConfig != null ? dataConfig.Iteration : 0;
                    data.TimeStamp = DateTime.Now;
                    await data.SaveAsync();
                    await dataDevice.DeviceData.AddAsync(data);
                    await dataDevice.SaveAsync();
                    Console.WriteLine("AnalogInputs: ");
                    if (dataConfig.AnalogConfig.Count() == data.AnalogInputs.Count()) {
                        for(int i=0; i<dataConfig.AnalogConfig.Count(); i++) {
                            if (dataConfig.AnalogConfig[i].Enabled) {
                                Console.WriteLine($"{dataConfig.AnalogConfig[i].Name}: {data.AnalogInputs[i]}");   
                            }
                        }
                    } else{
                        Console.WriteLine("Analog lengths don't match...");
                        Console.WriteLine($"AnalogConfig: {dataConfig.AnalogConfig.Count()}, AnalogInputs: {data.AnalogInputs.Count()}");
                    }

                    Console.WriteLine("DiscreteInputs: ");
                    if (dataConfig.DiscreteConfig.Count() == data.DiscreteInputs.Count()) {
                        for (int i = 0; i < dataConfig.DiscreteConfig.Count(); i++) {
                            if (dataConfig.DiscreteConfig[i].Enabled) {
                                Console.WriteLine($"{dataConfig.DiscreteConfig[i].Name}: {data.DiscreteInputs[i]}");
                            }
                        }
                    } else {
                        Console.WriteLine("Discrete lengths don't match...");
                        Console.WriteLine($"DiscreteConfig: {dataConfig.DiscreteConfig.Count()}, DiscreteInputs: {data.DiscreteInputs.Count()}");
                    }

                    Console.WriteLine("Alerts:");
                    if (dataConfig.AlertConfig.Count() == data.Alerts.Count()) {
                        for (int i = 0; i < dataConfig.AlertConfig.Count(); i++) {
                            if (dataConfig.AlertConfig[i].Enabled) {
                                Console.WriteLine($"{dataConfig.AlertConfig[i].Name}: {((ActionType)data.Alerts[i]).ToString()}");
                            }
                        }
                    } else {
                        Console.WriteLine("Alert lengths don't match...");
                        Console.WriteLine($"AlertConfig: {dataConfig.AlertConfig.Count()}, Alerts: {data.Alerts.Count()}");
                    }

                    Console.WriteLine("Outputs:");
                    if (dataConfig.OutputConfig.Count() == data.Outputs.Count()) {
                        for (int i = 0; i < dataConfig.OutputConfig.Count(); i++) {
                            if(dataConfig.OutputConfig[i].Enabled){
                                Console.WriteLine($"{dataConfig.OutputConfig[i].Name}: {data.Outputs[i]}");
                            }
                        }
                    } else {
                        Console.WriteLine("Output lengths don't match...");
                        Console.WriteLine($"OutputConfig: {dataConfig.OutputConfig.Count()}, Outputs: {data.Outputs.Count()}");
                    }

                    Console.WriteLine("Actions:");
                    if (dataConfig.ActionConfig.Count() == data.Actions.Count()) {
                        for (int i = 0; i < dataConfig.ActionConfig.Count(); i++) {
                            if (dataConfig.ActionConfig[i].Enabled) {
                                Console.WriteLine($"{dataConfig.ActionConfig[i].Name}: {data.Actions[i]}");
                            }
                        }
                    } else {
                        Console.WriteLine("Action lengths don't match...");
                        Console.WriteLine($"ActionConfig: {dataConfig.ActionConfig.Count()}, Actions: {data.Actions.Count()}");
                    }

                    Console.WriteLine("VirtualInputs: ");
                    if (dataConfig.VirtualConfig.Count() == data.VirtualInputs.Count()) {
                        for (int i = 0; i < dataConfig.VirtualConfig.Count(); i++) {
                            if (dataConfig.VirtualConfig[i].Enabled) {
                                Console.WriteLine($"{dataConfig.VirtualConfig[i].Name}: {data.VirtualInputs[i]}");
                            }
                        }
                    } else {
                        Console.WriteLine("VirtualInput lengths don't match...");
                        Console.WriteLine($"ActionConfig: {dataConfig.VirtualConfig.Count()}, VirtualInput: {data.VirtualInputs.Count()}");
                    }

                } else {
                    Console.WriteLine("Error: Could not find MongoDB device");
                }
            } else {
                Console.WriteLine("Error: Could not find monitoring device");
            }
        }
        static async Task CreateDisplayConfig() {
            await DB.InitAsync("monitoring_debug", "172.20.3.30", 27017);
            using var context = new FacilityContext();
            var box = await context.Devices.OfType<MonitoringBox>().Include(e => e.Channels).FirstOrDefaultAsync(e => e.Identifier == "Epi2");
            if(box!=null) {
                Console.WriteLine("Monitoring Box found, create database");
                MonitoringDevice device = new MonitoringDevice();
                device.DeviceName = box.DisplayName;
                await device.SaveAsync();
                var dataConfiguration = GenerateDisplayHeaders(context, box);
                dataConfiguration.Iteration = 1;
                await dataConfiguration.SaveAsync();
                await device.DataConfigurations.AddAsync(dataConfiguration);
                await device.SaveAsync();
                Console.WriteLine($"Device ObjectRef: {device.ID}");
                box.DataReference = device.ID;
                box.DataConfigIteration = 1;
                context.Update(box);
                var ret= await context.SaveChangesAsync();
                if (ret>0) {
                    Console.WriteLine("Check database");
                } else {
                    Console.WriteLine("Error: Failed to save monitoring box");
                }
                  
            } else {
                Console.WriteLine("Could not find monitoring box");
            }
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            //MonitoringDevice device = new MonitoringDevice();
            //device.DeviceName = "Epi2 Monitoring";
            //await device.SaveAsync();
            //var displayConfig = GenerateDisplayHeaders();
            //await displayConfig.SaveAsync();
            //await device.DisplayConfig.AddAsync(displayConfig);
            //await device.SaveAsync();
            //Console.WriteLine($"Device ObjectRef: {device.ID}");

        }
        static DataConfiguration GenerateDisplayHeaders(FacilityContext context,MonitoringBox monitoring) {
            DataConfiguration dataConfig = new DataConfiguration();
            dataConfig.Iteration = 1;
            if (monitoring != null) {
                dataConfig.DiscreteConfig=context.Channels.OfType<DiscreteInput>()
                    .Include(e=>e.Alert)
                    .Where(e => e.ModbusDeviceId == monitoring.Id)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => new DataConfig() { 
                        Name = e.DisplayName, 
                        Bypass = e.Bypass, 
                        Display = e.Display,
                        Enabled = e.Connected,
                        AlertIndex=e.Alert.ModbusAddress.Address
                    }).ToList();

                dataConfig.AnalogConfig = context.Channels.OfType<AnalogInput>()
                    .Include(e => e.Alert)
                    .Where(e => e.ModbusDeviceId == monitoring.Id)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => new DataConfig() {
                        Name = e.DisplayName,
                        Bypass = e.Bypass,
                        Display = e.Display,
                        Enabled = e.Connected,
                        AlertIndex = e.Alert.ModbusAddress.Address
                    }).ToList();

                dataConfig.OutputConfig = context.Channels.OfType<OutputChannel>()
                    .Where(e => e.ModbusDeviceId == monitoring.Id)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => new DataConfig() {
                        Name = e.DisplayName,
                        Bypass = e.Bypass,
                        Display = e.Display,
                        Enabled = e.Connected,
                        AlertIndex=-1
                    }).ToList();

                dataConfig.VirtualConfig = context.Channels.OfType<VirtualInput>()
                    .Include(e => e.Alert)
                    .Where(e => e.ModbusDeviceId == monitoring.Id)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => new DataConfig() {
                        Name = e.DisplayName,
                        Bypass = e.Bypass,
                        Display = e.Display,
                        Enabled = e.Connected,
                        AlertIndex = e.Alert.ModbusAddress.Address
                    }).ToList();

                dataConfig.ActionConfig = context.FacilityActions
                    .Select(e => new DataConfig() {
                        Name = e.ActionName,
                        Bypass = false,
                        Display = true,
                        Enabled = true
                    }).ToList();

                dataConfig.AlertConfig = context.Alerts.Include(e => e.InputChannel)
                    .Where(e => e.InputChannel.ModbusDeviceId == monitoring.Id)
                    .OrderBy(e => e.ModbusAddress.Address)
                    .Select(e => new DataConfig() {
                        Name = e.DisplayName,
                        Bypass = e.Bypass,
                        Display = e.InputChannel.Display,
                        Enabled = e.InputChannel.Connected
                    }).ToList();

                dataConfig.DeviceConfig = new DataConfig() {
                    Name = monitoring.DisplayName + " State",
                    Bypass = false,
                    Display = true,
                    Enabled = true,
                    AlertIndex = monitoring.NetworkConfiguration.ModbusConfig.ModbusAddress.Address
                };
                //displayConfig.AlertHeaders = context.Channels.OfType<InputChannel>().Include(e => e.Alert)
                //    .Where(e => e.ModbusDeviceId == monitoring.Id)
                //    .OrderBy(e => e.Alert.ModbusAddress.Address)
                //    .Select(e => e.Alert.DisplayName).ToArray();
            }
            return dataConfig;
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
        static async Task SetAlertNames() {
            using var context = new FacilityContext();
            var monitoring = await context.Devices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .FirstOrDefaultAsync(e=>e.Identifier=="Epi2");
            if (monitoring != null) {
                var channels = await context.Channels.OfType<AnalogInput>().Include(e => e.Alert).ToListAsync();
                foreach(var ain in channels) {
                    ain.Alert.DisplayName = ain.DisplayName + " Alert";
                }
                var alerts = channels.Select(e => e.Alert).ToList();
                context.UpdateRange(alerts);
                var ret = await context.SaveChangesAsync();
                if (ret > 0) {
                    Console.WriteLine("Alert names updated");
                } else {
                    Console.WriteLine("Save failed..");
                }
            } else {
                Console.WriteLine("Error: Could not find monitoring box");
            }
            Console.ReadKey();
        }
        static async Task TestModbusWithNames() {
            using var context = new FacilityContext();
            var monitoring = context.Devices.OfType<MonitoringBox>()
                .FirstOrDefault(e => e.Identifier == "Epi1");
            if (monitoring != null) {
                var discreteHeaders = context.Channels.OfType<DiscreteInput>().Where(e=>e.ModbusDeviceId==monitoring.Id)
                    .OrderBy(e=>e.SystemChannel)
                    .Select(e => e.DisplayName)
                    .ToList();

                var analogHeaders = context.Channels.OfType<AnalogInput>().Where(e => e.ModbusDeviceId == monitoring.Id)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => e.DisplayName)
                    .ToList();

                var outputHeaders = context.Channels.OfType<OutputChannel>().Where(e => e.ModbusDeviceId == monitoring.Id)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => e.DisplayName)
                    .ToList();

                var virtualHeaders = context.Channels.OfType<VirtualInput>().Where(e => e.ModbusDeviceId == monitoring.Id)
                    .OrderBy(e => e.SystemChannel)
                    .Select(e => e.DisplayName)
                    .ToList();

                var alertHeaders = context.Channels.OfType<InputChannel>().Include(e=>e.Alert)
                    .Where(e => e.ModbusDeviceId == monitoring.Id)
                    .OrderBy(e => e.Alert.ModbusAddress.Address)
                    .Select(e => e.Alert.DisplayName).ToList();

                var aHeaders = context.Alerts.Include(e => e.InputChannel)
                    .Where(e => e.InputChannel.ModbusDeviceId == monitoring.Id)
                    .OrderBy(e=>e.ModbusAddress.Address)
                    .Select(e => e.DisplayName)
                    .ToList();

                var networkConfig = monitoring.NetworkConfiguration;
                networkConfig.ModbusConfig.SlaveAddress = 1;
                var result = await ModbusService.Read(networkConfig.IPAddress, 502, networkConfig.ModbusConfig);
                var channelMapping = networkConfig.ModbusConfig.ChannelMapping;
                var discreteInputs = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.DiscreteStart, (channelMapping.DiscreteStop - channelMapping.DiscreteStart)+1).ToList();
                var outputs = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.OutputStart, (channelMapping.OutputStop - channelMapping.OutputStart)+1).ToList();
                var actions = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.ActionStart, (channelMapping.ActionStop - channelMapping.ActionStart)+1).ToList();
                var analogInputs = new ArraySegment<ushort>(result.InputRegisters, channelMapping.AnalogStart, (channelMapping.AnalogStop - channelMapping.AnalogStart)+1).ToList();
                var alerts = new ArraySegment<ushort>(result.HoldingRegisters, channelMapping.AlertStart, (channelMapping.AlertStop - channelMapping.AlertStart)+1).ToList();

                Console.WriteLine("DiscreteInputs: ");
                if (discreteInputs.Count == discreteHeaders.Count) {
                    for (int i = 0; i < discreteInputs.Count; i++) {
                        Console.WriteLine($"{discreteHeaders[i]}: {discreteInputs[i]}");
                    }
                } else {
                    Console.WriteLine("Discrete headers and inputs counts don't match");
                }

                Console.WriteLine("AnalogInputs: ");
                if (analogInputs.Count == analogHeaders.Count) {
                    for (int i = 0; i < analogInputs.Count; i++) {
                        Console.WriteLine($"{analogHeaders[i]}: {analogInputs[i]}");
                    }
                } else {
                    Console.WriteLine("Analog headers and inputs counts don't match");
                }

                Console.WriteLine("Outputs: ");
                if (outputHeaders.Count == outputs.Count) {
                    for (int i = 0; i < outputs.Count; i++) {
                        Console.WriteLine($"{outputHeaders[i]}: {outputs[i]}");
                    }
                } else {
                    Console.WriteLine("Analog headers and inputs counts don't match");
                }

                Console.WriteLine("Alerts: ");
                if (alertHeaders.Count == alerts.Count) {
                    for (int i = 0; i < alerts.Count; i++) {
                        Console.WriteLine($"{alertHeaders[i]}: {alerts[i]}");
                    }
                } else {
                    Console.WriteLine("Alert headers and inputs counts don't match");
                }
            } else {
                Console.WriteLine("Error: Could not find monitoring device");
            }
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
        static async Task FixDiscreteNames(string box) {
            Console.WriteLine("Fixing Discrete Names, Please Wait....");
            using var context = new FacilityContext();
            var monitoring = context.Devices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .FirstOrDefault(e => e.Identifier == box);
            var dInputs = monitoring.Channels.OfType<DiscreteInput>().OrderBy(e => e.SystemChannel).ToList();
            foreach (var dIn in dInputs) {
                dIn.Identifier = "Discrete " + dIn.SystemChannel;
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
        static async Task FixAnalogNames(string box) {
            Console.WriteLine("Fixing Analog Names, Please Wait....");
            using var context = new FacilityContext();
            var monitoring = context.Devices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .FirstOrDefault(e => e.Identifier == box);
            var dInputs = monitoring.Channels.OfType<AnalogInput>().OrderBy(e => e.SystemChannel).ToList();
            foreach (var dIn in dInputs) {
                dIn.Identifier = "Analog " + dIn.SystemChannel;
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
        static async Task FixOutputNames(string box) {
            Console.WriteLine("Fixing Output Names, Please Wait....");
            using var context = new FacilityContext();
            var monitoring = context.Devices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .FirstOrDefault(e => e.Identifier == box);
            var dInputs = monitoring.Channels.OfType<DiscreteOutput>().OrderBy(e => e.SystemChannel).ToList();
            foreach (var dIn in dInputs) {
                dIn.Identifier = "Analog " + dIn.SystemChannel;
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
    }
}
