using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class ProcessingLot : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string LotNumber { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [MaxLength(1000)]
        public string? IncomingMaterialNotes { get; set; }
        
        public decimal? ContaminationPercentage { get; set; }
        
        public decimal? ActualReceivedWeight { get; set; }
        
        [MaxLength(20)]
        public string? WeightUnit { get; set; }
        
        public int? SourceShipmentId { get; set; }
        public virtual Shipment? SourceShipment { get; set; }
        
        [MaxLength(100)]
        public string? ProcessingMethod { get; set; }
        
        public decimal? ProcessingCost { get; set; }
        
        [MaxLength(1000)]
        public string? ProcessingNotes { get; set; }
        
        public virtual ICollection<ShipmentItem> ShipmentItems { get; set; } = new List<ShipmentItem>();
    }
}
