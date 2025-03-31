using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ModbusController : ControllerBase
{
    [HttpPost("set")]
    public IActionResult Set([FromQuery] bool value)
    {
        using var modbus = new ModbusService();
        modbus.WriteCoilStatus(1, 0, value); 
        return Ok($"Value set to {value}");
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        using var modbus = new ModbusService();
        var status = modbus.ReadCoilStatus(1, 0);
        return Ok(status);
    }
}
