using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class ProcessedMaterialSales : BaseEntity
    {
        public int ProcessedMaterialId { get; set; }
        public ProcessedMaterial ProcessedMaterial { get; set; }
        
        public int? VendorId { get; set; }
        public Vendor? Vendor { get; set; }
        
        public decimal? SalesQuantity { get; set; }
        public decimal? AgreedPricePerUnit { get; set; }
        public DateTime? ShipmentDate { get; set; }
        
        [MaxLength(500)]
        public string? Carrier { get; set; }
        
        [MaxLength(500)]
        public string? TrackingNumber { get; set; }
        
        public decimal? FreightCost { get; set; }
        public decimal? LoadingCost { get; set; }
        
        [MaxLength(500)]
        public string? InvoiceId { get; set; }
        
        [MaxLength(500)]
        public string? ClientId { get; set; }
        
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DateInvoicePaid { get; set; }
        public decimal? InvoiceTotal { get; set; }
        
        [MaxLength(50)]
        public string? InvoiceStatus { get; set; }
    }
}
