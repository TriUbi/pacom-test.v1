using System.Net.Sockets;
using NModbus;

public class ModbusService
{
    private readonly IModbusMaster _master;
    private readonly TcpClient _client;

    public ModbusService()
    {
        _client = new TcpClient("127.0.0.1", 502); 
        var factory = new ModbusFactory();
        _master = factory.CreateMaster(_client);
    }

    public bool ReadCoilStatus(byte slaveId, ushort address)
    {
        bool[] coils = _master.ReadCoils(slaveId, address, 1);
        return coils[0];
    }

    public void WriteCoilStatus(byte slaveId, ushort address, bool value)
    {
        _master.WriteSingleCoil_
    }
    }