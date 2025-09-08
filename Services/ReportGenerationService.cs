using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Models;
using System.Text.Json;
using System.Text;
using OfficeOpenXml;

namespace irevlogix_backend.Services
{
    public class ReportGenerationService : IReportGenerationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReportGenerationService> _logger;

        public ReportGenerationService(ApplicationDbContext context, ILogger<ReportGenerationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<byte[]> GenerateExcelReportAsync(string dataSource, string[] selectedColumns, object? filters, object? sorting, string clientId)
        {
            try
            {
                var data = await GetDataForReportAsync(dataSource, filters, sorting, clientId);
                return GenerateExcelFile(data, selectedColumns, dataSource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Excel report for data source {DataSource}", dataSource);
                throw;
            }
        }

        private async Task<List<Dictionary<string, object>>> GetDataForReportAsync(string dataSource, object? filters, object? sorting, string clientId)
        {
            return dataSource.ToLower() switch
            {
                "assets" => await GetAssetsDataAsync(clientId),
                "shipments" => await GetShipmentsDataAsync(clientId),
                "processinglots" => await GetProcessingLotsDataAsync(clientId),
                "processedmaterials" => await GetProcessedMaterialsDataAsync(clientId),
                "vendors" => await GetVendorsDataAsync(clientId),
                _ => new List<Dictionary<string, object>>()
            };
        }

        private async Task<List<Dictionary<string, object>>> GetAssetsDataAsync(string clientId)
        {
            var assets = await _context.Assets
                .Where(a => a.ClientId == clientId)
                .ToListAsync();

            return assets.Select(a => new Dictionary<string, object>
            {
                ["AssetID"] = a.AssetID ?? "",
                ["Manufacturer"] = a.Manufacturer ?? "",
                ["Model"] = a.Model ?? "",
                ["SerialNumber"] = a.SerialNumber ?? "",
                ["Condition"] = a.Condition ?? "",
                ["IsDataBearing"] = a.IsDataBearing,
                ["CurrentLocation"] = a.CurrentLocation ?? "",
                ["EstimatedValue"] = a.EstimatedValue ?? 0,
                ["ActualValue"] = a.ActualValue ?? 0,
                ["DateCreated"] = a.DateCreated
            }).ToList();
        }

        private async Task<List<Dictionary<string, object>>> GetShipmentsDataAsync(string clientId)
        {
            var shipments = await _context.Shipments
                .Where(s => s.ClientId == clientId)
                .ToListAsync();

            return shipments.Select(s => new Dictionary<string, object>
            {
                ["ShipmentNumber"] = s.ShipmentNumber ?? "",
                ["Status"] = s.Status ?? "",
                ["TrackingNumber"] = s.TrackingNumber ?? "",
                ["Carrier"] = s.Carrier ?? "",
                ["Weight"] = s.Weight ?? 0,
                ["WeightUnit"] = s.WeightUnit ?? "",
                ["NumberOfBoxes"] = s.NumberOfBoxes ?? 0,
                ["ScheduledPickupDate"] = s.ScheduledPickupDate,
                ["ActualPickupDate"] = s.ActualPickupDate,
                ["EstimatedValue"] = s.EstimatedValue ?? 0,
                ["ActualValue"] = s.ActualValue ?? 0,
                ["DateCreated"] = s.DateCreated
            }).ToList();
        }

        private async Task<List<Dictionary<string, object>>> GetProcessingLotsDataAsync(string clientId)
        {
            var lots = await _context.ProcessingLots
                .Where(pl => pl.ClientId == clientId)
                .ToListAsync();

            return lots.Select(pl => new Dictionary<string, object>
            {
                ["LotNumber"] = pl.LotNumber ?? "",
                ["Status"] = pl.Status ?? "",
                ["Description"] = pl.Description ?? "",
                ["StartDate"] = pl.StartDate,
                ["CompletionDate"] = pl.CompletionDate,
                ["TotalIncomingWeight"] = pl.TotalIncomingWeight ?? 0,
                ["TotalProcessedWeight"] = pl.TotalProcessedWeight ?? 0,
                ["ProcessingCost"] = pl.ProcessingCost ?? 0,
                ["ExpectedRevenue"] = pl.ExpectedRevenue ?? 0,
                ["ActualRevenue"] = pl.ActualRevenue ?? 0,
                ["NetProfit"] = pl.NetProfit ?? 0,
                ["DateCreated"] = pl.DateCreated
            }).ToList();
        }

        private async Task<List<Dictionary<string, object>>> GetProcessedMaterialsDataAsync(string clientId)
        {
            var materials = await _context.ProcessedMaterials
                .Where(pm => pm.ClientId == clientId)
                .ToListAsync();

            return materials.Select(pm => new Dictionary<string, object>
            {
                ["Description"] = pm.Description ?? "",
                ["Quantity"] = pm.Quantity ?? 0,
                ["UnitOfMeasure"] = pm.UnitOfMeasure ?? "",
                ["QualityGrade"] = pm.QualityGrade ?? "",
                ["Location"] = pm.Location ?? "",
                ["ProcessedWeight"] = pm.ProcessedWeight ?? 0,
                ["Status"] = pm.Status ?? "",
                ["ExpectedSalesPrice"] = pm.ExpectedSalesPrice ?? 0,
                ["ActualSalesPrice"] = pm.ActualSalesPrice ?? 0,
                ["SaleDate"] = pm.SaleDate,
                ["DateCreated"] = pm.DateCreated
            }).ToList();
        }

        private async Task<List<Dictionary<string, object>>> GetVendorsDataAsync(string clientId)
        {
            var vendors = await _context.Vendors
                .Where(v => v.ClientId == clientId)
                .ToListAsync();

            return vendors.Select(v => new Dictionary<string, object>
            {
                ["VendorName"] = v.VendorName ?? "",
                ["ContactPerson"] = v.ContactPerson ?? "",
                ["Email"] = v.Email ?? "",
                ["Phone"] = v.Phone ?? "",
                ["Address"] = v.Address ?? "",
                ["City"] = v.City ?? "",
                ["State"] = v.State ?? "",
                ["Country"] = v.Country ?? "",
                ["MaterialsOfInterest"] = v.MaterialsOfInterest ?? "",
                ["VendorRating"] = v.VendorRating ?? 0,
                ["VendorTier"] = v.VendorTier ?? "",
                ["DateCreated"] = v.DateCreated
            }).ToList();
        }

        private byte[] GenerateExcelFile(List<Dictionary<string, object>> data, string[] selectedColumns, string dataSource)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add($"{dataSource} Report");

            var headerRow = 4;
            for (int i = 0; i < selectedColumns.Length; i++)
            {
                worksheet.Cells[headerRow, i + 1].Value = selectedColumns[i];
                worksheet.Cells[headerRow, i + 1].Style.Font.Bold = true;
            }

            worksheet.Cells[1, 1].Value = $"Recycling Lifecycle Report - {dataSource}";
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.Font.Size = 16;

            worksheet.Cells[2, 1].Value = $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC";
            worksheet.Cells[3, 1].Value = $"Total Records: {data.Count}";

            for (int row = 0; row < data.Count; row++)
            {
                var item = data[row];
                for (int col = 0; col < selectedColumns.Length; col++)
                {
                    var columnKey = selectedColumns[col];
                    var value = item.ContainsKey(columnKey) ? item[columnKey] : "";
                    worksheet.Cells[headerRow + 1 + row, col + 1].Value = value?.ToString() ?? "";
                }
            }

            worksheet.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }
    }
}
