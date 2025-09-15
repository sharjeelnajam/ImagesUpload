using ImageUploadWeb.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace ImageUploadWeb.Services
{
    public interface IImageService
    {
        Task<ApiResponse<List<CustomerImage>>> GetCustomerImagesAsync(int customerId);
        Task<ApiResponse<CustomerImage>> UploadBase64ImageAsync(Base64ImageUploadRequest request);
        Task<ApiResponse<object>> DeleteImageAsync(int imageId);
        Task<ApiResponse<object>> GetImageCountAsync(int customerId);
        Task<ApiResponse<object>> GetImageBase64Async(int imageId);
        Task<string> ConvertFileToBase64Async(IBrowserFile file);
    }
}
