using TheLoadingBean.Shared.DTOs;

namespace TheLoadingBean.Client.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerResponseDto>> GetAllCustomersAsync();
        Task<bool> MakeAdminAsync(string customerId);
        Task<bool> ToggleAdminAsync(string customerId);
    }
}
