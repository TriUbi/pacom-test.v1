using DeviceFrontend.Components;
using DeviceFrontend.Helpers;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Lägger till stöd för Razor-komponenter (Blazor) och interaktivt server-renderingsläge
/// </summary>
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

/// <summary>
/// Registrerar ett anpassat JWT-handler som lägger till token i varje HTTP-anrop
/// </summary>
builder.Services.AddTransient<JwtHandler>();

/// <summary>
/// Skapar en HttpClient med basadress till backend och kopplar den till vår JwtHandler
/// klienten blir "AuthorizedClient"
/// </summary>
builder.Services.AddHttpClient("AuthorizedClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5277");
})
.AddHttpMessageHandler<JwtHandler>();

/// <summary>
/// Gör så att vi kan injicera HttpClient i komponenter
/// Den här klienten använder "AuthorizedClient" som vi definierade ovan
/// </summary>
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthorizedClient"));

var app = builder.Build();

app.UseStaticFiles();
/// <summary>
/// Skyddar formulär mot CSRF-attacker.
/// </summary>
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
