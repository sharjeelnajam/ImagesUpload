using System.ComponentModel.DataAnnotations;

namespace ImageUploadTask.Models.DTOs
{
    public class ImageUploadRequest
    {
        [Required]
        public int CustomerId { get; set; }
        
        public string? Description { get; set; }
    }
}
