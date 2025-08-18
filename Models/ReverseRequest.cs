using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class ReverseRequest : BaseEntity
    {
        public int ReverseJobRequestId { get; set; }
        public bool RequestAcknowledged { get; set; }
        
        [MaxLength(2000)]
        public string? Description { get; set; }
        
        [MaxLength(200)]
        public string? LocationName { get; set; }
        [MaxLength(500)]
        public string? Address { get; set; }
        [MaxLength(100)]
        public string? AddressExt { get; set; }
        [MaxLength(100)]
        public string? City { get; set; }
        public int? StateId { get; set; }
        [MaxLength(20)]
        public string? PostalCode { get; set; }
        public int? CountryId { get; set; }
        
        [MaxLength(100)]
        public string? PrimaryContactFirstName { get; set; }
        [MaxLength(100)]
        public string? PrimaryContactLastName { get; set; }
        [MaxLength(100)]
        public string? SecondaryContactFirstName { get; set; }
        [MaxLength(100)]
        public string? SecondaryContactLastName { get; set; }
        [MaxLength(20)]
        public string? PrimaryContactCellPhoneNumber { get; set; }
        [MaxLength(20)]
        public string? SecondaryContactCellPhoneNumber { get; set; }
        [EmailAddress]
        [MaxLength(255)]
        public string? PrimaryContactEmailAddress { get; set; }
        [EmailAddress]
        [MaxLength(255)]
        public string? SecondaryContactEmailAddress { get; set; }
        
        public DateTime? RequestedPickUpDate { get; set; }
        public TimeSpan? RequestedPickUpTime { get; set; }
        public bool CanSiteAccommodate53FeetTruck { get; set; }
        public bool DoesSiteHaveADock { get; set; }
        public bool CanHardwareBeResold { get; set; }
        public bool CanHardDrivesBeResold { get; set; }
        public bool IsEquipmentLoose { get; set; }
        public bool IsEquipmentPalletized { get; set; }
        public bool IsLiftGateRequired { get; set; }
        public bool IsOnsiteDataDestructingRequiredWipingShredding { get; set; }
        [MaxLength(500)]
        public string? TypeOfMediaToBeDestroyed { get; set; }
        [MaxLength(2000)]
        public string? OtherInstructions { get; set; }
        
        [MaxLength(500)]
        public string? InstructionUpload { get; set; }
        [MaxLength(500)]
        public string? EquipmentListUpload { get; set; }
        [MaxLength(500)]
        public string? AssetsPhotoUpload1 { get; set; }
        [MaxLength(500)]
        public string? AssetsPhotoUpload2 { get; set; }
        [MaxLength(500)]
        public string? AssetsPhotoUpload3 { get; set; }
        [MaxLength(500)]
        public string? AssetsPhotoUpload4 { get; set; }
        [MaxLength(500)]
        public string? AssetsPhotoUpload5 { get; set; }
        [MaxLength(500)]
        public string? HelpfulPhotoUpload1 { get; set; }
        [MaxLength(500)]
        public string? HelpfulPhotoUpload2 { get; set; }
        [MaxLength(500)]
        public string? HelpfulPhotoUpload3 { get; set; }
        
        public bool IsActive { get; set; } = true;
        public DateTime? DateClosed { get; set; }
        [MaxLength(2000)]
        public string? ClosureComments { get; set; }
    }
}
