using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace irevlogix_backend.Models
{
    public class ProductAnalysisContactOptIn : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [MaxLength(1000)]
        public string? UploadedPhotoPath { get; set; }

        [Column(TypeName = "text")]
        public string? DetailedDescription { get; set; }

        [Column(TypeName = "text")]
        public string? ProductSummary { get; set; }

        [Column(TypeName = "text")]
        public string? SecondaryMarketPriceAnalysis { get; set; }

        [Column(TypeName = "text")]
        public string? RecyclersMatched { get; set; }

        [Column(TypeName = "jsonb")]
        public string? ProductAnalysisVisualizationData { get; set; }

        [Required]
        public bool OptIn { get; set; } = false;

        [MaxLength(200)]
        public string? PreferredContactEmail { get; set; }

        [MaxLength(200)]
        public string? PreferredContactPhone { get; set; }
    }
}
