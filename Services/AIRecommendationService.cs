using irevlogix_backend.Data;
using irevlogix_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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

        public async Task<string> GetPredictiveReturnsForecastAsync(string clientId, int daysAhead = 30)
        {
            try
            {
                var recentShipments = await _context.Shipments
                    .Where(s => s.ClientId == clientId && s.DateCreated >= DateTime.UtcNow.AddDays(-90))
                    .CountAsync();

                var prompt = $@"Based on historical data showing {recentShipments} shipments in the last 90 days, 
predict the expected number of returns/shipments for the next {daysAhead} days. 
Consider seasonal trends and provide a brief explanation of the forecast.";

                var response = await _openAIService.GetChatCompletionAsync(prompt);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting predictive returns forecast");
                return "Unable to generate forecast at this time.";
            }
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
}
