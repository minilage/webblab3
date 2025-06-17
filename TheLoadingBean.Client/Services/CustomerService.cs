using System.Net.Http.Json;
using TheLoadingBean.Shared.DTOs;

namespace TheLoadingBean.Client.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly HttpClient _http;

        public CustomerService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<CustomerResponseDto>> GetAllCustomersAsync()
        {
            return await _http.GetFromJsonAsync<List<CustomerResponseDto>>("api/customer") ?? new();
        }

        public async Task<bool> MakeAdminAsync(string customerId)
        {
            var response = await _http.PutAsync($"api/customer/{customerId}/make-admin", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ToggleAdminAsync(string customerId)
        {
            var response = await _http.PutAsync($"api/customer/{customerId}/toggle-admin", null);
            return response.IsSuccessStatusCode;
        }
    }
}
