using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace AppointmentManagementSystem.BlazorUI.Services
{
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public AuthorizationMessageHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            // Login ve Register endpoint'leri için token ekleme
            var requiresAuth = !request.RequestUri?.AbsolutePath.Contains("/auth/login") == true &&
                              !request.RequestUri?.AbsolutePath.Contains("/auth/register") == true;

            if (requiresAuth)
            {
                var token = await _localStorage.GetItemAsStringAsync("authToken");
                if (!string.IsNullOrEmpty(token))
                {
                    // Token'dan tırnak işaretlerini temizle
                    token = token.Trim('"');
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
