using Microsoft.EntityFrameworkCore;
using DeviceStatusApi.Data;
using DeviceStatusApi.Models;
using DeviceStatusApi;
using Microsoft.AspNetCore.Cors;
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

// Enable CORS - IMPORTANT: must be before endpoints
app.UseCors(x => x
    .SetIsOriginAllowed(origin => origin == "http://localhost:5028")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

// Get all statuses
app.MapGet("/api/status", async (AppDbContext db) =>
    await db.Status.ToListAsync());

// Get first status (for frontend indicator)
app.MapGet("/api/status/first", async (AppDbContext db) =>
{
    var current = await db.Status
        .OrderBy(s => s.Id)
        .FirstOrDefaultAsync();

    return current is null
        ? Results.Ok(new DeviceStatus { IsOn = false })
        : Results.Ok(current);
});

// Get status by ID
app.MapGet("/api/status/{id}", async (AppDbContext db, int id) =>
    await db.Status.FindAsync(id) is DeviceStatus status
        ? Results.Ok(status)
        : Results.NotFound());

// Create new status
app.MapPost("/api/status", async (AppDbContext db, DeviceStatus input) =>
{
    db.Status.Add(input);
    await db.SaveChangesAsync();

    //skicka till Modbus
    using var client = new TcpClient("127.0.0.1", 502);
    var factory = new ModbusFactory();
    var master = factory.CreateMaster(client);
    master.WriteSingleCoil(1, 0, input.IsOn); 

    return Results.Created($"/api/status/{input.Id}", input);
});

// Update status by ID
app.MapPut("/api/status/{id}", async (AppDbContext db, int id, DeviceStatus updated) =>
{
    var status = await db.Status.FindAsync(id);
    
//skicka till Modbus
    using var client = new TcpClient("127.0.0.1", 502);
    var factory = new ModbusFactory();
    var master = factory.CreateMaster(client);
    master.WriteSingleCoil(1, 0, updated.IsOn); 

    if (status is null) return Results.NotFound();

    status.IsOn = updated.IsOn;
    await db.SaveChangesAsync();
    return Results.Ok(status);
});

// Delete status by ID
app.MapDelete("/api/status/{id}", async (AppDbContext db, int id) =>
{
    var status = await db.Status.FindAsync(id);
    if (status is null) return Results.NotFound();

    db.Status.Remove(status);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
