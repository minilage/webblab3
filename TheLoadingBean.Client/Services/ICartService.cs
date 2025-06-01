using TheLoadingBean.Shared.DTOs;

namespace TheLoadingBean.Client.Services
{
    public interface ICartService
    {
        Task<bool> TryAddToCartAsync(ProductResponseDto product);
        Task<List<ProductResponseDto>> GetCartItemsAsync();
        Task RemoveFromCartAsync(string productId);
        Task ClearCartAsync();
        Task<decimal> GetTotalAsync();
    }
}