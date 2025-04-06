using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceFrontend.Helpers
{

    /// <summary>
    /// En egen HttpHandler som automatiskt lägger till JWT-token i varje API anrop
    /// </summary>
    public class JwtHandler : DelegatingHandler
    {
          /// <summary>
        /// Lägger till en "Authorization"-header med JWT-token om det finns
        /// Denna metod körs automatiskt varje gång appen gör ett HTTP anrop
        /// </summary>
        /// <param name="request">Det HTTP-anrop som skickas till servern</param>
        /// <returns>Svar från servern.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Om det finns en JWT-token, lägg till den i anropet
            if (!string.IsNullOrEmpty(JwtStore.Token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtStore.Token);
            }

            // Skicka vidare anropet
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
