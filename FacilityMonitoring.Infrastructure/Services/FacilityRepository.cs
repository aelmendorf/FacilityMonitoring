using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacilityMonitoring.Infrastructure.Data;
using System.Threading.Tasks;
using FacilityMonitoring.Infrastructure.Data.Model;
using Microsoft.EntityFrameworkCore;
using FacilityMonitoring.Infrastructure.Data.MongoDB;
using System.Linq.Expressions;

namespace FacilityMonitoring.Infrastructure.Services {

    public class DisplayHeaders {
        string Version { get; set; }
        string[] AnalogHeaders { get; set; }
        string[] DiscreteHeaders { get; set; }
        string[] VirtualHeaders { get; set; }
        string[] OutputHeaders { get; set; }
        string[] AlertHeaders { get; set; }
        string[] ActionHeaders { get; set; }
    }

    public interface IMonitoringBoxRepo {
        MonitoringBox GetMonitoringBox(string identifier);
        IList<Channel> GetBoxChannels(string identifier);
        DisplayHeaders GenerateHeaders(string boxIdentifier);
        int GetAlertCount(string box);
        int GetChannelCount<T>(string box) where T : Channel;
        int GetActionCount();
        (int inputCount, int holdingCount, int coilCount, int discreteCount) UpdateChannelMapping(string box);
        ChannelRegisterMapping UpdateChannelRegisterMap(string box);


    }

    public class FacilityRepository : IMonitoringBoxRepo {
        private readonly FacilityContext _context;

        public FacilityRepository(FacilityContext context) {
            this._context = context;
        }

        public DisplayHeaders GenerateHeaders(string boxIdentifier) {
            throw new NotImplementedException();
        }

        public IList<Channel> GetBoxChannels(string identifier) {
            throw new NotImplementedException();
        }

        public MonitoringBox GetMonitoringBox(string identifier) {
            throw new NotImplementedException();
        }

        public int GetChannelCount<T>(string box) where T:Channel {
            return this._context.Channels.OfType<T>()
                .Include(e => e.ModbusDevice)
                .Where(e => e.ModbusDevice.Identifier == box)
                .Count();
        }

        public int GetAlertCount(string box) {
            return this._context.Alerts
                .Include(e => e.InputChannel)
                .Where(e => e.InputChannel.ModbusDevice.Identifier == box)
                .Count();
        }
        public int GetActionCount() {
            return this._context.FacilityActions.Count();
        }

        public (int start, int stop) GetChannelRegisterMap<T>(string box) where T: Channel {
            var start = this._context.Channels.OfType<T>()
                .Where(e => e.ModbusDevice.Identifier == box)
                .Min(e => e.ModbusAddress.Address);

            var stop = this._context.Channels.OfType<T>()
                .Where(e => e.ModbusDevice.Identifier == box)
                .Max(e => e.ModbusAddress.Address);

            return (start, stop);
        }

        public (int start, int stop) GetAlertRgisterMap(string box) {
            var start = this._context.Alerts.Include(e=>e.InputChannel)
                .Where(e => e.InputChannel.ModbusDevice.Identifier == box)
                .Min(e => e.ModbusAddress.Address);
            var stop = this._context.Alerts.Include(e => e.InputChannel)
                .Where(e => e.InputChannel.ModbusDevice.Identifier == box)
                .Max(e => e.ModbusAddress.Address);
            return (start, stop);
        }

        public (int start, int stop) GetActionRegisterMap(string box) {
            //var start = this._context.Device
            //    .Include(e=>e.ModbusActionMap)
            //    .Where(e=>e.ModbusActionMap.MonitoringBox.Identifier==box)
            //    .Min(e => e.ModbusActionMap.ModbusAddress.Address);

            //var stop = this._context.FacilityActions
            //    .Include(e => e.ModbusActionMap)
            //    .Where(e => e.ModbusActionMap.MonitoringBox.Identifier == box)
            //    .Max(e => e.ModbusActionMap.ModbusAddress.Address);

            return (0, 0);
        }

        public (int inputCount,int holdingCount,int coilCount,int discreteCount) UpdateChannelMapping(string box) {
            var aCount = this.GetChannelCount<AnalogInput>(box);
            var dCount = this.GetChannelCount<DiscreteInput>(box);
            var coilCount = this.GetChannelCount<VirtualInput>(box);
            var outCount = this.GetChannelCount<OutputChannel>(box);
            var holdingCount = this.GetAlertCount(box) + 1;
            var actionCount = this.GetActionCount();
            var discreteCount = dCount + outCount+actionCount;
            var inputCount = aCount;
            return (inputCount, holdingCount, coilCount, discreteCount);
        }

        public ChannelRegisterMapping UpdateChannelRegisterMap(string box) {
            ChannelRegisterMapping mapping = new ChannelRegisterMapping();
            var analogMap = this.GetChannelRegisterMap<AnalogInput>(box);
            var discreteMap = this.GetChannelRegisterMap<DiscreteInput>(box);
            var virtualMap = this.GetChannelRegisterMap<VirtualInput>(box);
            var outputMap = this.GetChannelRegisterMap<OutputChannel>(box);
            var alertMap = this.GetAlertRgisterMap(box);
            var actionMap = this.GetActionRegisterMap(box);

            mapping.AnalogStart = analogMap.start;
            mapping.AnalogStop = analogMap.stop;

            mapping.DiscreteStart = discreteMap.start;
            mapping.DiscreteStop = discreteMap.stop;

            mapping.VirtualStart = virtualMap.start;
            mapping.VirtualStop = virtualMap.stop;

            mapping.AlertStart = alertMap.start;
            mapping.AlertStop = alertMap.stop;

            mapping.ActionStart = actionMap.start;
            mapping.ActionStop = actionMap.stop;

            mapping.OutputStart = outputMap.start;
            mapping.OutputStop = outputMap.stop;

            return mapping;
        }
    }
}
