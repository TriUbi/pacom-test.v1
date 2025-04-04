using DeviceFrontend.Components;
using DeviceFrontend.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddTransient<JwtHandler>();
builder.Services.AddScoped(sp => new HttpClient(new JwtHandler())
{
    BaseAddress = new Uri("http://localhost:5277")
});

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
