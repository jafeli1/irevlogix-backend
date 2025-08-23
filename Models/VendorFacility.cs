using System;
using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class VendorFacility : BaseEntity
    {
        [MaxLength(2000)]
        public string? Description { get; set; }
        
        public int VendorId { get; set; }
        public virtual Vendor Vendor { get; set; } = null!;

        [MaxLength(500)]
        public string? FacilityLocation { get; set; }
        public int? TotalEmployees { get; set; }
        public int? SizeofFacilitySquareFoot { get; set; }
        [MaxLength(100)]
        public string? FacilityOwnedLeased { get; set; }
        public int? NumberofShifts { get; set; }
        [MaxLength(500)]
        public string? HoursofOperations { get; set; }
        [MaxLength(2000)]
        public string? DescribeFacilitySecurity { get; set; }
        public int? TotalSquareFootCapacityForStorage { get; set; }
        public int? TotalSquareFootCapacityForProcessing { get; set; }
        [MaxLength(2000)]
        public string? FacilityProcessingMethod { get; set; }
        [MaxLength(2000)]
        public string? HazardousDisposalMethod { get; set; }
        [MaxLength(500)]
        public string? OrganizationChartUpload { get; set; }
        [MaxLength(500)]
        public string? SiteLayoutUpload { get; set; }
        [MaxLength(500)]
        public string? LastFacilityAuditReportUpload { get; set; }
        [MaxLength(500)]
        public string? LastAuditReportFindingsUpload { get; set; }
        [MaxLength(500)]
        public string? LastAuditReportCorrectiveActionsUpload { get; set; }
        [MaxLength(500)]
        public string? CurrentContractForThisFacilityUpload { get; set; }
        [MaxLength(500)]
        public string? FacilityClosurePlanUpload { get; set; }
        [MaxLength(500)]
        public string? FacilitiesMaintenancePlanUpload { get; set; }
        [MaxLength(500)]
        public string? PhysicalSecurityPlanUpload { get; set; }

        public int? YearsInOperation { get; set; }
        [MaxLength(200)]
        public string? CurrentOwner { get; set; }
        [MaxLength(200)]
        public string? ParentCompany { get; set; }
        [MaxLength(500)]
        public string? PreviousOwners { get; set; }
        [MaxLength(500)]
        public string? SensitiveReceptors { get; set; }
        [MaxLength(500)]
        public string? RegulatoryComplianceStatus { get; set; }

        [MaxLength(2000)]
        public string? DescribeProcessFlow { get; set; }
        [MaxLength(500)]
        public string? DescribeProcessFlowUpload { get; set; }
        [MaxLength(2000)]
        public string? DescribeWasteMaterialGenerated { get; set; }
        [MaxLength(2000)]
        public string? DescribeDownstreamAuditingProcess { get; set; }
        public bool UtilizeChildPrisonLabor { get; set; }
        public bool MaterialsShippedNonOECDCountries { get; set; }
        [MaxLength(2000)]
        public string? DescribeNonOECDCountryShipments { get; set; }
        public bool CompetentAuthorityPermission { get; set; }
        [MaxLength(500)]
        public string? DocumentRequestCompetentAuthorityPermissionUpload { get; set; }
        public DateTime? DocumentRequestCompetentAuthorityPermissionExpDate { get; set; }
        public bool ZeroLandfillPolicy { get; set; }
        [MaxLength(500)]
        public string? DocumentRequestZeroLandfillPolicyUpload { get; set; }
        [MaxLength(2000)]
        public string? DescribeTrackingInboundOutboundMaterials { get; set; }
        [MaxLength(500)]
        public string? DocumentRequestMassBalanceUpload { get; set; }
        [MaxLength(2000)]
        public string? DescribeDataWipingProcedures { get; set; }
        public bool DataDestructionVerified { get; set; }
        [MaxLength(500)]
        public string? DataDestructionValidationUpload { get; set; }
        [MaxLength(2000)]
        public string? FunctionalityTestingDescription { get; set; }
        [MaxLength(2000)]
        public string? AssetGradingDescription { get; set; }
        public bool DoYouOperateALandfill { get; set; }
        public bool DoYouOwnAnIncinerator { get; set; }
        public bool DoYouPerformChemicalFixationAndStabilization { get; set; }
        [MaxLength(2000)]
        public string? UpdatedNamesAndLocationsOfYourDownstreamVendors { get; set; }
        [MaxLength(500)]
        public string? ScopeOfOperationsDocumentUpload { get; set; }
        [MaxLength(500)]
        public string? EquipmentEndOfLifePolicyUpload { get; set; }
        [MaxLength(500)]
        public string? DownsteamVendorSelectionProcessDocumentUpload { get; set; }
        [MaxLength(500)]
        public string? ElectronicsDispositionPolicyUpload { get; set; }
        [MaxLength(500)]
        public string? DataSecurityPolicyUpload { get; set; }
        [MaxLength(500)]
        public string? NonDiscriminationPolicyUpload { get; set; }
        [MaxLength(500)]
        public string? HazardousMaterialManagementPlanUpload { get; set; }
        [MaxLength(500)]
        public string? DataSanitizationPlanAndProcedureUpload { get; set; }
        [MaxLength(500)]
        public string? DataStorageDeviceShipmentAndProcessingContractUpload { get; set; }

        [MaxLength(500)]
        public string? MaterialGenerated1 { get; set; }
        [MaxLength(2000)]
        public string? HowMaterialsProcessedDisposed1 { get; set; }
        [MaxLength(500)]
        public string? NextTierVendorNameAddress1 { get; set; }
        [MaxLength(500)]
        public string? MaterialGenerated2 { get; set; }
        [MaxLength(2000)]
        public string? HowmaterialsProcessedDisposed2 { get; set; }
        [MaxLength(500)]
        public string? NextTierVendorNameAddress2 { get; set; }
        [MaxLength(500)]
        public string? MaterialGenerated3 { get; set; }
        [MaxLength(2000)]
        public string? HowmaterialsProcessedDisposed3 { get; set; }
        [MaxLength(500)]
        public string? NextTierVendorNameAddress3 { get; set; }
        [MaxLength(500)]
        public string? MaterialGenerated4 { get; set; }
        [MaxLength(2000)]
        public string? HowmaterialsProcessedDisposed4 { get; set; }
        [MaxLength(500)]
        public string? NextTierVendorNameAddress4 { get; set; }
        [MaxLength(500)]
        public string? MaterialGenerated5 { get; set; }
        [MaxLength(2000)]
        public string? HowmaterialsProcessedDisposed5 { get; set; }
        [MaxLength(500)]
        public string? NextTierVendorNameAddress5 { get; set; }
        [MaxLength(500)]
        public string? MaterialGenerated6 { get; set; }
        [MaxLength(2000)]
        public string? HowmaterialsProcessedDisposed6 { get; set; }
        [MaxLength(500)]
        public string? NextTierVendorNameAddress6 { get; set; }
        [MaxLength(2000)]
        public string? DescribeTransportationIncomingOutgoingMaterials { get; set; }
        [MaxLength(2000)]
        public string? DescribeAuditingProcessThirdPartyTransporters { get; set; }

        public bool OccupationalHealthSafetyManagementSystem { get; set; }
        [MaxLength(2000)]
        public string? DocumentRequestOccupationalHealthSafetyManagementSystem { get; set; }
        public bool FacilityDocumentedHealthSafety { get; set; }
        public bool FacilityAnnualHealthSafetyTraining { get; set; }
        public bool HealthSafetyViolations { get; set; }
        [MaxLength(2000)]
        public string? HealthSafetyViolationsxplanation { get; set; }

        [MaxLength(200)]
        public string? EHSManager { get; set; }
        [MaxLength(200)]
        public string? ComplianceManager { get; set; }
        [MaxLength(200)]
        public string? OHSMManager { get; set; }
        [MaxLength(200)]
        public string? FacilityManager { get; set; }
        [MaxLength(200)]
        public string? ManagementRepresentativeName { get; set; }
        [MaxLength(200)]
        public string? ManagementRepresentativeTitle { get; set; }
    }
}
