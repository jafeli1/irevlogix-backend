using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using irevlogix_backend.Services;
using irevlogix_backend.DTOs;
using System.Security.Claims;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MarketIntelligenceController : ControllerBase
    {
        private readonly MarketIntelligenceService _marketIntelligenceService;
        private readonly ILogger<MarketIntelligenceController> _logger;

        public MarketIntelligenceController(
            MarketIntelligenceService marketIntelligenceService,
            ILogger<MarketIntelligenceController> logger)
        {
            _marketIntelligenceService = marketIntelligenceService;
            _logger = logger;
        }

        [HttpPost("analyze")]
        public async Task<ActionResult<ProductAnalysisResult>> AnalyzeProduct([FromBody] ProductAnalysisRequest request)
        {
            try
            {
                var clientId = User.FindFirst("ClientId")?.Value;
                if (string.IsNullOrEmpty(clientId))
                {
                    _logger.LogWarning("ClientId not found in user claims");
                    return Unauthorized(new { message = "Client ID not found" });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    _logger.LogWarning("UserId not found in user claims");
                    return Unauthorized(new { message = "User ID not found" });
                }

                if (string.IsNullOrEmpty(request.Base64Image) && string.IsNullOrEmpty(request.ProductDescription))
                {
                    return BadRequest(new { message = "Either product image or description must be provided" });
                }

                ProductAnalysisResult result;

                if (!string.IsNullOrEmpty(request.Base64Image))
                {
                    _logger.LogInformation("Analyzing product by image for client: {ClientId}", clientId);
                    result = await _marketIntelligenceService.AnalyzeProductByImageAsync(clientId, request.Base64Image, userId);
                }
                else
                {
                    _logger.LogInformation("Analyzing product by description for client: {ClientId}", clientId);
                    result = await _marketIntelligenceService.AnalyzeProductByDescriptionAsync(clientId, request.ProductDescription!, userId);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing product");
                return StatusCode(500, new { message = "An error occurred while analyzing the product", error = ex.Message });
            }
        }

        [HttpPost("component-price")]
        public async Task<ActionResult<ComponentPriceResult>> GetComponentPrice([FromBody] ComponentPriceRequest request)
        {
            try
            {
                var clientId = User.FindFirst("ClientId")?.Value;
                if (string.IsNullOrEmpty(clientId))
                {
                    _logger.LogWarning("ClientId not found in user claims");
                    return Unauthorized(new { message = "Client ID not found" });
                }

                if (string.IsNullOrEmpty(request.ComponentName))
                {
                    return BadRequest(new { message = "Component name must be provided" });
                }

                var result = await _marketIntelligenceService.GetComponentPriceAsync(clientId, request.ComponentName);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching component price");
                return StatusCode(500, new { message = "An error occurred while fetching component price", error = ex.Message });
            }
        }
    }
}
