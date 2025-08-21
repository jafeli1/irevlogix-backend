using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Models;
using System.Security.Claims;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VendorPricingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VendorPricingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] int? vendorId = null)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var query = _context.VendorPricing
                .Include(x => x.Vendor)
                .Include(x => x.MaterialType)
                .Where(x => x.ClientId == clientId);

            if (vendorId.HasValue)
            {
                query = query.Where(x => x.VendorId == vendorId.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.VendorId,
                    Vendor = x.Vendor == null ? null : new { x.Vendor.Id, x.Vendor.VendorName },
                    x.MaterialTypeId,
                    MaterialType = x.MaterialType == null ? null : new { x.MaterialType.Id, x.MaterialType.Name },
                    x.PricePerUnit,
                    x.EffectiveStartDate,
                    x.EffectiveEndDate,
                    x.DateCreated,
                    x.DateUpdated
                })
                .ToListAsync();

            return Ok(new { items, totalCount, page, pageSize });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var item = await _context.VendorPricing
                .Include(x => x.Vendor)
                .Include(x => x.MaterialType)
                .FirstOrDefaultAsync(x => x.Id == id && x.ClientId == clientId);

            if (item == null) return NotFound();

            return Ok(new
            {
                item.Id,
                item.VendorId,
                Vendor = item.Vendor == null ? null : new { item.Vendor.Id, item.Vendor.VendorName },
                item.MaterialTypeId,
                MaterialType = item.MaterialType == null ? null : new { item.MaterialType.Id, item.MaterialType.Name },
                item.PricePerUnit,
                item.EffectiveStartDate,
                item.EffectiveEndDate,
                item.DateCreated,
                item.DateUpdated
            });
        }

        public class CreateVendorPricingDto
        {
            public int VendorId { get; set; }
            public int? MaterialTypeId { get; set; }
            public decimal? PricePerUnit { get; set; }
            public DateTime? EffectiveStartDate { get; set; }
            public DateTime? EffectiveEndDate { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVendorPricingDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId)) return Unauthorized();

            var entity = new VendorPricing
            {
                VendorId = dto.VendorId,
                MaterialTypeId = dto.MaterialTypeId,
                PricePerUnit = dto.PricePerUnit,
                EffectiveStartDate = dto.EffectiveStartDate,
                EffectiveEndDate = dto.EffectiveEndDate,
                ClientId = clientId,
                CreatedBy = int.Parse(userId),
                UpdatedBy = int.Parse(userId),
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            _context.VendorPricing.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        public class UpdateVendorPricingDto
        {
            public int? MaterialTypeId { get; set; }
            public decimal? PricePerUnit { get; set; }
            public DateTime? EffectiveStartDate { get; set; }
            public DateTime? EffectiveEndDate { get; set; }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] UpdateVendorPricingDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId)) return Unauthorized();

            var entity = await _context.VendorPricing.FirstOrDefaultAsync(x => x.Id == id && x.ClientId == clientId);
            if (entity == null) return NotFound();

            if (dto.MaterialTypeId.HasValue) entity.MaterialTypeId = dto.MaterialTypeId;
            if (dto.PricePerUnit.HasValue) entity.PricePerUnit = dto.PricePerUnit;
            if (dto.EffectiveStartDate.HasValue) entity.EffectiveStartDate = dto.EffectiveStartDate;
            if (dto.EffectiveEndDate.HasValue) entity.EffectiveEndDate = dto.EffectiveEndDate;
            entity.UpdatedBy = int.Parse(userId);
            entity.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var entity = await _context.VendorPricing.FirstOrDefaultAsync(x => x.Id == id && x.ClientId == clientId);
            if (entity == null) return NotFound();

            _context.VendorPricing.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
