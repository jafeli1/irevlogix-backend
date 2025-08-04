using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class Tenant
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string ClientId { get; set; } = string.Empty;
        
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
        
        public int CreatedBy { get; set; }
        
        public int UpdatedBy { get; set; }
    }
}
