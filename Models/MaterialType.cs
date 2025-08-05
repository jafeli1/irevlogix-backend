using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class MaterialType : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public decimal? DefaultPricePerUnit { get; set; }
        
        public string? Unit { get; set; } // kg, lbs, pieces, etc.
        
        public virtual ICollection<ProcessedMaterial> ProcessedMaterials { get; set; } = new List<ProcessedMaterial>();

    }
}
