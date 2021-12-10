using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacilityMonitoring.Infrastructure.Data;
using System.Threading.Tasks;
using FacilityMonitoring.Infrastructure.Data.Model;
using Microsoft.EntityFrameworkCore;
using FacilityMonitoring.Infrastructure.Data.MongoDB;

namespace FacilityMonitoring.Infrastructure.Services {

    public interface IFacilityRepository {
        Task<ModbusDevice?> GetDeviceAsync(string device_id);
        Task<IList<Channel>?> GetChannelsAsync(string device_id);
        Task<string?> GetDataReferenceAsync(string device_id);
    }

    public class FacilityRepository:IFacilityRepository {
        private readonly FacilityContext _context;

        public FacilityRepository(FacilityContext context) {
            this._context = context;
        }

        public async Task<IList<Channel>?> GetChannelsAsync(string device_id) {
            return (await this._context.Channels.Include(e=>e.ModbusDevice).Where(e =>e.ModbusDevice.Identifier==device_id).ToListAsync());
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
            return await this._context.ModbusDevices.FirstOrDefaultAsync(e => e.Identifier == device_id);
        }
    }
}
