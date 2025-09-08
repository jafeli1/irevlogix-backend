using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace irevlogix_backend.Models
{
    public class ScheduledReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string DataSource { get; set; } = string.Empty;

        [Required]
        public string SelectedColumns { get; set; } = string.Empty; // JSON array

        public string? Filters { get; set; } // JSON object

        public string? Sorting { get; set; } // JSON object

        [Required]
        [MaxLength(20)]
        public string Frequency { get; set; } = string.Empty; // Daily, Weekly, Monthly

        [Required]
        public string Recipients { get; set; } = string.Empty; // JSON array of email addresses

        public TimeSpan DeliveryTime { get; set; } = TimeSpan.FromHours(9); // Default 9 AM

        public int? DayOfWeek { get; set; } // For weekly reports (0-6, Sunday=0)

        public int? DayOfMonth { get; set; } // For monthly reports (1-31)

        public bool IsActive { get; set; } = true;

        public DateTime? LastRunDate { get; set; }

        public DateTime? NextRunDate { get; set; }

        [Required]
        public string ClientId { get; set; } = string.Empty;

        [Required]
        public int CreatedByUserId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime DateModified { get; set; } = DateTime.UtcNow;

        [ForeignKey("CreatedByUserId")]
        public virtual User? CreatedByUser { get; set; }
    }
}
