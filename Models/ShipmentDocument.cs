using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class ShipmentDocument : BaseEntity
    {
        public int ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; } = null!;
        
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? ContentType { get; set; }
        
        public long FileSize { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
