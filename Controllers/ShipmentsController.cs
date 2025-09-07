using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using irevlogix_backend.Data;
using irevlogix_backend.Models;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ShipmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShipmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdministrator()
        {
            return User.IsInRole("Administrator");
        }

        private string GetClientId()
        {
            return User.FindFirst("ClientId")?.Value ?? "ADMIN_CLIENT_001";
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shipment>>> GetShipments(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] DateTime? actualStartDate = null,
            [FromQuery] DateTime? actualEndDate = null,
            [FromQuery] int? materialTypeId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var clientId = GetClientId();
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator())
                return Unauthorized();

            var query = _context.Shipments
                .Include(s => s.OriginatorClient)
                .Include(s => s.ReverseRequest)
                .Include(s => s.ShipmentItems)
                .AsQueryable();

            if (!IsAdministrator())
            {
                query = query.Where(s => s.ClientId == clientId);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.ShipmentNumber.Contains(search) ||
                                        (s.OriginatorClient != null && s.OriginatorClient.CompanyName.Contains(search)));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(s => s.Status == status);
            }

            if (startDate.HasValue)
            {
                query = query.Where(s => s.ScheduledPickupDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(s => s.ScheduledPickupDate <= endDate.Value);
            }

            if (actualStartDate.HasValue)
            {
                query = query.Where(s => s.ActualPickupDate >= actualStartDate.Value);
            }

            if (actualEndDate.HasValue)
            {
                query = query.Where(s => s.ActualPickupDate <= actualEndDate.Value);
            }

            if (materialTypeId.HasValue)
            {
                query = query.Where(s => s.ShipmentItems.Any(si => si.MaterialTypeId == materialTypeId.Value));
            }

            var totalCount = await query.CountAsync();
            var shipments = await query
                .OrderByDescending(s => s.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                data = shipments,
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Shipment>> GetShipment(int id)
        {
            var clientId = GetClientId();
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator())
                return Unauthorized();

            var query = _context.Shipments
                .Where(s => s.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(s => s.ClientId == clientId);
            }

            var shipment = await query
                .Include(s => s.OriginatorClient)
                .Include(s => s.ReverseRequest)
                .Include(s => s.ShipmentItems)
                    .ThenInclude(si => si.MaterialType)
                .Include(s => s.ShipmentItems)
                    .ThenInclude(si => si.AssetCategory)
                .FirstOrDefaultAsync();

            if (shipment == null)
                return NotFound();

            return shipment;
        }

        [HttpPost]
        public async Task<ActionResult<Shipment>> CreateShipment(Shipment shipment)
        {
            var clientId = GetClientId();
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (userId == 0)
                return Unauthorized();

            shipment.ClientId = clientId;
            shipment.CreatedBy = userId;
            shipment.UpdatedBy = userId;
            shipment.DateCreated = DateTime.UtcNow;
            shipment.DateUpdated = DateTime.UtcNow;

            if (string.IsNullOrEmpty(shipment.ShipmentNumber))
            {
                var count = await _context.Shipments.CountAsync(s => s.ClientId == clientId);
                shipment.ShipmentNumber = $"SH-{DateTime.UtcNow:yyyyMMdd}-{(count + 1):D4}";
            }

            foreach (var item in shipment.ShipmentItems)
            {
                item.ClientId = clientId;
                item.CreatedBy = userId;
                item.UpdatedBy = userId;
                item.DateCreated = DateTime.UtcNow;
                item.DateUpdated = DateTime.UtcNow;
            }

            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShipment), new { id = shipment.Id }, shipment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShipment(int id, Shipment shipment)
        {
            var clientId = GetClientId();
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (userId == 0)
                return Unauthorized();

            if (id != shipment.Id)
                return BadRequest();

            var query = _context.Shipments
                .Where(s => s.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(s => s.ClientId == clientId);
            }

            var existingShipment = await query.FirstOrDefaultAsync();

            if (existingShipment == null)
                return NotFound();

            shipment.ClientId = clientId;
            shipment.UpdatedBy = userId;
            shipment.DateUpdated = DateTime.UtcNow;

            _context.Entry(existingShipment).CurrentValues.SetValues(shipment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShipment(int id)
        {
            var clientId = GetClientId();
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator())
                return Unauthorized();

            var query = _context.Shipments
                .Where(s => s.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(s => s.ClientId == clientId);
            }

            var shipment = await query.FirstOrDefaultAsync();

            if (shipment == null)
                return NotFound();

            _context.Shipments.Remove(shipment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{shipmentId}/items/{itemId}")]
        public async Task<IActionResult> UpdateShipmentItem(int shipmentId, int itemId, ShipmentItemUpdateRequest request)
        {
            var clientId = GetClientId();
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (userId == 0)
                return Unauthorized();

            var query = _context.ShipmentItems
                .Where(si => si.Id == itemId && si.ShipmentId == shipmentId);

            if (!IsAdministrator())
            {
                query = query.Where(si => si.ClientId == clientId);
            }

            var shipmentItem = await query.FirstOrDefaultAsync();

            if (shipmentItem == null)
                return NotFound();

            shipmentItem.ProcessingLotId = request.ProcessingLotId ?? shipmentItem.ProcessingLotId;
            shipmentItem.DispositionMethod = request.DispositionMethod ?? shipmentItem.DispositionMethod;
            shipmentItem.DispositionCost = request.DispositionCost ?? shipmentItem.DispositionCost;
            shipmentItem.ProcessingStatus = request.ProcessingStatus ?? shipmentItem.ProcessingStatus;
            shipmentItem.UpdatedBy = userId;
            shipmentItem.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/status-history")]
        public async Task<ActionResult<IEnumerable<ShipmentStatusHistory>>> GetShipmentStatusHistory(int id)
        {
            var clientId = GetClientId();
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator())
                return Unauthorized();

            var query = _context.Shipments
                .Where(s => s.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(s => s.ClientId == clientId);
            }

            var shipment = await query.FirstOrDefaultAsync();

            if (shipment == null)
                return NotFound();

            var historyQuery = _context.ShipmentStatusHistories
                .Where(h => h.ShipmentId == id);

            if (!IsAdministrator())
            {
                historyQuery = historyQuery.Where(h => h.ClientId == clientId);
            }

            var history = await historyQuery
                .Include(h => h.User)
                .OrderByDescending(h => h.Timestamp)
                .ToListAsync();

            return Ok(history);
        }

        [HttpPost("{id}/status-history")]
        public async Task<ActionResult> AddStatusHistory(int id, AddStatusHistoryRequest request)
        {
            var clientId = GetClientId();
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (userId == 0)
                return Unauthorized();

            var query = _context.Shipments
                .Where(s => s.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(s => s.ClientId == clientId);
            }

            var shipment = await query.FirstOrDefaultAsync();

            if (shipment == null)
                return NotFound();

            var history = new ShipmentStatusHistory
            {
                ShipmentId = id,
                FromStatus = shipment.Status,
                ToStatus = request.ToStatus,
                Notes = request.Notes,
                ActionType = request.ActionType,
                ClientId = clientId,
                CreatedBy = userId,
                UpdatedBy = userId
            };

            shipment.Status = request.ToStatus;
            shipment.UpdatedBy = userId;
            shipment.DateUpdated = DateTime.UtcNow;

            _context.ShipmentStatusHistories.Add(history);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id}/documents")]
        public async Task<ActionResult<IEnumerable<ShipmentDocument>>> GetShipmentDocuments(int id)
        {
            var clientId = GetClientId();
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator())
                return Unauthorized();

            var query = _context.Shipments
                .Where(s => s.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(s => s.ClientId == clientId);
            }

            var shipment = await query.FirstOrDefaultAsync();

            if (shipment == null)
                return NotFound();

            var documentsQuery = _context.ShipmentDocuments
                .Where(d => d.ShipmentId == id);

            if (!IsAdministrator())
            {
                documentsQuery = documentsQuery.Where(d => d.ClientId == clientId);
            }

            var documents = await documentsQuery
                .OrderByDescending(d => d.DateCreated)
                .ToListAsync();

            return Ok(documents);
        }

        [HttpPost("{id}/documents")]
        public async Task<ActionResult> UploadDocument(int id, IFormFile file, [FromForm] string? description = null)
        {
            var clientId = GetClientId();
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized();

            var query = _context.Shipments
                .Where(s => s.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(s => s.ClientId == clientId);
            }

            var shipment = await query.FirstOrDefaultAsync();

            if (shipment == null)
                return NotFound();

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var uploadsPath = Path.Combine("upload", clientId, "Shipments", id.ToString());
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new ShipmentDocument
            {
                ShipmentId = id,
                FileName = file.FileName,
                FilePath = filePath,
                ContentType = file.ContentType,
                FileSize = file.Length,
                Description = description,
                ClientId = clientId,
                CreatedBy = userId,
                UpdatedBy = userId
            };

            _context.ShipmentDocuments.Add(document);
            await _context.SaveChangesAsync();

            return Ok(document);
        }

        [HttpDelete("{id}/documents/{documentId}")]
        public async Task<IActionResult> DeleteDocument(int id, int documentId)
        {
            var clientId = GetClientId();
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized();

            var document = await _context.ShipmentDocuments
                .Where(d => d.Id == documentId && d.ShipmentId == id && d.ClientId == clientId)
                .FirstOrDefaultAsync();

            if (document == null)
                return NotFound();

            if (System.IO.File.Exists(document.FilePath))
            {
                System.IO.File.Delete(document.FilePath);
            }

            _context.ShipmentDocuments.Remove(document);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/documents/{documentId}/download")]
        public async Task<IActionResult> DownloadDocument(int id, int documentId)
        {
            var clientId = GetClientId();
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized();

            var document = await _context.ShipmentDocuments
                .Where(d => d.Id == documentId && d.ShipmentId == id && d.ClientId == clientId)
                .FirstOrDefaultAsync();

            if (document == null)
                return NotFound();

            if (!System.IO.File.Exists(document.FilePath))
                return NotFound("File not found on disk");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(document.FilePath);
            return File(fileBytes, document.ContentType ?? "application/octet-stream", document.FileName);
        }

        [HttpGet("originators")]
        public async Task<ActionResult<IEnumerable<object>>> GetOriginators()
        {
            var clientId = GetClientId();
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator())
                return Unauthorized();

            var query = _context.Shipments
                .Where(s => s.OriginatorClient != null);

            if (!IsAdministrator())
            {
                query = query.Where(s => s.ClientId == clientId);
            }

            var originators = await query
                .Select(s => new { 
                    Id = s.OriginatorClient!.Id, 
                    Name = s.OriginatorClient.CompanyName 
                })
                .Distinct()
                .OrderBy(o => o.Name)
                .ToListAsync();

            return Ok(originators);
        }
    }

    public class ShipmentItemUpdateRequest
    {
        public int? ProcessingLotId { get; set; }
        public string? DispositionMethod { get; set; }
        public decimal? DispositionCost { get; set; }
        public string? ProcessingStatus { get; set; }
    }

    public class AddStatusHistoryRequest
    {
        public string ToStatus { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? ActionType { get; set; }
    }
}
