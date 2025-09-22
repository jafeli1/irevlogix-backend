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
    public class ComplianceTrackerDocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ComplianceTrackerDocumentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdministrator()
        {
            return User.IsInRole("Administrator");
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 25)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator()) return Unauthorized();

            var query = _context.ComplianceTrackerDocuments.AsQueryable();

            if (!IsAdministrator())
            {
                query = query.Where(x => x.ClientId == clientId);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.DocumentUrl,
                    x.Filename,
                    x.ContentType,
                    x.DocumentType,
                    x.IssueDate,
                    x.ExpirationDate,
                    x.DateReceived,
                    x.ReviewComment,
                    x.LastReviewDate,
                    x.ReviewedBy,
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

            var query = _context.ComplianceTrackerDocuments.Where(x => x.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(x => x.ClientId == clientId);
            }

            var item = await query.FirstOrDefaultAsync();
            if (item == null) return NotFound();

            return Ok(new
            {
                item.Id,
                item.DocumentUrl,
                item.Filename,
                item.ContentType,
                item.DocumentType,
                item.IssueDate,
                item.ExpirationDate,
                item.DateReceived,
                item.ReviewComment,
                item.LastReviewDate,
                item.ReviewedBy,
                item.DateCreated,
                item.DateUpdated
            });
        }

        public class CreateDto
        {
            public string? DocumentUrl { get; set; }
            public string? Filename { get; set; }
            public string? ContentType { get; set; }
            public string? DocumentType { get; set; }
            public DateTime? IssueDate { get; set; }
            public DateTime? ExpirationDate { get; set; }
            public DateTime? DateReceived { get; set; }
            public string? ReviewComment { get; set; }
            public DateTime? LastReviewDate { get; set; }
            public int? ReviewedBy { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if ((string.IsNullOrEmpty(clientId) && !IsAdministrator()) || string.IsNullOrEmpty(userId)) return Unauthorized();

            var entity = new ComplianceTrackerDocument
            {
                DocumentUrl = dto.DocumentUrl,
                Filename = dto.Filename,
                ContentType = dto.ContentType,
                DocumentType = dto.DocumentType,
                IssueDate = dto.IssueDate,
                ExpirationDate = dto.ExpirationDate,
                DateReceived = dto.DateReceived,
                ReviewComment = dto.ReviewComment,
                LastReviewDate = dto.LastReviewDate,
                ReviewedBy = dto.ReviewedBy,
                ClientId = clientId,
                CreatedBy = int.Parse(userId),
                UpdatedBy = int.Parse(userId),
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            _context.ComplianceTrackerDocuments.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        public class UpdateDto
        {
            public string? DocumentUrl { get; set; }
            public string? Filename { get; set; }
            public string? ContentType { get; set; }
            public string? DocumentType { get; set; }
            public DateTime? IssueDate { get; set; }
            public DateTime? ExpirationDate { get; set; }
            public DateTime? DateReceived { get; set; }
            public string? ReviewComment { get; set; }
            public DateTime? LastReviewDate { get; set; }
            public int? ReviewedBy { get; set; }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] UpdateDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if ((string.IsNullOrEmpty(clientId) && !IsAdministrator()) || string.IsNullOrEmpty(userId)) return Unauthorized();

            var query = _context.ComplianceTrackerDocuments.Where(x => x.Id == id);
            if (!IsAdministrator())
            {
                query = query.Where(x => x.ClientId == clientId);
            }

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.DocumentUrl)) entity.DocumentUrl = dto.DocumentUrl;
            if (!string.IsNullOrEmpty(dto.Filename)) entity.Filename = dto.Filename;
            if (!string.IsNullOrEmpty(dto.ContentType)) entity.ContentType = dto.ContentType;
            if (!string.IsNullOrEmpty(dto.DocumentType)) entity.DocumentType = dto.DocumentType;
            if (dto.IssueDate.HasValue) entity.IssueDate = dto.IssueDate;
            if (dto.ExpirationDate.HasValue) entity.ExpirationDate = dto.ExpirationDate;
            if (dto.DateReceived.HasValue) entity.DateReceived = dto.DateReceived;
            if (!string.IsNullOrEmpty(dto.ReviewComment)) entity.ReviewComment = dto.ReviewComment;
            if (dto.LastReviewDate.HasValue) entity.LastReviewDate = dto.LastReviewDate;
            if (dto.ReviewedBy.HasValue) entity.ReviewedBy = dto.ReviewedBy;

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

            var query = _context.ComplianceTrackerDocuments.Where(x => x.Id == id);
            if (!IsAdministrator())
            {
                query = query.Where(x => x.ClientId == clientId);
            }

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            _context.ComplianceTrackerDocuments.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument(IFormFile file, [FromForm] string? description = null, [FromForm] string? documentType = null, [FromForm] DateTime? issueDate = null, [FromForm] DateTime? expirationDate = null, [FromForm] DateTime? dateReceived = null, [FromForm] string? reviewComment = null, [FromForm] DateTime? lastReviewDate = null, [FromForm] int? reviewedBy = null)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if ((string.IsNullOrEmpty(clientId) && !IsAdministrator()) || string.IsNullOrEmpty(userId)) return Unauthorized();

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("File type not allowed");

            var uploadsPath = Path.Combine("upload", clientId, "ComplianceTrackerDocuments");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("upload", clientId, "ComplianceTrackerDocuments", fileName).Replace("\\", "/");

            var document = new ComplianceTrackerDocument
            {
                DocumentUrl = relativePath,
                Filename = file.FileName,
                ContentType = file.ContentType,
                DocumentType = documentType,
                IssueDate = issueDate,
                ExpirationDate = expirationDate,
                DateReceived = dateReceived,
                ReviewComment = reviewComment,
                LastReviewDate = lastReviewDate,
                ReviewedBy = reviewedBy,
                ClientId = clientId,
                CreatedBy = int.Parse(userId),
                UpdatedBy = int.Parse(userId),
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            _context.ComplianceTrackerDocuments.Add(document);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = document.Id,
                documentUrl = relativePath,
                filename = file.FileName,
                contentType = file.ContentType,
                fileSize = file.Length
            });
        [HttpPost("{id:int}/upload")]
        public async Task<IActionResult> ReplaceFile([FromRoute] int id, IFormFile file)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if ((string.IsNullOrEmpty(clientId) && !IsAdministrator()) || string.IsNullOrEmpty(userId)) return Unauthorized();

            var query = _context.ComplianceTrackerDocuments.Where(x => x.Id == id);
            if (!IsAdministrator()) query = query.Where(x => x.ClientId == clientId);
            var entity = await query.FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            if (file == null || file.Length == 0) return BadRequest("No file uploaded");

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension)) return BadRequest("File type not allowed");

            if (!string.IsNullOrEmpty(entity.DocumentUrl))
            {
                var oldPath = entity.DocumentUrl.Replace("/", Path.DirectorySeparatorChar.ToString());
                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
            }

            var uploadsPath = Path.Combine("upload", entity.ClientId, "ComplianceTrackerDocuments");
            Directory.CreateDirectory(uploadsPath);

            var newFileName = $"{Guid.NewGuid()}{fileExtension}";
            var newPath = Path.Combine(uploadsPath, newFileName);
            using (var stream = new FileStream(newPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var relativePath = Path.Combine("upload", entity.ClientId, "ComplianceTrackerDocuments", newFileName).Replace("\\", "/");

            entity.DocumentUrl = relativePath;
            entity.Filename = file.FileName;
            entity.ContentType = file.ContentType;
            entity.UpdatedBy = int.Parse(userId!);
            entity.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { id = entity.Id, documentUrl = entity.DocumentUrl, filename = entity.Filename, contentType = entity.ContentType });
        }

        [HttpDelete("{id:int}/file")]
        public async Task<IActionResult> DeleteFile([FromRoute] int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if ((string.IsNullOrEmpty(clientId) && !IsAdministrator()) || string.IsNullOrEmpty(userId)) return Unauthorized();

            var query = _context.ComplianceTrackerDocuments.Where(x => x.Id == id);
            if (!IsAdministrator()) query = query.Where(x => x.ClientId == clientId);
            var entity = await query.FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            if (!string.IsNullOrEmpty(entity.DocumentUrl))
            {
                var path = entity.DocumentUrl.Replace("/", Path.DirectorySeparatorChar.ToString());
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }

            entity.DocumentUrl = null;
            entity.Filename = null;
            entity.ContentType = null;
            entity.UpdatedBy = int.Parse(userId!);
            entity.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        }
    }
}
