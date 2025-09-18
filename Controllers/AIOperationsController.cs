using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using irevlogix_backend.Services;
using System.Security.Claims;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/ai-operations")]
    [Authorize]
    public class AIOperationsController : ControllerBase
    {
        private readonly AIRecommendationService _aiService;
        private readonly ILogger<AIOperationsController> _logger;

        public AIOperationsController(AIRecommendationService aiService, ILogger<AIOperationsController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        [HttpGet("returns-forecast")]
        public async Task<IActionResult> GetReturnsForecast(
            [FromQuery] string? materialType = null,
            [FromQuery] int? originatorClientId = null,
            [FromQuery] string aggregationPeriod = "weekly",
            [FromQuery] int weeksAhead = 4)
        {
            try
            {
                var clientId = GetClientId();
                if (string.IsNullOrEmpty(clientId))
                {
                    return Unauthorized(new { message = "Client ID not found in token" });
                }

                var result = await _aiService.GetPredictiveReturnsForecastAsync(
                    clientId, materialType, originatorClientId, aggregationPeriod, weeksAhead);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting returns forecast");
                return StatusCode(500, new { message = "Internal server error occurred while generating forecast" });
            }
        }

        private string GetClientId()
        {
            return User.FindFirst("ClientId")?.Value ?? string.Empty;
        }

        private bool IsAdministrator()
        {
            return User.IsInRole("Administrator");
        }
    }
}
