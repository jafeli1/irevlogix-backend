using System.Text;
using System.Text.Json;

namespace irevlogix_backend.Services
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<OpenAIService> _logger;

        public OpenAIService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenAIService> logger)
        {
            _httpClient = httpClient;
            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? 
                     configuration["OpenAI:ApiKey"] ?? 
                     string.Empty;
            _logger = logger;
            
            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            }
        }

        public async Task<string> GetChatCompletionAsync(string prompt, string systemMessage = "You are a helpful assistant for recycling and asset recovery operations.")
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("OpenAI API key not configured, returning fallback response");
                return "AI recommendations are currently unavailable due to missing API configuration.";
            }

            try
            {
                var requestBody = new
                {
                    model = "gpt-4o-mini",
                    messages = new[]
                    {
                        new { role = "system", content = systemMessage },
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 500,
                    temperature = 0.7,
                    response_format = new { type = "json_object" }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var endpoint = "https://api.openai.com/v1/chat/completions";
                var response = await _httpClient.PostAsync(endpoint, content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("OpenAI API call failed. StatusCode={StatusCode}, Endpoint={Endpoint}, Body={Body}", (int)response.StatusCode, endpoint, Truncate(responseContent, 2000));
                    throw new HttpRequestException($"OpenAI request failed with status {(int)response.StatusCode}");
                }

                try
                {
                    var responseData = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    return responseData
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString() ?? string.Empty;
                }
                catch (Exception parseEx)
                {
                    _logger.LogError(parseEx, "Failed to parse OpenAI response JSON. RawBody={Body}", Truncate(responseContent, 2000));
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling OpenAI API");
                throw;
            }
        }

        private static string Truncate(string? value, int maxLen)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Length <= maxLen ? value : value.Substring(0, maxLen);
        }

        public async Task<T?> GetStructuredResponseAsync<T>(string prompt, string systemMessage = "You are a helpful assistant. Respond only with valid JSON.")
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("OpenAI API key not configured, returning default response");
                return default(T);
            }

            try
            {
                var response = await GetChatCompletionAsync(prompt, systemMessage);
                try
                {
                    return JsonSerializer.Deserialize<T>(response);
                }
                catch (Exception parseEx)
                {
                    _logger.LogError(parseEx, "Error deserializing OpenAI structured response to {Type}. RawText={Raw}", typeof(T).Name, Truncate(response, 2000));
                    return default(T);
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "OpenAI HTTP error while requesting structured response");
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error obtaining structured response from OpenAI");
                return default(T);
            }
        }
    }
}
