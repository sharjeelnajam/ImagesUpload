using Microsoft.AspNetCore.Mvc;
using ImageUploadTask.Services;
using ImageUploadTask.Models.DTOs;
using ImageUploadTask.Models;

namespace ImageUploadTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ILogger<ImageController> _logger;

        public ImageController(IImageService imageService, ILogger<ImageController> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        /// <summary>
        /// Upload one or more images for a customer
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <param name="files">The image files to upload</param>
        /// <param name="description">Optional description for the images</param>
        /// <returns>List of uploaded images</returns>
        [HttpPost("upload/{customerId}")]
        public async Task<ActionResult<ApiResponse<List<ImageResponse>>>> UploadImages(
            int customerId, 
            [FromForm] List<IFormFile> files,
            [FromForm] string? description = null)
        {
            try
            {
                if (files == null || !files.Any())
                {
                    return BadRequest(ApiResponse<List<ImageResponse>>.ErrorResult("No files provided"));
                }

                var uploadedImages = new List<ImageResponse>();
                var errors = new List<string>();

                foreach (var file in files)
                {
                    // Check if we can add more images
                    if (!await _imageService.CanAddImageAsync(customerId))
                    {
                        errors.Add($"Cannot upload more images. Customer {customerId} has reached the maximum limit of 10 images.");
                        break;
                    }

                    var uploadedImage = await _imageService.UploadImageAsync(customerId, file, description);
                    if (uploadedImage != null)
                    {
                        uploadedImages.Add(MapToImageResponse(uploadedImage));
                    }
                    else
                    {
                        errors.Add($"Failed to upload file: {file.FileName}");
                    }
                }

                if (uploadedImages.Any())
                {
                    return Ok(ApiResponse<List<ImageResponse>>.SuccessResult(
                        uploadedImages, 
                        $"Successfully uploaded {uploadedImages.Count} image(s)"));
                }
                else
                {
                    return BadRequest(ApiResponse<List<ImageResponse>>.ErrorResult(
                        "Failed to upload any images", errors));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading images for customer {CustomerId}", customerId);
                return StatusCode(500, ApiResponse<List<ImageResponse>>.ErrorResult(
                    "An error occurred while uploading images"));
            }
        }

        /// <summary>
        /// Get all images for a customer
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <returns>List of customer images</returns>
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<ApiResponse<List<ImageResponse>>>> GetCustomerImages(int customerId)
        {
            try
            {
                var images = await _imageService.GetCustomerImagesAsync(customerId);
                var imageResponses = images.Select(MapToImageResponse).ToList();

                return Ok(ApiResponse<List<ImageResponse>>.SuccessResult(
                    imageResponses, 
                    $"Found {imageResponses.Count} image(s) for customer {customerId}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving images for customer {CustomerId}", customerId);
                return StatusCode(500, ApiResponse<List<ImageResponse>>.ErrorResult(
                    "An error occurred while retrieving images"));
            }
        }

        /// <summary>
        /// Delete a specific image
        /// </summary>
        /// <param name="imageId">The image ID to delete</param>
        /// <returns>Success status</returns>
        [HttpDelete("{imageId}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteImage(int imageId)
        {
            try
            {
                var success = await _imageService.DeleteImageAsync(imageId);
                
                if (success)
                {
                    return Ok(ApiResponse<object>.SuccessResult(
                        null, 
                        $"Image {imageId} deleted successfully"));
                }
                else
                {
                    return NotFound(ApiResponse<object>.ErrorResult(
                        $"Image {imageId} not found"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image {ImageId}", imageId);
                return StatusCode(500, ApiResponse<object>.ErrorResult(
                    "An error occurred while deleting the image"));
            }
        }

        /// <summary>
        /// Get image count for a customer
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <returns>Image count and limit information</returns>
        [HttpGet("count/{customerId}")]
        public async Task<ActionResult<ApiResponse<object>>> GetImageCount(int customerId)
        {
            try
            {
                var count = await _imageService.GetImageCountAsync(customerId);
                var canAddMore = await _imageService.CanAddImageAsync(customerId);

                var result = new
                {
                    CustomerId = customerId,
                    CurrentCount = count,
                    MaxAllowed = 10,
                    CanAddMore = canAddMore,
                    RemainingSlots = 10 - count
                };

                return Ok(ApiResponse<object>.SuccessResult(result, "Image count retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting image count for customer {CustomerId}", customerId);
                return StatusCode(500, ApiResponse<object>.ErrorResult(
                    "An error occurred while retrieving image count"));
            }
        }

        /// <summary>
        /// Serve an image file
        /// </summary>
        /// <param name="imageId">The image ID</param>
        /// <returns>The image file</returns>
        [HttpGet("serve/{imageId}")]
        public async Task<IActionResult> ServeImage(int imageId)
        {
            try
            {
                var image = await _imageService.GetImageByIdAsync(imageId);
                if (image == null)
                {
                    return NotFound();
                }

                if (!System.IO.File.Exists(image.FilePath))
                {
                    return NotFound("Image file not found on disk");
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(image.FilePath);
                return File(fileBytes, image.ContentType, image.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serving image {ImageId}", imageId);
                return StatusCode(500, "An error occurred while serving the image");
            }
        }

        private ImageResponse MapToImageResponse(CustomerImage image)
        {
            return new ImageResponse
            {
                Id = image.Id,
                CustomerId = image.CustomerId,
                FileName = image.FileName,
                ContentType = image.ContentType,
                FileSizeBytes = image.FileSizeBytes,
                UploadedAt = image.UploadedAt,
                Description = image.Description,
                FileUrl = Url.Action("ServeImage", "Image", new { imageId = image.Id }, Request.Scheme) ?? string.Empty
            };
        }
    }
}
