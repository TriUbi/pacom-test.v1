using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ModbusController : ControllerBase
{
    // POST: api/modbus/set?coilAddress=1&value=true
    [HttpPost("set")]
    public IActionResult Set([FromQuery] int coilAddress, [FromQuery] bool value)
    {
        using var modbus = new ModbusService();
        modbus.WriteCoilStatus(1, (ushort)coilAddress, value);
        return Ok($"Coil {coilAddress} set to {value}");
    }

    // GET: api/modbus/status?coilAddress=1
    [HttpGet("status")]
    public IActionResult GetStatus([FromQuery] int coilAddress)
    {
        using var modbus = new ModbusService();
        var status = modbus.ReadCoilStatus(1, (ushort)coilAddress);
        return Ok($"Coil {coilAddress} is {(status ? "ON" : "OFF")}");
    }
}
