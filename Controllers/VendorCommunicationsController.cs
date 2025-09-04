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
    public class VendorCommunicationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VendorCommunicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdministrator()
        {
            return User.IsInRole("Administrator");
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] int? vendorId = null)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator()) return Unauthorized();

            var query = _context.VendorCommunications
                .Include(x => x.Vendor)
                .AsQueryable();

            if (!IsAdministrator())
            {
                query = query.Where(x => x.ClientId == clientId);
            }

            if (vendorId.HasValue)
            {
                query = query.Where(x => x.VendorId == vendorId.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.Date ?? x.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.VendorId,
                    Vendor = x.Vendor == null ? null : new { x.Vendor.Id, x.Vendor.VendorName },
                    x.Date,
                    x.Type,
                    x.Summary,
                    x.NextSteps,
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
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator()) return Unauthorized();

            var query = _context.VendorCommunications
                .Include(x => x.Vendor)
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
                item.VendorId,
                Vendor = item.Vendor == null ? null : new { item.Vendor.Id, item.Vendor.VendorName },
                item.Date,
                item.Type,
                item.Summary,
                item.NextSteps,
                item.DateCreated,
                item.DateUpdated
            });
        }

        public class CreateVendorCommunicationsDto
        {
            public int VendorId { get; set; }
            public DateTime? Date { get; set; }
            public string? Type { get; set; }
            public string? Summary { get; set; }
            public string? NextSteps { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVendorCommunicationsDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId)) return Unauthorized();

            var entity = new VendorCommunications
            {
                VendorId = dto.VendorId,
                Date = dto.Date,
                Type = dto.Type,
                Summary = dto.Summary,
                NextSteps = dto.NextSteps,
                ClientId = clientId,
                CreatedBy = int.Parse(userId),
                UpdatedBy = int.Parse(userId),
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            _context.VendorCommunications.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        public class UpdateVendorCommunicationsDto
        {
            public DateTime? Date { get; set; }
            public string? Type { get; set; }
            public string? Summary { get; set; }
            public string? NextSteps { get; set; }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] UpdateVendorCommunicationsDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId)) return Unauthorized();

            var query = _context.VendorCommunications.Where(x => x.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(x => x.ClientId == clientId);
            }

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            if (dto.Date.HasValue) entity.Date = dto.Date;
            if (!string.IsNullOrEmpty(dto.Type)) entity.Type = dto.Type;
            if (!string.IsNullOrEmpty(dto.Summary)) entity.Summary = dto.Summary;
            if (dto.NextSteps != null) entity.NextSteps = dto.NextSteps;
            entity.UpdatedBy = int.Parse(userId);
            entity.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator()) return Unauthorized();

            var query = _context.VendorCommunications.Where(x => x.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(x => x.ClientId == clientId);
            }

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            _context.VendorCommunications.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
