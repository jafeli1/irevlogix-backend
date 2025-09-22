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
        [MaxLength(100)]
        public string ReportType { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Schedule { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string DataSource { get; set; } = string.Empty;

        [Required]
        public string SelectedColumns { get; set; } = string.Empty;

        public string? Filters { get; set; }

        public string? Sorting { get; set; }

        [Required]
        [MaxLength(20)]
        public string Frequency { get; set; } = string.Empty;

        [Required]
        public string Recipients { get; set; } = string.Empty;

        [Required]
        public TimeSpan DeliveryTime { get; set; } = TimeSpan.FromHours(9);

        public int? DayOfWeek { get; set; }

        public int? DayOfMonth { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastRunDate { get; set; }

        public DateTime? NextRunDate { get; set; }

        [Required]
        public string ClientId { get; set; } = string.Empty;

        [Required]
        public int CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User? CreatedByUser { get; set; }

        [Required]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public string? LastModifiedBy { get; set; }

        public DateTime? DateModified { get; set; }

        public string? Parameters { get; set; }
    }
}
