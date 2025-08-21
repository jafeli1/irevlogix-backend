using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class VendorCommunications : BaseEntity
    {
        public int VendorId { get; set; }
        public DateTime? Date { get; set; }
        public string? Type { get; set; }
        public string? Summary { get; set; }
        public string? NextSteps { get; set; }
        
        public virtual Vendor? Vendor { get; set; }
    }
}
