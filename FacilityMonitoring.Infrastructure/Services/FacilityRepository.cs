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
    public interface IFacilityRepository {
        Task<Alert?> GetAlertAsync(int id);
        Task<IList<AnalogInput>> GetAnalogInputsAsync(string device_id);
        Task<IList<Channel>?> GetChannelsAsync(string device_id);
        Task<string?> GetDataReferenceAsync(string device_id);
        Task<ModbusDevice?> GetDeviceAsync(string device_id);
        Task<IList<DiscreteInput>> GetDiscreteInputsAsync(string device_id);
        Task<IList<DiscreteOutput>> GetDiscreteOutputsAsync(string device_id);
        Task<IList<FacilityAction>> GetFacilityActions();
        Task<IList<VirtualInput>> GetVirtualInputsAsync(string device_id);
        Task<IList<string?>> GetHeaders<T>() where T : Channel;
    }

    public class FacilityRepository : IFacilityRepository {
        private readonly FacilityContext _context;

        public FacilityRepository(FacilityContext context) {
            this._context = context;
        }

        public async Task<IList<Channel>?> GetChannelsAsync(string device_id) {
            return (await this._context.Channels.AsNoTracking()
                .Include(e => e.ModbusDevice)
                .Include(e => e.ModbusAddress)
                .Where(e => e.ModbusDevice.Identifier == device_id)
                .ToListAsync());
        }

        public async Task<IList<AnalogInput>> GetAnalogInputsAsync(string device_id) {
            return await this._context.Channels.OfType<AnalogInput>()
                .AsNoTracking()
                .Include(e => e.AnalogAlerts)
                .Include(e => e.ModbusAddress)
                .Where(e => e.ModbusDevice.Identifier == device_id)
                .OrderBy(e => e.SystemChannel)
                .ToListAsync();
        }

        public async Task<IList<DiscreteInput>> GetDiscreteInputsAsync(string device_id) {
            return await this._context.Channels.OfType<DiscreteInput>()
                .AsNoTracking()
                .Include(e => e.DiscreteAlert)
                .Include(e => e.ModbusAddress)
                .Where(e => e.ModbusDevice.Identifier == device_id && !(e is VirtualInput))
                .OrderBy(e => e.SystemChannel)
                .ToListAsync();
        }

        public async Task<IList<DiscreteOutput>> GetDiscreteOutputsAsync(string device_id) {
            return await this._context.Channels.OfType<DiscreteOutput>()
                .AsNoTracking()
                .Include(e => e.ModbusAddress)
                .Where(e => e.ModbusDevice.Identifier == device_id)
                .OrderBy(e => e.SystemChannel)
                .ToListAsync();
        }

        public async Task<IList<VirtualInput>> GetVirtualInputsAsync(string device_id ) {
            return await this._context.Channels.OfType<VirtualInput>()
                .AsNoTracking()
                .Include(e => e.ModbusAddress)
                .Where(e => e.ModbusDevice.Identifier == device_id)
                .OrderBy(e=>e.SystemChannel)
                .ToListAsync();
        }

        public async Task<Alert?> GetAlertAsync(int id) {
            return await this._context.Alerts.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<string?> GetDataReferenceAsync(string device_id) {
            var device = await this.GetDeviceAsync(device_id);
            if (device != null) {
                return device.DataReference;
            } else {
                return null;
            }
        }

        public async Task<ModbusDevice?> GetDeviceAsync(string device_id) {
            return await this._context.ModbusDevices.AsNoTracking()
                .Include(e => e.Channels)
                    .ThenInclude(e => e.ModbusAddress)
                .FirstOrDefaultAsync(e => e.Identifier == device_id);
        }

        public async Task<IList<FacilityAction>> GetFacilityActions() {
            return await this._context.FacilityActions.AsNoTracking().ToListAsync();
        }

        public async Task<IList<string?>> GetAnalogHeaders<T>() where T:Channel {
            return await this._context.Channels
                    .AsNoTracking()
                    .OfType<T>()
                    .OrderBy(e => e.SystemChannel)
                    .Select(e=>e.Identifier)
                    .ToListAsync();
        }
    }
}
