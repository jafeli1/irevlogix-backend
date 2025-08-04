using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class Tenant : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [MaxLength(255)]
        public string? LogoUrl { get; set; }
        
        [MaxLength(50)]
        public string? PrimaryColor { get; set; }
        
        [MaxLength(50)]
        public string? SecondaryColor { get; set; }
        
        [MaxLength(100)]
        public string? TimeZone { get; set; }
        
        [MaxLength(10)]
        public string? Currency { get; set; }
        
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
