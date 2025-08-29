using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class ProcessingStep : BaseEntity
    {
        public int ProcessingLotId { get; set; }
        public virtual ProcessingLot ProcessingLot { get; set; } = null!;
        
        [Required]
        [MaxLength(100)]
        public string StepName { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public int? StepOrder { get; set; }
        
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";
        
        public int? ResponsibleUserId { get; set; }
        public virtual User? ResponsibleUser { get; set; }
        
        public decimal? LaborHours { get; set; }
        public decimal? MachineHours { get; set; }
        public decimal? EnergyConsumption { get; set; }
        public decimal? ProcessingCostPerUnit { get; set; }
        public decimal? TotalStepCost { get; set; }
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        [MaxLength(100)]
        public string? Equipment { get; set; }
        
        public decimal? InputWeight { get; set; }
        public decimal? OutputWeight { get; set; }
        public decimal? WasteWeight { get; set; }
    }
}
