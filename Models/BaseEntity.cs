using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
        
        public int CreatedBy { get; set; }
        
        public int UpdatedBy { get; set; }
        
        [Required]
        public string ClientId { get; set; } = string.Empty;
    }
}
