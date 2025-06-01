using TheLoadingBean.Shared.DTOs;

namespace TheLoadingBean.Client.Services
{
    public class CartService : ICartService
    {
        private readonly List<ProductResponseDto> _cartItems = new();

        public Task<List<ProductResponseDto>> GetCartItemsAsync()
        {
            return Task.FromResult(_cartItems.ToList());
        }

        public Task<bool> TryAddToCartAsync(ProductResponseDto product)
        {
            if (product == null || !product.IsAvailable || product.IsDiscontinued)
                return Task.FromResult(false);

            _cartItems.Add(product);
            return Task.FromResult(true);
        }

        public Task RemoveFromCartAsync(string productId)
        {
            var item = _cartItems.FirstOrDefault(p => p.Id == productId);
            if (item != null)
                _cartItems.Remove(item);

            return Task.CompletedTask;
        }

        public Task ClearCartAsync()
        {
            _cartItems.Clear();
            return Task.CompletedTask;
        }

        public Task<decimal> GetTotalAsync()
        {
            var total = _cartItems.Sum(p => p.Price);
            return Task.FromResult(total);
        }
    }
}
