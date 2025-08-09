using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class Shipment : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string ShipmentNumber { get; set; } = string.Empty;
        
        public int? OriginatorClientId { get; set; }
        public virtual Client? OriginatorClient { get; set; }
        
        public int? ClientContactId { get; set; }
        public virtual ClientContact? ClientContact { get; set; }
        
        public DateTime? ScheduledPickupDate { get; set; }
        public DateTime? ActualPickupDate { get; set; }
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
        
        [MaxLength(1000)]
        public string? DispositionNotes { get; set; }
        
        public decimal? EstimatedValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? TransportationCost { get; set; }
        public decimal? LogisticsCost { get; set; }
        public decimal? DispositionCost { get; set; }
        
        [MaxLength(500)]
        public string? PickupAddress { get; set; }
        
        [MaxLength(500)]
        public string? DeliveryAddress { get; set; }
        
        [MaxLength(500)]
        public string? OriginAddress { get; set; }
        
        public virtual ICollection<ShipmentItem> ShipmentItems { get; set; } = new List<ShipmentItem>();
        public virtual ICollection<ShipmentStatusHistory> ShipmentStatusHistories { get; set; } = new List<ShipmentStatusHistory>();
        public virtual ICollection<ShipmentDocument> ShipmentDocuments { get; set; } = new List<ShipmentDocument>();
    }
}
