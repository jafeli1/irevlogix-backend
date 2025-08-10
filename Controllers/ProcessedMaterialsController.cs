using System;
using System.Linq;
using System.Threading.Tasks;
using irevlogix_backend.Data;
using irevlogix_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProcessedMaterialsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProcessedMaterialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetClientId()
        {
            return User.FindFirst("ClientId")?.Value ?? string.Empty;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] int? materialTypeId, [FromQuery] string? qualityGrade, [FromQuery] string? location, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var clientId = GetClientId();

            var q = _context.ProcessedMaterials
                .Include(p => p.MaterialType)
                .AsQueryable();

            q = q.Where(x => x.ClientId == clientId);

            if (materialTypeId.HasValue) q = q.Where(x => x.MaterialTypeId == materialTypeId.Value);
            if (!string.IsNullOrWhiteSpace(qualityGrade)) q = q.Where(x => x.QualityGrade != null && x.QualityGrade.Contains(qualityGrade));
            if (!string.IsNullOrWhiteSpace(location)) q = q.Where(x => x.Location != null && x.Location.Contains(location));
            if (!string.IsNullOrWhiteSpace(status)) q = q.Where(x => x.Status == status);

            var total = await q.CountAsync();
            Response.Headers["X-Total-Count"] = total.ToString();

            var items = await q
                .OrderByDescending(x => x.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    MaterialType = x.MaterialType == null ? null : new { x.MaterialType.Id, x.MaterialType.Name },
                    x.Quantity,
                    x.UnitOfMeasure,
                    x.QualityGrade,
                    x.Location,
                    Status = x.Status,
                    ProcessingLotId = x.ProcessingLotId
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var clientId = GetClientId();

            var item = await _context.ProcessedMaterials
                .Include(p => p.MaterialType)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null) return NotFound();
            if (!string.IsNullOrEmpty(clientId) && item.ClientId != clientId) return NotFound();

            return Ok(new
            {
                item.Id,
                MaterialType = item.MaterialType == null ? null : new { item.MaterialType.Id, item.MaterialType.Name },
                item.Description,
                item.Quantity,
                item.UnitOfMeasure,
                item.QualityGrade,
                item.Location,
                Status = item.Status,
                item.ProcessingLotId
            });
        }

        public class CreateProcessedMaterialDto
        {
            public int? MaterialTypeId { get; set; }
            public string Description { get; set; }
            public decimal? Quantity { get; set; }
            public string UnitOfMeasure { get; set; }
            public string QualityGrade { get; set; }
            public string Location { get; set; }
            public string Status { get; set; }
            public decimal? PurchaseCostPerUnit { get; set; }
            public int? ProcessingLotId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProcessedMaterialDto dto)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int userId = 1;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsed)) userId = parsed;
            var clientId = GetClientId();

            var entity = new ProcessedMaterial
            {
                MaterialTypeId = dto.MaterialTypeId,
                Description = dto.Description ?? string.Empty,
                Quantity = dto.Quantity ?? 0,
                UnitOfMeasure = dto.UnitOfMeasure,
                QualityGrade = dto.QualityGrade,
                Location = dto.Location,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Available" : dto.Status,
                ProcessingLotId = dto.ProcessingLotId ?? 0,
                PurchaseCostPerUnit = dto.PurchaseCostPerUnit,
                ClientId = clientId,
                CreatedBy = userId,
                UpdatedBy = userId,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            _context.ProcessedMaterials.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new { entity.Id });
        }

        public class UpdateProcessedMaterialDto
        {
            public string? Status { get; set; }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] UpdateProcessedMaterialDto dto)
        {
            var clientId = GetClientId();
            var entity = await _context.ProcessedMaterials.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return NotFound();
            if (!string.IsNullOrEmpty(clientId) && entity.ClientId != clientId) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Status)) entity.Status = dto.Status;
            entity.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
