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
            return User.FindFirst("ClientId")?.Value ?? throw new UnauthorizedAccessException("ClientId not found in token");
        }

        private bool IsAdministrator()
        {
            return User.IsInRole("Administrator");
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] int? materialTypeId, [FromQuery] string? qualityGrade, [FromQuery] string? location, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var clientId = GetClientId();

            var q = _context.ProcessedMaterials
                .AsQueryable();

            if (!IsAdministrator())
            {
                q = q.Where(x => x.ClientId == clientId);
            }

            if (materialTypeId.HasValue) q = q.Where(x => x.MaterialTypeId == materialTypeId.Value);
            if (!string.IsNullOrWhiteSpace(qualityGrade)) q = q.Where(x => x.QualityGrade != null && x.QualityGrade.Contains(qualityGrade));
            if (!string.IsNullOrWhiteSpace(location)) q = q.Where(x => x.Location != null && x.Location.Contains(location));
            if (!string.IsNullOrWhiteSpace(status)) q = q.Where(x => x.Status == status);

            var total = await q.CountAsync();
            Response.Headers["X-Total-Count"] = total.ToString();

            var queryProjected = q
                .OrderByDescending(x => x.DateCreated)
                .Select(x => new
                {
                    x.Id,
                    MaterialType = x.MaterialType != null ? new { x.MaterialType.Id, x.MaterialType.Name } : null,
                    x.Quantity,
                    x.UnitOfMeasure,
                    x.QualityGrade,
                    x.Location,
                    Status = x.Status,
                    ProcessingLotId = x.ProcessingLotId
                })
                .AsNoTracking();

            var items = await queryProjected
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var clientId = GetClientId();

            var query = _context.ProcessedMaterials
                .Include(p => p.MaterialType)
                .Where(x => x.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(x => x.ClientId == clientId);
            }

            var item = await query.FirstOrDefaultAsync();

            if (item == null) return NotFound();

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
                item.ProcessingLotId,
                item.PurchaseCostPerUnit,
                item.ProcessingCostPerUnit,
                item.ActualSalesPrice,
                item.ExpectedSalesPrice
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
            public decimal? ProcessingCostPerUnit { get; set; }
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
                ProcessingCostPerUnit = dto.ProcessingCostPerUnit,
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
            public decimal? ProcessingCostPerUnit { get; set; }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] UpdateProcessedMaterialDto dto)
        {
            var clientId = GetClientId();
            var query = _context.ProcessedMaterials.Where(x => x.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(x => x.ClientId == clientId);
            }

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Status)) entity.Status = dto.Status;
            if (dto.ProcessingCostPerUnit.HasValue) entity.ProcessingCostPerUnit = dto.ProcessingCostPerUnit.Value;
            entity.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportProcessedMaterials(
            [FromQuery] int? materialTypeId = null,
            [FromQuery] string? qualityGrade = null,
            [FromQuery] string? location = null,
            [FromQuery] string? status = null,
            [FromQuery] string export = "csv")
        {
            var clientId = GetClientId();

            var query = _context.ProcessedMaterials
                .Include(p => p.MaterialType)
                .Include(p => p.ProcessingLot)
                .AsQueryable();

            if (!IsAdministrator())
            {
                query = query.Where(x => x.ClientId == clientId);
            }

            if (materialTypeId.HasValue) query = query.Where(x => x.MaterialTypeId == materialTypeId.Value);
            if (!string.IsNullOrWhiteSpace(qualityGrade)) query = query.Where(x => x.QualityGrade != null && x.QualityGrade.Contains(qualityGrade));
            if (!string.IsNullOrWhiteSpace(location)) query = query.Where(x => x.Location != null && x.Location.Contains(location));
            if (!string.IsNullOrWhiteSpace(status)) query = query.Where(x => x.Status == status);

            var materials = await query
                .OrderByDescending(x => x.DateCreated)
                .ToListAsync();

            if (export.ToLower() == "csv")
            {
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Description,Quantity,UnitOfMeasure,QualityGrade,Location,ProcessedWeight,WeightUnit,Status,ExpectedSalesPrice,ActualSalesPrice,SaleDate,IsHazardous,HazardousClassification,PurchaseCostPerUnit,ProcessingCostPerUnit,CertificationNumber,Notes,CreatedDate");

                foreach (var material in materials)
                {
                    csv.AppendLine($"\"{material.Description}\",{material.Quantity},\"{material.UnitOfMeasure}\",\"{material.QualityGrade}\",\"{material.Location}\",{material.ProcessedWeight},\"{material.WeightUnit}\",\"{material.Status}\",{material.ExpectedSalesPrice},{material.ActualSalesPrice},\"{material.SaleDate:yyyy-MM-dd}\",{material.IsHazardous},\"{material.HazardousClassification}\",{material.PurchaseCostPerUnit},{material.ProcessingCostPerUnit},\"{material.CertificationNumber}\",\"{material.Notes}\",\"{material.DateCreated:yyyy-MM-dd}\"");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                return File(bytes, "text/csv", $"processed_materials_export_{DateTime.UtcNow:yyyyMMdd}.csv");
            }

            return BadRequest("Unsupported export format");
        }
    }
}
