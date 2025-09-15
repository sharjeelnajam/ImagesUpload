using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImageUploadTask.Models
{
    public class CustomerImage
    {
        public int Id { get; set; }
        
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string ContentType { get; set; } = string.Empty;
        
        public long FileSizeBytes { get; set; }
        
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        // Navigation property
        public virtual Customer Customer { get; set; } = null!;
    }
}
