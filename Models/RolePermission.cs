namespace irevlogix_backend.Models
{
    public class RolePermission : BaseEntity
    {
        public int RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;
        
        public int PermissionId { get; set; }
        public virtual Permission Permission { get; set; } = null!;
    }
}
