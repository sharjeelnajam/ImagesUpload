using ImageUploadWeb.Models;
using System.Net.Http.Json;

namespace ImageUploadWeb.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(IHttpClientFactory httpClientFactory, ILogger<CustomerService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ImageUploadAPI");
            _logger = logger;
        }

        public async Task<ApiResponse<List<Customer>>> GetCustomersAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<Customer>>>("api/Customer");
                return response ?? ApiResponse<List<Customer>>.ErrorResult("Failed to retrieve customers");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customers");
                return ApiResponse<List<Customer>>.ErrorResult("An error occurred while retrieving customers");
            }
        }

        public async Task<ApiResponse<Customer>> GetCustomerAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<Customer>>($"api/Customer/{id}");
                return response ?? ApiResponse<Customer>.ErrorResult("Failed to retrieve customer");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer {CustomerId}", id);
                return ApiResponse<Customer>.ErrorResult("An error occurred while retrieving the customer");
            }
        }

        public async Task<ApiResponse<Customer>> CreateCustomerAsync(Customer customer)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Customer", customer);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<Customer>>();
                    return result ?? ApiResponse<Customer>.ErrorResult("Failed to create customer");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Create customer failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);
                    return ApiResponse<Customer>.ErrorResult($"Create failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return ApiResponse<Customer>.ErrorResult("An error occurred while creating the customer");
            }
        }

        public async Task<ApiResponse<Customer>> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Customer/{customer.Id}", customer);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<Customer>>();
                    return result ?? ApiResponse<Customer>.ErrorResult("Failed to update customer");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Update customer failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);
                    return ApiResponse<Customer>.ErrorResult($"Update failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer {CustomerId}", customer.Id);
                return ApiResponse<Customer>.ErrorResult("An error occurred while updating the customer");
            }
        }

        public async Task<ApiResponse<object>> DeleteCustomerAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Customer/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                    return result ?? ApiResponse<object>.ErrorResult("Failed to delete customer");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Delete customer failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);
                    return ApiResponse<object>.ErrorResult($"Delete failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer {CustomerId}", id);
                return ApiResponse<object>.ErrorResult("An error occurred while deleting the customer");
            }
        }
    }
}
