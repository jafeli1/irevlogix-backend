using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace irevlogix_backend.Models
{
    public class ProductAnalysis : BaseEntity
    {
        [Required]
        [MaxLength(500)]
        public string ProductName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Brand { get; set; }

        [MaxLength(200)]
        public string? Model { get; set; }

        [MaxLength(200)]
        public string? Category { get; set; }

        [Column(TypeName = "text")]
        public string? ProductDescription { get; set; }

        [MaxLength(1000)]
        public string? ImagePath { get; set; }

        [Column(TypeName = "jsonb")]
        public string? Specifications { get; set; }

        [Column(TypeName = "jsonb")]
        public string? Components { get; set; }

        [Column(TypeName = "jsonb")]
        public string? MarketPrice { get; set; }

        [Column(TypeName = "text")]
        public string? Summary { get; set; }

        [Column(TypeName = "jsonb")]
        public string? EbayListings { get; set; }

        [Required]
        public DateTime AnalysisDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastMarketRefresh { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
