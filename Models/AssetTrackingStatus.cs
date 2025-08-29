using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class AssetTrackingStatus : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string StatusName { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public int? SortOrder { get; set; } = 0;
        
        [MaxLength(20)]
        public string? ColorCode { get; set; }
        
        public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }
}
