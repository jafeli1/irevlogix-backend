using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using irevlogix_backend.Data;
using irevlogix_backend.Models;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VendorFacilitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VendorFacilitiesController> _logger;

        public VendorFacilitiesController(ApplicationDbContext context, ILogger<VendorFacilitiesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetClientId()
        {
            return User.FindFirst("ClientId")?.Value ?? throw new UnauthorizedAccessException("ClientId not found in token");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetVendorFacilities(
            [FromQuery] string? vendorName = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.VendorFacilities
                    .Include(vf => vf.Vendor)
                    .Where(vf => vf.ClientId == clientId);

                if (!string.IsNullOrEmpty(vendorName))
                    query = query.Where(vf => vf.Vendor.VendorName.Contains(vendorName));

                var totalCount = await query.CountAsync();
                var facilities = await query
                    .OrderBy(vf => vf.Vendor.VendorName)
                    .ThenBy(vf => vf.FacilityLocation)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(vf => new
                    {
                        vf.Id,
                        VendorName = vf.Vendor.VendorName,
                        vf.FacilityLocation,
                        vf.YearsInOperation,
                        vf.DateCreated,
                        vf.DateUpdated
                    })
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                return Ok(facilities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vendor facilities");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetVendorFacility(int id)
        {
            try
            {
                var clientId = GetClientId();
                var facility = await _context.VendorFacilities
                    .Include(vf => vf.Vendor)
                    .Where(vf => vf.Id == id && vf.ClientId == clientId)
                    .Select(vf => new
                    {
                        vf.Id,
                        vf.ClientId,
                        vf.VendorId,
                        VendorName = vf.Vendor.VendorName,
                        vf.Description,
                        vf.FacilityLocation,
                        vf.TotalEmployees,
                        vf.SizeofFacilitySquareFoot,
                        vf.FacilityOwnedLeased,
                        vf.NumberofShifts,
                        vf.HoursofOperations,
                        vf.DescribeFacilitySecurity,
                        vf.TotalSquareFootCapacityForStorage,
                        vf.TotalSquareFootCapacityForProcessing,
                        vf.FacilityProcessingMethod,
                        vf.HazardousDisposalMethod,
                        vf.OrganizationChartUpload,
                        vf.SiteLayoutUpload,
                        vf.LastFacilityAuditReportUpload,
                        vf.LastAuditReportFindingsUpload,
                        vf.LastAuditReportCorrectiveActionsUpload,
                        vf.CurrentContractForThisFacilityUpload,
                        vf.FacilityClosurePlanUpload,
                        vf.FacilitiesMaintenancePlanUpload,
                        vf.PhysicalSecurityPlanUpload,
                        vf.YearsInOperation,
                        vf.CurrentOwner,
                        vf.ParentCompany,
                        vf.PreviousOwners,
                        vf.SensitiveReceptors,
                        vf.RegulatoryComplianceStatus,
                        vf.DescribeProcessFlow,
                        vf.DescribeProcessFlowUpload,
                        vf.DescribeWasteMaterialGenerated,
                        vf.DescribeDownstreamAuditingProcess,
                        vf.UtilizeChildPrisonLabor,
                        vf.MaterialsShippedNonOECDCountries,
                        vf.DescribeNonOECDCountryShipments,
                        vf.CompetentAuthorityPermission,
                        vf.DocumentRequestCompetentAuthorityPermissionUpload,
                        vf.DocumentRequestCompetentAuthorityPermissionExpDate,
                        vf.ZeroLandfillPolicy,
                        vf.DocumentRequestZeroLandfillPolicyUpload,
                        vf.DescribeTrackingInboundOutboundMaterials,
                        vf.DocumentRequestMassBalanceUpload,
                        vf.DescribeDataWipingProcedures,
                        vf.DataDestructionVerified,
                        vf.DataDestructionValidationUpload,
                        vf.FunctionalityTestingDescription,
                        vf.AssetGradingDescription,
                        vf.DoYouOperateALandfill,
                        vf.DoYouOwnAnIncinerator,
                        vf.DoYouPerformChemicalFixationAndStabilization,
                        vf.UpdatedNamesAndLocationsOfYourDownstreamVendors,
                        vf.ScopeOfOperationsDocumentUpload,
                        vf.EquipmentEndOfLifePolicyUpload,
                        vf.DownsteamVendorSelectionProcessDocumentUpload,
                        vf.ElectronicsDispositionPolicyUpload,
                        vf.DataSecurityPolicyUpload,
                        vf.NonDiscriminationPolicyUpload,
                        vf.HazardousMaterialManagementPlanUpload,
                        vf.DataSanitizationPlanAndProcedureUpload,
                        vf.DataStorageDeviceShipmentAndProcessingContractUpload,
                        vf.MaterialGenerated1,
                        vf.HowMaterialsProcessedDisposed1,
                        vf.NextTierVendorNameAddress1,
                        vf.MaterialGenerated2,
                        vf.HowmaterialsProcessedDisposed2,
                        vf.NextTierVendorNameAddress2,
                        vf.MaterialGenerated3,
                        vf.HowmaterialsProcessedDisposed3,
                        vf.NextTierVendorNameAddress3,
                        vf.MaterialGenerated4,
                        vf.HowmaterialsProcessedDisposed4,
                        vf.NextTierVendorNameAddress4,
                        vf.MaterialGenerated5,
                        vf.HowmaterialsProcessedDisposed5,
                        vf.NextTierVendorNameAddress5,
                        vf.MaterialGenerated6,
                        vf.HowmaterialsProcessedDisposed6,
                        vf.NextTierVendorNameAddress6,
                        vf.DescribeTransportationIncomingOutgoingMaterials,
                        vf.DescribeAuditingProcessThirdPartyTransporters,
                        vf.OccupationalHealthSafetyManagementSystem,
                        vf.DocumentRequestOccupationalHealthSafetyManagementSystem,
                        vf.FacilityDocumentedHealthSafety,
                        vf.FacilityAnnualHealthSafetyTraining,
                        vf.HealthSafetyViolations,
                        vf.HealthSafetyViolationsxplanation,
                        vf.EHSManager,
                        vf.ComplianceManager,
                        vf.OHSMManager,
                        vf.FacilityManager,
                        vf.ManagementRepresentativeName,
                        vf.ManagementRepresentativeTitle,
                        vf.DateCreated,
                        vf.DateUpdated,
                        vf.CreatedBy,
                        vf.UpdatedBy
                    })
                    .FirstOrDefaultAsync();

                if (facility == null)
                    return NotFound();

                return Ok(facility);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vendor facility {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateVendorFacility(VendorFacilityRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var facility = new VendorFacility
                {
                    ClientId = clientId,
                    VendorId = request.VendorId,
                    Description = request.Description,
                    FacilityLocation = request.FacilityLocation,
                    TotalEmployees = request.TotalEmployees,
                    SizeofFacilitySquareFoot = request.SizeofFacilitySquareFoot,
                    FacilityOwnedLeased = request.FacilityOwnedLeased,
                    NumberofShifts = request.NumberofShifts,
                    HoursofOperations = request.HoursofOperations,
                    DescribeFacilitySecurity = request.DescribeFacilitySecurity,
                    TotalSquareFootCapacityForStorage = request.TotalSquareFootCapacityForStorage,
                    TotalSquareFootCapacityForProcessing = request.TotalSquareFootCapacityForProcessing,
                    FacilityProcessingMethod = request.FacilityProcessingMethod,
                    HazardousDisposalMethod = request.HazardousDisposalMethod,
                    OrganizationChartUpload = request.OrganizationChartUpload,
                    SiteLayoutUpload = request.SiteLayoutUpload,
                    LastFacilityAuditReportUpload = request.LastFacilityAuditReportUpload,
                    LastAuditReportFindingsUpload = request.LastAuditReportFindingsUpload,
                    LastAuditReportCorrectiveActionsUpload = request.LastAuditReportCorrectiveActionsUpload,
                    CurrentContractForThisFacilityUpload = request.CurrentContractForThisFacilityUpload,
                    FacilityClosurePlanUpload = request.FacilityClosurePlanUpload,
                    FacilitiesMaintenancePlanUpload = request.FacilitiesMaintenancePlanUpload,
                    PhysicalSecurityPlanUpload = request.PhysicalSecurityPlanUpload,
                    YearsInOperation = request.YearsInOperation,
                    CurrentOwner = request.CurrentOwner,
                    ParentCompany = request.ParentCompany,
                    PreviousOwners = request.PreviousOwners,
                    SensitiveReceptors = request.SensitiveReceptors,
                    RegulatoryComplianceStatus = request.RegulatoryComplianceStatus,
                    DescribeProcessFlow = request.DescribeProcessFlow,
                    DescribeProcessFlowUpload = request.DescribeProcessFlowUpload,
                    DescribeWasteMaterialGenerated = request.DescribeWasteMaterialGenerated,
                    DescribeDownstreamAuditingProcess = request.DescribeDownstreamAuditingProcess,
                    UtilizeChildPrisonLabor = request.UtilizeChildPrisonLabor,
                    MaterialsShippedNonOECDCountries = request.MaterialsShippedNonOECDCountries,
                    DescribeNonOECDCountryShipments = request.DescribeNonOECDCountryShipments,
                    CompetentAuthorityPermission = request.CompetentAuthorityPermission,
                    DocumentRequestCompetentAuthorityPermissionUpload = request.DocumentRequestCompetentAuthorityPermissionUpload,
                    DocumentRequestCompetentAuthorityPermissionExpDate = request.DocumentRequestCompetentAuthorityPermissionExpDate,
                    ZeroLandfillPolicy = request.ZeroLandfillPolicy,
                    DocumentRequestZeroLandfillPolicyUpload = request.DocumentRequestZeroLandfillPolicyUpload,
                    DescribeTrackingInboundOutboundMaterials = request.DescribeTrackingInboundOutboundMaterials,
                    DocumentRequestMassBalanceUpload = request.DocumentRequestMassBalanceUpload,
                    DescribeDataWipingProcedures = request.DescribeDataWipingProcedures,
                    DataDestructionVerified = request.DataDestructionVerified,
                    DataDestructionValidationUpload = request.DataDestructionValidationUpload,
                    FunctionalityTestingDescription = request.FunctionalityTestingDescription,
                    AssetGradingDescription = request.AssetGradingDescription,
                    DoYouOperateALandfill = request.DoYouOperateALandfill,
                    DoYouOwnAnIncinerator = request.DoYouOwnAnIncinerator,
                    DoYouPerformChemicalFixationAndStabilization = request.DoYouPerformChemicalFixationAndStabilization,
                    UpdatedNamesAndLocationsOfYourDownstreamVendors = request.UpdatedNamesAndLocationsOfYourDownstreamVendors,
                    ScopeOfOperationsDocumentUpload = request.ScopeOfOperationsDocumentUpload,
                    EquipmentEndOfLifePolicyUpload = request.EquipmentEndOfLifePolicyUpload,
                    DownsteamVendorSelectionProcessDocumentUpload = request.DownsteamVendorSelectionProcessDocumentUpload,
                    ElectronicsDispositionPolicyUpload = request.ElectronicsDispositionPolicyUpload,
                    DataSecurityPolicyUpload = request.DataSecurityPolicyUpload,
                    NonDiscriminationPolicyUpload = request.NonDiscriminationPolicyUpload,
                    HazardousMaterialManagementPlanUpload = request.HazardousMaterialManagementPlanUpload,
                    DataSanitizationPlanAndProcedureUpload = request.DataSanitizationPlanAndProcedureUpload,
                    DataStorageDeviceShipmentAndProcessingContractUpload = request.DataStorageDeviceShipmentAndProcessingContractUpload,
                    MaterialGenerated1 = request.MaterialGenerated1,
                    HowMaterialsProcessedDisposed1 = request.HowMaterialsProcessedDisposed1,
                    NextTierVendorNameAddress1 = request.NextTierVendorNameAddress1,
                    MaterialGenerated2 = request.MaterialGenerated2,
                    HowmaterialsProcessedDisposed2 = request.HowmaterialsProcessedDisposed2,
                    NextTierVendorNameAddress2 = request.NextTierVendorNameAddress2,
                    MaterialGenerated3 = request.MaterialGenerated3,
                    HowmaterialsProcessedDisposed3 = request.HowmaterialsProcessedDisposed3,
                    NextTierVendorNameAddress3 = request.NextTierVendorNameAddress3,
                    MaterialGenerated4 = request.MaterialGenerated4,
                    HowmaterialsProcessedDisposed4 = request.HowmaterialsProcessedDisposed4,
                    NextTierVendorNameAddress4 = request.NextTierVendorNameAddress4,
                    MaterialGenerated5 = request.MaterialGenerated5,
                    HowmaterialsProcessedDisposed5 = request.HowmaterialsProcessedDisposed5,
                    NextTierVendorNameAddress5 = request.NextTierVendorNameAddress5,
                    MaterialGenerated6 = request.MaterialGenerated6,
                    HowmaterialsProcessedDisposed6 = request.HowmaterialsProcessedDisposed6,
                    NextTierVendorNameAddress6 = request.NextTierVendorNameAddress6,
                    DescribeTransportationIncomingOutgoingMaterials = request.DescribeTransportationIncomingOutgoingMaterials,
                    DescribeAuditingProcessThirdPartyTransporters = request.DescribeAuditingProcessThirdPartyTransporters,
                    OccupationalHealthSafetyManagementSystem = request.OccupationalHealthSafetyManagementSystem,
                    DocumentRequestOccupationalHealthSafetyManagementSystem = request.DocumentRequestOccupationalHealthSafetyManagementSystem,
                    FacilityDocumentedHealthSafety = request.FacilityDocumentedHealthSafety,
                    FacilityAnnualHealthSafetyTraining = request.FacilityAnnualHealthSafetyTraining,
                    HealthSafetyViolations = request.HealthSafetyViolations,
                    HealthSafetyViolationsxplanation = request.HealthSafetyViolationsxplanation,
                    EHSManager = request.EHSManager,
                    ComplianceManager = request.ComplianceManager,
                    OHSMManager = request.OHSMManager,
                    FacilityManager = request.FacilityManager,
                    ManagementRepresentativeName = request.ManagementRepresentativeName,
                    ManagementRepresentativeTitle = request.ManagementRepresentativeTitle,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.VendorFacilities.Add(facility);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetVendorFacility), new { id = facility.Id }, new { id = facility.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vendor facility");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendorFacility(int id, VendorFacilityRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var facility = await _context.VendorFacilities
                    .Where(vf => vf.Id == id && vf.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (facility == null)
                    return NotFound();

                facility.VendorId = request.VendorId;
                facility.Description = request.Description;
                facility.FacilityLocation = request.FacilityLocation;
                facility.TotalEmployees = request.TotalEmployees;
                facility.SizeofFacilitySquareFoot = request.SizeofFacilitySquareFoot;
                facility.FacilityOwnedLeased = request.FacilityOwnedLeased;
                facility.NumberofShifts = request.NumberofShifts;
                facility.HoursofOperations = request.HoursofOperations;
                facility.DescribeFacilitySecurity = request.DescribeFacilitySecurity;
                facility.TotalSquareFootCapacityForStorage = request.TotalSquareFootCapacityForStorage;
                facility.TotalSquareFootCapacityForProcessing = request.TotalSquareFootCapacityForProcessing;
                facility.FacilityProcessingMethod = request.FacilityProcessingMethod;
                facility.HazardousDisposalMethod = request.HazardousDisposalMethod;
                facility.OrganizationChartUpload = request.OrganizationChartUpload;
                facility.SiteLayoutUpload = request.SiteLayoutUpload;
                facility.LastFacilityAuditReportUpload = request.LastFacilityAuditReportUpload;
                facility.LastAuditReportFindingsUpload = request.LastAuditReportFindingsUpload;
                facility.LastAuditReportCorrectiveActionsUpload = request.LastAuditReportCorrectiveActionsUpload;
                facility.CurrentContractForThisFacilityUpload = request.CurrentContractForThisFacilityUpload;
                facility.FacilityClosurePlanUpload = request.FacilityClosurePlanUpload;
                facility.FacilitiesMaintenancePlanUpload = request.FacilitiesMaintenancePlanUpload;
                facility.PhysicalSecurityPlanUpload = request.PhysicalSecurityPlanUpload;
                facility.YearsInOperation = request.YearsInOperation;
                facility.CurrentOwner = request.CurrentOwner;
                facility.ParentCompany = request.ParentCompany;
                facility.PreviousOwners = request.PreviousOwners;
                facility.SensitiveReceptors = request.SensitiveReceptors;
                facility.RegulatoryComplianceStatus = request.RegulatoryComplianceStatus;
                facility.DescribeProcessFlow = request.DescribeProcessFlow;
                facility.DescribeProcessFlowUpload = request.DescribeProcessFlowUpload;
                facility.DescribeWasteMaterialGenerated = request.DescribeWasteMaterialGenerated;
                facility.DescribeDownstreamAuditingProcess = request.DescribeDownstreamAuditingProcess;
                facility.UtilizeChildPrisonLabor = request.UtilizeChildPrisonLabor;
                facility.MaterialsShippedNonOECDCountries = request.MaterialsShippedNonOECDCountries;
                facility.DescribeNonOECDCountryShipments = request.DescribeNonOECDCountryShipments;
                facility.CompetentAuthorityPermission = request.CompetentAuthorityPermission;
                facility.DocumentRequestCompetentAuthorityPermissionUpload = request.DocumentRequestCompetentAuthorityPermissionUpload;
                facility.DocumentRequestCompetentAuthorityPermissionExpDate = request.DocumentRequestCompetentAuthorityPermissionExpDate;
                facility.ZeroLandfillPolicy = request.ZeroLandfillPolicy;
                facility.DocumentRequestZeroLandfillPolicyUpload = request.DocumentRequestZeroLandfillPolicyUpload;
                facility.DescribeTrackingInboundOutboundMaterials = request.DescribeTrackingInboundOutboundMaterials;
                facility.DocumentRequestMassBalanceUpload = request.DocumentRequestMassBalanceUpload;
                facility.DescribeDataWipingProcedures = request.DescribeDataWipingProcedures;
                facility.DataDestructionVerified = request.DataDestructionVerified;
                facility.DataDestructionValidationUpload = request.DataDestructionValidationUpload;
                facility.FunctionalityTestingDescription = request.FunctionalityTestingDescription;
                facility.AssetGradingDescription = request.AssetGradingDescription;
                facility.DoYouOperateALandfill = request.DoYouOperateALandfill;
                facility.DoYouOwnAnIncinerator = request.DoYouOwnAnIncinerator;
                facility.DoYouPerformChemicalFixationAndStabilization = request.DoYouPerformChemicalFixationAndStabilization;
                facility.UpdatedNamesAndLocationsOfYourDownstreamVendors = request.UpdatedNamesAndLocationsOfYourDownstreamVendors;
                facility.ScopeOfOperationsDocumentUpload = request.ScopeOfOperationsDocumentUpload;
                facility.EquipmentEndOfLifePolicyUpload = request.EquipmentEndOfLifePolicyUpload;
                facility.DownsteamVendorSelectionProcessDocumentUpload = request.DownsteamVendorSelectionProcessDocumentUpload;
                facility.ElectronicsDispositionPolicyUpload = request.ElectronicsDispositionPolicyUpload;
                facility.DataSecurityPolicyUpload = request.DataSecurityPolicyUpload;
                facility.NonDiscriminationPolicyUpload = request.NonDiscriminationPolicyUpload;
                facility.HazardousMaterialManagementPlanUpload = request.HazardousMaterialManagementPlanUpload;
                facility.DataSanitizationPlanAndProcedureUpload = request.DataSanitizationPlanAndProcedureUpload;
                facility.DataStorageDeviceShipmentAndProcessingContractUpload = request.DataStorageDeviceShipmentAndProcessingContractUpload;
                facility.MaterialGenerated1 = request.MaterialGenerated1;
                facility.HowMaterialsProcessedDisposed1 = request.HowMaterialsProcessedDisposed1;
                facility.NextTierVendorNameAddress1 = request.NextTierVendorNameAddress1;
                facility.MaterialGenerated2 = request.MaterialGenerated2;
                facility.HowmaterialsProcessedDisposed2 = request.HowmaterialsProcessedDisposed2;
                facility.NextTierVendorNameAddress2 = request.NextTierVendorNameAddress2;
                facility.MaterialGenerated3 = request.MaterialGenerated3;
                facility.HowmaterialsProcessedDisposed3 = request.HowmaterialsProcessedDisposed3;
                facility.NextTierVendorNameAddress3 = request.NextTierVendorNameAddress3;
                facility.MaterialGenerated4 = request.MaterialGenerated4;
                facility.HowmaterialsProcessedDisposed4 = request.HowmaterialsProcessedDisposed4;
                facility.NextTierVendorNameAddress4 = request.NextTierVendorNameAddress4;
                facility.MaterialGenerated5 = request.MaterialGenerated5;
                facility.HowmaterialsProcessedDisposed5 = request.HowmaterialsProcessedDisposed5;
                facility.NextTierVendorNameAddress5 = request.NextTierVendorNameAddress5;
                facility.MaterialGenerated6 = request.MaterialGenerated6;
                facility.HowmaterialsProcessedDisposed6 = request.HowmaterialsProcessedDisposed6;
                facility.NextTierVendorNameAddress6 = request.NextTierVendorNameAddress6;
                facility.DescribeTransportationIncomingOutgoingMaterials = request.DescribeTransportationIncomingOutgoingMaterials;
                facility.DescribeAuditingProcessThirdPartyTransporters = request.DescribeAuditingProcessThirdPartyTransporters;
                facility.OccupationalHealthSafetyManagementSystem = request.OccupationalHealthSafetyManagementSystem;
                facility.DocumentRequestOccupationalHealthSafetyManagementSystem = request.DocumentRequestOccupationalHealthSafetyManagementSystem;
                facility.FacilityDocumentedHealthSafety = request.FacilityDocumentedHealthSafety;
                facility.FacilityAnnualHealthSafetyTraining = request.FacilityAnnualHealthSafetyTraining;
                facility.HealthSafetyViolations = request.HealthSafetyViolations;
                facility.HealthSafetyViolationsxplanation = request.HealthSafetyViolationsxplanation;
                facility.EHSManager = request.EHSManager;
                facility.ComplianceManager = request.ComplianceManager;
                facility.OHSMManager = request.OHSMManager;
                facility.FacilityManager = request.FacilityManager;
                facility.ManagementRepresentativeName = request.ManagementRepresentativeName;
                facility.ManagementRepresentativeTitle = request.ManagementRepresentativeTitle;
                facility.UpdatedBy = userId;
                facility.DateUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vendor facility {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendorFacility(int id)
        {
            try
            {
                var clientId = GetClientId();
                var facility = await _context.VendorFacilities
                    .Where(vf => vf.Id == id && vf.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (facility == null)
                    return NotFound();

                _context.VendorFacilities.Remove(facility);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vendor facility {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/upload")]
        public async Task<ActionResult<object>> UploadDocument(int id, IFormFile file, [FromForm] string documentType, [FromForm] string? description = null)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var facility = await _context.VendorFacilities
                    .Where(vf => vf.Id == id && vf.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (facility == null)
                    return NotFound();

                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                var uploadsPath = Path.Combine("upload", clientId, "VendorFacilities", id.ToString());
                Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = Path.Combine("upload", clientId, "VendorFacilities", id.ToString(), fileName);

                switch (documentType.ToLower())
                {
                    case "organizationchartupload":
                        facility.OrganizationChartUpload = relativePath;
                        break;
                    case "sitelayoutupload":
                        facility.SiteLayoutUpload = relativePath;
                        break;
                    case "lastfacilityauditreportupload":
                        facility.LastFacilityAuditReportUpload = relativePath;
                        break;
                    case "lastauditreportfindingsupload":
                        facility.LastAuditReportFindingsUpload = relativePath;
                        break;
                    case "lastauditreportcorrectiveactionsupload":
                        facility.LastAuditReportCorrectiveActionsUpload = relativePath;
                        break;
                    case "currentcontractforthisfacilityupload":
                        facility.CurrentContractForThisFacilityUpload = relativePath;
                        break;
                    case "facilityclosureplanupload":
                        facility.FacilityClosurePlanUpload = relativePath;
                        break;
                    case "facilitiesmaintenanceplanupload":
                        facility.FacilitiesMaintenancePlanUpload = relativePath;
                        break;
                    case "physicalsecurityplanupload":
                        facility.PhysicalSecurityPlanUpload = relativePath;
                        break;
                    case "describeprocessflowupload":
                        facility.DescribeProcessFlowUpload = relativePath;
                        break;
                    case "documentrequestcompetentauthoritypermissionupload":
                        facility.DocumentRequestCompetentAuthorityPermissionUpload = relativePath;
                        break;
                    case "documentrequestzerolandfillpolicyupload":
                        facility.DocumentRequestZeroLandfillPolicyUpload = relativePath;
                        break;
                    case "documentrequestmassbalanceupload":
                        facility.DocumentRequestMassBalanceUpload = relativePath;
                        break;
                    case "datadestructionvalidationupload":
                        facility.DataDestructionValidationUpload = relativePath;
                        break;
                    case "scopeofoperationsdocumentupload":
                        facility.ScopeOfOperationsDocumentUpload = relativePath;
                        break;
                    case "equipmentendoflifepolicyupload":
                        facility.EquipmentEndOfLifePolicyUpload = relativePath;
                        break;
                    case "downsteamvendorselectionprocessdocumentupload":
                        facility.DownsteamVendorSelectionProcessDocumentUpload = relativePath;
                        break;
                    case "electronicsdispositionpolicyupload":
                        facility.ElectronicsDispositionPolicyUpload = relativePath;
                        break;
                    case "datasecuritypolicyupload":
                        facility.DataSecurityPolicyUpload = relativePath;
                        break;
                    case "nondiscriminationpolicyupload":
                        facility.NonDiscriminationPolicyUpload = relativePath;
                        break;
                    case "hazardousmaterialmanagementplanupload":
                        facility.HazardousMaterialManagementPlanUpload = relativePath;
                        break;
                    case "datasanitizationplandandprocedureupload":
                        facility.DataSanitizationPlanAndProcedureUpload = relativePath;
                        break;
                    case "datastoragedeviceshipmentandprocessingcontractupload":
                        facility.DataStorageDeviceShipmentAndProcessingContractUpload = relativePath;
                        break;
                    default:
                        return BadRequest("Invalid document type");
                }

                facility.UpdatedBy = userId;
                facility.DateUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { id = facility.Id, fileName = file.FileName, filePath = relativePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document for vendor facility {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class VendorFacilityRequest
    {
        public int VendorId { get; set; }
        public string? Description { get; set; }
        public string? FacilityLocation { get; set; }
        public int? TotalEmployees { get; set; }
        public int? SizeofFacilitySquareFoot { get; set; }
        public string? FacilityOwnedLeased { get; set; }
        public int? NumberofShifts { get; set; }
        public string? HoursofOperations { get; set; }
        public string? DescribeFacilitySecurity { get; set; }
        public int? TotalSquareFootCapacityForStorage { get; set; }
        public int? TotalSquareFootCapacityForProcessing { get; set; }
        public string? FacilityProcessingMethod { get; set; }
        public string? HazardousDisposalMethod { get; set; }
        public string? OrganizationChartUpload { get; set; }
        public string? SiteLayoutUpload { get; set; }
        public string? LastFacilityAuditReportUpload { get; set; }
        public string? LastAuditReportFindingsUpload { get; set; }
        public string? LastAuditReportCorrectiveActionsUpload { get; set; }
        public string? CurrentContractForThisFacilityUpload { get; set; }
        public string? FacilityClosurePlanUpload { get; set; }
        public string? FacilitiesMaintenancePlanUpload { get; set; }
        public string? PhysicalSecurityPlanUpload { get; set; }
        public int? YearsInOperation { get; set; }
        public string? CurrentOwner { get; set; }
        public string? ParentCompany { get; set; }
        public string? PreviousOwners { get; set; }
        public string? SensitiveReceptors { get; set; }
        public string? RegulatoryComplianceStatus { get; set; }
        public string? DescribeProcessFlow { get; set; }
        public string? DescribeProcessFlowUpload { get; set; }
        public string? DescribeWasteMaterialGenerated { get; set; }
        public string? DescribeDownstreamAuditingProcess { get; set; }
        public bool UtilizeChildPrisonLabor { get; set; }
        public bool MaterialsShippedNonOECDCountries { get; set; }
        public string? DescribeNonOECDCountryShipments { get; set; }
        public bool CompetentAuthorityPermission { get; set; }
        public string? DocumentRequestCompetentAuthorityPermissionUpload { get; set; }
        public DateTime? DocumentRequestCompetentAuthorityPermissionExpDate { get; set; }
        public bool ZeroLandfillPolicy { get; set; }
        public string? DocumentRequestZeroLandfillPolicyUpload { get; set; }
        public string? DescribeTrackingInboundOutboundMaterials { get; set; }
        public string? DocumentRequestMassBalanceUpload { get; set; }
        public string? DescribeDataWipingProcedures { get; set; }
        public bool DataDestructionVerified { get; set; }
        public string? DataDestructionValidationUpload { get; set; }
        public string? FunctionalityTestingDescription { get; set; }
        public string? AssetGradingDescription { get; set; }
        public bool DoYouOperateALandfill { get; set; }
        public bool DoYouOwnAnIncinerator { get; set; }
        public bool DoYouPerformChemicalFixationAndStabilization { get; set; }
        public string? UpdatedNamesAndLocationsOfYourDownstreamVendors { get; set; }
        public string? ScopeOfOperationsDocumentUpload { get; set; }
        public string? EquipmentEndOfLifePolicyUpload { get; set; }
        public string? DownsteamVendorSelectionProcessDocumentUpload { get; set; }
        public string? ElectronicsDispositionPolicyUpload { get; set; }
        public string? DataSecurityPolicyUpload { get; set; }
        public string? NonDiscriminationPolicyUpload { get; set; }
        public string? HazardousMaterialManagementPlanUpload { get; set; }
        public string? DataSanitizationPlanAndProcedureUpload { get; set; }
        public string? DataStorageDeviceShipmentAndProcessingContractUpload { get; set; }
        public string? MaterialGenerated1 { get; set; }
        public string? HowMaterialsProcessedDisposed1 { get; set; }
        public string? NextTierVendorNameAddress1 { get; set; }
        public string? MaterialGenerated2 { get; set; }
        public string? HowmaterialsProcessedDisposed2 { get; set; }
        public string? NextTierVendorNameAddress2 { get; set; }
        public string? MaterialGenerated3 { get; set; }
        public string? HowmaterialsProcessedDisposed3 { get; set; }
        public string? NextTierVendorNameAddress3 { get; set; }
        public string? MaterialGenerated4 { get; set; }
        public string? HowmaterialsProcessedDisposed4 { get; set; }
        public string? NextTierVendorNameAddress4 { get; set; }
        public string? MaterialGenerated5 { get; set; }
        public string? HowmaterialsProcessedDisposed5 { get; set; }
        public string? NextTierVendorNameAddress5 { get; set; }
        public string? MaterialGenerated6 { get; set; }
        public string? HowmaterialsProcessedDisposed6 { get; set; }
        public string? NextTierVendorNameAddress6 { get; set; }
        public string? DescribeTransportationIncomingOutgoingMaterials { get; set; }
        public string? DescribeAuditingProcessThirdPartyTransporters { get; set; }
        public bool OccupationalHealthSafetyManagementSystem { get; set; }
        public string? DocumentRequestOccupationalHealthSafetyManagementSystem { get; set; }
        public bool FacilityDocumentedHealthSafety { get; set; }
        public bool FacilityAnnualHealthSafetyTraining { get; set; }
        public bool HealthSafetyViolations { get; set; }
        public string? HealthSafetyViolationsxplanation { get; set; }
        public string? EHSManager { get; set; }
        public string? ComplianceManager { get; set; }
        public string? OHSMManager { get; set; }
        public string? FacilityManager { get; set; }
        public string? ManagementRepresentativeName { get; set; }
        public string? ManagementRepresentativeTitle { get; set; }
    }
}
