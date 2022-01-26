using System;
using FacilityMonitoring.Infrastructure.Data.Model;
using System.Net.Sockets;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using FacilityMonitoring.Infrastructure.Data.MongoDB;
using FacilityMonitoring.Infrastructure.Services;
using MongoDB.Entities;
using Modbus.Device;

namespace FacilityMonitoring.ConsoleTesting {
    public class Program {
        static async Task Main(string[] args) {
            var context = new FacilityContext();
            IModbusService modbusService = new ModbusService();
            var dev = await context.Devices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .Include(e => e.ChannelMapping)
                .Include(e=>e.NetworkConfiguration.ModbusConfig)
                .FirstOrDefaultAsync(e=>e.Identifier=="Epi2");
            if (dev != null) {
                var channelMapping = dev.ChannelMapping;
                var modbusConfig = dev.NetworkConfiguration.ModbusConfig;

                Console.WriteLine("IP: {0}  Port: {1}",dev.NetworkConfiguration.IPAddress,dev.NetworkConfiguration.Port);
                modbusService.Connect(dev.NetworkConfiguration.IPAddress, dev.NetworkConfiguration.Port);
                //try {
                //    using var client = new TcpClient(dev.NetworkConfiguration.IPAddress, dev.NetworkConfiguration.Port);
                //    using var modbus = ModbusIpMaster.CreateIp(client);


                //} catch {

                //}
                var dInputs=await modbusService.ReadDiscreteInputsAsync(1, 0, modbusConfig.DiscreteInputs);
                //var holding = await modbusService.ReadHoldingRegistersAsync(1, 0, modbusConfig.HoldingRegisters);
                var holding = await modbusService.ReadHoldingRegistersAsync(1, 0, 50);
                var inputs = await modbusService.ReadInputRegistersAsync(1, 0, modbusConfig.InputRegisters);
                var coils = await modbusService.ReadCoilsAsync(1, 0, modbusConfig.Coils);

                Console.WriteLine("Discrete Inputs: ");
                for(int i = 0; i < dInputs.Length; i++) {
                    Console.WriteLine("D[{0}]: {1}", i, dInputs[i]);
                }

                Console.WriteLine("Input Registers");
                for (int i = 0; i < inputs.Length; i++) {
                    Console.WriteLine("A[{0}]: {1}", i, inputs[i]);
                }

                Console.WriteLine(" Holding Registers: ");
                for (int i = 0; i < holding.Length; i++) {
                    Console.WriteLine("H[{0}]: {1}", i, holding[i]);
                }

                Console.WriteLine("Coils: ");
                for (int i = 0; i < coils.Length; i++) {
                    Console.WriteLine("C[{0}]: {1}", i, coils[i]);
                }

                modbusService.Disconnect();

            } else {
                Console.WriteLine("Error: Could not find device");
            }
            Console.ReadKey();
        }

        static async Task UpdateModbusAddress() {
            using var context = new FacilityContext();
            var channels = await context.Channels
                .Include(e => e.ModbusDevice)
                .Where(e => e.ModbusDevice.DisplayName == "EpiLab2")
                .ToListAsync();
            if (channels.Count > 0) {
                var analogInputs = channels.OfType<AnalogInput>().OrderBy(e => e.SystemChannel).ToList();
                var discreteInput = channels.OfType<DiscreteInput>().OrderBy(e => e.SystemChannel).ToList();
                var outputs = channels.OfType<DiscreteOutput>().OrderBy(e => e.SystemChannel).ToList();
                var vChannels = channels.OfType<VirtualInput>().OrderBy(e => e.SystemChannel).ToList();


                int alertCount = 0;
                Console.WriteLine("DiscreteInput Count {0}", discreteInput.Count);
                if (discreteInput.Count == 40) {
                    for (int i = 0; i < 40; i++) {
                        discreteInput[i].ModbusAddress.Address = i;
                        discreteInput[i].ModbusAddress.RegisterType = ModbusRegister.DiscreteInput;

                        ModbusAddress alertAddress = new ModbusAddress();
                        alertAddress.Address = alertCount;
                        alertAddress.RegisterType = ModbusRegister.Holding;
                        alertAddress.RegisterLength = 0;

                        discreteInput[i].AlertAddress = alertAddress;
                        alertCount++;
                    }
                }

                if (analogInputs.Count == 16) {
                    for (int i = 0; i < 16; i++) {
                        analogInputs[i].ModbusAddress.Address = i;
                        analogInputs[i].ModbusAddress.RegisterType = ModbusRegister.Input;
                        analogInputs[i].ModbusAddress.RegisterLength = 0;

                        ModbusAddress alertAddress = new ModbusAddress();
                        alertAddress.Address = alertCount;
                        alertAddress.RegisterType = ModbusRegister.Holding;
                        alertAddress.RegisterLength = 0;

                        analogInputs[i].AlertAddress = alertAddress;
                        alertCount++;
                    }
                }

                if (outputs.Count == 8) {
                    for (int i = 0; i < 8; i++) {
                        outputs[i].ModbusAddress.Address = i + 40;
                        outputs[i].ModbusAddress.RegisterType = ModbusRegister.Input;
                    }
                }

                if (vChannels.Count == 4) {
                    for (int i = 0; i < 4; i++) {
                        vChannels[i].ModbusAddress.Address = i;
                        vChannels[i].ModbusAddress.RegisterType = ModbusRegister.Coil;
                    }
                }

                context.UpdateRange(analogInputs);
                context.UpdateRange(discreteInput);
                context.UpdateRange(outputs);
                context.UpdateRange(vChannels);
                var ret = await context.SaveChangesAsync();

                if (ret > 0) {
                    Console.WriteLine("Success, data should be updated");
                } else {
                    Console.WriteLine("Error Failed to save configurations");
                }
            } else {
                Console.WriteLine("Error: Channel.Count=0");
            }
            Console.ReadKey();
        }

        static async Task UpdateDeviceConfig() {
            using var context = new FacilityContext();
            var monitoring = await context.Devices.OfType<MonitoringBox>().FirstOrDefaultAsync(e => e.DisplayName == "EpiLab2");
            if (monitoring != null) {
                ChannelRegisterMapping mapping = new ChannelRegisterMapping();
                mapping.AlertRegisterType = ModbusRegister.Holding;
                mapping.AlertStart = 0;
                mapping.AlertStop = 55;

                mapping.AnalogRegisterType = ModbusRegister.Input;
                mapping.AnalogStart = 0;
                mapping.AnalogStop = 15;

                mapping.DiscreteRegisterType = ModbusRegister.DiscreteInput;
                mapping.DiscreteStart = 0;
                mapping.DiscreteStop = 39;

                mapping.VirtualRegisterType = ModbusRegister.Coil;
                mapping.VirtualStart = 0;
                mapping.VirtualStop = 3;

                mapping.OutputRegisterType = ModbusRegister.DiscreteInput;
                mapping.OutputStart = 40;
                mapping.OutputStop = 47;

                mapping.DeviceRegisterType = ModbusRegister.Holding;
                mapping.DeviceStart = 56;
                mapping.DeviceStop = 56;

                mapping.ActionRegisterType = ModbusRegister.DiscreteInput;
                mapping.ActionStart = 48;
                mapping.ActionStop = 53;


                monitoring.ChannelMapping = mapping;

                ModbusConfig config = new ModbusConfig();
                config.Coils = 4;
                config.HoldingRegisters = 57;
                config.DiscreteInputs = 54;
                config.InputRegisters = 16;

                monitoring.NetworkConfiguration.ModbusConfig = config;

                ModbusAddress addr = new ModbusAddress();
                addr.RegisterType = ModbusRegister.Holding;
                addr.Address = 56;
                addr.RegisterLength = 1;

                monitoring.ModbusAddress = addr;

                context.Update(monitoring);
                var ret = await context.SaveChangesAsync();
                if (ret > 0) {
                    Console.WriteLine("Success, data should be updated");
                } else {
                    Console.WriteLine("Error Failed to save configurations");
                }
            } else {
                Console.WriteLine("Could not find box");
            }
            Console.ReadKey();
        }

        static async Task Test() {
            var context = new FacilityContext();
            await DB.InitAsync("wrapper", "172.20.3.30", 27017);

            IFacilityRepository repo = new FacilityRepository(context);
            IModbusService modbusService = new ModbusService();

            IDataRecordService dataService = new DataRecordService();
            var dev = await repo.GetDeviceAsync("Epi2");
            if (dev != null) {
                var data = await dataService.GetAllDataAsync(dev.DataReference);
                foreach (var d in data) {
                    Console.WriteLine("Log: {0}", d.TimeStamp.ToUniversalTime());
                }
            } else {
                Console.WriteLine("Error: Could not find device");
            }

            //IDeviceController controller = new DeviceController(modbusService, dataService, repo);
            //await controller.LoadDeviceAsync();
            //await controller.Read();
            Console.WriteLine("Shoud be done");
            Console.ReadKey();
        }

        //static async Task Testing() {
        //    //CreateModbusDevices();
        //    //await CreateOutputs();
        //    //await CreateFacilityActions();
        //    //await CreateDiscreteInputs();
        //    //await CreateAnalogInputs();
        //    //await CreateVirtualInputs();
        //    //TestMongoInsert();
        //    // await ReadCollection();
        //    //await CreateAndInsertModel();
        //    //await ReadAllCollection();
        //    //await CreateCollectionFromModel();
        //    //await FindCollectionFromModel();
        //    //await AppendMeasurment();
        //}

        //static async Task AppendMeasurment() {
        //    using var context = new FacilityContext();
        //    var device = await context.ModbusDevices
        //        .OfType<MonitoringBox>()
        //        .Include(e=>e.Channels)
        //        .FirstOrDefaultAsync(e => e.Id == 1);

        //    if (device != null) {
        //        var client = new MongoClient("mongodb://172.20.3.30");
        //        var database = client.GetDatabase("monitoring");
        //        var mDevices = database.GetCollection<Device>("data");
        //        var deviceData = new DeviceData();
        //        Random rand = new Random();
        //        foreach(var analog in device.Channels.OfType<AnalogInput>()) {
        //            if (analog.Connected) {
        //                deviceData.AnalogData.Add(new AnalogData { Name = analog.Identifier, Value = rand.NextDouble()});
        //            }
        //        }

        //        foreach(var discrete in device.Channels.OfType<DiscreteInput>()) {
        //            if (discrete.Connected) {
        //                deviceData.DiscreteData.Add(new DiscreteData { Name = discrete.Identifier, Value = false });
        //            }
        //        }

        //        foreach(var coil in device.Channels.OfType<VirtualInput>()) {
        //            if (coil.Connected) {
        //                deviceData.CoilData.Add(new VirtualData { Name = coil.Identifier, Value = true });
        //            }
        //        }

        //        foreach(var output in device.Channels.OfType<DiscreteOutput>()) {
        //            if (output.Connected) {
        //                deviceData.OutputData.Add(new OutputData { Name = output.Identifier, Value = true });
        //            }
        //        }

        //        foreach(var action in context.FacilityActions.ToList()) {
        //            deviceData.ActionData.Add(new ActionData { Name = action.ActionName, Value = false });
        //        }

        //        var dev = Builders<Device>.Filter.Eq(d => d.Id, device.DataReference);
        //        var pushDef = Builders<Device>.Update.Push(d => d.DeviceData, deviceData);
        //        var addNew = await mDevices.UpdateOneAsync(dev, pushDef);
        //        Console.WriteLine("Should be updated");
        //    } else {
        //        Console.WriteLine("Error: Could not find device");
        //    }
        //}

        //static async Task CreateCollectionFromModel() {
        //    using var context = new FacilityContext();
        //    var device = await context.ModbusDevices.FirstOrDefaultAsync(e => e.Id == 1);
        //    if (device != null) {
        //        var client = new MongoClient("mongodb://172.20.3.30");
        //        var database = client.GetDatabase("monitoring");
        //        Device dev = new Device();
        //        dev.DeviceName = device.Identifier != null ? device.Identifier : "Not Set";
        //        var mDevices = database.GetCollection<Device>("data");
        //        dev.Id = new BsonObjectId(ObjectId.GenerateNewId()).ToString();
        //        device.DataReference = dev.Id;
        //        await mDevices.InsertOneAsync(dev);
        //        context.Update(device);
        //        await context.SaveChangesAsync();
        //        Console.WriteLine("Device should be created.  id: {0}",dev.Id);
        //    } else {
        //        Console.WriteLine("Error: Could not find device");
        //    }
        //}

        //static async Task FindCollectionFromModel() {
        //    using var context = new FacilityContext();
        //    var device = await context.ModbusDevices.FirstOrDefaultAsync(e => e.Id == 1);
        //    if (device != null) {
        //        var client = new MongoClient("mongodb://172.20.3.30");
        //        var database = client.GetDatabase("monitoring");
        //        var mDevices = database.GetCollection<Device>("data");
        //        var dev = await mDevices.Find(e => e.Id == device.DataReference).FirstOrDefaultAsync();
        //        if (dev != null) {
        //            Console.WriteLine("Device Found: id {0}",dev.Id);
        //        } else {
        //            Console.WriteLine("Error: Could not find device with id {0}",device.DataReference);
        //        }
        //    } else {
        //        Console.WriteLine("Error: Could not find device");
        //    }
        //}

        //static async Task ReadOneCollection() {
        //    var client = new MongoClient("mongodb://172.20.3.30");
        //    var database = client.GetDatabase("monitoring");
        //    var collection = database.GetCollection<BsonDocument>("data");
        //    var filter = Builders<BsonDocument>.Filter.Eq("_id",new BsonObjectId(new ObjectId("61b104432d4cacaf4d3e4164")));
        //    var data = await collection.Find(filter).FirstOrDefaultAsync();
        //    Console.WriteLine(data.ToString());
        //}

        //static async Task ReadAllCollection() {
        //    var client = new MongoClient("mongodb://172.20.3.30");
        //    var database = client.GetDatabase("monitoring");
        //    var collection = database.GetCollection<Device>("data");
        //    var data = await collection.Find(e => true).ToListAsync();
        //    foreach(var record in data) {
        //        Console.WriteLine(record.DeviceName);
        //        Console.WriteLine("AnalogInputs: ");
        //        foreach(var d in record.DeviceData) {
        //            Console.Write($"{d.TimeStamp}   ");
        //            foreach(var a in d.AnalogData) {
        //                Console.Write($"{a.Value}   ");
        //            }
        //            Console.WriteLine();
        //        }
        //    }
        //}

        //static async Task CreateAndInsertModel() {
        //    var client = new MongoClient("mongodb://172.20.3.30");
        //    var database = client.GetDatabase("monitoring");
        //    var collection = database.GetCollection<Device>("data");
        //    Device device = new Device();
        //    device.DeviceName = "Monitoring Box ";
        //    device.DeviceData = new List<DeviceData> {
        //       new DeviceData() {
        //           TimeStamp = DateTime.Now,
        //           AnalogData = new List<AnalogData> {
        //            new AnalogData { Name = "Ch1", Value = 45.6 },
        //            new AnalogData { Name = "Ch2", Value = 87.4 },
        //            new AnalogData { Name = "Ch3", Value = 45.6 },
        //            new AnalogData { Name = "Ch4", Value = 12.3 },
        //            new AnalogData { Name = "Ch5", Value = 20.78 }
        //        },
        //           DiscreteData = new List<DiscreteData> {
        //            new DiscreteData { Name = "Ch1", Value = true  },
        //            new DiscreteData { Name = "Ch2", Value = false },
        //            new DiscreteData { Name = "Ch3", Value = true  },
        //            new DiscreteData { Name = "Ch4", Value = false },
        //            new DiscreteData { Name = "Ch5", Value = false }
        //        },
        //           CoilData = new List<VirtualData> {
        //            new VirtualData { Name = "Ch1", Value = true  },
        //            new VirtualData { Name = "Ch2", Value = false },
        //            new VirtualData { Name = "Ch3", Value = true  },
        //            new VirtualData { Name = "Ch4", Value = false },
        //            new VirtualData { Name = "Ch5", Value = false }
        //        }
        //       },
        //       new DeviceData() {
        //           TimeStamp = new DateTime(2021,10,15),
        //           AnalogData = new List<AnalogData> {
        //            new AnalogData { Name = "Ch1", Value = 45.6 },
        //            new AnalogData { Name = "Ch2", Value = 45.6 },
        //            new AnalogData { Name = "Ch3", Value = 45.6 },
        //            new AnalogData { Name = "Ch4", Value = 45.6 },
        //            new AnalogData { Name = "Ch5", Value = 45.6 }
        //        },
        //           DiscreteData = new List<DiscreteData> {
        //            new DiscreteData { Name = "Ch1", Value = true  },
        //            new DiscreteData { Name = "Ch2", Value = false },
        //            new DiscreteData { Name = "Ch3", Value = true  },
        //            new DiscreteData { Name = "Ch4", Value = false },
        //            new DiscreteData { Name = "Ch5", Value = false }
        //        },
        //           CoilData = new List<VirtualData> {
        //            new VirtualData { Name = "Ch1", Value = true  },
        //            new VirtualData { Name = "Ch2", Value = false },
        //            new VirtualData { Name = "Ch3", Value = true  },
        //            new VirtualData { Name = "Ch4", Value = false },
        //            new VirtualData { Name = "Ch5", Value = false }
        //        }
        //       }
        //    };
        //    await collection.InsertOneAsync(device);
        //}

        //static void TestMongoInsert() {
        //    var client = new MongoClient("mongodb://172.20.3.30");
        //    var database = client.GetDatabase("monitoring");
        //    var collectiion = database.GetCollection<BsonDocument>("data");
            
        //    var document = new BsonDocument {
        //        {"DeviceName","Epi2Monitoring" },
        //        {"data",new BsonDocument{
        //            {"TimeStamp",DateTime.Now },
        //            {"AnalogInputs",new BsonArray {
        //                new BsonDocument{{"Name","Channel1"},{"Value",42.9}},
        //                new BsonDocument{{"Name","Channel2"},{"Value",78.5}},
        //                new BsonDocument{{"Name","Channel3"},{"Value",24.98}},
        //                new BsonDocument{{"Name","Channel4"},{"Value",45.77}}
        //            }},
        //            {"DiscreteInputs",new BsonArray {
        //                new BsonDocument{{"Name","Channel1"},{"Value",true}},
        //                new BsonDocument{{"Name","Channel2"},{"Value",false}},
        //                new BsonDocument{{"Name","Channel3"},{"Value",false}},
        //                new BsonDocument{{"Name","Channel4"},{"Value",true}}
        //            }},
        //            {"Coils",new BsonArray {
        //                new BsonDocument{{"Name","Channel1"},{"Value",false}},
        //                new BsonDocument{{"Name","Channel2"},{"Value",true}},
        //                new BsonDocument{{"Name","Channel3"},{"Value",false}},
        //                new BsonDocument{{"Name","Channel4"},{"Value",true}}
        //            }},
        //          }
        //        }
        //    };
        //    collectiion.InsertOne(document);
        //}

    }
}
