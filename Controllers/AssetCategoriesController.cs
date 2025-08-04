using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Models;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssetCategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AssetCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssetCategory>>> GetAssetCategories()
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized();

            var assetCategories = await _context.AssetCategories
                .Where(ac => ac.ClientId == clientId && ac.IsActive)
                .OrderBy(ac => ac.Name)
                .ToListAsync();

            return Ok(assetCategories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssetCategory>> GetAssetCategory(int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized();

            var assetCategory = await _context.AssetCategories
                .Where(ac => ac.Id == id && ac.ClientId == clientId)
                .FirstOrDefaultAsync();

            if (assetCategory == null)
                return NotFound();

            return assetCategory;
        }

        [HttpPost]
        public async Task<ActionResult<AssetCategory>> CreateAssetCategory(AssetCategory assetCategory)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst("UserId")?.Value;
            
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId))
                return Unauthorized();

            assetCategory.ClientId = clientId;
            assetCategory.CreatedBy = int.Parse(userId);
            assetCategory.UpdatedBy = int.Parse(userId);
            assetCategory.DateCreated = DateTime.UtcNow;
            assetCategory.DateUpdated = DateTime.UtcNow;

            _context.AssetCategories.Add(assetCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAssetCategory), new { id = assetCategory.Id }, assetCategory);
        }
    }
}
