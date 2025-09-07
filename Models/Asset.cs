using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class Asset : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string AssetID { get; set; } = string.Empty;
        
        public int? AssetCategoryId { get; set; }
        public virtual AssetCategory? AssetCategory { get; set; }
        
        public int? SourceShipmentId { get; set; }
        public virtual Shipment? SourceShipment { get; set; }
        
        [MaxLength(100)]
        public string? Manufacturer { get; set; }
        
        [MaxLength(100)]
        public string? Model { get; set; }
        
        [MaxLength(100)]
        public string? SerialNumber { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public DateTime? OriginalPurchaseDate { get; set; }
        public decimal? OriginalCost { get; set; }
        
        [MaxLength(50)]
        public string Condition { get; set; } = "Unknown";
        
        public decimal? EstimatedValue { get; set; }
        public decimal? ActualSalePrice { get; set; }
        public decimal? CostOfRecovery { get; set; }
        
        public bool IsDataBearing { get; set; } = false;
        
        [MaxLength(50)]
        public string? StorageDeviceType { get; set; }
        
        [MaxLength(50)]
        public string? StorageCapacity { get; set; }
        
        [MaxLength(100)]
        public string? DataSanitizationMethod { get; set; }
        
        [MaxLength(50)]
        public string? DataSanitizationStatus { get; set; }
        
        public DateTime? DataSanitizationDate { get; set; }
        
        [MaxLength(500)]
        public string? DataSanitizationCertificate { get; set; }
        
        public decimal? DataDestructionCost { get; set; }
        
        [MaxLength(1000)]
        public string? InternalAuditNotes { get; set; }
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        public int? InternalAuditScore { get; set; }
        
        [MaxLength(100)]
        public string? CurrentLocation { get; set; }
        
        public int? CurrentResponsibleUserId { get; set; }
        public virtual User? CurrentResponsibleUser { get; set; }
        
        public int? CurrentStatusId { get; set; }
        public virtual AssetTrackingStatus? CurrentStatus { get; set; }
        
        public bool ReuseDisposition { get; set; } = false;
        public bool ResaleDisposition { get; set; } = false;
        
        [MaxLength(200)]
        public string? ReuseRecipient { get; set; }
        
        [MaxLength(200)]
        public string? ReusePurpose { get; set; }
        
        public DateTime? ReuseDate { get; set; }
        public decimal? FairMarketValue { get; set; }
        
        [MaxLength(200)]
        public string? Buyer { get; set; }
        
        public DateTime? SaleDate { get; set; }
        
        [MaxLength(100)]
        public string? ResalePlatform { get; set; }
        
        public decimal? CostOfSale { get; set; }
        
        [MaxLength(200)]
        public string? SalesInvoice { get; set; }
        
        public int? RecyclingVendorId { get; set; }
        public virtual Vendor? RecyclingVendor { get; set; }
        
        public DateTime? RecyclingDate { get; set; }
        public decimal? RecyclingCost { get; set; }
        
        [MaxLength(500)]
        public string? CertificateOfRecycling { get; set; }
        
        public int? ProcessingLotId { get; set; }
        public virtual ProcessingLot? ProcessingLot { get; set; }
        
        public virtual ICollection<ChainOfCustody> ChainOfCustodyRecords { get; set; } = new List<ChainOfCustody>();
    }
}
