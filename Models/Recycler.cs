using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class Recycler
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;
        
        [MaxLength(300)]
        public string? Address { get; set; }
        
        [MaxLength(50)]
        public string? ContactPhone { get; set; }
        
        [MaxLength(300)]
        public string? ContactEmail { get; set; }
        
        public string? MaterialTypesHandled { get; set; }
        
        [MaxLength(50)]
        public string? CertificationType { get; set; }
        
        [MaxLength(100)]
        public string? ServicesOffered { get; set; }
        
        [Required]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
        
        [Required]
        public int CreatedBy { get; set; }
        
        [Required]
        public int UpdatedBy { get; set; }
    }
}
