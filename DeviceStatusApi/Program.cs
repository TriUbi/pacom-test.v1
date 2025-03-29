using Microsoft.EntityFrameworkCore;
using DeviceStatusApi.Data;
using DeviceStatusApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Registrera databasen
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        "Server=localhost;Port=8889;Database=device_db;User=root;Password=root;",
        new MySqlServerVersion(new Version(8, 0, 40))
    )
);

var app = builder.Build();

// Hämta alla statusar
app.MapGet("/api/status", async (AppDbContext db) =>
    await db.Status.ToListAsync());

// Hämta första (för frontend-indikator)
app.MapGet("/api/status/first", async (AppDbContext db) =>
{
    var current = await db.Status
        .OrderBy(s => s.Id)
        .FirstOrDefaultAsync();

    return current is null
        ? Results.Ok(new DeviceStatus { IsOn = false })
        : Results.Ok(current);
});

// Hämta en status via ID
app.MapGet("/api/status/{id}", async (AppDbContext db, int id) =>
    await db.Status.FindAsync(id) is DeviceStatus status
        ? Results.Ok(status)
        : Results.NotFound());

// Skapa ny status
app.MapPost("/api/status", async (AppDbContext db, DeviceStatus input) =>
{    Console.WriteLine($"POST recibido: IsOn={input.IsOn}");
    db.Status.Add(input);
    await db.SaveChangesAsync();
    return Results.Created($"/api/status/{input.Id}", input);
});

// Uppdatera status via ID
app.MapPut("/api/status/{id}", async (AppDbContext db, int id, DeviceStatus updated) =>
{
    var status = await db.Status.FindAsync(id);
    if (status is null) return Results.NotFound();

    status.IsOn = updated.IsOn;
    await db.SaveChangesAsync();
    return Results.Ok(status);
});

// Radera status via ID
app.MapDelete("/api/status/{id}", async (AppDbContext db, int id) =>
{
    var status = await db.Status.FindAsync(id);
    if (status is null) return Results.NotFound();

    db.Status.Remove(status);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


// TEST
app.MapPost("/api/test", async (AppDbContext db) =>
{
    var nuevo = new DeviceStatus { IsOn = true };
    db.Status.Add(nuevo);
    await db.SaveChangesAsync();
    return Results.Ok(nuevo);
});


app.Run();
