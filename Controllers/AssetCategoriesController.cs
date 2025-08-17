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
        public async Task<ActionResult<IEnumerable<AssetCategory>>> GetAssetCategories(
            [FromQuery] string? name = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized();

            var query = _context.AssetCategories
                .Where(ac => ac.ClientId == clientId && ac.IsActive);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(ac => ac.Name.Contains(name));
            }

            var totalCount = await query.CountAsync();
            
            var assetCategories = await query
                .OrderBy(ac => ac.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssetCategory(int id, AssetCategory assetCategory)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst("UserId")?.Value;
            
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (id != assetCategory.Id)
                return BadRequest();

            var existingCategory = await _context.AssetCategories
                .Where(ac => ac.Id == id && ac.ClientId == clientId)
                .FirstOrDefaultAsync();

            if (existingCategory == null)
                return NotFound();

            existingCategory.Name = assetCategory.Name;
            existingCategory.Description = assetCategory.Description;
            existingCategory.IsActive = assetCategory.IsActive;
            existingCategory.DefaultDisposition = assetCategory.DefaultDisposition;
            existingCategory.RequiresDataSanitization = assetCategory.RequiresDataSanitization;
            existingCategory.RequiresDataDestruction = assetCategory.RequiresDataDestruction;
            existingCategory.IsRecoverable = assetCategory.IsRecoverable;
            existingCategory.ParentCategory = assetCategory.ParentCategory;
            existingCategory.UpdatedBy = int.Parse(userId);
            existingCategory.DateUpdated = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssetCategoryExists(id, clientId))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssetCategory(int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized();

            var assetCategory = await _context.AssetCategories
                .Where(ac => ac.Id == id && ac.ClientId == clientId)
                .FirstOrDefaultAsync();

            if (assetCategory == null)
                return NotFound();

            _context.AssetCategories.Remove(assetCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AssetCategoryExists(int id, string clientId)
        {
            return _context.AssetCategories.Any(ac => ac.Id == id && ac.ClientId == clientId);
        }
    }
}
