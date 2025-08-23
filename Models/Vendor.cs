using System;

namespace irevlogix_backend.Models
{
    public class Vendor : BaseEntity
    {
        public string VendorName { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string MaterialsOfInterest { get; set; }
        public string PaymentTerms { get; set; }
        public decimal? VendorRating { get; set; }
        public string VendorTier { get; set; }
        public int? UpstreamTierVendor { get; set; }
    }
}
