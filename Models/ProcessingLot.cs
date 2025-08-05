using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class ProcessingLot : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string LotID { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Created";
        
        [MaxLength(200)]
        public string? Description { get; set; }
        
        public int? OperatorUserId { get; set; }
        public virtual User? Operator { get; set; }
        
        public DateTime? StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        
        public decimal? TotalIncomingWeight { get; set; }
        public decimal? TotalProcessedWeight { get; set; }
        public decimal? ProcessingCost { get; set; }
        public decimal? IncomingMaterialCost { get; set; }
        public decimal? ExpectedRevenue { get; set; }
        public decimal? ActualRevenue { get; set; }
        
        [MaxLength(1000)]
        public string? IncomingMaterialNotes { get; set; }
        
        public decimal? ContaminationPercentage { get; set; }
        
        [MaxLength(1000)]
        public string? QualityControlNotes { get; set; }
        
        [MaxLength(100)]
        public string? CertificationStatus { get; set; }
        
        [MaxLength(200)]
        public string? CertificationNumber { get; set; }
        
        public virtual ICollection<ProcessingStep> ProcessingSteps { get; set; } = new List<ProcessingStep>();
        public virtual ICollection<ProcessedMaterial> ProcessedMaterials { get; set; } = new List<ProcessedMaterial>();
        public virtual ICollection<ShipmentItem> IncomingShipmentItems { get; set; } = new List<ShipmentItem>();
    }
}
