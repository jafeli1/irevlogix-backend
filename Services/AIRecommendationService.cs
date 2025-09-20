using irevlogix_backend.Data;
using irevlogix_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Globalization;

namespace irevlogix_backend.Services
{
    public partial class AIRecommendationService
    {
        private readonly OpenAIService _openAIService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AIRecommendationService> _logger;

        public AIRecommendationService(OpenAIService openAIService, ApplicationDbContext context, ILogger<AIRecommendationService> logger)
        {
            _openAIService = openAIService;
            _context = context;
            _logger = logger;
        }

        public async Task<MaterialClassificationSuggestion> GetMaterialClassificationAsync(string description, string clientId)
        {
            try
            {
                var materialTypes = await _context.MaterialTypes
                    .Where(mt => mt.ClientId == clientId)
                    .Select(mt => mt.Name)
                    .ToListAsync();

                var assetCategories = await _context.AssetCategories
                    .Where(ac => ac.ClientId == clientId)
                    .Select(ac => ac.Name)
                    .ToListAsync();

                var allCategories = materialTypes.Concat(assetCategories).ToList();

                var prompt = $@"Given the following description of a recycling item: '{description}'. 
From the following list of categories: [{string.Join(", ", allCategories)}], 
which are the top 3 most relevant categories? Provide only the category names in a JSON array. 
If none are highly relevant, return an empty array. 
Examples: 'old Dell laptop' -> ['Electronics', 'Laptop']; 'shredded paper waste' -> ['Paper']; 'copper wire scraps' -> ['Metals'].
Respond with valid JSON only.";

                var response = await _openAIService.GetStructuredResponseAsync<string[]>(prompt);
                
                return new MaterialClassificationSuggestion
                {
                    SuggestedCategories = response ?? new string[0],
                    Confidence = response?.Length > 0 ? 0.8 : 0.0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting material classification");
                return new MaterialClassificationSuggestion { SuggestedCategories = new string[0], Confidence = 0.0 };
            }
        }

        public async Task<DispositionRecommendation> GetOptimalDispositionAsync(Asset asset)
        {
            try
            {
                var prompt = $@"Given an asset with the following characteristics: 
Asset Category: {asset.AssetCategory?.Name ?? "Unknown"}, 
Manufacturer: {asset.Manufacturer ?? "Unknown"}, 
Model: {asset.Model ?? "Unknown"}, 
Condition: {asset.Condition}, 
Is Data Bearing: {(asset.IsDataBearing ? "Yes" : "No")}, 
Estimated Value: ${asset.EstimatedValue ?? 0}. 
Recommend the optimal disposition path (Reuse, Resale, or Certified Recycling) based on maximizing value and environmental responsibility. 
Provide a brief justification for the recommendation. Assume standard regulatory compliance for ITAD if data-bearing. 
Output as JSON: {{'recommended_disposition': 'Reuse/Resale/Certified Recycling', 'justification': '...'}}";

                var response = await _openAIService.GetStructuredResponseAsync<DispositionRecommendation>(prompt);
                
                return response ?? new DispositionRecommendation 
                { 
                    RecommendedDisposition = "Certified Recycling", 
                    Justification = "Unable to determine optimal disposition" 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting disposition recommendation");
                return new DispositionRecommendation 
                { 
                    RecommendedDisposition = "Certified Recycling", 
                    Justification = "Error occurred during analysis" 
                };
            }
        }

        public async Task<AssetCategorizationSuggestion> GetAssetCategorizationAsync(string description)
        {
            try
            {
                var prompt = $@"Given the following asset description: '{description}'. 
Suggest the most likely Asset Category, Manufacturer, and Model. 
Provide output as JSON: {{'suggested_category': '', 'suggested_manufacturer': '', 'suggested_model': ''}}";

                var response = await _openAIService.GetStructuredResponseAsync<AssetCategorizationSuggestion>(prompt);
                
                return response ?? new AssetCategorizationSuggestion();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting asset categorization");
                return new AssetCategorizationSuggestion();
            }
        }

        public async Task<QualityGradePrediction> GetQualityGradePredictionAsync(ProcessingLot lot)
        {
            try
            {
                var prompt = $@"Based on the following processing lot data:
Incoming Material Notes: {lot.IncomingMaterialNotes ?? "None"}
Contamination Percentage: {lot.ContaminationPercentage ?? 0}%
Total Incoming Weight: {lot.TotalIncomingWeight ?? 0} kg
Processing Steps: {lot.ProcessingSteps.Count} steps completed
Predict the quality grade (A, B, C, D) and provide reasoning.
Output as JSON: {{'predicted_grade': 'A/B/C/D', 'confidence': 0.0-1.0, 'reasoning': '...'}}";

                var response = await _openAIService.GetStructuredResponseAsync<QualityGradePrediction>(prompt);
                
                return response ?? new QualityGradePrediction 
                { 
                    PredictedGrade = "C", 
                    Confidence = 0.5, 
                    Reasoning = "Unable to analyze data" 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quality grade prediction");
                return new QualityGradePrediction 
                { 
                    PredictedGrade = "C", 
                    Confidence = 0.0, 
                    Reasoning = "Error occurred during analysis" 
                };
            }
        }

        public async Task<string> GetOptimalDispositionRecommendationAsync(string assetCategory, string condition, decimal estimatedValue, bool isDataBearing)
        {
            try
            {
                var prompt = $@"Given an asset with the following characteristics: 
Asset Category: {assetCategory}, 
Condition: {condition}, 
Is Data Bearing: {(isDataBearing ? "Yes" : "No")}, 
Estimated Value: ${estimatedValue}. 
Recommend the optimal disposition path (Reuse, Resale, or Certified Recycling) based on maximizing value and environmental responsibility. 
Provide a brief justification for the recommendation. Assume standard regulatory compliance for ITAD if data-bearing.";

                var response = await _openAIService.GetChatCompletionAsync(prompt);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting optimal disposition recommendation");
                return "Unable to determine optimal disposition. Consider certified recycling for safety.";
            }
        }

        public async Task<string> GetAutomatedAssetCategorizationAsync(string manufacturer, string model, string description)
        {
            try
            {
                var prompt = $@"Given the following asset information: 
Manufacturer: {manufacturer}
Model: {model}
Description: {description}
Suggest the most likely asset category (e.g., Laptop, Desktop, Server, Monitor, Printer, etc.).
Provide only the category name.";

                var response = await _openAIService.GetChatCompletionAsync(prompt);
                return response.Trim();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting automated asset categorization");
                return "Unknown";
            }
        }

        public async Task<ReturnsForecastResult> GetPredictiveReturnsForecastAsync(string clientId, string? materialType = null, int? originatorClientId = null, string aggregationPeriod = "weekly", int weeksAhead = 4)
        {
            try
            {
                var totalShipments = await _context.Shipments
                    .Where(s => s.ClientId == clientId && s.DateCreated >= DateTime.UtcNow.AddMonths(-24))
                    .CountAsync();

                if (totalShipments < 10)
                {
                    return new ReturnsForecastResult
                    {
                        HasSufficientData = false,
                        InsufficientDataMessage = "Insufficient data for computation. Please ensure the following tables have adequate data:\n" +
                            "• Shipments table: ScheduledPickupDate, ActualPickupDate, OriginatorClientId fields\n" +
                            "• ShipmentItems table: MaterialType, Quantity fields\n" +
                            "Minimum 10 shipments with 12-24 months of historical data required.",
                        RequiredTables = new[] { "Shipments", "ShipmentItems" },
                        RequiredFields = new[] { "ScheduledPickupDate", "ActualPickupDate", "MaterialType", "Quantity", "OriginatorClientId" }
                    };
                }

                var shipmentsQuery = _context.Shipments
                    .Include(s => s.ShipmentItems)
                    .ThenInclude(si => si.MaterialType)
                    .Where(s => s.ClientId == clientId && s.DateCreated >= DateTime.UtcNow.AddMonths(-24));

                if (originatorClientId.HasValue)
                    shipmentsQuery = shipmentsQuery.Where(s => s.OriginatorClientId == originatorClientId.Value);

                var shipments = await shipmentsQuery.ToListAsync();

                var filteredItems = shipments.SelectMany(s => s.ShipmentItems)
                    .Where(si => materialType == null || si.MaterialType?.Name == materialType);

                var aggregatedData = aggregationPeriod.ToLower() switch
                {
                    "weekly" => filteredItems
                        .GroupBy(si => new { 
                            Year = si.Shipment.DateCreated.Year,
                            Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(si.Shipment.DateCreated, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                        })
                        .Select(g => new { 
                            Period = $"{g.Key.Year}-W{g.Key.Week:D2}",
                            Volume = g.Sum(si => si.Quantity),
                            Date = g.Min(si => si.Shipment.DateCreated)
                        })
                        .OrderBy(x => x.Date)
                        .ToList(),
                    "monthly" => filteredItems
                        .GroupBy(si => new { si.Shipment.DateCreated.Year, si.Shipment.DateCreated.Month })
                        .Select(g => new { 
                            Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                            Volume = g.Sum(si => si.Quantity),
                            Date = new DateTime(g.Key.Year, g.Key.Month, 1)
                        })
                        .OrderBy(x => x.Date)
                        .ToList(),
                    _ => throw new ArgumentException("Invalid aggregation period. Use 'weekly' or 'monthly'.")
                };

                var historicalDataString = string.Join(", ", aggregatedData.Select(d => $"{d.Period}: {d.Volume}"));
                var materialTypeFilter = materialType ?? "all material types";
                var clientFilter = originatorClientId.HasValue ? $" from client {originatorClientId}" : "";

                var prompt = $@"You are an API that only returns valid JSON. Given the following {aggregationPeriod} return volumes for {materialTypeFilter}{clientFilter}: [{historicalDataString}],
predict the return volumes for the next {weeksAhead} {aggregationPeriod.TrimEnd('y')}s.
Requirements:
- Respond ONLY with a JSON object with a single key 'values' whose value is an array of {weeksAhead} integers.
- No prose, no backticks, no comments, no keys other than 'values'.
Example: {{""values"": [12, 14, 11, 13] }}";

                var response = await _openAIService.GetStructuredResponseAsync<IntArrayEnvelope>(prompt);
                var preds = response?.values ?? Array.Empty<int>();
                
                return new ReturnsForecastResult
                {
                    HasSufficientData = true,
                    HistoricalData = aggregatedData.Select(d => new ForecastDataPoint { Period = d.Period, Volume = d.Volume }).ToList(),
                    PredictedData = preds.Select((vol, idx) => new ForecastDataPoint 
                    { 
                        Period = GetFuturePeriod(aggregationPeriod, idx + 1), 
                        Volume = vol 
                    }).ToList(),
                    MaterialType = materialType,
                    OriginatorClientId = originatorClientId,
                    AggregationPeriod = aggregationPeriod,
                    GeneratedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting predictive returns forecast");
                return new ReturnsForecastResult
                {
                    HasSufficientData = false,
                    InsufficientDataMessage = "Error occurred during forecast generation. Please try again later."
                };
            }
        }

        private string GetFuturePeriod(string aggregationPeriod, int periodsAhead)
        {
            var now = DateTime.UtcNow;
            return aggregationPeriod.ToLower() switch
            {
                "weekly" => GetWeeklyPeriod(now.AddDays(7 * periodsAhead)),
                "monthly" => GetMonthlyPeriod(now.AddMonths(periodsAhead)),
                _ => throw new ArgumentException("Invalid aggregation period")
            };
        }

        private string GetWeeklyPeriod(DateTime date)
        {
            var week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return $"{date.Year}-W{week:D2}";
        }

        private string GetMonthlyPeriod(DateTime date)
        {
            return $"{date.Year}-{date.Month:D2}";
        }

        public async Task<string> GetMaterialContaminationAnalysisAsync(string materialDescription, double contaminationPercentage)
        {
            try
            {
                var prompt = $@"Analyze the contamination level for the following material: {materialDescription}
Contamination percentage: {contaminationPercentage}%
Provide recommendations for processing and quality improvement strategies.";

                var response = await _openAIService.GetChatCompletionAsync(prompt);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting material contamination analysis");
                return "Unable to analyze contamination at this time.";
            }
        }

        public async Task<ContaminationAnalysisResult> GetMaterialContaminationInsightsAsync(
            string clientId, string? materialType = null, int? originatorClientId = null, int periodWeeks = 4)
        {
            try
            {
                var cutoff = DateTime.UtcNow.AddDays(-7 * periodWeeks);

                var lotsQuery = _context.ProcessingLots
                    .Where(l => l.ClientId == clientId && l.DateCreated >= cutoff);

                var lots = await lotsQuery.ToListAsync();
                if (lots.Count == 0)
                {
                    return new ContaminationAnalysisResult
                    {
                        CommonContaminants = new List<string>(),
                        PreventativeMeasures = new List<string>(),
                        SupplierImprovements = new List<string>(),
                        UsedRows = new List<ContaminationSourceRow>(),
                        TotalRows = 0,
                        MaterialType = materialType,
                        OriginatorClientId = originatorClientId,
                        PeriodWeeks = periodWeeks,
                        GeneratedAt = DateTime.UtcNow
                    };
                }

                var shipmentIds = lots.Where(l => l.SourceShipmentId.HasValue).Select(l => l.SourceShipmentId!.Value).Distinct().ToList();

                var shipmentsQuery = _context.Shipments
                    .Include(s => s.ShipmentItems)
                        .ThenInclude(si => si.MaterialType)
                    .Where(s => s.ClientId == clientId && shipmentIds.Contains(s.Id));

                if (originatorClientId.HasValue)
                {
                    shipmentsQuery = shipmentsQuery.Where(s => s.OriginatorClientId == originatorClientId.Value);
                }

                var shipments = await shipmentsQuery.ToListAsync();
                var shipmentsById = shipments.ToDictionary(s => s.Id, s => s);

                if (originatorClientId.HasValue)
                {
                    lots = lots.Where(l => l.SourceShipmentId.HasValue && shipmentsById.ContainsKey(l.SourceShipmentId.Value)).ToList();
                }

                if (!string.IsNullOrWhiteSpace(materialType))
                {
                    lots = lots.Where(l =>
                    {
                        if (!l.SourceShipmentId.HasValue) return false;
                        if (!shipmentsById.TryGetValue(l.SourceShipmentId.Value, out var s)) return false;
                        return s.ShipmentItems.Any(si => si.MaterialType != null && si.MaterialType.Name == materialType);
                    }).ToList();
                }

                var totalRows = lots.Count;

                var rows = new List<ContaminationSourceRow>();
                foreach (var lot in lots)
                {
                    Shipment? s = null;
                    if (lot.SourceShipmentId.HasValue)
                    {
                        shipmentsById.TryGetValue(lot.SourceShipmentId.Value, out s);
                    }

                    string? mtName = null;
                    if (s != null)
                    {
                        mtName = s.ShipmentItems.FirstOrDefault(si => si.MaterialType != null)?.MaterialType?.Name;
                    }

                    rows.Add(new ContaminationSourceRow
                    {
                        ProcessingLotId = lot.Id,
                        MaterialType = mtName,
                        ContaminationPercentage = lot.ContaminationPercentage,
                        IncomingMaterialNotes = lot.IncomingMaterialNotes,
                        ActualReceivedWeight = lot.TotalIncomingWeight ?? 0,
                        OriginatorClientId = s?.OriginatorClientId,
                        OriginatorName = null,
                        ShipmentId = s?.Id,
                        DateCreated = lot.DateCreated
                    });
                }

                var usedRows = rows.Take(50).ToList();

                var summaries = usedRows.Select(r =>
                    $"Material: {(string.IsNullOrWhiteSpace(r.MaterialType) ? "Unknown" : r.MaterialType)}; " +
                    $"Contamination: {(r.ContaminationPercentage ?? 0):0.##}%; " +
                    $"Notes: {(string.IsNullOrWhiteSpace(r.IncomingMaterialNotes) ? "None" : r.IncomingMaterialNotes)}; " +
                    $"Weight: {(r.ActualReceivedWeight ?? 0):0.##} kg"
                ).ToList();

                var description = string.Join(" | ", summaries);

                var mtFilter = string.IsNullOrWhiteSpace(materialType) ? "all material types" : materialType!;
                var weeks = periodWeeks;

                var prompt = $@"You are an API that only returns valid JSON.
Given the following material details for recycling lots over the last {weeks} weeks for {mtFilter}:
{description}
Analyze this information to identify common contaminants and suggest specific preventative measures or improvements to discuss with suppliers.
Respond ONLY with a JSON object that exactly matches:
{{""common_contaminants"": [], ""preventative_measures"": [], ""supplier_improvements"": []}}";

                var ai = await _openAIService.GetStructuredResponseAsync<ContaminationAIResponse>(prompt);
                var result = new ContaminationAnalysisResult
                {
                    CommonContaminants = ai?.common_contaminants ?? new List<string>(),
                    PreventativeMeasures = ai?.preventative_measures ?? new List<string>(),
                    SupplierImprovements = ai?.supplier_improvements ?? new List<string>(),
                    UsedRows = usedRows,
                    TotalRows = totalRows,
                    MaterialType = materialType,
                    OriginatorClientId = originatorClientId,
                    PeriodWeeks = periodWeeks,
                    GeneratedAt = DateTime.UtcNow
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating material contamination analysis");
                return new ContaminationAnalysisResult
                {
                    CommonContaminants = new List<string>(),
                    PreventativeMeasures = new List<string>(),
                    SupplierImprovements = new List<string>(),
                    UsedRows = new List<ContaminationSourceRow>(),
                    TotalRows = 0,
                    MaterialType = materialType,
                    OriginatorClientId = originatorClientId,
                    PeriodWeeks = periodWeeks,
                    GeneratedAt = DateTime.UtcNow
                };
            }
        }

        public async Task<string> GetESGImpactForecastAsync(string clientId)
        {
            try
            {
                var totalAssets = await _context.Assets
                    .Where(a => a.ClientId == clientId)
                    .CountAsync();

                var totalWeight = await _context.ProcessingLots
                    .Where(p => p.ClientId == clientId)
                    .SumAsync(p => p.TotalIncomingWeight ?? 0);

                var prompt = $@"Based on the following recycling data:
Total assets processed: {totalAssets}
Total material weight processed: {totalWeight} kg
Provide an ESG (Environmental, Social, Governance) impact forecast including carbon footprint reduction, 
waste diversion from landfills, and social impact metrics.";

                var response = await _openAIService.GetChatCompletionAsync(prompt);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ESG impact forecast");
                return "Unable to generate ESG forecast at this time.";
            }
        }
    }

    public class MaterialClassificationSuggestion
    {
        public string[] SuggestedCategories { get; set; } = new string[0];
        public double Confidence { get; set; }
    }

    public class DispositionRecommendation
    {
        public string RecommendedDisposition { get; set; } = string.Empty;
        public string Justification { get; set; } = string.Empty;
    }

    public class AssetCategorizationSuggestion
    {
        public string SuggestedCategory { get; set; } = string.Empty;
        public string SuggestedManufacturer { get; set; } = string.Empty;
        public string SuggestedModel { get; set; } = string.Empty;
    }

    public class QualityGradePrediction
    {
        public string PredictedGrade { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string Reasoning { get; set; } = string.Empty;
    }

    public class ReturnsForecastResult
    {
        public bool HasSufficientData { get; set; }
        public string? InsufficientDataMessage { get; set; }
        public string[]? RequiredTables { get; set; }
        public string[]? RequiredFields { get; set; }
        public List<ForecastDataPoint> HistoricalData { get; set; } = new();
        public List<ForecastDataPoint> PredictedData { get; set; } = new();
        public string? MaterialType { get; set; }
        public int? OriginatorClientId { get; set; }
        public string AggregationPeriod { get; set; } = "weekly";
        public DateTime GeneratedAt { get; set; }
    }

    public class ForecastDataPoint
    {
        public string Period { get; set; } = string.Empty;
        public int Volume { get; set; }
    }

    public class ContaminationAIResponse
    {
        public List<string> common_contaminants { get; set; } = new();
        public List<string> preventative_measures { get; set; } = new();
        public List<string> supplier_improvements { get; set; } = new();
    }

    public class ContaminationSourceRow
    {
        public int ProcessingLotId { get; set; }
        public string? MaterialType { get; set; }
        public double? ContaminationPercentage { get; set; }
        public string? IncomingMaterialNotes { get; set; }
        public double? ActualReceivedWeight { get; set; }
        public int? OriginatorClientId { get; set; }
        public string? OriginatorName { get; set; }
        public int? ShipmentId { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class ContaminationAnalysisResult
    {
        public List<string> CommonContaminants { get; set; } = new();
        public List<string> PreventativeMeasures { get; set; } = new();
        public List<string> SupplierImprovements { get; set; } = new();
        public List<ContaminationSourceRow> UsedRows { get; set; } = new();
        public int TotalRows { get; set; }
        public string? MaterialType { get; set; }
        public int? OriginatorClientId { get; set; }
        public int PeriodWeeks { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }


 
    public class IntArrayEnvelope
    {
        public int[]? values { get; set; }
    }
}
