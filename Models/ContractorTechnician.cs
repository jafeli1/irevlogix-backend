using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class ContractorTechnician : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
        
        public bool Approved { get; set; }
        public bool Preferred { get; set; }
        
        [MaxLength(200)]
        public string? TechnicianSource { get; set; }
        
        [MaxLength(2000)]
        public string? Comments { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? City { get; set; }
        
        public int? StateId { get; set; }
        
        [MaxLength(20)]
        public string? ZipCode { get; set; }
        
        [MaxLength(20)]
        public string? Phone { get; set; }
        
        [EmailAddress]
        [MaxLength(255)]
        public string? Email { get; set; }
        
        [MaxLength(500)]
        public string? ShippingAddress { get; set; }
        
        public DateTime? OnboardDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool BackgroundCheckOnboarding { get; set; }
        public bool DrugTestOnboarding { get; set; }
        public bool ThirdPartyAgreementOnboarding { get; set; }
        public DateTime? BackgroundCheckDate { get; set; }
        public DateTime? DrugTestDate { get; set; }
        public DateTime? ThirdPartyServiceProviderAgreementVersion { get; set; }
        public DateTime? TrainingCompletionDate { get; set; }
        
        [MaxLength(500)]
        public string? BackgroundCheckFormUpload { get; set; }
        [MaxLength(500)]
        public string? DrugTestUpload { get; set; }
        [MaxLength(500)]
        public string? ThirdPartyServiceProviderAgreementUpload { get; set; }
        [MaxLength(500)]
        public string? MiscUpload { get; set; }
        
        [MaxLength(2000)]
        public string? UpdateSummary { get; set; }
    }
}
