using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class ClientContact : BaseEntity
    {
        public int ClientId { get; set; }
        public virtual Client Client { get; set; } = null!;
        
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Phone { get; set; }
        
        [MaxLength(100)]
        public string? JobTitle { get; set; }
        
        [MaxLength(100)]
        public string? Department { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public bool IsPrimaryContact { get; set; } = false;
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}
