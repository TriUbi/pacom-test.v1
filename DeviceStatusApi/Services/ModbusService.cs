using System.Net.Sockets;
using NModbus;

/// <summary>
/// Hjälpklass för att läsa och skriva till Modbus-coils
/// Ansluter till Modbus-servern via TCP
/// </summary>
public class ModbusService : IDisposable

{
    private readonly IModbusMaster _master;
    private readonly TcpClient _client;

      /// <summary>
    /// Startar Modbus anslutning till localhost på port 5020
    /// </summary>
    public ModbusService()
    {
       _client = new TcpClient("127.0.0.1", 5020);
        var factory = new ModbusFactory();
        _master = factory.CreateMaster(_client);
    }

    /// <summary>
    /// Läser status (on/off) för en coil på en viss adress
    /// </summary>
    /// <param name="slaveId">ID för Modbus enheten</param>
    /// <param name="address">Adress till coil som ska läsas</param>
    /// <returns>True om coil är på (ON), annars false (OFF)</returns>
    public bool ReadCoilStatus(byte slaveId, ushort address)
    {
        bool[] coils = _master.ReadCoils(slaveId, address, 1);
        return coils[0];
    }

    /// <summary>
    /// Skriver status till en coil (on/offf)
    /// </summary>
    /// <param name="slaveId">ID för Modbus enheten</param>
    /// <param name="address">Adress till coil som ska ändras</param>
    /// <param name="value">True för att sätta ON, false för OFF</param>
    public void WriteCoilStatus(byte slaveId, ushort address, bool value)
    {
        _master.WriteSingleCoil(slaveId, address, value);
    }

    /// <summary>
    /// Stänger Modbus anslutningen
    /// </summary>
    public void Dispose()
    {
        _client?.Close();
    }
}
