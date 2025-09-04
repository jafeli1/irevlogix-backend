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

        private bool IsAdministrator()
        {
            return User.IsInRole("Administrator");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialType>>> GetMaterialTypes(
            [FromQuery] string? name = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator())
                return Unauthorized();

            var query = _context.MaterialTypes
                .Where(mt => mt.IsActive);

            if (!IsAdministrator())
            {
                query = query.Where(mt => mt.ClientId == clientId);
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(mt => mt.Name.Contains(name));
            }

            var totalCount = await query.CountAsync();
            
            var materialTypes = await query
                .OrderBy(mt => mt.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            return Ok(materialTypes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialType>> GetMaterialType(int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator())
                return Unauthorized();

            var query = _context.MaterialTypes
                .Where(mt => mt.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(mt => mt.ClientId == clientId);
            }

            var materialType = await query.FirstOrDefaultAsync();

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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterialType(int id, MaterialType materialType)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst("UserId")?.Value;
            
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (id != materialType.Id)
                return BadRequest();

            var query = _context.MaterialTypes
                .Where(mt => mt.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(mt => mt.ClientId == clientId);
            }

            var existingMaterialType = await query.FirstOrDefaultAsync();

            if (existingMaterialType == null)
                return NotFound();

            existingMaterialType.Name = materialType.Name;
            existingMaterialType.Description = materialType.Description;
            existingMaterialType.IsActive = materialType.IsActive;
            existingMaterialType.DefaultPricePerUnit = materialType.DefaultPricePerUnit;
            existingMaterialType.UnitOfMeasure = materialType.UnitOfMeasure;
            existingMaterialType.UpdatedBy = int.Parse(userId);
            existingMaterialType.DateUpdated = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialTypeExists(id, clientId))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialType(int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator())
                return Unauthorized();

            var query = _context.MaterialTypes
                .Where(mt => mt.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(mt => mt.ClientId == clientId);
            }

            var materialType = await query.FirstOrDefaultAsync();

            if (materialType == null)
                return NotFound();

            _context.MaterialTypes.Remove(materialType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaterialTypeExists(int id, string clientId)
        {
            if (IsAdministrator())
            {
                return _context.MaterialTypes.Any(mt => mt.Id == id);
            }
            return _context.MaterialTypes.Any(mt => mt.Id == id && mt.ClientId == clientId);
        }
    }
}
