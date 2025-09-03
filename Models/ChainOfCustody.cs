using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class ChainOfCustody : BaseEntity
    {
        public int AssetId { get; set; }
        public virtual Asset Asset { get; set; } = null!;
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        [MaxLength(100)]
        public string? Location { get; set; }
        
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
        
        public int VendorId { get; set; }
        public virtual Vendor Vendor { get; set; } = null!;
        
        [Required]
        [MaxLength(200)]
        public string StatusChange { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        [MaxLength(50)]
        public string? ActionType { get; set; }
    }
}
