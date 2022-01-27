using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FacilityMonitoring.Infrastructure.Data.Model;
using Modbus.Device;

namespace FacilityMonitoring.Infrastructure.Services {

    public class ModbusResult {
        public bool[] DiscreteInputs { get; set; }
        public bool[] Coils { get; set; }
        public ushort[] HoldingRegisters { get; set; }
        public ushort[] InputRegisters { get; set; }
    }

    public interface IModbusService {
        bool Connect(string Ip, int port);
        void Disconnect();

        ushort[] ReadInputRegisters(int slaveId,int baseAddress,int length);
        ushort[] ReadHoldingRegisters(int slaveId, int baseAddress, int length);
        bool[] ReadDiscreteInputs(int slaveId, int baseAddress, int length);
        bool[] ReadCoils(int slaveId, int baseAddress,int length);

        Task<ushort[]> ReadInputRegistersAsync(byte slaveId, ushort baseAddress, ushort length);
        Task<ushort[]> ReadHoldingRegistersAsync(byte slaveId, ushort baseAddress, ushort length);
        Task<bool[]> ReadDiscreteInputsAsync(byte slaveId, ushort baseAddress, ushort length);
        Task<bool[]> ReadCoilsAsync(byte slaveId, ushort baseAddress, ushort length);

        Task<ModbusResult> ReadAll(string ip,int port,int slaveid,ModbusConfig config);

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

        public async Task<ModbusResult> ReadAll(string ip, int port, int slaveid, ModbusConfig config) {
            if (this.Connect(ip, port)) {
                var dInputs = await this.ReadDiscreteInputsAsync((byte)slaveid, 0, (ushort)config.DiscreteInputs);
                var holding = await this.ReadHoldingRegistersAsync((byte)slaveid, 0, (ushort)config.HoldingRegisters);
                var inputs = await this.ReadInputRegistersAsync((byte)slaveid, 0, (ushort)config.InputRegisters);
                var coils = await this.ReadCoilsAsync((byte)slaveid, 0, (ushort)config.Coils);
                this.Disconnect();
                return new ModbusResult() { Coils = coils, DiscreteInputs = dInputs, HoldingRegisters = holding, InputRegisters = inputs };
            } else {
                return null; 
            }
        }

        public bool[] ReadCoils(int slaveId,int baseAddress, int length) {
            if (this.connected) {
                return this.modbus.ReadCoils((byte)slaveId, (ushort)baseAddress, (ushort)length);
            } else {
                return null;
            }
        }

        public Task<bool[]> ReadCoilsAsync(byte slaveId, ushort baseAddress, ushort length) {
            if (this.connected) {
                return this.modbus.ReadCoilsAsync(slaveId, baseAddress, length);
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

        public Task<bool[]> ReadDiscreteInputsAsync(byte slaveId,ushort baseAddress, ushort length) {
            if (this.connected) {
                return this.modbus.ReadInputsAsync(slaveId, baseAddress, length);
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

        public Task<ushort[]> ReadHoldingRegistersAsync(byte slaveId, ushort baseAddress, ushort length) {
            if (this.connected) {
                return this.modbus.ReadHoldingRegistersAsync(slaveId, baseAddress, length);
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

        public Task<ushort[]> ReadInputRegistersAsync(byte slaveId, ushort baseAddress, ushort length) {
            if (this.connected) {
                return this.modbus.ReadInputRegistersAsync(slaveId,baseAddress,length);
            } else {
                return null;
            }
        }
    }
}
