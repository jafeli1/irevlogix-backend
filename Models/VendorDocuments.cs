using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class VendorDocuments : BaseEntity
    {
        public int VendorId { get; set; }
        public string? DocumentUrl { get; set; }
        public string? Filename { get; set; }
        public string? ContentType { get; set; }
        
        public virtual Vendor? Vendor { get; set; }
    }
}
