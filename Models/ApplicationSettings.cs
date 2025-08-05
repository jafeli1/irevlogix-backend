using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class ApplicationSettings : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string SettingKey { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(1000)]
        public string SettingValue { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [MaxLength(50)]
        public string? Category { get; set; }
        
        public bool IsEncrypted { get; set; } = false;
        
        public bool IsReadOnly { get; set; } = false;
    }
}
