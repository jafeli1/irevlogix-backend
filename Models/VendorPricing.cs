using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class VendorPricing : BaseEntity
    {
        public int VendorId { get; set; }
        public int? MaterialTypeId { get; set; }
        public decimal? PricePerUnit { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        
        public virtual Vendor? Vendor { get; set; }
        public virtual MaterialType? MaterialType { get; set; }
    }
}
