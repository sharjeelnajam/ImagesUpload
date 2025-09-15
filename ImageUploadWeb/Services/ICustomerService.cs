using ImageUploadWeb.Models;

namespace ImageUploadWeb.Services
{
    public interface ICustomerService
    {
        Task<ApiResponse<List<Customer>>> GetCustomersAsync();
        Task<ApiResponse<Customer>> GetCustomerAsync(int id);
        Task<ApiResponse<Customer>> CreateCustomerAsync(Customer customer);
        Task<ApiResponse<Customer>> UpdateCustomerAsync(Customer customer);
        Task<ApiResponse<object>> DeleteCustomerAsync(int id);
    }
}
