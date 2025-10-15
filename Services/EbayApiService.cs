using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Models;

namespace irevlogix_backend.Services
{
    public class EbayApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EbayApiService> _logger;
        private readonly ApplicationDbContext _context;

        private const string API_SCOPE = "https://api.ebay.com/oauth/api_scope";

        public enum EnvironmentType { Sandbox, Production }

        public EbayApiService(
            HttpClient httpClient, 
            IConfiguration configuration, 
            ILogger<EbayApiService> logger,
            ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        private string GetTokenUrl(EnvironmentType env) =>
            env == EnvironmentType.Production
                ? "https://api.ebay.com/identity/v1/oauth2/token"
                : "https://api.sandbox.ebay.com/identity/v1/oauth2/token";

        private string GetBrowseUrl(EnvironmentType env) =>
            env == EnvironmentType.Production
                ? "https://api.ebay.com/buy/browse/v1"
                : "https://api.sandbox.ebay.com/buy/browse/v1";

        private async Task<(string ClientId, string ClientSecret, EnvironmentType Environment)> GetCredentialsAsync(string clientId)
        {
            var ebayEnvSetting = await _context.ApplicationSettings
                .Where(s => s.ClientId == clientId && s.SettingKey == "EBayEnvironment")
                .FirstOrDefaultAsync();

            var environment = ebayEnvSetting?.SettingValue == "Sandbox" 
                ? EnvironmentType.Sandbox 
                : EnvironmentType.Production;

            string clientIdValue;
            string clientSecretValue;

            if (environment == EnvironmentType.Sandbox)
            {
                clientIdValue = Environment.GetEnvironmentVariable("EBAY_CLIENT_ID_SANDBOX") 
                    ?? _configuration["EBay:ClientIdSandbox"] 
                    ?? throw new InvalidOperationException("eBay Sandbox Client ID not configured");
                
                clientSecretValue = Environment.GetEnvironmentVariable("EBAY_CLIENT_SECRET_SANDBOX") 
                    ?? _configuration["EBay:ClientSecretSandbox"] 
                    ?? throw new InvalidOperationException("eBay Sandbox Client Secret not configured");
            }
            else
            {
                clientIdValue = Environment.GetEnvironmentVariable("EBAY_CLIENT_ID_PRODUCTION") 
                    ?? _configuration["EBay:ClientIdProduction"] 
                    ?? throw new InvalidOperationException("eBay Production Client ID not configured");
                
                clientSecretValue = Environment.GetEnvironmentVariable("EBAY_CLIENT_SECRET_PRODUCTION") 
                    ?? _configuration["EBay:ClientSecretProduction"] 
                    ?? throw new InvalidOperationException("eBay Production Client Secret not configured");
            }

            return (clientIdValue, clientSecretValue, environment);
        }

        public async Task<string> GetApplicationAccessTokenAsync(string clientId)
        {
            var (ebayClientId, clientSecret, environment) = await GetCredentialsAsync(clientId);
            var tokenUrl = GetTokenUrl(environment);

            var authString = $"{ebayClientId}:{clientSecret}";
            var base64Auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));

            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);

            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", API_SCOPE)
            });

            request.Content = requestBody;

            _logger.LogInformation("Requesting eBay access token for {Environment}", environment);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("eBay token request failed. Status: {StatusCode}, Response: {Response}", 
                    response.StatusCode, responseContent);
                throw new HttpRequestException($"Failed to get eBay access token. Status: {response.StatusCode}");
            }

            using (JsonDocument doc = JsonDocument.Parse(responseContent))
            {
                var accessToken = doc.RootElement.GetProperty("access_token").GetString();
                _logger.LogInformation("eBay token successfully retrieved. Expires in: {ExpiresIn} seconds", 
                    doc.RootElement.GetProperty("expires_in").GetInt32());
                return accessToken ?? throw new InvalidOperationException("Access token was null");
            }
        }

        public async Task<string> SearchItemsByKeywordAsync(string clientId, string query)
        {
            var token = await GetApplicationAccessTokenAsync(clientId);
            var (_, _, environment) = await GetCredentialsAsync(clientId);
            
            var baseUrl = GetBrowseUrl(environment);
            var endpoint = $"{baseUrl}/item_summary/search?q={Uri.EscapeDataString(query)}&limit=5";

            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("X-EBAY-C-MARKETPLACE-ID", "EBAY_US");

            _logger.LogInformation("Searching eBay by keyword: {Query} ({Environment})", query, environment);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("eBay keyword search failed. Status: {StatusCode}, Query: {Query}", 
                    response.StatusCode, query);
                throw new HttpRequestException($"eBay keyword search failed. Status: {response.StatusCode}");
            }

            _logger.LogInformation("eBay keyword search successful for: {Query}", query);
            return responseContent;
        }

        public async Task<string> SearchItemsByImageAsync(string clientId, string base64Image)
        {
            var token = await GetApplicationAccessTokenAsync(clientId);
            var (_, _, environment) = await GetCredentialsAsync(clientId);
            
            var baseUrl = GetBrowseUrl(environment);
            var endpoint = $"{baseUrl}/item_summary/search_by_image";

            var payload = new { image = base64Image };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("X-EBAY-C-MARKETPLACE-ID", "EBAY_US");
            request.Content = httpContent;

            _logger.LogInformation("Searching eBay by image ({Environment})", environment);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("eBay image search failed. Status: {StatusCode}", response.StatusCode);
                throw new HttpRequestException($"eBay image search failed. Status: {response.StatusCode}");
            }

            _logger.LogInformation("eBay image search successful");
            return responseContent;
        }
    }
}
