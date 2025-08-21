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
    public class VendorDocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VendorDocumentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] int? vendorId = null)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var query = _context.VendorDocuments
                .Include(x => x.Vendor)
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
                    x.DocumentUrl,
                    x.Filename,
                    x.ContentType,
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

            var item = await _context.VendorDocuments
                .Include(x => x.Vendor)
                .FirstOrDefaultAsync(x => x.Id == id && x.ClientId == clientId);

            if (item == null) return NotFound();

            return Ok(new
            {
                item.Id,
                item.VendorId,
                Vendor = item.Vendor == null ? null : new { item.Vendor.Id, item.Vendor.VendorName },
                item.DocumentUrl,
                item.Filename,
                item.ContentType,
                item.DateCreated,
                item.DateUpdated
            });
        }

        public class CreateVendorDocumentsDto
        {
            public int VendorId { get; set; }
            public string? DocumentUrl { get; set; }
            public string? Filename { get; set; }
            public string? ContentType { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVendorDocumentsDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId)) return Unauthorized();

            var entity = new VendorDocuments
            {
                VendorId = dto.VendorId,
                DocumentUrl = dto.DocumentUrl,
                Filename = dto.Filename,
                ContentType = dto.ContentType,
                ClientId = clientId,
                CreatedBy = int.Parse(userId),
                UpdatedBy = int.Parse(userId),
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            _context.VendorDocuments.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        public class UpdateVendorDocumentsDto
        {
            public string? DocumentUrl { get; set; }
            public string? Filename { get; set; }
            public string? ContentType { get; set; }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] UpdateVendorDocumentsDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId)) return Unauthorized();

            var entity = await _context.VendorDocuments.FirstOrDefaultAsync(x => x.Id == id && x.ClientId == clientId);
            if (entity == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.DocumentUrl)) entity.DocumentUrl = dto.DocumentUrl;
            if (!string.IsNullOrEmpty(dto.Filename)) entity.Filename = dto.Filename;
            if (!string.IsNullOrEmpty(dto.ContentType)) entity.ContentType = dto.ContentType;
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

            var entity = await _context.VendorDocuments.FirstOrDefaultAsync(x => x.Id == id && x.ClientId == clientId);
            if (entity == null) return NotFound();

            _context.VendorDocuments.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{vendorId:int}/upload")]
        public async Task<IActionResult> UploadDocument([FromRoute] int vendorId, IFormFile file, [FromForm] string? description = null)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId)) return Unauthorized();

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("File type not allowed");

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "vendor-documents");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("uploads", "vendor-documents", fileName).Replace("\\", "/");

            var document = new VendorDocuments
            {
                VendorId = vendorId,
                DocumentUrl = relativePath,
                Filename = file.FileName,
                ContentType = file.ContentType,
                ClientId = clientId,
                CreatedBy = int.Parse(userId),
                UpdatedBy = int.Parse(userId),
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            _context.VendorDocuments.Add(document);
            await _context.SaveChangesAsync();

            return Ok(new { 
                id = document.Id,
                documentUrl = relativePath,
                filename = file.FileName,
                contentType = file.ContentType,
                fileSize = file.Length
            });
        }
    }
}
