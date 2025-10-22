using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using irevlogix_backend.DTOs;
using irevlogix_backend.Data;
using irevlogix_backend.Models;

namespace irevlogix_backend.Services
{
    public class MarketIntelligenceService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MarketIntelligenceService> _logger;
        private readonly EbayApiService _ebayApiService;
        private readonly ApplicationDbContext _context;

        public MarketIntelligenceService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<MarketIntelligenceService> logger,
            EbayApiService ebayApiService,
            ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _ebayApiService = ebayApiService;
            _context = context;
        }

        public async Task<ProductAnalysisResult> AnalyzeProductByImageAsync(string clientId, string base64Image, int userId)
        {
            _logger.LogInformation("Starting product analysis by image for client: {ClientId}", clientId);

            var ebayResponse = await _ebayApiService.SearchItemsByImageAsync(clientId, base64Image);
            var ebayData = JsonDocument.Parse(ebayResponse);

            var productInfo = await ExtractProductInfoFromEbayAsync(ebayData, isImageSearch: true);

            var gptAnalysis = await AnalyzeWithGPTAsync(productInfo, base64Image: base64Image);

            gptAnalysis.MatchedRecyclers = await FindMatchingRecyclersAsync(clientId, userId, gptAnalysis.Components);

            await SaveProductAnalysisAsync(clientId, gptAnalysis, userId, productDescription: null, imagePath: null);

            return gptAnalysis;
        }

        public async Task<ProductAnalysisResult> AnalyzeProductByDescriptionAsync(string clientId, string description, int userId)
        {
            _logger.LogInformation("Starting product analysis by description for client: {ClientId}", clientId);

            var ebayResponse = await _ebayApiService.SearchItemsByKeywordAsync(clientId, description);
            var ebayData = JsonDocument.Parse(ebayResponse);

            var productInfo = await ExtractProductInfoFromEbayAsync(ebayData, isImageSearch: false);

            var gptAnalysis = await AnalyzeWithGPTAsync(productInfo, textDescription: description);

            gptAnalysis.MatchedRecyclers = await FindMatchingRecyclersAsync(clientId, userId, gptAnalysis.Components);

            await SaveProductAnalysisAsync(clientId, gptAnalysis, userId, productDescription: description, imagePath: null);

            return gptAnalysis;
        }

        private async Task<EbayProductInfo> ExtractProductInfoFromEbayAsync(JsonDocument ebayData, bool isImageSearch)
        {
            var productInfo = new EbayProductInfo
            {
                Items = new List<EbayItem>()
            };

            if (ebayData.RootElement.TryGetProperty("itemSummaries", out var itemSummaries))
            {
                foreach (var item in itemSummaries.EnumerateArray())
                {
                    var ebayItem = new EbayItem
                    {
                        Title = item.TryGetProperty("title", out var title) ? title.GetString() : "",
                        Price = 0.0m,
                        Currency = "USD",
                        Condition = item.TryGetProperty("condition", out var condition) ? condition.GetString() : "",
                        ItemUrl = item.TryGetProperty("itemWebUrl", out var url) ? url.GetString() : "",
                        ImageUrl = ""
                    };

                    if (item.TryGetProperty("price", out var priceObj))
                    {
                        if (priceObj.TryGetProperty("value", out var priceValue))
                        {
                            ebayItem.Price = decimal.Parse(priceValue.GetString() ?? "0");
                        }
                        if (priceObj.TryGetProperty("currency", out var currency))
                        {
                            ebayItem.Currency = currency.GetString() ?? "USD";
                        }
                    }

                    if (item.TryGetProperty("image", out var imageObj))
                    {
                        if (imageObj.TryGetProperty("imageUrl", out var imageUrl))
                        {
                            ebayItem.ImageUrl = imageUrl.GetString() ?? "";
                        }
                    }

                    productInfo.Items.Add(ebayItem);
                }
            }

            return productInfo;
        }

        private async Task<ProductAnalysisResult> AnalyzeWithGPTAsync(EbayProductInfo ebayInfo, string? base64Image = null, string? textDescription = null)
        {
            var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? _configuration["OpenAI:ApiKey"]
                ?? throw new InvalidOperationException("OpenAI API key not configured");

            var messages = new List<object>();

            var systemPrompt = @"You are an AI assistant specialized in analyzing recyclable products and their market value. 
Your task is to:
1. Identify the product name, brand, model, and key specifications
2. List all recyclable components and materials (metals, plastics, rare earth elements, etc.)
3. Categorize components into: valuable scrap metals, components for resale, general recyclables, and hazardous components
4. Provide estimated resale values based on current market data
5. Be concise and factual in your analysis

Return your analysis in a structured JSON format with the following schema:
{
  ""productName"": ""string"",
  ""brand"": ""string"",
  ""model"": ""string"",
  ""category"": ""string"",
  ""specifications"": {
    ""key"": ""value""
  },
  ""components"": [
    {
      ""name"": ""string"",
      ""material"": ""string (e.g., aluminum, copper, steel, plastic, etc.)"",
      ""estimatedWeight"": ""string"",
      ""estimatedValue"": ""string""
    }
  ],
  ""resaleComponents"": [
    {
      ""name"": ""string (e.g., circuit board, motor, transformer)"",
      ""description"": ""string"",
      ""estimatedValue"": ""string""
    }
  ],
  ""generalRecyclables"": [
    {
      ""name"": ""string (e.g., plastic casing, glass, wiring)"",
      ""material"": ""string"",
      ""recyclingNotes"": ""string""
    }
  ],
  ""hazardousComponents"": [
    {
      ""name"": ""string (e.g., capacitor, battery, mercury switch)"",
      ""hazardType"": ""string (e.g., electrical, chemical, radioactive)"",
      ""safetyWarning"": ""string (specific handling instructions)""
    }
  ],
  ""marketPrice"": {
    ""averagePrice"": ""string"",
    ""priceRange"": ""string"",
    ""marketTrend"": ""string""
  },
  ""summary"": ""string""
}";

            messages.Add(new { role = "system", content = systemPrompt });

            var userMessageContent = new List<object>();

            if (!string.IsNullOrEmpty(textDescription))
            {
                var contextText = $"Product Description: {textDescription}\n\n";
                
                if (ebayInfo.Items.Any())
                {
                    contextText += "eBay Market Data:\n";
                    foreach (var item in ebayInfo.Items.Take(3))
                    {
                        contextText += $"- {item.Title}: {item.Currency} {item.Price} ({item.Condition})\n";
                    }
                }

                contextText += "\nPlease analyze this product and provide the recyclable components and market intelligence.";
                userMessageContent.Add(new { type = "text", text = contextText });
            }
            else if (!string.IsNullOrEmpty(base64Image))
            {
                var contextText = "Please analyze the product in this image and identify its recyclable components and market value.\n\n";
                
                if (ebayInfo.Items.Any())
                {
                    contextText += "eBay Market Data:\n";
                    foreach (var item in ebayInfo.Items.Take(3))
                    {
                        contextText += $"- {item.Title}: {item.Currency} {item.Price} ({item.Condition})\n";
                    }
                }

                userMessageContent.Add(new { type = "text", text = contextText });
                userMessageContent.Add(new 
                { 
                    type = "image_url", 
                    image_url = new { url = $"data:image/jpeg;base64,{base64Image}" }
                });
            }

            messages.Add(new { role = "user", content = userMessageContent });

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = messages,
                response_format = new { type = "json_object" },
                max_tokens = 2000
            };

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAiApiKey);

            _logger.LogInformation("Sending request to OpenAI GPT-4o-mini");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", httpContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("OpenAI API request failed. Status: {StatusCode}, Response: {Response}", 
                    response.StatusCode, responseContent);
                throw new HttpRequestException($"OpenAI API request failed. Status: {response.StatusCode}");
            }

            using (var doc = JsonDocument.Parse(responseContent))
            {
                var gptResponse = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                _logger.LogInformation("OpenAI analysis complete");

                var analysisJson = JsonDocument.Parse(gptResponse ?? "{}");
                
                var result = new ProductAnalysisResult
                {
                    ProductName = GetJsonString(analysisJson, "productName") ?? "Unknown Product",
                    Brand = GetJsonString(analysisJson, "brand") ?? "",
                    Model = GetJsonString(analysisJson, "model") ?? "",
                    Category = GetJsonString(analysisJson, "category") ?? "",
                    Specifications = new Dictionary<string, string>(),
                    Components = new List<RecyclableComponent>(),
                    MarketPrice = new MarketPriceInfo(),
                    Summary = GetJsonString(analysisJson, "summary") ?? "",
                    EbayListings = ebayInfo.Items.Take(3).Select(i => new EbayListingDto
                    {
                        Title = i.Title,
                        Price = i.Price,
                        Currency = i.Currency,
                        Condition = i.Condition,
                        ItemUrl = i.ItemUrl,
                        ImageUrl = i.ImageUrl
                    }).ToList(),
                    MatchedRecyclers = new List<MatchedRecyclerDto>(),
                    ResaleComponents = new List<ResaleComponent>(),
                    GeneralRecyclables = new List<GeneralRecyclable>(),
                    HazardousComponents = new List<HazardousComponent>()
                };

                if (analysisJson.RootElement.TryGetProperty("specifications", out var specs))
                {
                    foreach (var prop in specs.EnumerateObject())
                    {
                        result.Specifications[prop.Name] = prop.Value.GetString() ?? "";
                    }
                }

                if (analysisJson.RootElement.TryGetProperty("components", out var components))
                {
                    foreach (var comp in components.EnumerateArray())
                    {
                        result.Components.Add(new RecyclableComponent
                        {
                            Name = GetJsonString(comp, "name") ?? "",
                            Material = GetJsonString(comp, "material") ?? "",
                            EstimatedWeight = GetJsonString(comp, "estimatedWeight") ?? "",
                            EstimatedValue = GetJsonString(comp, "estimatedValue") ?? ""
                        });
                    }
                }

                if (analysisJson.RootElement.TryGetProperty("marketPrice", out var marketPrice))
                {
                    result.MarketPrice.AveragePrice = GetJsonString(marketPrice, "averagePrice") ?? "";
                    result.MarketPrice.PriceRange = GetJsonString(marketPrice, "priceRange") ?? "";
                    result.MarketPrice.MarketTrend = GetJsonString(marketPrice, "marketTrend") ?? "";
                }

                if (analysisJson.RootElement.TryGetProperty("resaleComponents", out var resaleComponents))
                {
                    foreach (var comp in resaleComponents.EnumerateArray())
                    {
                        result.ResaleComponents.Add(new ResaleComponent
                        {
                            Name = GetJsonString(comp, "name") ?? "",
                            Description = GetJsonString(comp, "description") ?? "",
                            EstimatedValue = GetJsonString(comp, "estimatedValue") ?? ""
                        });
                    }
                }

                if (analysisJson.RootElement.TryGetProperty("generalRecyclables", out var generalRecyclables))
                {
                    foreach (var item in generalRecyclables.EnumerateArray())
                    {
                        result.GeneralRecyclables.Add(new GeneralRecyclable
                        {
                            Name = GetJsonString(item, "name") ?? "",
                            Material = GetJsonString(item, "material") ?? "",
                            RecyclingNotes = GetJsonString(item, "recyclingNotes") ?? ""
                        });
                    }
                }

                if (analysisJson.RootElement.TryGetProperty("hazardousComponents", out var hazardousComponents))
                {
                    foreach (var hazard in hazardousComponents.EnumerateArray())
                    {
                        result.HazardousComponents.Add(new HazardousComponent
                        {
                            Name = GetJsonString(hazard, "name") ?? "",
                            HazardType = GetJsonString(hazard, "hazardType") ?? "",
                            SafetyWarning = GetJsonString(hazard, "safetyWarning") ?? ""
                        });
                    }
                }

                return result;
            }
        }

        private string? GetJsonString(JsonDocument doc, string propertyName)
        {
            if (doc.RootElement.TryGetProperty(propertyName, out var prop))
            {
                return prop.GetString();
            }
            return null;
        }

        private string? GetJsonString(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop))
            {
                return prop.GetString();
            }
            return null;
        }

        private async Task SaveProductAnalysisAsync(string clientId, ProductAnalysisResult analysisResult, int userId, string? productDescription, string? imagePath)
        {
            try
            {
                var productAnalysis = new ProductAnalysis
                {
                    ClientId = clientId,
                    ProductName = analysisResult.ProductName,
                    Brand = analysisResult.Brand,
                    Model = analysisResult.Model,
                    Category = analysisResult.Category,
                    ProductDescription = productDescription,
                    ImagePath = imagePath,
                    Specifications = analysisResult.Specifications != null ? JsonSerializer.Serialize(analysisResult.Specifications) : null,
                    Components = analysisResult.Components != null ? JsonSerializer.Serialize(analysisResult.Components) : null,
                    MarketPrice = analysisResult.MarketPrice != null ? JsonSerializer.Serialize(analysisResult.MarketPrice) : null,
                    Summary = analysisResult.Summary,
                    EbayListings = analysisResult.EbayListings != null ? JsonSerializer.Serialize(analysisResult.EbayListings) : null,
                    AnalysisDate = DateTime.UtcNow,
                    LastMarketRefresh = DateTime.UtcNow,
                    IsActive = true,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                };

                _context.ProductAnalyses.Add(productAnalysis);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Saved product analysis for {ProductName} (Client: {ClientId})", 
                    analysisResult.ProductName, clientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving product analysis to database");
            }
        }

        public async Task<int> RefreshMarketDataAsync()
        {
            _logger.LogInformation("Starting daily market data refresh job");

            var analysesToRefresh = await _context.ProductAnalyses
                .Where(pa => pa.IsActive && 
                      (pa.LastMarketRefresh == null || 
                       pa.LastMarketRefresh.Value.Date < DateTime.UtcNow.Date))
                .Take(100)
                .ToListAsync();

            int refreshedCount = 0;

            foreach (var analysis in analysesToRefresh)
            {
                try
                {
                    var searchQuery = $"{analysis.Brand} {analysis.Model} {analysis.ProductName}".Trim();
                    var ebayResponse = await _ebayApiService.SearchItemsByKeywordAsync(analysis.ClientId, searchQuery);
                    var ebayData = JsonDocument.Parse(ebayResponse);
                    var productInfo = await ExtractProductInfoFromEbayAsync(ebayData, isImageSearch: false);

                    if (productInfo.Items.Any())
                    {
                        var avgPrice = productInfo.Items.Average(i => i.Price);
                        var minPrice = productInfo.Items.Min(i => i.Price);
                        var maxPrice = productInfo.Items.Max(i => i.Price);

                        var updatedMarketPrice = new
                        {
                            averagePrice = $"${avgPrice:F2}",
                            priceRange = $"${minPrice:F2} - ${maxPrice:F2}",
                            marketTrend = "Stable"
                        };

                        analysis.MarketPrice = JsonSerializer.Serialize(updatedMarketPrice);
                        analysis.EbayListings = JsonSerializer.Serialize(productInfo.Items.Take(3));
                        analysis.LastMarketRefresh = DateTime.UtcNow;
                        analysis.DateUpdated = DateTime.UtcNow;

                        refreshedCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error refreshing market data for product analysis ID: {Id}", analysis.Id);
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Market data refresh completed. Refreshed {Count} products", refreshedCount);

            return refreshedCount;
        }

        public async Task<List<MatchedRecyclerDto>> FindMatchingRecyclersAsync(string clientId, int userId, List<RecyclableComponent> components)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found for recycler matching", userId);
                    return new List<MatchedRecyclerDto>();
                }

                var materials = components.Select(c => c.Material.ToLower()).Distinct().ToList();

                var allRecyclers = await _context.Recyclers.ToListAsync();

                var scoredRecyclers = new List<(Models.Recycler recycler, int score, string reason)>();

                foreach (var recycler in allRecyclers)
                {
                    int score = 0;
                    var reasons = new List<string>();

                    var recyclerMaterials = (recycler.MaterialTypesHandled ?? "").ToLower();
                    var recyclerServices = (recycler.ServicesOffered ?? "").ToLower();

                    int materialMatches = 0;
                    foreach (var material in materials)
                    {
                        if (recyclerMaterials.Contains(material) || recyclerServices.Contains(material))
                        {
                            materialMatches++;
                        }
                    }

                    if (materialMatches > 0)
                    {
                        score += materialMatches * 10;
                        reasons.Add($"{materialMatches} material match{(materialMatches > 1 ? "es" : "")}");
                    }

                    var recyclerAddress = (recycler.Address ?? "").ToLower();
                    var userPostal = (user.PostalCode ?? "").ToLower();
                    var userCity = (user.City ?? "").ToLower();
                    var userState = (user.State ?? "").ToLower();

                    if (!string.IsNullOrEmpty(userPostal) && recyclerAddress.Contains(userPostal))
                    {
                        score += 30;
                        reasons.Add("Same postal code area");
                    }
                    else if (!string.IsNullOrEmpty(userCity) && recyclerAddress.Contains(userCity))
                    {
                        score += 20;
                        reasons.Add("Same city");
                    }
                    else if (!string.IsNullOrEmpty(userState) && recyclerAddress.Contains(userState))
                    {
                        score += 10;
                        reasons.Add("Same state");
                    }

                    if (score > 0)
                    {
                        scoredRecyclers.Add((recycler, score, string.Join(", ", reasons)));
                    }
                }

                if (scoredRecyclers.Count == 0)
                {
                    _logger.LogInformation("No proximity matches found, selecting top recyclers by material match only");
                    
                    foreach (var recycler in allRecyclers)
                    {
                        var recyclerMaterials = (recycler.MaterialTypesHandled ?? "").ToLower();
                        var recyclerServices = (recycler.ServicesOffered ?? "").ToLower();

                        int materialMatches = 0;
                        foreach (var material in materials)
                        {
                            if (recyclerMaterials.Contains(material) || recyclerServices.Contains(material))
                            {
                                materialMatches++;
                            }
                        }

                        if (materialMatches > 0)
                        {
                            scoredRecyclers.Add((recycler, materialMatches * 10, $"{materialMatches} material match{(materialMatches > 1 ? "es" : "")}"));
                        }
                    }

                    if (scoredRecyclers.Count == 0)
                    {
                        _logger.LogInformation("No material matches found, selecting any recyclers");
                        foreach (var recycler in allRecyclers.Take(10))
                        {
                            scoredRecyclers.Add((recycler, 1, "Active recycler"));
                        }
                    }
                }

                var topRecyclers = scoredRecyclers
                    .OrderByDescending(r => r.score)
                    .Take(10)
                    .Select(r => new MatchedRecyclerDto
                    {
                        CompanyName = r.recycler.CompanyName,
                        Address = r.recycler.Address ?? "",
                        CertificationType = r.recycler.CertificationType ?? "",
                        ContactPhone = r.recycler.ContactPhone ?? "",
                        ContactEmail = r.recycler.ContactEmail ?? "",
                        MatchScore = r.score,
                        MatchReason = r.reason
                    })
                    .ToList();

                _logger.LogInformation("Found {Count} matching recyclers", topRecyclers.Count);

                return topRecyclers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding matching recyclers");
                return new List<MatchedRecyclerDto>();
            }
        }

        public async Task<ComponentPriceResult> GetComponentPriceAsync(string clientId, string componentName)
        {
            try
            {
                _logger.LogInformation("Fetching price for component: {ComponentName}", componentName);

                var ebayResponse = await _ebayApiService.SearchItemsByKeywordAsync(clientId, componentName);
                var ebayData = JsonDocument.Parse(ebayResponse);
                var productInfo = await ExtractProductInfoFromEbayAsync(ebayData, isImageSearch: false);

                var result = new ComponentPriceResult
                {
                    ComponentName = componentName,
                    Listings = new List<EbayListingDto>()
                };

                if (productInfo.Items.Any())
                {
                    var avgPrice = productInfo.Items.Average(i => i.Price);
                    var minPrice = productInfo.Items.Min(i => i.Price);
                    var maxPrice = productInfo.Items.Max(i => i.Price);

                    result.AveragePrice = $"${avgPrice:F2}";
                    result.PriceRange = $"${minPrice:F2} - ${maxPrice:F2}";
                    result.Listings = productInfo.Items.Take(3).Select(i => new EbayListingDto
                    {
                        Title = i.Title,
                        Price = i.Price,
                        Currency = i.Currency,
                        Condition = i.Condition,
                        ItemUrl = i.ItemUrl,
                        ImageUrl = i.ImageUrl
                    }).ToList();
                }
                else
                {
                    result.AveragePrice = "No prices available";
                    result.PriceRange = "No prices available";
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching component price for {ComponentName}", componentName);
                return new ComponentPriceResult
                {
                    ComponentName = componentName,
                    AveragePrice = "Error fetching price",
                    PriceRange = "Error fetching price",
                    Listings = new List<EbayListingDto>()
                };
            }
        }
    }

    public class EbayProductInfo
    {
        public List<EbayItem> Items { get; set; } = new();
    }

    public class EbayItem
    {
        public string Title { get; set; } = "";
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";
        public string Condition { get; set; } = "";
        public string ItemUrl { get; set; } = "";
        public string ImageUrl { get; set; } = "";
    }
}
