namespace ImageUploadWeb.Models
{
    public class Base64ImageUploadRequest
    {
        public int CustomerId { get; set; }
        public string Base64Data { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
