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
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ApplicationDbContext context, ILogger<DashboardController> logger)
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

        [HttpGet("summary-metrics")]
        public async Task<ActionResult<SummaryMetricsDto>> GetSummaryMetrics([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            try
            {
                var clientId = GetClientId();
                
                var shipmentsQuery = _context.Shipments.AsQueryable();
                var lotsQuery = _context.ProcessingLots.AsQueryable();
                var assetsQuery = _context.Assets.AsQueryable();
                
                if (!IsAdministrator())
                {
                    shipmentsQuery = shipmentsQuery.Where(s => s.ClientId == clientId);
                    lotsQuery = lotsQuery.Where(pl => pl.ClientId == clientId);
                    assetsQuery = assetsQuery.Where(a => a.ClientId == clientId);
                }

                if (from.HasValue)
                {
                    shipmentsQuery = shipmentsQuery.Where(s => s.DateCreated >= from.Value);
                    lotsQuery = lotsQuery.Where(pl => pl.DateCreated >= from.Value);
                    assetsQuery = assetsQuery.Where(a => a.DateCreated >= from.Value);
                }
                if (to.HasValue)
                {
                    shipmentsQuery = shipmentsQuery.Where(s => s.DateCreated <= to.Value);
                    lotsQuery = lotsQuery.Where(pl => pl.DateCreated <= to.Value);
                    assetsQuery = assetsQuery.Where(a => a.DateCreated <= to.Value);
                }

                var activeShipments = await shipmentsQuery.CountAsync(s => s.Status != "Completed" && s.Status != "Delivered");
                var processingLots = await lotsQuery.CountAsync(pl => pl.Status == "In Progress" || pl.Status == "Processing");
                var totalShipments = await shipmentsQuery.CountAsync();
                var totalAssetsProcessed = await assetsQuery.CountAsync();
                
                var monthlyRevenue = await lotsQuery
                    .Where(pl => pl.DateCreated >= DateTime.UtcNow.AddDays(-30))
                    .SumAsync(pl => pl.ActualRevenue ?? pl.ExpectedRevenue ?? 0);
                
                var totalRevenue = await lotsQuery.SumAsync(pl => pl.ActualRevenue ?? pl.ExpectedRevenue ?? 0);

                return Ok(new SummaryMetricsDto
                {
                    ActiveShipments = activeShipments,
                    ProcessingLots = processingLots,
                    MonthlyRevenue = monthlyRevenue,
                    TotalShipments = totalShipments,
                    TotalAssetsProcessed = totalAssetsProcessed,
                    TotalRevenue = totalRevenue
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting summary metrics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("cost-revenue-data")]
        public async Task<ActionResult<List<CostRevenueDataDto>>> GetCostRevenueData([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            try
            {
                var clientId = GetClientId();
                var lotsQuery = _context.ProcessingLots.AsQueryable();
                
                if (!IsAdministrator())
                {
                    lotsQuery = lotsQuery.Where(pl => pl.ClientId == clientId);
                }

                if (from.HasValue)
                {
                    lotsQuery = lotsQuery.Where(pl => pl.DateCreated >= from.Value);
                }
                if (to.HasValue)
                {
                    lotsQuery = lotsQuery.Where(pl => pl.DateCreated <= to.Value);
                }

                var allLots = await lotsQuery.ToListAsync();
                var monthlyData = allLots
                    .GroupBy(pl => new { Year = pl.DateCreated.Year, Month = pl.DateCreated.Month })
                    .Select(g => new CostRevenueDataDto
                    {
                        Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Cost = g.Sum(pl => (pl.ProcessingCost ?? 0) + (pl.IncomingMaterialCost ?? 0)),
                        Revenue = g.Sum(pl => pl.ActualRevenue ?? pl.ExpectedRevenue ?? 0)
                    })
                    .OrderBy(x => x.Period)
                    .ToList();

                return Ok(monthlyData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cost revenue data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("vendor-performance")]
        public async Task<ActionResult<List<VendorPerformanceDto>>> GetVendorPerformance([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            try
            {
                var clientId = GetClientId();
                var salesQuery = _context.ProcessedMaterialSales
                    .Include(pms => pms.Vendor)
                    .Include(pms => pms.ProcessedMaterial)
                    .ThenInclude(pm => pm.ProcessingLot)
                    .AsQueryable();
                
                if (!IsAdministrator())
                {
                    salesQuery = salesQuery.Where(pms => pms.ClientId == clientId);
                }

                if (from.HasValue)
                {
                    salesQuery = salesQuery.Where(pms => pms.DateCreated >= from.Value);
                }
                if (to.HasValue)
                {
                    salesQuery = salesQuery.Where(pms => pms.DateCreated <= to.Value);
                }

                var allSales = await salesQuery.ToListAsync();
                
                var vendorPerformance = allSales
                    .GroupBy(pms => new { pms.VendorId, VendorName = pms.Vendor?.VendorName ?? "Unknown" })
                    .Select(g => new VendorPerformanceDto
                    {
                        VendorId = g.Key.VendorId,
                        VendorName = g.Key.VendorName,
                        Revenue = g.Sum(pms => pms.InvoiceTotal ?? 0),
                        ProcessingLots = g.Select(pms => pms.ProcessedMaterial?.ProcessingLotId).Distinct().Count()
                    })
                    .OrderByDescending(v => v.Revenue)
                    .Take(5)
                    .ToList();

                return Ok(vendorPerformance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vendor performance data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("processing-time-data")]
        public async Task<ActionResult<List<ProcessingTimeDataDto>>> GetProcessingTimeData([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            try
            {
                var clientId = GetClientId();
                var lotsQuery = _context.ProcessingLots.AsQueryable();
                
                if (!IsAdministrator())
                {
                    lotsQuery = lotsQuery.Where(pl => pl.ClientId == clientId);
                }

                if (from.HasValue)
                {
                    lotsQuery = lotsQuery.Where(pl => pl.DateCreated >= from.Value);
                }
                if (to.HasValue)
                {
                    lotsQuery = lotsQuery.Where(pl => pl.DateCreated <= to.Value);
                }

                var completedLots = await lotsQuery
                    .Where(pl => pl.StartDate.HasValue && pl.CompletionDate.HasValue)
                    .ToListAsync();

                var processingTimeData = new List<ProcessingTimeDataDto>();
                
                var shortLots = completedLots.Where(pl => (pl.CompletionDate!.Value - pl.StartDate!.Value).TotalDays <= 7).ToList();
                var mediumLots = completedLots.Where(pl => (pl.CompletionDate!.Value - pl.StartDate!.Value).TotalDays > 7 && (pl.CompletionDate!.Value - pl.StartDate!.Value).TotalDays <= 14).ToList();
                var longLots = completedLots.Where(pl => (pl.CompletionDate!.Value - pl.StartDate!.Value).TotalDays > 14).ToList();

                processingTimeData.Add(new ProcessingTimeDataDto
                {
                    Stage = "0-7 days",
                    Count = shortLots.Count,
                    AverageTime = shortLots.Any() ? (decimal)shortLots.Average(pl => (pl.CompletionDate!.Value - pl.StartDate!.Value).TotalDays) : 0
                });

                processingTimeData.Add(new ProcessingTimeDataDto
                {
                    Stage = "8-14 days",
                    Count = mediumLots.Count,
                    AverageTime = mediumLots.Any() ? (decimal)mediumLots.Average(pl => (pl.CompletionDate!.Value - pl.StartDate!.Value).TotalDays) : 0
                });

                processingTimeData.Add(new ProcessingTimeDataDto
                {
                    Stage = "15+ days",
                    Count = longLots.Count,
                    AverageTime = longLots.Any() ? (decimal)longLots.Average(pl => (pl.CompletionDate!.Value - pl.StartDate!.Value).TotalDays) : 0
                });

                return Ok(processingTimeData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting processing time data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("inbound-volume-data")]
        public async Task<ActionResult<List<InboundVolumeDataDto>>> GetInboundVolumeData([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            try
            {
                var clientId = GetClientId();
                var itemsQuery = _context.ShipmentItems
                    .Include(si => si.MaterialType)
                    .AsQueryable();
                
                if (!IsAdministrator())
                {
                    itemsQuery = itemsQuery.Where(si => si.ClientId == clientId);
                }

                if (from.HasValue)
                {
                    itemsQuery = itemsQuery.Where(si => si.DateCreated >= from.Value);
                }
                if (to.HasValue)
                {
                    itemsQuery = itemsQuery.Where(si => si.DateCreated <= to.Value);
                }

                var allItems = await itemsQuery.ToListAsync();
                var volumeData = allItems
                    .GroupBy(si => si.MaterialType != null ? si.MaterialType.Name : "Unknown")
                    .Select(g => new InboundVolumeDataDto
                    {
                        MaterialType = g.Key,
                        Weight = g.Sum(si => si.Weight ?? 0),
                        Count = g.Count()
                    })
                    .OrderByDescending(v => v.Weight)
                    .ToList();

                return Ok(volumeData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inbound volume data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("waste-diversion-data")]
        public async Task<ActionResult<WasteDiversionDataDto>> GetWasteDiversionData([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            try
            {
                var clientId = GetClientId();
                var lotsQuery = _context.ProcessingLots.AsQueryable();
                
                if (!IsAdministrator())
                {
                    lotsQuery = lotsQuery.Where(pl => pl.ClientId == clientId);
                }

                if (from.HasValue)
                {
                    lotsQuery = lotsQuery.Where(pl => pl.DateCreated >= from.Value);
                }
                if (to.HasValue)
                {
                    lotsQuery = lotsQuery.Where(pl => pl.DateCreated <= to.Value);
                }

                var totalProcessed = await lotsQuery.SumAsync(pl => pl.TotalProcessedWeight ?? pl.TotalIncomingWeight ?? 0);
                var totalDiverted = totalProcessed;
                var diversionRate = totalProcessed > 0 ? totalDiverted / totalProcessed : 0;

                return Ok(new WasteDiversionDataDto
                {
                    DiversionRate = diversionRate,
                    TotalProcessed = totalProcessed,
                    TotalDiverted = totalDiverted
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting waste diversion data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("certificates-issued-data")]
        public async Task<ActionResult<List<CertificatesIssuedDataDto>>> GetCertificatesIssuedData([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            try
            {
                var clientId = GetClientId();
                var lotsQuery = _context.ProcessingLots.AsQueryable();
                
                if (!IsAdministrator())
                {
                    lotsQuery = lotsQuery.Where(pl => pl.ClientId == clientId);
                }

                if (from.HasValue)
                {
                    lotsQuery = lotsQuery.Where(pl => pl.DateCreated >= from.Value);
                }
                if (to.HasValue)
                {
                    lotsQuery = lotsQuery.Where(pl => pl.DateCreated <= to.Value);
                }

                var completedLots = await lotsQuery
                    .Where(pl => pl.CertificationStatus == "Completed" && pl.CompletionDate.HasValue)
                    .ToListAsync();

                var certificatesData = completedLots
                    .GroupBy(pl => pl.CompletionDate!.Value.Date)
                    .Select(g => new CertificatesIssuedDataDto
                    {
                        Date = g.Key.ToString("yyyy-MM-dd"),
                        Count = g.Count()
                    })
                    .OrderBy(c => c.Date)
                    .ToList();

                return Ok(certificatesData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting certificates issued data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("asset-disposition-data")]
        public async Task<ActionResult<List<AssetDispositionDataDto>>> GetAssetDispositionData([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            try
            {
                var clientId = GetClientId();
                var assetsQuery = _context.Assets.AsQueryable();
                
                if (!IsAdministrator())
                {
                    assetsQuery = assetsQuery.Where(a => a.ClientId == clientId);
                }

                if (from.HasValue)
                {
                    assetsQuery = assetsQuery.Where(a => a.DateCreated >= from.Value);
                }
                if (to.HasValue)
                {
                    assetsQuery = assetsQuery.Where(a => a.DateCreated <= to.Value);
                }

                var allAssets = await assetsQuery.ToListAsync();
                var totalAssets = allAssets.Count;
                
                var dispositionGroups = allAssets
                    .GroupBy(a => a.ReuseDisposition ? "Reuse" : a.ResaleDisposition ? "Resale" : "Recycling")
                    .ToList();

                var dispositionData = dispositionGroups
                    .Select(g => new AssetDispositionDataDto
                    {
                        DispositionType = g.Key,
                        Count = g.Count(),
                        Percentage = totalAssets > 0 ? (decimal)g.Count() / totalAssets * 100 : 0
                    })
                    .ToList();

                return Ok(dispositionData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting asset disposition data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("monthly-revenue-data")]
        public async Task<ActionResult<List<MonthlyRevenueDataDto>>> GetMonthlyRevenueData([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            try
            {
                var clientId = GetClientId();
                var lotsQuery = _context.ProcessingLots.AsQueryable();
                
                if (!IsAdministrator())
                {
                    lotsQuery = lotsQuery.Where(pl => pl.ClientId == clientId);
                }

                if (from.HasValue)
                {
                    lotsQuery = lotsQuery.Where(pl => pl.DateCreated >= from.Value);
                }
                if (to.HasValue)
                {
                    lotsQuery = lotsQuery.Where(pl => pl.DateCreated <= to.Value);
                }

                var allLots = await lotsQuery.ToListAsync();
                var monthlyRevenue = allLots
                    .GroupBy(pl => new { Year = pl.DateCreated.Year, Month = pl.DateCreated.Month })
                    .Select(g => new MonthlyRevenueDataDto
                    {
                        Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Revenue = g.Sum(pl => pl.ActualRevenue ?? pl.ExpectedRevenue ?? 0)
                    })
                    .OrderBy(x => x.Month)
                    .ToList();

                return Ok(monthlyRevenue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monthly revenue data");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
