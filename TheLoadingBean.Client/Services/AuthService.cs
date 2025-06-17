using Blazored.LocalStorage;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using TheLoadingBean.Client.Auth;
using TheLoadingBean.Shared.DTOs;

namespace TheLoadingBean.Client.Services
{
    public interface IAuthService
    {
        Task<TokenDto> LoginAsync(LoginDto loginDto);
        Task<TokenDto> RegisterAsync(RegisterDto registerDto);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<bool> IsAdminAsync();
        Task<string> GetUserIdAsync();
        Task<CustomerResponseDto> GetCurrentUserAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly IToastService _toastService;
        private readonly CustomAuthStateProvider _authStateProvider;
        private const string BaseUrl = "api/auth/";

        public AuthService(
            HttpClient httpClient,
            ILocalStorageService localStorage,
            IToastService toastService,
            AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _toastService = toastService;
            _authStateProvider = (CustomAuthStateProvider)authStateProvider;
        }

        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}login", loginDto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _toastService.ShowError($"Inloggning misslyckades: {error}");
                throw new Exception("Login failed");
            }

            var token = await response.Content.ReadFromJsonAsync<TokenDto>() ?? throw new Exception("Token is null");

            await _localStorage.SetItemAsync("authToken", token.Token);
            await _localStorage.SetItemAsync("tokenExpiration", token.Expiration);
            _authStateProvider.NotifyUserAuthentication(token.Token);
            _toastService.ShowSuccess("Inloggning lyckades!");

            return token;
        }

        public async Task<TokenDto> RegisterAsync(RegisterDto registerDto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}register", registerDto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _toastService.ShowError($"Registrering misslyckades: {error}");
                throw new Exception("Registration failed");
            }

            var token = await response.Content.ReadFromJsonAsync<TokenDto>() ?? throw new Exception("Token is null");

            await _localStorage.SetItemAsync("authToken", token.Token);
            await _localStorage.SetItemAsync("tokenExpiration", token.Expiration);
            _authStateProvider.NotifyUserAuthentication(token.Token);
            _toastService.ShowSuccess("Registrering lyckades!");

            return token;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("tokenExpiration");
            _authStateProvider.NotifyUserLogout();
            _toastService.ShowSuccess("Utloggning lyckades!");
        }

        public async Task<CustomerResponseDto> GetCurrentUserAsync()
        {
            var userId = await GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
                throw new Exception("User ID is missing.");

            var response = await _httpClient.GetFromJsonAsync<CustomerResponseDto>($"api/customer/{userId}")
                ?? throw new Exception("User not found.");

            return response;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            return await _authStateProvider.GetIsAuthenticatedAsync();
        }

        public async Task<bool> IsAdminAsync()
        {
            return await _authStateProvider.GetIsAdminAsync();
        }

        public async Task<string> GetUserIdAsync()
        {
            return await _authStateProvider.GetUserIdAsync();
        }
    }
}
