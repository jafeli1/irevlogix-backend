using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.DTOs.Reports;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(ApplicationDbContext context, ILogger<ReportsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetClientId()
        {
            return User.FindFirst("ClientId")?.Value ?? throw new UnauthorizedAccessException("ClientId not found in token");
        }

        private bool IsAdministrator()
        {
            return User.IsInRole("Administrator");
        }

        private bool IsAdministrator()
        {
            return User.IsInRole("Administrator");
        }

        [HttpGet("esg-summary")]
        public async Task<ActionResult<EsgSummaryDto>> GetEsgSummary([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            try
            {
                var clientId = GetClientId();
                var lots = _context.ProcessingLots.AsQueryable();
                
                if (!IsAdministrator())
                {
                    lots = lots.Where(pl => pl.ClientId == clientId);
                }

                if (from.HasValue) lots = lots.Where(pl => pl.StartDate >= from.Value);
                if (to.HasValue) lots = lots.Where(pl => pl.StartDate <= to.Value);

                var data = await lots.Select(pl => new
                {
                    Incoming = pl.TotalIncomingWeight ?? 0,
                    Processed = pl.TotalProcessedWeight ?? 0
                }).ToListAsync();

                var totalIncoming = data.Sum(x => x.Incoming);
                var totalProcessed = data.Sum(x => x.Processed);
                var diversionRate = totalIncoming > 0 ? totalProcessed / totalIncoming : 0m;

                var factors = new EsgFactorsDto
                {
                    Co2ePerLb = 0.6m,
                    WaterGalPerLb = 3.2m,
                    EnergyKwhPerLb = 0.8m
                };

                var dto = new EsgSummaryDto
                {
                    TotalIncomingWeight = totalIncoming,
                    TotalProcessedWeight = totalProcessed,
                    DiversionRate = diversionRate,
                    Co2eSavedLbs = totalProcessed * factors.Co2ePerLb,
                    WaterSavedGallons = totalProcessed * factors.WaterGalPerLb,
                    EnergySavedKwh = totalProcessed * factors.EnergyKwhPerLb,
                    Factors = factors
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error computing ESG summary");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("financial-summary")]
        public async Task<ActionResult<FinancialSummaryDto>> GetFinancialSummary([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            try
            {
                var clientId = GetClientId();
                var lots = _context.ProcessingLots.AsQueryable();
                
                if (!IsAdministrator())
                {
                    lots = lots.Where(pl => pl.ClientId == clientId);
                }

                if (from.HasValue) lots = lots.Where(pl => pl.StartDate >= from.Value);
                if (to.HasValue) lots = lots.Where(pl => pl.StartDate <= to.Value);

                var data = await lots.Select(pl => new
                {
                    ActualRevenue = pl.ActualRevenue ?? 0,
                    ExpectedRevenue = pl.ExpectedRevenue ?? 0,
                    ProcessingCost = pl.ProcessingCost ?? 0,
                    IncomingMaterialCost = pl.IncomingMaterialCost ?? 0
                }).ToListAsync();

                var totalActualRevenue = data.Sum(x => x.ActualRevenue);
                var totalExpectedRevenue = data.Sum(x => x.ExpectedRevenue);
                var totalProcessingCost = data.Sum(x => x.ProcessingCost);
                var totalIncomingCost = data.Sum(x => x.IncomingMaterialCost);
                var netProfit = totalActualRevenue - (totalProcessingCost + totalIncomingCost);

                var dto = new FinancialSummaryDto
                {
                    TotalActualRevenue = totalActualRevenue,
                    TotalExpectedRevenue = totalExpectedRevenue,
                    TotalProcessingCost = totalProcessingCost,
                    TotalIncomingMaterialCost = totalIncomingCost,
                    NetProfit = netProfit
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error computing financial summary");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("compliance-summary")]
        public async Task<ActionResult<ComplianceSummaryDto>> GetComplianceSummary([FromQuery] DateTime? asOf = null)
        {
            try
            {
                var clientId = GetClientId();
                var now = asOf ?? DateTime.UtcNow;

                var lots = _context.ProcessingLots.AsQueryable();
                
                if (!IsAdministrator())
                {
                    lots = lots.Where(pl => pl.ClientId == clientId);
                }

                var list = await lots.Select(pl => new
                {
                    pl.CertificationStatus,
                    pl.CompletionDate
                }).ToListAsync();

                int total = list.Count;
                int overdue = list.Count(x =>
                    !string.Equals(x.CertificationStatus ?? "", "Completed", StringComparison.OrdinalIgnoreCase)
                    && x.CompletionDate.HasValue
                    && x.CompletionDate.Value < now
                );
                int pending = list.Count(x =>
                    !string.Equals(x.CertificationStatus ?? "", "Completed", StringComparison.OrdinalIgnoreCase)
                );

                var dto = new ComplianceSummaryDto
                {
                    TotalLots = total,
                    OverdueCertifications = overdue,
                    PendingCertifications = pending
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error computing compliance summary");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("drilldown")]
        public async Task<ActionResult<DrilldownResponseDto>> GetDrilldown(
            [FromQuery] string type = "processinglots",
            [FromQuery] string? status = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clientId = GetClientId();
                page = page < 1 ? 1 : page;
                pageSize = pageSize is < 1 or > 100 ? 10 : pageSize;

                if (string.Equals(type, "processedmaterials", StringComparison.OrdinalIgnoreCase))
                {
                    var q = _context.ProcessedMaterials
                        .Include(pm => pm.MaterialType)
                        .AsQueryable();
                    
                    if (!IsAdministrator())
                    {
                        q = q.Where(pm => pm.ClientId == clientId);
                    }

                    if (!string.IsNullOrWhiteSpace(status)) q = q.Where(pm => pm.Status == status);
                    if (from.HasValue) q = q.Where(pm => pm.DateCreated >= from.Value);
                    if (to.HasValue) q = q.Where(pm => pm.DateCreated <= to.Value);

                    var total = await q.CountAsync();
                    var items = await q
                        .OrderByDescending(pm => pm.DateCreated)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(pm => new DrilldownItemDto
                        {
                            RecordType = "ProcessedMaterial",
                            Id = pm.Id,
                            NameOrType = pm.MaterialType != null ? pm.MaterialType.Name : pm.Description,
                            Date = pm.DateCreated,
                            WeightLbs = pm.ProcessedWeight,
                            Status = pm.Status
                        }).ToListAsync();

                    return Ok(new DrilldownResponseDto { TotalCount = total, Page = page, PageSize = pageSize, Items = items });
                }
                else
                {
                    var q = _context.ProcessingLots
                        .AsQueryable();
                    
                    if (!IsAdministrator())
                    {
                        q = q.Where(pl => pl.ClientId == clientId);
                    }

                    if (!string.IsNullOrWhiteSpace(status)) q = q.Where(pl => pl.Status == status);
                    if (from.HasValue) q = q.Where(pl => pl.StartDate >= from.Value);
                    if (to.HasValue) q = q.Where(pl => pl.StartDate <= to.Value);

                    var total = await q.CountAsync();
                    var items = await q
                        .OrderByDescending(pl => pl.DateCreated)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(pl => new DrilldownItemDto
                        {
                            RecordType = "ProcessingLot",
                            Id = pl.Id,
                            NameOrType = pl.LotNumber,
                            Date = pl.StartDate ?? pl.DateCreated,
                            WeightLbs = pl.TotalProcessedWeight,
                            Status = pl.Status
                        }).ToListAsync();

                    return Ok(new DrilldownResponseDto { TotalCount = total, Page = page, PageSize = pageSize, Items = items });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting drilldown items");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
