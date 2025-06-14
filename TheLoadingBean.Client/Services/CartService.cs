using Blazored.LocalStorage;
using TheLoadingBean.Shared.DTOs;

namespace TheLoadingBean.Client.Services
{
    public class CartService : ICartService
    {
        private const string CartKey = "local_cart";
        private readonly ILocalStorageService _localStorage;
        private List<ProductResponseDto> _cartItems = new();

        public CartService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<List<ProductResponseDto>> GetCartItemsAsync()
        {
            _cartItems = await _localStorage.GetItemAsync<List<ProductResponseDto>>(CartKey) ?? new();
            return _cartItems;
        }

        public async Task<bool> TryAddToCartAsync(ProductResponseDto product)
        {
            if (product == null || !product.IsAvailable || product.IsDiscontinued)
                return false;

            _cartItems = await GetCartItemsAsync();

            var existingItem = _cartItems.FirstOrDefault(p => p.Id == product.Id);
            if (existingItem != null)
                existingItem.Quantity++;
            else
            {
                product.Quantity = 1;
                _cartItems.Add(product);
            }

            await SaveCartAsync();
            return true;
        }

        public async Task RemoveFromCartAsync(string productId)
        {
            _cartItems = await GetCartItemsAsync();
            var item = _cartItems.FirstOrDefault(p => p.Id == productId);
            if (item != null)
                _cartItems.Remove(item);

            await SaveCartAsync();
        }

        public async Task ClearCartAsync()
        {
            _cartItems.Clear();
            await _localStorage.RemoveItemAsync(CartKey);
        }

        public async Task<decimal> GetTotalAsync()
        {
            _cartItems = await GetCartItemsAsync();
            return _cartItems.Sum(p => p.Price * p.Quantity);
        }

        public async Task UpdateQuantityAsync(string productId, int change)
        {
            _cartItems = await GetCartItemsAsync();
            var item = _cartItems.FirstOrDefault(p => p.Id == productId);
            if (item != null)
            {
                item.Quantity += change;
                if (item.Quantity <= 0)
                    _cartItems.Remove(item);
            }

            await SaveCartAsync();
        }

        private async Task SaveCartAsync()
        {
            await _localStorage.SetItemAsync(CartKey, _cartItems);
        }
    }
}