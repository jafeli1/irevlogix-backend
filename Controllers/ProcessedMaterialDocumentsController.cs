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
    public class ProcessedMaterialDocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProcessedMaterialDocumentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] int? processedMaterialId = null)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var query = _context.ProcessedMaterialDocuments
                .Include(x => x.ProcessedMaterial)
                .ThenInclude(x => x.MaterialType)
                .Where(x => x.ClientId == clientId);

            if (processedMaterialId.HasValue)
            {
                query = query.Where(x => x.ProcessedMaterialId == processedMaterialId.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.ProcessedMaterialId,
                    ProcessedMaterial = x.ProcessedMaterial == null ? null : new
                    {
                        x.ProcessedMaterial.Id,
                        x.ProcessedMaterial.Description,
                        MaterialType = x.ProcessedMaterial.MaterialType == null ? null : new
                        {
                            x.ProcessedMaterial.MaterialType.Id,
                            x.ProcessedMaterial.MaterialType.Name
                        }
                    },
                    x.FileName,
                    x.FilePath,
                    x.ContentType,
                    x.FileSize,
                    x.Description,
                    x.DocumentType,
                    x.DateCreated
                })
                .ToListAsync();

            return Ok(new { items, totalCount, page, pageSize });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var item = await _context.ProcessedMaterialDocuments
                .Include(x => x.ProcessedMaterial)
                .ThenInclude(x => x.MaterialType)
                .FirstOrDefaultAsync(x => x.Id == id && x.ClientId == clientId);

            if (item == null) return NotFound();

            return Ok(new
            {
                item.Id,
                item.ProcessedMaterialId,
                ProcessedMaterial = item.ProcessedMaterial == null ? null : new
                {
                    item.ProcessedMaterial.Id,
                    item.ProcessedMaterial.Description,
                    MaterialType = item.ProcessedMaterial.MaterialType == null ? null : new
                    {
                        item.ProcessedMaterial.MaterialType.Id,
                        item.ProcessedMaterial.MaterialType.Name
                    }
                },
                item.FileName,
                item.FilePath,
                item.ContentType,
                item.FileSize,
                item.Description,
                item.DocumentType,
                item.DateCreated
            });
        }

        public class CreateProcessedMaterialDocumentsDto
        {
            public int ProcessedMaterialId { get; set; }
            public string FileName { get; set; } = string.Empty;
            public string FilePath { get; set; } = string.Empty;
            public string? ContentType { get; set; }
            public long FileSize { get; set; }
            public string? Description { get; set; }
            public string? DocumentType { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProcessedMaterialDocumentsDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId)) return Unauthorized();

            var entity = new ProcessedMaterialDocuments
            {
                ProcessedMaterialId = dto.ProcessedMaterialId,
                FileName = dto.FileName,
                FilePath = dto.FilePath,
                ContentType = dto.ContentType,
                FileSize = dto.FileSize,
                Description = dto.Description,
                DocumentType = dto.DocumentType,
                ClientId = clientId,
                CreatedBy = int.Parse(userId),
                UpdatedBy = int.Parse(userId),
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            _context.ProcessedMaterialDocuments.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        public class UpdateProcessedMaterialDocumentsDto
        {
            public string? FileName { get; set; }
            public string? FilePath { get; set; }
            public string? ContentType { get; set; }
            public long? FileSize { get; set; }
            public string? Description { get; set; }
            public string? DocumentType { get; set; }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] UpdateProcessedMaterialDocumentsDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var entity = await _context.ProcessedMaterialDocuments.FirstOrDefaultAsync(x => x.Id == id && x.ClientId == clientId);
            if (entity == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.FileName)) entity.FileName = dto.FileName;
            if (!string.IsNullOrWhiteSpace(dto.FilePath)) entity.FilePath = dto.FilePath;
            if (!string.IsNullOrWhiteSpace(dto.ContentType)) entity.ContentType = dto.ContentType;
            if (dto.FileSize.HasValue) entity.FileSize = dto.FileSize.Value;
            if (!string.IsNullOrWhiteSpace(dto.Description)) entity.Description = dto.Description;
            if (!string.IsNullOrWhiteSpace(dto.DocumentType)) entity.DocumentType = dto.DocumentType;
            entity.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var entity = await _context.ProcessedMaterialDocuments.FirstOrDefaultAsync(x => x.Id == id && x.ClientId == clientId);
            if (entity == null) return NotFound();

            _context.ProcessedMaterialDocuments.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Invalid file type. Allowed types: PDF, DOC, DOCX, JPG, JPEG, PNG");

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "processed-material-documents");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("uploads", "processed-material-documents", fileName).Replace("\\", "/");
            return Ok(new { 
                filePath = relativePath,
                fileName = file.FileName,
                contentType = file.ContentType,
                fileSize = file.Length
            });
        }
    }
}
