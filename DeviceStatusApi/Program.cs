using Microsoft.EntityFrameworkCore;
using DeviceStatusApi.Data;
using DeviceStatusApi.Models;
using System.Net.Sockets;
using NModbus;

var builder = WebApplication.CreateBuilder(args);

// Register database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        "Server=localhost;Port=8889;Database=device_db;User=root;Password=root;",
        new MySqlServerVersion(new Version(8, 0, 40))
    )
);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.SetIsOriginAllowed(origin => origin == "http://localhost:5028")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors(x => x
    .SetIsOriginAllowed(origin => origin == "http://localhost:5028")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

// Get all statuses
app.MapGet("/api/status", async (AppDbContext db) =>
    await db.Status.ToListAsync());

// Get first status
app.MapGet("/api/status/first", async (AppDbContext db) =>
{
    var current = await db.Status
        .OrderByDescending(s => s.Id)
        .FirstOrDefaultAsync();

    return current is null
        ? Results.Ok(new DeviceStatus { IsOn = false })
        : Results.Ok(current);
});

// Get by ID
app.MapGet("/api/status/{id}", async (AppDbContext db, int id) =>
    await db.Status.FindAsync(id) is DeviceStatus status
        ? Results.Ok(status)
        : Results.NotFound());

// Create
app.MapPost("/api/status", async (AppDbContext db, DeviceStatus input) =>
{
    var nextCoil = await db.Status.CountAsync();
    input.CoilAddress = nextCoil;

    db.Status.Add(input);
    await db.SaveChangesAsync();

    using var client = new TcpClient("127.0.0.1", 5020);
    var factory = new ModbusFactory();
    var master = factory.CreateMaster(client);
    master.WriteSingleCoil(1, (ushort)input.CoilAddress, input.IsOn);


    return Results.Created($"/api/status/{input.Id}", input);
});

// Update
app.MapPut("/api/status/{id}", async (AppDbContext db, int id, DeviceStatus updated) =>
{
    var status = await db.Status.FindAsync(id);
    if (status is null) return Results.NotFound();

    status.IsOn = updated.IsOn;
    await db.SaveChangesAsync();

    using var client = new TcpClient("127.0.0.1", 5020);
    var factory = new ModbusFactory();
    var master = factory.CreateMaster(client);
    master.WriteSingleCoil(1, (ushort)status.CoilAddress, status.IsOn);

    return Results.Ok(status);
});

// Delete
app.MapDelete("/api/status/{id}", async (AppDbContext db, int id) =>
{
    var status = await db.Status.FindAsync(id);
    if (status is null) return Results.NotFound();

    db.Status.Remove(status);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Read Modbus statuses
app.MapGet("/api/modbus/status", async (AppDbContext db) =>
{
    try
    {
        var devices = await db.Status.OrderBy(d => d.Id).ToListAsync();

        using var client = new TcpClient("127.0.0.1", 5020);
        var factory = new ModbusFactory();
        var master = factory.CreateMaster(client);

        var result = new List<object>();

        foreach (var device in devices)
        {
            bool[] coil = master.ReadCoils(1, (ushort)device.CoilAddress, 1);

            result.Add(new
            {
                Id = device.Id,
                Name = device.Name,
                IsOn = coil[0]
            });
        }

        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Modbus-läsfel: {ex.Message}");
    }
});

app.Run();
