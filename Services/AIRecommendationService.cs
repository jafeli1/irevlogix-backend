using irevlogix_backend.Data;
using irevlogix_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Globalization;

namespace irevlogix_backend.Services
{
    public class AIRecommendationService
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

                var prompt = $@"Given the following {aggregationPeriod} return volumes for {materialTypeFilter}{clientFilter}: [{historicalDataString}], 
predict the return volumes for the next {weeksAhead} {aggregationPeriod.TrimEnd('y')}s. 
Consider seasonal trends, growth patterns, and any cyclical behavior in the data.
Provide only the predicted values in a JSON array format: [period1_volume, period2_volume, period3_volume, period4_volume].
Each value should be a number representing the predicted quantity.";

                var response = await _openAIService.GetStructuredResponseAsync<int[]>(prompt);
                
                return new ReturnsForecastResult
                {
                    HasSufficientData = true,
                    HistoricalData = aggregatedData.Select(d => new ForecastDataPoint { Period = d.Period, Volume = d.Volume }).ToList(),
                    PredictedData = response?.Select((vol, idx) => new ForecastDataPoint 
                    { 
                        Period = GetFuturePeriod(aggregationPeriod, idx + 1), 
                        Volume = vol 
                    }).ToList() ?? new List<ForecastDataPoint>(),
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
                "weekly" => {
                    var futureDate = now.AddDays(7 * periodsAhead);
                    var week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(futureDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    return $"{futureDate.Year}-W{week:D2}";
                },
                "monthly" => {
                    var futureDate = now.AddMonths(periodsAhead);
                    return $"{futureDate.Year}-{futureDate.Month:D2}";
                },
                _ => throw new ArgumentException("Invalid aggregation period")
            };
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
}
