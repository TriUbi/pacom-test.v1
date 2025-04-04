using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceFrontend.Helpers
{
    public class JwtHandler : DelegatingHandler
    {
        public JwtHandler()
        {
            InnerHandler = new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(JwtStore.Token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtStore.Token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
