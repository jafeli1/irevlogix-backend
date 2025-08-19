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
    public class ProcessedMaterialSalesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProcessedMaterialSalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 25)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var query = _context.ProcessedMaterialSales
                .Include(x => x.ProcessedMaterial)
                .ThenInclude(x => x.MaterialType)
                .Where(x => x.ClientId == clientId);

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
                    x.VendorId,
                    x.SalesQuantity,
                    x.AgreedPricePerUnit,
                    x.ShipmentDate,
                    x.Carrier,
                    x.TrackingNumber,
                    x.FreightCost,
                    x.LoadingCost,
                    x.InvoiceId,
                    x.InvoiceDate,
                    x.DateInvoicePaid,
                    x.InvoiceTotal,
                    x.InvoiceStatus
                })
                .ToListAsync();

            return Ok(new { items, totalCount, page, pageSize });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var item = await _context.ProcessedMaterialSales
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
                item.VendorId,
                item.SalesQuantity,
                item.AgreedPricePerUnit,
                item.ShipmentDate,
                item.Carrier,
                item.TrackingNumber,
                item.FreightCost,
                item.LoadingCost,
                item.InvoiceId,
                item.InvoiceDate,
                item.DateInvoicePaid,
                item.InvoiceTotal,
                item.InvoiceStatus
            });
        }

        public class CreateProcessedMaterialSalesDto
        {
            public int ProcessedMaterialId { get; set; }
            public int? VendorId { get; set; }
            public decimal? SalesQuantity { get; set; }
            public decimal? AgreedPricePerUnit { get; set; }
            public DateTime? ShipmentDate { get; set; }
            public string? Carrier { get; set; }
            public string? TrackingNumber { get; set; }
            public decimal? FreightCost { get; set; }
            public decimal? LoadingCost { get; set; }
            public string? InvoiceId { get; set; }
            public DateTime? InvoiceDate { get; set; }
            public DateTime? DateInvoicePaid { get; set; }
            public decimal? InvoiceTotal { get; set; }
            public string? InvoiceStatus { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProcessedMaterialSalesDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId)) return Unauthorized();

            var entity = new ProcessedMaterialSales
            {
                ProcessedMaterialId = dto.ProcessedMaterialId,
                VendorId = dto.VendorId,
                SalesQuantity = dto.SalesQuantity,
                AgreedPricePerUnit = dto.AgreedPricePerUnit,
                ShipmentDate = dto.ShipmentDate,
                Carrier = dto.Carrier,
                TrackingNumber = dto.TrackingNumber,
                FreightCost = dto.FreightCost,
                LoadingCost = dto.LoadingCost,
                InvoiceId = dto.InvoiceId,
                InvoiceDate = dto.InvoiceDate,
                DateInvoicePaid = dto.DateInvoicePaid,
                InvoiceTotal = dto.InvoiceTotal,
                InvoiceStatus = dto.InvoiceStatus,
                ClientId = clientId,
                CreatedBy = int.Parse(userId),
                UpdatedBy = int.Parse(userId),
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            _context.ProcessedMaterialSales.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        public class UpdateProcessedMaterialSalesDto
        {
            public int? VendorId { get; set; }
            public decimal? SalesQuantity { get; set; }
            public decimal? AgreedPricePerUnit { get; set; }
            public DateTime? ShipmentDate { get; set; }
            public string? Carrier { get; set; }
            public string? TrackingNumber { get; set; }
            public decimal? FreightCost { get; set; }
            public decimal? LoadingCost { get; set; }
            public string? InvoiceId { get; set; }
            public DateTime? InvoiceDate { get; set; }
            public DateTime? DateInvoicePaid { get; set; }
            public decimal? InvoiceTotal { get; set; }
            public string? InvoiceStatus { get; set; }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] UpdateProcessedMaterialSalesDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var entity = await _context.ProcessedMaterialSales.FirstOrDefaultAsync(x => x.Id == id && x.ClientId == clientId);
            if (entity == null) return NotFound();

            if (dto.VendorId.HasValue) entity.VendorId = dto.VendorId;
            if (dto.SalesQuantity.HasValue) entity.SalesQuantity = dto.SalesQuantity;
            if (dto.AgreedPricePerUnit.HasValue) entity.AgreedPricePerUnit = dto.AgreedPricePerUnit;
            if (dto.ShipmentDate.HasValue) entity.ShipmentDate = dto.ShipmentDate;
            if (!string.IsNullOrWhiteSpace(dto.Carrier)) entity.Carrier = dto.Carrier;
            if (!string.IsNullOrWhiteSpace(dto.TrackingNumber)) entity.TrackingNumber = dto.TrackingNumber;
            if (dto.FreightCost.HasValue) entity.FreightCost = dto.FreightCost;
            if (dto.LoadingCost.HasValue) entity.LoadingCost = dto.LoadingCost;
            if (!string.IsNullOrWhiteSpace(dto.InvoiceId)) entity.InvoiceId = dto.InvoiceId;
            if (dto.InvoiceDate.HasValue) entity.InvoiceDate = dto.InvoiceDate;
            if (dto.DateInvoicePaid.HasValue) entity.DateInvoicePaid = dto.DateInvoicePaid;
            if (dto.InvoiceTotal.HasValue) entity.InvoiceTotal = dto.InvoiceTotal;
            if (!string.IsNullOrWhiteSpace(dto.InvoiceStatus)) entity.InvoiceStatus = dto.InvoiceStatus;
            entity.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var entity = await _context.ProcessedMaterialSales.FirstOrDefaultAsync(x => x.Id == id && x.ClientId == clientId);
            if (entity == null) return NotFound();

            _context.ProcessedMaterialSales.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
