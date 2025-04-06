/// <summary>
/// Denna klass sparar JWT-token tillfälligt när appen körs
/// Token används för att visa att användaren är inloggad och för att skicka med vid API-anrop
/// </summary>

namespace DeviceFrontend.Helpers
{
    public static class JwtStore
    {
        public static string? Token { get; set; }
    }
}
