using ImageUploadWeb.Models;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Forms;

namespace ImageUploadWeb.Services
{
    public class ImageService : IImageService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly ILogger<ImageService> _logger;

        public ImageService(IHttpClientFactory httpClientFactory, IJSRuntime jsRuntime, ILogger<ImageService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ImageUploadAPI");
            _jsRuntime = jsRuntime;
            _logger = logger;
        }

        public async Task<ApiResponse<List<CustomerImage>>> GetCustomerImagesAsync(int customerId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<CustomerImage>>>($"api/Image/customer/{customerId}");
                return response ?? ApiResponse<List<CustomerImage>>.ErrorResult("Failed to retrieve images");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving images for customer {CustomerId}", customerId);
                return ApiResponse<List<CustomerImage>>.ErrorResult("An error occurred while retrieving images");
            }
        }

        public async Task<ApiResponse<CustomerImage>> UploadBase64ImageAsync(Base64ImageUploadRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Image/upload-base64", request);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<CustomerImage>>();
                    return result ?? ApiResponse<CustomerImage>.ErrorResult("Failed to upload image");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Upload failed with status {StatusCode}: {Error}. Request: CustomerId={CustomerId}, FileName={FileName}, ContentType={ContentType}", 
                        response.StatusCode, errorContent, request.CustomerId, request.FileName, request.ContentType);
                    return ApiResponse<CustomerImage>.ErrorResult($"Upload failed: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image for customer {CustomerId}", request.CustomerId);
                return ApiResponse<CustomerImage>.ErrorResult("An error occurred while uploading the image");
            }
        }

        public async Task<ApiResponse<object>> DeleteImageAsync(int imageId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Image/{imageId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                    return result ?? ApiResponse<object>.ErrorResult("Failed to delete image");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Delete failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);
                    return ApiResponse<object>.ErrorResult($"Delete failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image {ImageId}", imageId);
                return ApiResponse<object>.ErrorResult("An error occurred while deleting the image");
            }
        }

        public async Task<ApiResponse<object>> GetImageCountAsync(int customerId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<object>>($"api/Image/count/{customerId}");
                return response ?? ApiResponse<object>.ErrorResult("Failed to retrieve image count");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving image count for customer {CustomerId}", customerId);
                return ApiResponse<object>.ErrorResult("An error occurred while retrieving image count");
            }
        }

        public async Task<ApiResponse<object>> GetImageBase64Async(int imageId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<object>>($"api/Image/base64/{imageId}");
                return response ?? ApiResponse<object>.ErrorResult("Failed to retrieve image data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Base64 data for image {ImageId}", imageId);
                return ApiResponse<object>.ErrorResult("An error occurred while retrieving image data");
            }
        }

        public async Task<string> ConvertFileToBase64Async(IBrowserFile file)
        {
            try
            {
                using var stream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024); // 5MB limit
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();
                return Convert.ToBase64String(fileBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting file to Base64");
                throw;
            }
        }
    }
}
