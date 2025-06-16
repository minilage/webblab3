using System.Net.Http.Json;
using TheLoadingBean.Shared.DTOs;

namespace TheLoadingBean.Client.Services
{
    public interface IOrderService
    {
        Task<List<OrderResponseDto>> GetOrdersByUserAsync();
    }

    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private const string _baseUrl = "api/order"; // används nu korrekt

        public OrderService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<List<OrderResponseDto>> GetOrdersByUserAsync()
        {
            var userId = _authService.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                throw new Exception("User ID is missing.");

            var endpoint = $"{_baseUrl}/customer/{userId}";
            var response = await _httpClient.GetFromJsonAsync<List<OrderResponseDto>>(endpoint);
            return response ?? new();
        }
    }
}