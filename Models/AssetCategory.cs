using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class AssetCategory : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public string? DefaultDisposition { get; set; } // Reuse, Resale, Recycle
        
        public bool RequiresDataSanitization { get; set; } = false;
        
        public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }
}
