using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class AssetDocument : BaseEntity
    {
        public int AssetId { get; set; }
        public virtual Asset Asset { get; set; } = null!;
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;
        [MaxLength(150)]
        public string? ContentType { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
