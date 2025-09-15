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

            // Validate file size (max 10MB)
            const long maxFileSize = 10 * 1024 * 1024; // 10MB
            if (file.Length > maxFileSize)
            {
                _logger.LogWarning("File size {FileSize} exceeds maximum allowed size {MaxSize}", 
                    file.Length, maxFileSize);
                return null;
            }

            try
            {
                // Create uploads directory if it doesn't exist
                var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                _logger.LogInformation("WebRootPath: {WebRootPath}, Fallback: {Fallback}", 
                    _environment.WebRootPath, webRootPath);
                
                var uploadsDir = Path.Combine(webRootPath, "uploads", customerId.ToString());
                _logger.LogInformation("Creating uploads directory: {UploadsDir}", uploadsDir);
                
                Directory.CreateDirectory(uploadsDir);

                // Generate unique filename
                var fileExtension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsDir, uniqueFileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Create database record
                var customerImage = new CustomerImage
                {
                    CustomerId = customerId,
                    FileName = file.FileName,
                    FilePath = filePath,
                    ContentType = file.ContentType,
                    FileSizeBytes = file.Length,
                    Description = description,
                    UploadedAt = DateTime.UtcNow
                };

                _context.CustomerImages.Add(customerImage);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully uploaded image {ImageId} for customer {CustomerId}", 
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

                // Delete physical file
                if (File.Exists(image.FilePath))
                {
                    File.Delete(image.FilePath);
                }

                // Delete database record
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
    }
}
