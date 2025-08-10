using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
        
        public string CreatedBy { get; set; } = string.Empty;
        
        public string UpdatedBy { get; set; } = string.Empty;
        
        [Required]
        public string ClientId { get; set; } = string.Empty;
    }
}
