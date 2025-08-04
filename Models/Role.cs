using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class Role : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public bool IsSystemRole { get; set; } = false;
        
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
