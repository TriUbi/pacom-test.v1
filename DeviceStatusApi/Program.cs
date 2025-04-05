using Microsoft.EntityFrameworkCore;
using DeviceStatusApi.Data;
using DeviceStatusApi.Models;
using System.Net.Sockets;
using NModbus;
using DeviceStatusApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Ansluter till databasen med MySQL och lägger till Entity Framework
/// </summary>

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        "Server=localhost;Port=8889;Database=device_db;User=root;Password=root;",
        new MySqlServerVersion(new Version(8, 0, 40))
    )
);

/// <summary>
/// Tillåter frontend att prata med backend (CORS)
/// </summary>
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

/// <summary>
/// Gör det möjligt att använda Controllers (t.ex. i detta fall: AuthController, ModbusController)
/// </summary>
builder.Services.AddControllers();

/// <summary>
/// JWT-tjänst för att skapa och validera tokens
/// </summary>
builder.Services.AddSingleton<TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenService = new TokenService(builder.Configuration);
        options.TokenValidationParameters = tokenService.GetValidationParameters();
    });

builder.Services.AddAuthorization(); 

var app = builder.Build();

/// <summary>
/// Aktiverar CORS-policy
/// </summary>
app.UseCors(x => x
    .SetIsOriginAllowed(origin => origin == "http://localhost:5028")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

/// <summary>
/// Aktiverar autentisering + behörighetskontroll (JWT)
/// </summary>
app.UseAuthentication();
app.UseAuthorization();

/// <summary>
/// Hämtar alla enheter från databasen
/// </summary>
app.MapGet("/api/status", async (AppDbContext db) =>
    await db.Status.ToListAsync());

/// <summary>
/// Hämtar en specifik enhet med ID
/// </summary>
app.MapGet("/api/status/{id}", async (AppDbContext db, int id) =>
    await db.Status.FindAsync(id) is DeviceStatus status
        ? Results.Ok(status)
        : Results.NotFound());

/// <summary>
/// Lägger till en ny enhet i databasen + skickar info till Modbus
/// </summary>
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

/// <summary>
/// Uppdaterar en enhets status (on/off) + skriver till Modbus
/// </summary>
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

/// <summary>
/// Tar bort en enhet från databasen
/// </summary>
app.MapDelete("/api/status/{id}", async (AppDbContext db, int id) =>
{
    var status = await db.Status.FindAsync(id);
    if (status is null) return Results.NotFound();

    db.Status.Remove(status);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


/// <summary>
/// Aktiverar controllers
/// </summary>
app.MapControllers();
app.Run();
