using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class ProcessedMaterialTests : BaseEntity
    {
        public int ProcessedMaterialId { get; set; }
        public virtual ProcessedMaterial ProcessedMaterial { get; set; } = null!;
        
        public DateTime? TestDate { get; set; }
        
        [MaxLength(200)]
        public string? Lab { get; set; }
        
        [MaxLength(1000)]
        public string? Parameters { get; set; }
        
        [MaxLength(1000)]
        public string? Results { get; set; }
        
        [MaxLength(50)]
        public string? ComplianceStatus { get; set; }
        
        [MaxLength(500)]
        public string? ReportDocumentUrl { get; set; }
    }
}
