using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class Permission : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Module { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
