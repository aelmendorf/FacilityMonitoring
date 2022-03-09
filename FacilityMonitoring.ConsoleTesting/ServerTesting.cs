using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Entities;
using MongoDB.Driver.Linq;
using FacilityMonitoring.Infrastructure.Data.Model;
using FacilityMonitoring.Infrastructure.Data.MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using FacilityMonitoring.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace FacilityMonitoring.ConsoleTesting {
    public class ServerTesting {
        public static async Task Main(string[] args) {
            var client = new MongoClient("mongodb://172.20.3.30");
            var database = client.GetDatabase("epi1_data");
            using var context = new FacilityContext();
            var device = context.Devices.OfType<MonitoringBox>()
                .FirstOrDefault(e => e.Identifier == "Epi1");
            if (device != null) {

                Console.WriteLine("Retrieving channel and alert definitions");
                var analogChannels = context.Channels.OfType<AnalogInput>().Where(e=>e.ModbusDeviceId==device.Id).OrderBy(e => e.SystemChannel).ToArray();
                var discreteChannels = context.Channels.OfType<DiscreteInput>().Where(e => e.ModbusDeviceId == device.Id).OrderBy(e => e.SystemChannel).ToArray();
                var outputChannels = context.Channels.OfType<OutputChannel>().Where(e => e.ModbusDeviceId == device.Id).OrderBy(e => e.SystemChannel).ToArray();
                var virtualChannels = context.Channels.OfType<VirtualInput>().Where(e => e.ModbusDeviceId == device.Id).OrderBy(e => e.SystemChannel).ToArray();
                var alertRegisters = context.Channels.OfType<InputChannel>().Where(e => e.ModbusDeviceId == device.Id).Select(e => e.Alert).ToArray();
                
               
                var networkConfig = device.NetworkConfiguration;
                var modbusConfig = networkConfig.ModbusConfig;
                var channelMapping = networkConfig.ModbusConfig.ChannelMapping;
                Console.WriteLine("Reading Modbus");

                //await database.CreateCollectionAsync("analog_readings", new CreateCollectionOptions() { TimeSeriesOptions = new TimeSeriesOptions("timestamp", "channelid", granularity: TimeSeriesGranularity.Seconds) });
                //await database.CreateCollectionAsync("discrete_readings", new CreateCollectionOptions() { TimeSeriesOptions = new TimeSeriesOptions("timestamp", "channelid", granularity: TimeSeriesGranularity.Seconds) });
                //await database.CreateCollectionAsync("output_readings", new CreateCollectionOptions() { TimeSeriesOptions = new TimeSeriesOptions("timestamp", "channelid", granularity: TimeSeriesGranularity.Seconds) });
                //await database.CreateCollectionAsync("virtual_readings", new CreateCollectionOptions() { TimeSeriesOptions = new TimeSeriesOptions("timestamp", "channelid", granularity: TimeSeriesGranularity.Seconds) });
                //await database.CreateCollectionAsync("alert_readings", new CreateCollectionOptions() { TimeSeriesOptions = new TimeSeriesOptions("timestamp", "alertid", granularity: TimeSeriesGranularity.Seconds) });
                //await database.CreateCollectionAsync("action_readings", new CreateCollectionOptions() { TimeSeriesOptions = new TimeSeriesOptions("timestamp", "actionid", granularity: TimeSeriesGranularity.Seconds) });

                var areadings = database.GetCollection<AnalogReading>("analog_readings");
                var dreadings = database.GetCollection<DiscreteReading>("discrete_readings");
                var oreadings = database.GetCollection<OutputReading>("output_readings");
                var vreadings = database.GetCollection<VirtualReading>("virtual_readings");
                var alertReadings = database.GetCollection<AlertReading>("alert_readings");
                var actionReadings = database.GetCollection<ActionReading>("action_readings");


                //areadings.Indexes.CreateOne(new CreateIndexModel<AnalogReading>(Builders<AnalogReading>.IndexKeys.Ascending(x => x.channelid),
                //    new CreateIndexOptions()),
                //    new CreateOneIndexOptions());

                //dreadings.Indexes.CreateOne(new CreateIndexModel<DiscreteReading>(Builders<DiscreteReading>.IndexKeys.Ascending(x => x.channelid),
                //    new CreateIndexOptions()),
                //    new CreateOneIndexOptions());

                //oreadings.Indexes.CreateOne(new CreateIndexModel<OutputReading>(Builders<OutputReading>.IndexKeys.Ascending(x => x.channelid),
                //    new CreateIndexOptions()),
                //    new CreateOneIndexOptions());

                //vreadings.Indexes.CreateOne(new CreateIndexModel<VirtualReading>(Builders<VirtualReading>.IndexKeys.Ascending(x => x.channelid),
                //    new CreateIndexOptions()),
                //    new CreateOneIndexOptions());

                //alertReadings.Indexes.CreateOne(new CreateIndexModel<AlertReading>(Builders<AlertReading>.IndexKeys.Ascending(x => x.alertid),
                //    new CreateIndexOptions()),
                //    new CreateOneIndexOptions());

                //actionReadings.Indexes.CreateOne(new CreateIndexModel<ActionReading>(Builders<ActionReading>.IndexKeys.Ascending(x => x.actionid),
                //    new CreateIndexOptions()),
                //    new CreateOneIndexOptions());
                var watch = new System.Diagnostics.Stopwatch();
                Console.WriteLine("Start read loop, force close to exit");
                while (true) {
                    watch.Start();
                    var result = await ModbusService.Read(networkConfig.IPAddress, networkConfig.Port, networkConfig.ModbusConfig);
                    var now = DateTime.Now;
                    var discreteInputs = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.DiscreteStart, (channelMapping.DiscreteStop - channelMapping.DiscreteStart) + 1).ToArray();
                    var outputs = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.OutputStart, (channelMapping.OutputStop - channelMapping.OutputStart) + 1).ToArray();
                    var actions = new ArraySegment<bool>(result.DiscreteInputs, channelMapping.ActionStart, (channelMapping.ActionStop - channelMapping.ActionStart) + 1).ToArray();
                    var analogInputs = new ArraySegment<ushort>(result.InputRegisters, channelMapping.AnalogStart, (channelMapping.AnalogStop - channelMapping.AnalogStart) + 1).ToArray();
                    var alerts = new ArraySegment<ushort>(result.HoldingRegisters, channelMapping.AlertStart, (channelMapping.AlertStop - channelMapping.AlertStart) + 1).ToArray();
                    var virts = new ArraySegment<bool>(result.Coils, channelMapping.VirtualStart, (channelMapping.VirtualStop - channelMapping.VirtualStart) + 1).ToArray();

                    for (int i = 0; i < analogInputs.Length; i++) {
                        var analogReading = new AnalogReading() {
                            channelid = analogChannels[i].Id,
                            timestamp = now,
                            value = analogInputs[i]
                        };
                        areadings.InsertOne(analogReading);
                    }

                    for (int i = 0; i < discreteInputs.Length; i++) {
                        var discreteReading = new DiscreteReading() {
                            channelid = discreteChannels[i].Id,
                            timestamp = now,
                            value = discreteInputs[i]
                        };
                        dreadings.InsertOne(discreteReading);
                    }


                    for (int i = 0; i < outputs.Length; i++) {
                        var outputReading = new OutputReading() {
                            channelid = outputChannels[i].Id,
                            timestamp = now,
                            value = outputs[i]
                        };
                        oreadings.InsertOne(outputReading);
                    }


                    for (int i = 0; i < virts.Length; i++) {
                        var virtualReading = new VirtualReading() {
                            channelid = virtualChannels[i].Id,
                            timestamp = now,
                            value = virts[i]
                        };
                        vreadings.InsertOne(virtualReading);
                    }
                    Console.WriteLine($"AlertCount: {alerts.Length}");
                    for (int i = 0; i < alerts.Length; i++) {
                        var alertReading = new AlertReading() {
                            alertid = alertRegisters[i].Id,
                            timestamp = now,
                            value = alerts[i]
                        };
                        alertReadings.InsertOne(alertReading);
                    }
                    watch.Stop();
                    Console.WriteLine($"Elapsed: {watch.ElapsedMilliseconds}");
                    watch.Reset();
                }


                Console.WriteLine("Check Database");
            } else {
                Console.WriteLine("Error: could not find device");
            }

            Console.WriteLine("Press any key to exit");

            //Console.WriteLine("Creating Database");
            //await database.CreateCollectionAsync("readings", new CreateCollectionOptions() { TimeSeriesOptions = new TimeSeriesOptions("timestamp", "channelid", granularity: TimeSeriesGranularity.Seconds) });
            //var tseries = database.GetCollection<AnalogReading>("readings");
            //Console.WriteLine("Creating Indexes");
            //tseries.Indexes.CreateOne(new CreateIndexModel<MonitorReading>(Builders<MonitorReading>.IndexKeys.Ascending(x => x.channelid),
            //    new CreateIndexOptions()),
            //    new CreateOneIndexOptions());

            //Console.WriteLine("Writing test data");
            //for (int i = 0; i < 100000; i++) {
            //    AnalogReading areading = new AnalogReading();
            //    areading.timestamp = DateTime.Now;
            //    areading.channelid = rand.Next(1, 16);
            //    areading.value = rand.NextDouble() * 100;

            //    DiscreteReading dreading = new DiscreteReading();
            //    dreading.timestamp = DateTime.Now;
            //    dreading.channelid = rand.Next(32, 42);
            //    dreading.value = rand.Next(0, 1) > 0 ? true:false;

            //    tseries.InsertMany(new List<MonitorReading>(){areading,dreading});
            //}
            //Console.WriteLine("Check Database");


            //Console.WriteLine($"{DateTime.Now} Starting");
            //var ret = await (await tseries.FindAsync(x => x.channelid == 4)).ToListAsync();
            //Console.WriteLine($"{DateTime.Now} Done {ret.Count} records found");


            //var collection = database.GetCollection<BsonDocument>("data");
            //var filter = Builders<BsonDocument>.Filter.Eq("_id", new BsonObjectId(new ObjectId("61b104432d4cacaf4d3e4164")));
            //var data = await collection.Find(filter).FirstOrDefaultAsync();
            //Console.WriteLine(data.ToString());
        }

        public static async Task OutputData() {
            await DB.InitAsync("monitoring", "172.20.3.30", 27017);
            using var context = new FacilityContext();
            var monitoring = context.Devices.OfType<MonitoringBox>()
                .FirstOrDefault(e => e.Identifier == "Epi2");
            if (monitoring != null) {
                //var dev = await DB.Find<MonitoringDevice>().OneAsync(monitoring.DataReference);
                var dev = await (from a in DB.Queryable<MonitoringDevice>()
                                 where a.ID == monitoring.DataReference
                                 select a).FirstOrDefaultAsync();
                if (dev != null) {
                    DateTime start = new DateTime(2022, 3, 7);
                    var devData = (from a in dev.DeviceData.ChildrenQueryable()
                                   where a.TimeStamp >= start
                                   select a).ToList();
                    List<string> lines = new List<string>();
                    foreach (var data in devData) {
                        StringBuilder builder = new StringBuilder();
                        //Console.Write(data.TimeStamp.ToString() + " :");
                        if (data.AnalogInputs != null) {
                            builder.AppendFormat("{0}\t", data.TimeStamp.ToString());
                            foreach (var ain in data.AnalogInputs) {
                                builder.AppendFormat("{0}\t", ain / 10);
                            }
                            lines.Add(builder.ToString());
                            //Console.WriteLine(builder.ToString());
                        } else {
                            Console.Write(" null");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine("Outputing to file");
                    File.WriteAllLines(@"C:\MonitorFiles\epi2_3_7-3_9.txt", lines);
                } else {
                    Console.WriteLine("Error Finding ");
                }
            } else {
                Console.WriteLine("Error: Monitoring Box not found");
            }
        }
    }

    public class ChannelReading {
        public int channelid { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class AnalogReading: ChannelReading {
        public double value { get; set; }
    }

    public class DiscreteReading : ChannelReading {
        public bool value { get; set; }
    }

    public class OutputReading : ChannelReading {
        public bool value { get; set; }
    }

    public class VirtualReading : ChannelReading {
        public bool value { get; set; }
    }

    public class ActionReading {
        public int actionid { get; set; }
        public DateTime timestamp { get; set; }
        public bool value { get; set; }
    }

    public class AlertReading {
        public int alertid { get; set; }
        public DateTime timestamp { get; set; }
        public int value { get; set; }
    }
}
