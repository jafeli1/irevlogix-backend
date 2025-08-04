using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class ShipmentItem : BaseEntity
    {
        public int ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; } = null!;
        
        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? Category { get; set; }
        
        [MaxLength(100)]
        public string? Brand { get; set; }
        
        [MaxLength(100)]
        public string? Model { get; set; }
        
        [MaxLength(100)]
        public string? SerialNumber { get; set; }
        
        public int Quantity { get; set; } = 1;
        
        [MaxLength(50)]
        public string? Condition { get; set; }
        
        public decimal? EstimatedValue { get; set; }
        
        public decimal? ActualValue { get; set; }
        
        public decimal? Weight { get; set; }
        
        [MaxLength(20)]
        public string? WeightUnit { get; set; }
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        [MaxLength(50)]
        public string? ProcessingStatus { get; set; }
        
        public bool IsDataBearingDevice { get; set; } = false;
        
        [MaxLength(50)]
        public string? DataDestructionStatus { get; set; }
        
        public DateTime? DataDestructionDate { get; set; }
        
        [MaxLength(100)]
        public string? DataDestructionMethod { get; set; }
        
        [MaxLength(100)]
        public string? CertificateNumber { get; set; }
    }
}
