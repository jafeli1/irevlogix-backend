using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class Shipment : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string ShipmentNumber { get; set; } = string.Empty;
        
        public new string ClientId { get; set; } = string.Empty;
        
        public DateTime ShipmentDate { get; set; }
        
        public DateTime? ReceivedDate { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? TrackingNumber { get; set; }
        
        [MaxLength(100)]
        public string? Carrier { get; set; }
        
        public decimal? Weight { get; set; }
        
        [MaxLength(20)]
        public string? WeightUnit { get; set; }
        
        public int? NumberOfBoxes { get; set; }
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        public decimal? EstimatedValue { get; set; }
        
        public decimal? ActualValue { get; set; }
        
        [MaxLength(500)]
        public string? PickupAddress { get; set; }
        
        [MaxLength(500)]
        public string? DeliveryAddress { get; set; }
        
        public virtual ICollection<ShipmentItem> ShipmentItems { get; set; } = new List<ShipmentItem>();
    }
}
