using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class FreightLossDamageClaim : BaseEntity
    {
        public int FreightLossDamageClaimId { get; set; }
        
        [MaxLength(200)]
        public string? Description { get; set; }
        
        public int? RequestId { get; set; }
        
        public DateTime? DateOfShipment { get; set; }
        public DateTime? DateOfClaim { get; set; }
        
        [MaxLength(100)]
        public string? ClaimantReferenceNumber { get; set; }
        
        [EmailAddress]
        [MaxLength(255)]
        public string? ClaimantEmail { get; set; }
        
        [MaxLength(200)]
        public string? ClaimantCompanyName { get; set; }
        
        [MaxLength(500)]
        public string? ClaimantAddress { get; set; }
        
        [MaxLength(100)]
        public string? ClaimantCity { get; set; }
        
        public int? StateId { get; set; }
        
        [MaxLength(20)]
        public string? PostalCode { get; set; }
        
        [MaxLength(20)]
        public string? ClaimantPhone { get; set; }
        
        [MaxLength(20)]
        public string? ClaimantFax { get; set; }
        
        [MaxLength(100)]
        public string? ClaimantName { get; set; }
        
        [MaxLength(100)]
        public string? ClaimantJobTitle { get; set; }
        
        public DateTime? DateClaimWasSigned { get; set; }
        
        [MaxLength(200)]
        public string? NotificationOfLossDamageGivenTo { get; set; }
        
        [MaxLength(200)]
        public string? NotificationOfLossDamageGivenAt { get; set; }
        
        [MaxLength(100)]
        public string? NotificationOfLossDamageGivenByWhatMethod { get; set; }
        
        public DateTime? NotificationOfLossDamageGivenOn { get; set; }
        
        [MaxLength(500)]
        public string? CommodityLostDamaged { get; set; }
        
        public decimal? TotalWeight { get; set; }
        public decimal? Quantity { get; set; }
        
        [MaxLength(1000)]
        public string? DamageDescription { get; set; }
        
        public decimal? TotalValue { get; set; }
        
        [MaxLength(500)]
        public string? ClaimAttachmentUpload1 { get; set; }
        [MaxLength(500)]
        public string? ClaimAttachmentUpload2 { get; set; }
        [MaxLength(500)]
        public string? ClaimAttachmentUpload3 { get; set; }
        [MaxLength(500)]
        public string? ClaimAttachmentUpload4 { get; set; }
    }
}
