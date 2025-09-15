namespace ImageUploadTask.Models.DTOs
{
    public class ImageResponse
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public DateTime UploadedAt { get; set; }
        public string? Description { get; set; }
        public string FileUrl { get; set; } = string.Empty;
    }
}
