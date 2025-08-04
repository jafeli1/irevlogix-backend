using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class AssetCategory : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [MaxLength(100)]
        public string? ParentCategory { get; set; }
        
        public bool RequiresDataDestruction { get; set; } = false;
        
        public bool IsRecoverable { get; set; } = true;
    }
}
