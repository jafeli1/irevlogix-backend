using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using irevlogix_backend.Services;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

                var result = await _aiService.GetPredictiveReturnsForecastAsync(
                    clientId, materialType, originatorClientId, aggregationPeriod, weeksAhead);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting returns forecast");
                return StatusCode(500, "Internal server error");
            }
        }

        private string GetClientId()
        {
            return User.FindFirst("ClientId")?.Value ?? throw new UnauthorizedAccessException("ClientId not found in token");
        }

        private bool IsAdministrator()
        {
            return User.IsInRole("Administrator");
        }
    }
}
