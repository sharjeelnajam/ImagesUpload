using System.ComponentModel.DataAnnotations;

namespace ImageUploadTask.Models.DTOs
{
    public class Base64ImageUploadRequest
    {
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        public string Base64Data { get; set; } = string.Empty;
        
        [Required]
        public string FileName { get; set; } = string.Empty;
        
        [Required]
        public string ContentType { get; set; } = string.Empty;
        
        public string? Description { get; set; }
    }
}
