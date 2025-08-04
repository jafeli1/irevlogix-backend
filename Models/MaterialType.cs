using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class MaterialType : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [MaxLength(50)]
        public string? Category { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [MaxLength(100)]
        public string? RecyclingMethod { get; set; }
        
        [MaxLength(100)]
        public string? UnitOfMeasure { get; set; }
    }
}
