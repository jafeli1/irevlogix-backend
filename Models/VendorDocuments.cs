using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class VendorDocuments : BaseEntity
    {
        public int VendorId { get; set; }
        public string? DocumentUrl { get; set; }
        public string? Filename { get; set; }
        public string? ContentType { get; set; }
        public string? DocumentType { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? DateReceived { get; set; }
        public string? ReviewComment { get; set; }
        public DateTime? LastReviewDate { get; set; }
        public int? ReviewedBy { get; set; }
        
        public virtual Vendor? Vendor { get; set; }
    }
}
