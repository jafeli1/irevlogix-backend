using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime? LastLoginDate { get; set; }
        
        public int FailedLoginAttempts { get; set; } = 0;
        
        public DateTime? LockoutEndDate { get; set; }
        
        public bool IsEmailConfirmed { get; set; } = false;
        
        public string? EmailConfirmationToken { get; set; }
        
        public string? PasswordResetToken { get; set; }
        
        public DateTime? PasswordResetTokenExpiry { get; set; }
        
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
