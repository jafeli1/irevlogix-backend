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
    public class VendorContractsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VendorContractsController(ApplicationDbContext context)
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

            var query = _context.VendorContracts
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
                .OrderByDescending(x => x.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.VendorId,
                    Vendor = x.Vendor == null ? null : new { x.Vendor.Id, x.Vendor.VendorName },
                    x.DocumentUrl,
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
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator()) return Unauthorized();

            var query = _context.VendorContracts
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
                item.DocumentUrl,
                item.EffectiveStartDate,
                item.EffectiveEndDate,
                item.DateCreated,
                item.DateUpdated
            });
        }

        public class CreateVendorContractDto
        {
            public int VendorId { get; set; }
            public string? DocumentUrl { get; set; }
            public DateTime? EffectiveStartDate { get; set; }
            public DateTime? EffectiveEndDate { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVendorContractDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId)) return Unauthorized();

            var entity = new VendorContract
            {
                VendorId = dto.VendorId,
                DocumentUrl = dto.DocumentUrl,
                EffectiveStartDate = dto.EffectiveStartDate,
                EffectiveEndDate = dto.EffectiveEndDate,
                ClientId = clientId,
                CreatedBy = int.Parse(userId),
                UpdatedBy = int.Parse(userId),
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            _context.VendorContracts.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        public class UpdateVendorContractDto
        {
            public string? DocumentUrl { get; set; }
            public DateTime? EffectiveStartDate { get; set; }
            public DateTime? EffectiveEndDate { get; set; }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] UpdateVendorContractDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId)) return Unauthorized();

            var query = _context.VendorContracts.Where(x => x.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(x => x.ClientId == clientId);
            }

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.DocumentUrl)) entity.DocumentUrl = dto.DocumentUrl;
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
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator()) return Unauthorized();

            var query = _context.VendorContracts.Where(x => x.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(x => x.ClientId == clientId);
            }

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            _context.VendorContracts.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Invalid file type. Allowed types: PDF, DOC, DOCX, JPG, JPEG, PNG");

            var uploadsPath = Path.Combine("upload", clientId, "VendorContracts");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("upload", clientId, "VendorContracts", fileName).Replace("\\", "/");
            return Ok(new { filePath = relativePath });
        }
    }
}
