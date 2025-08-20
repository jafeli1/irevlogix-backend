using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class ProcessedMaterial : BaseEntity
    {
        public int ProcessingLotId { get; set; }
        public virtual ProcessingLot ProcessingLot { get; set; } = null!;
        
        public int? MaterialTypeId { get; set; }
        public virtual MaterialType? MaterialType { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
        
        public decimal Quantity { get; set; }
        
        [MaxLength(50)]
        public string? UnitOfMeasure { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }
        
        public decimal? ProcessedWeight { get; set; }
        
        [MaxLength(20)]
        public string? WeightUnit { get; set; }
        
        [MaxLength(50)]
        public string? QualityGrade { get; set; }
        
        [MaxLength(100)]
        public string? DestinationVendor { get; set; }
        
        public decimal? ExpectedSalesPrice { get; set; }
        public decimal? ActualSalesPrice { get; set; }
        
        public DateTime? SaleDate { get; set; }
        
        [MaxLength(50)]
        public string Status { get; set; } = "Processed";
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        [MaxLength(100)]
        public string? CertificationNumber { get; set; }
        
        public bool IsHazardous { get; set; } = false;
        
        [MaxLength(200)]
        public string? HazardousClassification { get; set; }

        public decimal? PurchaseCostPerUnit { get; set; }
        public decimal? ProcessingCostPerUnit { get; set; }
    }
}
