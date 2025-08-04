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
    public class MaterialTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MaterialTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialType>>> GetMaterialTypes()
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized();

            var materialTypes = await _context.MaterialTypes
                .Where(mt => mt.ClientId == clientId && mt.IsActive)
                .OrderBy(mt => mt.Name)
                .ToListAsync();

            return Ok(materialTypes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialType>> GetMaterialType(int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized();

            var materialType = await _context.MaterialTypes
                .Where(mt => mt.Id == id && mt.ClientId == clientId)
                .FirstOrDefaultAsync();

            if (materialType == null)
                return NotFound();

            return materialType;
        }

        [HttpPost]
        public async Task<ActionResult<MaterialType>> CreateMaterialType(MaterialType materialType)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst("UserId")?.Value;
            
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId))
                return Unauthorized();

            materialType.ClientId = clientId;
            materialType.CreatedBy = int.Parse(userId);
            materialType.UpdatedBy = int.Parse(userId);
            materialType.DateCreated = DateTime.UtcNow;
            materialType.DateUpdated = DateTime.UtcNow;

            _context.MaterialTypes.Add(materialType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMaterialType), new { id = materialType.Id }, materialType);
        }
    }
}
