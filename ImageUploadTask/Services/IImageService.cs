using ImageUploadTask.Models;

namespace ImageUploadTask.Services
{
    public interface IImageService
    {
        Task<bool> CanAddImageAsync(int customerId);
        Task<int> GetImageCountAsync(int customerId);
        Task<CustomerImage?> UploadImageAsync(int customerId, IFormFile file, string? description = null);
        Task<List<CustomerImage>> GetCustomerImagesAsync(int customerId);
        Task<bool> DeleteImageAsync(int imageId);
        Task<CustomerImage?> GetImageByIdAsync(int imageId);
    }
}
