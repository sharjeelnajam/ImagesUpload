using Microsoft.EntityFrameworkCore;
using ImageUploadTask.Data;
using ImageUploadTask.Models;

namespace ImageUploadTask.Services
{
    public class ImageService : IImageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ImageService> _logger;
        private const int MAX_IMAGES_PER_CUSTOMER = 10;

        public ImageService(ApplicationDbContext context, IWebHostEnvironment environment, ILogger<ImageService> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        public async Task<bool> CanAddImageAsync(int customerId)
        {
            var count = await _context.CustomerImages
                .Where(img => img.CustomerId == customerId)
                .CountAsync();
            
            return count < MAX_IMAGES_PER_CUSTOMER;
        }

        public async Task<int> GetImageCountAsync(int customerId)
        {
            return await _context.CustomerImages
                .Where(img => img.CustomerId == customerId)
                .CountAsync();
        }

        public async Task<CustomerImage?> UploadImageAsync(int customerId, IFormFile file, string? description = null)
        {
            // Check if customer exists
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                _logger.LogWarning("Customer with ID {CustomerId} not found", customerId);
                return null;
            }

            // Check image limit
            if (!await CanAddImageAsync(customerId))
            {
                _logger.LogWarning("Customer {CustomerId} has reached the maximum limit of {MaxImages} images", 
                    customerId, MAX_IMAGES_PER_CUSTOMER);
                return null;
            }

            // Validate file
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No file provided for upload");
                return null;
            }

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                _logger.LogWarning("Invalid file type: {ContentType}", file.ContentType);
                return null;
            }

            // Validate file size (max 5MB for Base64 storage to avoid database bloat)
            const long maxFileSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxFileSize)
            {
                _logger.LogWarning("File size {FileSize} exceeds maximum allowed size {MaxSize}", 
                    file.Length, maxFileSize);
                return null;
            }

            try
            {
                // Convert file to Base64
                string base64Data;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();
                    base64Data = Convert.ToBase64String(fileBytes);
                }

                // Create database record with Base64 data
                var customerImage = new CustomerImage
                {
                    CustomerId = customerId,
                    FileName = file.FileName,
                    FilePath = null, // No longer storing file path since we're using Base64
                    ContentType = file.ContentType,
                    FileSizeBytes = file.Length,
                    Description = description,
                    Base64Data = base64Data,
                    UploadedAt = DateTime.UtcNow
                };

                _context.CustomerImages.Add(customerImage);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully uploaded image {ImageId} for customer {CustomerId} as Base64", 
                    customerImage.Id, customerId);

                return customerImage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image for customer {CustomerId}", customerId);
                return null;
            }
        }

        public async Task<List<CustomerImage>> GetCustomerImagesAsync(int customerId)
        {
            return await _context.CustomerImages
                .Where(img => img.CustomerId == customerId)
                .OrderBy(img => img.UploadedAt)
                .ToListAsync();
        }

        public async Task<bool> DeleteImageAsync(int imageId)
        {
            try
            {
                var image = await _context.CustomerImages.FindAsync(imageId);
                if (image == null)
                {
                    _logger.LogWarning("Image with ID {ImageId} not found", imageId);
                    return false;
                }

                // Delete database record (no physical file to delete since we're using Base64)
                _context.CustomerImages.Remove(image);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted image {ImageId} for customer {CustomerId}", 
                    imageId, image.CustomerId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image {ImageId}", imageId);
                return false;
            }
        }

        public async Task<CustomerImage?> GetImageByIdAsync(int imageId)
        {
            return await _context.CustomerImages
                .Include(img => img.Customer)
                .FirstOrDefaultAsync(img => img.Id == imageId);
        }

        public async Task<CustomerImage?> UploadBase64ImageAsync(int customerId, string base64Data, string fileName, string contentType, string? description = null)
        {
            // Check if customer exists
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                _logger.LogWarning("Customer with ID {CustomerId} not found", customerId);
                return null;
            }

            // Check image limit
            if (!await CanAddImageAsync(customerId))
            {
                _logger.LogWarning("Customer {CustomerId} has reached the maximum limit of {MaxImages} images", 
                    customerId, MAX_IMAGES_PER_CUSTOMER);
                return null;
            }

            // Validate Base64 data
            if (string.IsNullOrEmpty(base64Data))
            {
                _logger.LogWarning("No Base64 data provided for upload");
                return null;
            }

            // Validate content type
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(contentType.ToLower()))
            {
                _logger.LogWarning("Invalid content type: {ContentType}", contentType);
                return null;
            }

            try
            {
                // Validate Base64 format and get size
                byte[] fileBytes;
                try
                {
                    fileBytes = Convert.FromBase64String(base64Data);
                }
                catch (FormatException)
                {
                    _logger.LogWarning("Invalid Base64 format provided");
                    return null;
                }

                // Validate file size (max 5MB for Base64 storage)
                const long maxFileSize = 5 * 1024 * 1024; // 5MB
                if (fileBytes.Length > maxFileSize)
                {
                    _logger.LogWarning("File size {FileSize} exceeds maximum allowed size {MaxSize}", 
                        fileBytes.Length, maxFileSize);
                    return null;
                }

                // Create database record with Base64 data
                var customerImage = new CustomerImage
                {
                    CustomerId = customerId,
                    FileName = fileName,
                    FilePath = string.Empty, // No file path since we're using Base64
                    ContentType = contentType,
                    FileSizeBytes = fileBytes.Length,
                    Description = description,
                    Base64Data = base64Data,
                    UploadedAt = DateTime.UtcNow
                };

                _context.CustomerImages.Add(customerImage);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully uploaded Base64 image {ImageId} for customer {CustomerId}", 
                    customerImage.Id, customerId);

                return customerImage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading Base64 image for customer {CustomerId}", customerId);
                return null;
            }
        }
    }
}
