using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace TheLoadingBean.Client.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public bool IsAuthenticated => _currentUser.Identity?.IsAuthenticated ?? false;
        public bool IsAdmin => _currentUser.IsInRole("Admin");
        public string UserId => _currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(_anonymous);

            var claims = ParseClaimsFromJwt(token);
            var claimsIdentity = new ClaimsIdentity(claims, "jwt");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            _currentUser = claimsPrincipal;

            return new AuthenticationState(claimsPrincipal);
        }

        public void NotifyUserAuthentication(string token)
        {
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            _currentUser = user;

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            _currentUser = _anonymous;
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

        public async Task<bool> GetIsAuthenticatedAsync()
        {
            var state = await GetAuthenticationStateAsync();
            return state.User.Identity?.IsAuthenticated ?? false;
        }

        public async Task<bool> GetIsAdminAsync()
        {
            var state = await GetAuthenticationStateAsync();
            return state.User.IsInRole("Admin");
        }

        public async Task<string> GetUserIdAsync()
        {
            var state = await GetAuthenticationStateAsync();
            return state.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        private List<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var json = System.Text.Encoding.UTF8.GetString(jsonBytes);

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            if (keyValuePairs == null)
                return claims;

            // Lägg till detta (för att tolka vanlig "role")
            if (keyValuePairs.TryGetValue("role", out object? roleObj) && roleObj is not null)
            {
                if (roleObj.ToString()!.Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roleObj.ToString()!) ?? Array.Empty<string>();
                    claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleObj.ToString()!));
                }

                keyValuePairs.Remove("role"); // ta bort så det inte läggs dubbelt
            }

            claims.AddRange(keyValuePairs
                .Where(kvp => kvp.Value != null)
                .Select(kvp => new Claim(kvp.Key, kvp.Value!.ToString()!)));

            return claims;
        }


        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
                case 1: base64 += "==="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
