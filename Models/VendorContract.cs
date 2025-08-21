using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class VendorContract : BaseEntity
    {
        public int VendorId { get; set; }
        public string? DocumentUrl { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        
        public virtual Vendor? Vendor { get; set; }
    }
}
