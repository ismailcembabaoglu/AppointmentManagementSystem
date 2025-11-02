using Blazored.LocalStorage; // BU SATIRI EKLE
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AppointmentManagementSystem.BlazorUI.Services.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsStringAsync("authToken");

            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Token expiration kontrolü kaldırıldı - kullanıcı manuel logout yapana kadar session devam eder
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            await _localStorage.SetItemAsStringAsync("authToken", token);
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            // Clear all authentication data from localStorage
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("userId");
            await _localStorage.RemoveItemAsync("userName");
            await _localStorage.RemoveItemAsync("userRole");
            await _localStorage.RemoveItemAsync("businessId");
            
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private void StartTokenExpirationCheck()
        {
            // Check token expiration every 60 seconds
            _tokenExpirationTimer = new System.Threading.Timer(async _ =>
            {
                try
                {
                    var token = await _localStorage.GetItemAsStringAsync("authToken");
                    if (!string.IsNullOrEmpty(token) && IsTokenExpired(token))
                    {
                        await MarkUserAsLoggedOut();
                    }
                }
                catch
                {
                    // Silently fail - don't crash the app
                }
            }, null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
        }

        private bool IsTokenExpired(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token) || !token.Contains('.'))
                    return true;

                var payload = token.Split('.')[1];
                payload = payload.Replace('-', '+').Replace('_', '/');
                var jsonBytes = ParseBase64WithoutPadding(payload);
                var json = Encoding.UTF8.GetString(jsonBytes);

                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                
                if (keyValuePairs != null && keyValuePairs.TryGetValue("exp", out var expValue))
                {
                    // JWT exp is in Unix timestamp (seconds since epoch)
                    long exp = 0;
                    
                    if (expValue is JsonElement element)
                    {
                        if (element.ValueKind == JsonValueKind.Number)
                        {
                            exp = element.GetInt64();
                        }
                        else if (element.ValueKind == JsonValueKind.String && long.TryParse(element.GetString(), out var parsedExp))
                        {
                            exp = parsedExp;
                        }
                    }
                    else if (expValue is long longValue)
                    {
                        exp = longValue;
                    }
                    else if (long.TryParse(expValue.ToString(), out var parsedExp))
                    {
                        exp = parsedExp;
                    }

                    if (exp > 0)
                    {
                        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp);
                        var currentTime = DateTimeOffset.UtcNow;
                        
                        // Token is expired if current time is past expiration time
                        return currentTime >= expirationTime;
                    }
                }

                // If no exp claim found, consider token as valid (shouldn't happen in production)
                return false;
            }
            catch
            {
                // If any error occurs, consider token as expired for safety
                return true;
            }
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt) || !jwt.Contains('.'))
                return new List<Claim>();

            var payload = jwt.Split('.')[1];

            // URL-safe Base64 düzeltmesi
            payload = payload.Replace('-', '+').Replace('_', '/');

            var jsonBytes = ParseBase64WithoutPadding(payload);
            var json = Encoding.UTF8.GetString(jsonBytes);

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            var claims = new List<Claim>();

            if (keyValuePairs != null)
            {
                foreach (var kvp in keyValuePairs)
                {
                    if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var role in element.EnumerateArray())
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.GetString() ?? string.Empty));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty));
                    }
                }
            }

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        public void Dispose()
        {
            _tokenExpirationTimer?.Dispose();
        }
    }
}
