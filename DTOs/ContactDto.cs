using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.DTOs
{
    public class ContactRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Company { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        public string Recipient { get; set; } = string.Empty;
    }

    public class ContactResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
