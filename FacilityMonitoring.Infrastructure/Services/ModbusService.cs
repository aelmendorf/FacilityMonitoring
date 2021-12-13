using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Modbus.Device;

namespace FacilityMonitoring.Infrastructure.Services {

    public interface IModbusService {
        string IpAddress { get; set; }
        int Port { get; set; }
        byte SlaveAddress { get; set; }

        ushort[]? ReadInputRegisters(int baseAddress,int length);
        ushort[]? ReadHoldingRegisters(int baseAddress, int length);
        bool[]? ReadDiscreteInputs(int baseAddress, int length);
        bool[]? ReadCoils(int baseAddress,int length);

        Task<ushort[]?> ReadInputRegistersAsync(int baseAddress, int length);
        Task<ushort[]?> ReadHoldingRegistersAsync(int baseAddress, int length);
        Task<bool[]?> ReadDiscreteInputsAsync(int baseAddress, int length);
        Task<bool[]?> ReadCoilsAsync(int baseAddress, int length);

    }

    public class ModbusService : IModbusService {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public byte SlaveAddress { get; set; }

        public ModbusService() {
            this.IpAddress = "";
            this.Port = 0;
            this.SlaveAddress = 0;
        }

        private bool CheckConnection() {
            try {
                Ping check = new Ping();
                PingReply reply = check.Send(this.IpAddress, 1000);
                return reply.Status == IPStatus.Success;
            } catch {
                return false;
            }
        }

        public ModbusService(string ip,int port,byte slaveAddr) {
            this.IpAddress = ip;
            this.Port = port;
            this.SlaveAddress = slaveAddr;
        }

        public bool[]? ReadCoils(int baseAddress, int length) {
            if (this.CheckConnection()) {
                try {
                    using TcpClient client = new TcpClient(this.IpAddress, this.Port);
                    ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
                    var regData = master.ReadCoils(this.SlaveAddress, (ushort)baseAddress, (ushort)length);
                    client.Close();
                    master.Dispose();
                    return regData;
                } catch {
                    return null;
                }
            } else {
                return null;
            }
        }

        public async Task<bool[]?> ReadCoilsAsync(int baseAddress, int length) {
            if (this.CheckConnection()) {
                try {
                    using TcpClient client = new TcpClient(this.IpAddress, this.Port);
                    ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
                    var regData = await master.ReadCoilsAsync(this.SlaveAddress, (ushort)baseAddress, (ushort)length);
                    client.Close();
                    master.Dispose();
                    return regData;
                } catch {
                    return null;
                }
            } else {
                return null;
            }
        }

        public bool[]? ReadDiscreteInputs(int baseAddress, int length) {
            if (this.CheckConnection()) {
                try {
                    using TcpClient client = new TcpClient(this.IpAddress, this.Port);
                    ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
                    var regData = master.ReadInputs(this.SlaveAddress, (ushort)baseAddress, (ushort)length);
                    client.Close();
                    master.Dispose();
                    return regData;
                } catch {
                    return null;
                }
            } else {
                return null;
            }
        }

        public async Task<bool[]?> ReadDiscreteInputsAsync(int baseAddress, int length) {
            if (this.CheckConnection()) {
                try {
                    using TcpClient client = new TcpClient(this.IpAddress, this.Port);
                    ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
                    var regData = await master.ReadInputsAsync(this.SlaveAddress, (ushort)baseAddress, (ushort)length);
                    client.Close();
                    master.Dispose();
                    return regData;
                } catch {
                    return null;
                }
            } else {
                return null;
            }
        }

        public ushort[]? ReadHoldingRegisters(int baseAddress, int length) {
            if (this.CheckConnection()) {
                try {
                    using TcpClient client = new TcpClient(this.IpAddress, this.Port);
                    ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
                    var regData = master.ReadInputRegisters(this.SlaveAddress, (ushort)baseAddress, (ushort)length);
                    client.Close();
                    master.Dispose();
                    return regData;
                } catch {
                    return null;
                }
            } else {
                return null;
            }
        }

        public async Task<ushort[]?> ReadHoldingRegistersAsync(int baseAddress, int length) {
            if (this.CheckConnection()) {
                try {
                    using TcpClient client = new TcpClient(this.IpAddress, this.Port);
                    ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
                    var regData = await master.ReadInputRegistersAsync(this.SlaveAddress, (ushort)baseAddress, (ushort)length);
                    client.Close();
                    master.Dispose();
                    return regData;
                } catch {
                    return null;
                }
            } else {
                return null;
            }
        }

        public ushort[]? ReadInputRegisters(int baseAddress, int length) {
            if (this.CheckConnection()) {
                try {
                    using TcpClient client = new TcpClient(this.IpAddress, this.Port);
                    ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
                    var regData = master.ReadInputRegisters(this.SlaveAddress, (ushort)baseAddress, (ushort)length);
                    client.Close();
                    master.Dispose();
                    return regData;
                } catch {
                    return null;
                }
            } else {
                return null;
            }
        }

        public async Task<ushort[]?> ReadInputRegistersAsync(int baseAddress, int length) {
            if (this.CheckConnection()) {
                try {
                    using TcpClient client = new TcpClient(this.IpAddress, this.Port);
                    ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
                    var regData = await master.ReadInputRegistersAsync(this.SlaveAddress, (ushort)baseAddress, (ushort)length);
                    client.Close();
                    master.Dispose();
                    return regData;
                } catch {
                    return null;
                }
            } else {
                return null;
            }
        }
    }
}
