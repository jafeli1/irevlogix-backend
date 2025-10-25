using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Models;
using System.Security.Claims;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductAnalysisContactOptInController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductAnalysisContactOptInController> _logger;

        public ProductAnalysisContactOptInController(
            ApplicationDbContext context,
            ILogger<ProductAnalysisContactOptInController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ProductAnalysisContactOptIn>> CreateContactOptIn([FromBody] ProductAnalysisContactOptInRequest request)
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

                var contactOptIn = new ProductAnalysisContactOptIn
                {
                    ClientId = clientId,
                    UserId = userId,
                    UploadedPhotoPath = request.UploadedPhotoPath,
                    DetailedDescription = request.DetailedDescription,
                    ProductSummary = request.ProductSummary,
                    SecondaryMarketPriceAnalysis = request.SecondaryMarketPriceAnalysis,
                    RecyclersMatched = request.RecyclersMatched,
                    ProductAnalysisVisualizationData = request.ProductAnalysisVisualizationData,
                    OptIn = request.OptIn,
                    PreferredContactEmail = request.PreferredContactEmail,
                    PreferredContactPhone = request.PreferredContactPhone,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                };

                _context.ProductAnalysisContactOptIns.Add(contactOptIn);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created contact opt-in for user {UserId}, OptIn: {OptIn}", userId, request.OptIn);

                return Ok(contactOptIn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contact opt-in");
                return StatusCode(500, new { message = "An error occurred while saving contact opt-in", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductAnalysisContactOptIn>>> GetContactOptIns()
        {
            try
            {
                var clientId = User.FindFirst("ClientId")?.Value;
                if (string.IsNullOrEmpty(clientId))
                {
                    return Unauthorized(new { message = "Client ID not found" });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "User ID not found" });
                }

                var contactOptIns = await _context.ProductAnalysisContactOptIns
                    .Where(c => c.ClientId == clientId && c.UserId == userId)
                    .OrderByDescending(c => c.DateCreated)
                    .ToListAsync();

                return Ok(contactOptIns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contact opt-ins");
                return StatusCode(500, new { message = "An error occurred while retrieving contact opt-ins" });
            }
        }
    }

    public class ProductAnalysisContactOptInRequest
    {
        public string? UploadedPhotoPath { get; set; }
        public string? DetailedDescription { get; set; }
        public string? ProductSummary { get; set; }
        public string? SecondaryMarketPriceAnalysis { get; set; }
        public string? RecyclersMatched { get; set; }
        public string? ProductAnalysisVisualizationData { get; set; }
        public bool OptIn { get; set; }
        public string? PreferredContactEmail { get; set; }
        public string? PreferredContactPhone { get; set; }
    }
}
