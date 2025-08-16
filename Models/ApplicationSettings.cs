using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class ApplicationSettings : BaseEntity
    {
        [MaxLength(100)]
        public string? SettingKey { get; set; }
        
        [MaxLength(1000)]
        public string? SettingValue { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [MaxLength(50)]
        public string? Category { get; set; }
        
        public bool IsEncrypted { get; set; } = false;
        
        public bool IsReadOnly { get; set; } = false;

        [MaxLength(500)]
        public string? ApplicationLogoPath { get; set; }

        [MaxLength(500)]
        public string? DefaultLogoutPageUrl { get; set; }

        public int? LoginTimeoutMinutes { get; set; }

        [MaxLength(500)]
        public string? ApplicationUploadFolderPath { get; set; }

        [MaxLength(500)]
        public string? ApplicationErrorLogFolderPath { get; set; }

        public int? PasswordExpiryDays { get; set; } = 45;

        public int? UnsuccessfulLoginAttemptsBeforeLockout { get; set; } = 3;

        public int? LockoutDurationMinutes { get; set; } = 30;

        [MaxLength(50)]
        public string? TwoFactorAuthenticationFrequency { get; set; } = "Never";

        [MaxLength(1000)]
        public string? PasswordComplexityRequirements { get; set; } = "Minimum 8 characters, at least one uppercase letter, one lowercase letter, one number, and one special character.";
    }
}
