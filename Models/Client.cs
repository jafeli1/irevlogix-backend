using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class Client : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? ContactFirstName { get; set; }
        
        [MaxLength(100)]
        public string? ContactLastName { get; set; }
        
        [EmailAddress]
        [MaxLength(255)]
        public string? ContactEmail { get; set; }
        
        [MaxLength(20)]
        public string? ContactPhone { get; set; }
        
        [MaxLength(500)]
        public string? Address { get; set; }
        
        [MaxLength(100)]
        public string? City { get; set; }
        
        [MaxLength(50)]
        public string? State { get; set; }
        
        [MaxLength(20)]
        public string? ZipCode { get; set; }
        
        [MaxLength(100)]
        public string? Country { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [MaxLength(50)]
        public string? ClientType { get; set; }
        
        public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}
