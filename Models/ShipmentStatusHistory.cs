using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class ShipmentStatusHistory : BaseEntity
    {
        public int ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; } = null!;
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public int? UserId { get; set; }
        public virtual User? User { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string FromStatus { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string ToStatus { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        [MaxLength(50)]
        public string? ActionType { get; set; }
    }
}
