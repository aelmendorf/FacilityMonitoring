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
        bool Connect(string Ip, int port);
        void Disconnect();

        ushort[] ReadInputRegisters(int slaveId,int baseAddress,int length);
        ushort[] ReadHoldingRegisters(int slaveId, int baseAddress, int length);
        bool[] ReadDiscreteInputs(int slaveId, int baseAddress, int length);
        bool[] ReadCoils(int slaveId, int baseAddress,int length);

        Task<ushort[]> ReadInputRegistersAsync(int slaveId, int baseAddress, int length);
        Task<ushort[]> ReadHoldingRegistersAsync(int slaveId, int baseAddress, int length);
        Task<bool[]> ReadDiscreteInputsAsync(int slaveId, int baseAddress, int length);
        Task<bool[]> ReadCoilsAsync(int slaveId,int baseAddress, int length);

    }

    public class ModbusService : IModbusService {

        private TcpClient client;
        private ModbusIpMaster modbus;
        private bool connected;

        public ModbusService() {
            this.connected = false;
        }

        public bool Connect(string Ip,int port) {
            try {
                this.client = new TcpClient(Ip,port);
                this.modbus = ModbusIpMaster.CreateIp(client);
                this.connected = true;
                return true;
            } catch {
                Console.WriteLine("Exception while connecting");
                return false;
            }
        }

        public void Disconnect() {
            if(this.client!=null && this.modbus != null) {
                if (this.client.Connected) {
                    this.client.Close();
                    this.modbus.Dispose();
                    
                }
            }
            this.connected = false;
        }

        public bool[] ReadCoils(int slaveId,int baseAddress, int length) {
            if (this.connected) {
                return this.modbus.ReadCoils((byte)slaveId, (ushort)baseAddress, (ushort)length);
            } else {
                return null;
            }
        }

        public async Task<bool[]> ReadCoilsAsync(int slaveId,int baseAddress, int length) {
            if (this.connected) {
                return await this.modbus.ReadCoilsAsync((byte)slaveId, (ushort)baseAddress, (ushort)length);
            } else {
                return null;
            }
        }

        public bool[] ReadDiscreteInputs(int slaveId,int baseAddress, int length) {
            if (this.connected) {
                return this.modbus.ReadInputs((byte)slaveId, (ushort)baseAddress, (ushort)length);
            } else {
                return null;
            }
        }

        public async Task<bool[]> ReadDiscreteInputsAsync(int slaveId,int baseAddress, int length) {
            if (this.connected) {
                return await this.modbus.ReadInputsAsync((byte)slaveId, (ushort)baseAddress, (ushort)length);
            } else {
                return null;
            }
        }

        public ushort[] ReadHoldingRegisters(int slaveId,int baseAddress, int length) {
            if (this.connected) {
                return this.modbus.ReadInputRegisters((byte)slaveId, (ushort)baseAddress, (ushort)length);
            } else {
                return null;
            }
        }

        public async Task<ushort[]> ReadHoldingRegistersAsync(int slaveId,int baseAddress, int length) {
            if (this.connected) {
                return await this.modbus.ReadInputRegistersAsync((byte)slaveId, (ushort)baseAddress, (ushort)length);
            } else {
                return null;
            }
        }

        public ushort[] ReadInputRegisters(int slaveId,int baseAddress, int length) {
            if (this.connected) {
                return this.modbus.ReadInputRegisters((byte)slaveId, (ushort)baseAddress, (ushort)length);
            } else {
                return null;
            }
        }

        public async Task<ushort[]> ReadInputRegistersAsync(int slaveId, int baseAddress, int length) {
            if (this.connected) {
                return await this.modbus.ReadInputRegistersAsync((byte)slaveId, (ushort)baseAddress, (ushort)length);
            } else {
                return null;
            }
        }
    }
}
