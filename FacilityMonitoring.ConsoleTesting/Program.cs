using System;
using FacilityMonitoring.Infrastructure.Data.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FacilityMonitoring.ConsoleTesting {
    public class Program {
        static void Main(string[] args) {
            Console.WriteLine("Config File");
            ReadConfiguration();

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

        static void ReadConfiguration() {
            var config = ;
            var myObject = JObject.Parse(File.ReadAllText(@"C:\DiscreteInputs.json"));
            foreach(var jObject in myObject) {
                Console.WriteLine(jObject.Key);
                foreach(var token in jObject.Value.ToList()) {
                    Console.WriteLine(token.ToString());
                }
            }
        }
    }
}
