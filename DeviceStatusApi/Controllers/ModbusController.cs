using Microsoft.AspNetCore.Mvc;
using DeviceStatusApi.Data;
using Microsoft.EntityFrameworkCore;
using NModbus;
using System.Net.Sockets;

[ApiController]
[Route("api/[controller]")]
public class ModbusController : ControllerBase
{
    private readonly AppDbContext _db;

    public ModbusController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetAllCoilStatus()
    {
        var devices = await _db.Status.OrderBy(d => d.Id).ToListAsync();

        using var client = new TcpClient("127.0.0.1", 5020);
        var factory = new ModbusFactory();
        var master = factory.CreateMaster(client);

        var result = new List<object>();

        foreach (var device in devices)
        {
            bool[] coil = master.ReadCoils(1, (ushort)device.CoilAddress, 1);
            result.Add(new
            {
                device.Id,
                device.Name,
                IsOn = coil[0]
            });
        }

        return Ok(result);
    }
}
