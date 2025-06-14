using System.Net.Http.Json;
using TheLoadingBean.Shared.DTOs;

namespace TheLoadingBean.Client.Services
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductResponseDto> GetProductByIdAsync(string id);
        Task<List<ProductResponseDto>> SearchProductsAsync(string searchTerm);
        Task<ProductResponseDto> CreateProductAsync(CreateProductDto product);
        Task<ProductResponseDto> UpdateProductAsync(string id, UpdateProductDto product);
        Task DeleteProductAsync(string id);
        Task<List<ProductResponseDto>> GetDiscontinuedProductsAsync();
        Task<List<ProductResponseDto>> GetAvailableProductsAsync();
    }

    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProductResponseDto>> GetAllProductsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ProductResponseDto>>("api/product")
                       ?? new List<ProductResponseDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fetching products: {ex.Message}");
                return new List<ProductResponseDto>();
            }
        }

        public async Task<ProductResponseDto> GetProductByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<ProductResponseDto>($"api/product/{id}")
                   ?? throw new Exception("Produkt kunde inte hittas.");
        }

        public async Task<List<ProductResponseDto>> SearchProductsAsync(string searchTerm)
        {
            return await _httpClient.GetFromJsonAsync<List<ProductResponseDto>>($"api/product/search?searchTerm={searchTerm}")
                   ?? new List<ProductResponseDto>();
        }

        public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto product)
        {
            var response = await _httpClient.PostAsJsonAsync("api/product", product);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductResponseDto>()
                   ?? throw new Exception("Svar saknade innehåll vid skapande av produkt.");
        }

        public async Task<ProductResponseDto> UpdateProductAsync(string id, UpdateProductDto product)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/product/{id}", product);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductResponseDto>()
                   ?? throw new Exception("Svar saknade innehåll vid uppdatering av produkt.");
        }

        public async Task DeleteProductAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/product/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<ProductResponseDto>> GetDiscontinuedProductsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ProductResponseDto>>("api/product/discontinued")
                   ?? new List<ProductResponseDto>();
        }

        public async Task<List<ProductResponseDto>> GetAvailableProductsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ProductResponseDto>>("api/product/available")
                   ?? new List<ProductResponseDto>();
        }
    }
}
