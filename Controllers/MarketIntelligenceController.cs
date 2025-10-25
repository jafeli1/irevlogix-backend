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

        [HttpPost("upload")]
        public async Task<ActionResult<object>> UploadProductImage(IFormFile file)
        {
            try
            {
                var clientId = User.FindFirst("ClientId")?.Value;
                if (string.IsNullOrEmpty(clientId))
                {
                    _logger.LogWarning("ClientId not found in user claims");
                    return Unauthorized(new { message = "Client ID not found" });
                }

                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                var uploadsPath = Path.Combine("upload", clientId, "MarketIntelligence");
                Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("Uploaded market intelligence image for client: {ClientId}, file: {FileName}", clientId, fileName);

                return Ok(new { fileName = file.FileName, filePath = fileName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading market intelligence image");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
