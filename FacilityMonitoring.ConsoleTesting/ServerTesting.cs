using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Entities;
using MongoDB.Driver.Linq;
using FacilityMonitoring.Infrastructure.Data.Model;
using FacilityMonitoring.Infrastructure.Data.MongoDB;

namespace FacilityMonitoring.ConsoleTesting {
    public class ServerTesting {
        public static async Task Main(string[] args) {
            await DB.InitAsync("monitoring", "172.20.3.30", 27017);
            using var context = new FacilityContext();
            var monitoring = context.Devices.OfType<MonitoringBox>()
                .FirstOrDefault(e => e.Identifier == "Epi1");
            if (monitoring != null) {
                //var dev = await DB.Find<MonitoringDevice>().OneAsync(monitoring.DataReference);
                var dev = await (from a in DB.Queryable<MonitoringDevice>()
                                 where a.ID == monitoring.DataReference
                                 select a).FirstOrDefaultAsync();
                if (dev != null) {
                    var devData = (from a in dev.DeviceData.ChildrenQueryable()
                                   select a).ToList().GetNth<DataRecord>(60);
                    List<string> lines = new List<string>();
                    foreach (var data in devData) {
                        StringBuilder builder = new StringBuilder();
                        //Console.Write(data.TimeStamp.ToString() + " :");
                        if (data.AnalogInputs != null) {
                            builder.AppendFormat("{0}\t", data.TimeStamp.ToString());
                            foreach (var ain in data.AnalogInputs) {
                                builder.AppendFormat("{0}\t", ain);
                            }
                            lines.Add(builder.ToString());
                            Console.WriteLine(builder.ToString());
                        } else {
                            Console.Write(" null");
                        }
                        Console.WriteLine();
                    }
                    File.WriteAllLines(@"C:\MonitorFiles\epi2_2_11-2_14.txt", lines);
                } else {
                    Console.WriteLine("Error Finding ");
                }
            } else {
                Console.WriteLine("Error: Monitoring Box not found");
            }
        }
    }

    public static class ListExtensions {
        public static IEnumerable<T> GetNth<T>(this IList<T> list, int n) {
            for (int i = n - 1; i < list.Count; i += n) {
                yield return list[i];
            }
        }
    }
}
