using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shipment>>> GetShipments(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized();

            var query = _context.Shipments
                .Where(s => s.ClientId == clientId)
                .Include(s => s.OriginatorClient)
                .Include(s => s.ClientContact)
                .Include(s => s.ShipmentItems)
                .AsQueryable();

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
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized();

            var shipment = await _context.Shipments
                .Where(s => s.Id == id && s.ClientId == clientId)
                .Include(s => s.OriginatorClient)
                .Include(s => s.ClientContact)
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
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst("UserId")?.Value;
            
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId))
                return Unauthorized();

            shipment.ClientId = clientId;
            shipment.CreatedBy = int.Parse(userId);
            shipment.UpdatedBy = int.Parse(userId);
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
                item.CreatedBy = int.Parse(userId);
                item.UpdatedBy = int.Parse(userId);
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
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst("UserId")?.Value;
            
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (id != shipment.Id)
                return BadRequest();

            var existingShipment = await _context.Shipments
                .Where(s => s.Id == id && s.ClientId == clientId)
                .FirstOrDefaultAsync();

            if (existingShipment == null)
                return NotFound();

            shipment.ClientId = clientId;
            shipment.UpdatedBy = int.Parse(userId);
            shipment.DateUpdated = DateTime.UtcNow;

            _context.Entry(existingShipment).CurrentValues.SetValues(shipment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShipment(int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized();

            var shipment = await _context.Shipments
                .Where(s => s.Id == id && s.ClientId == clientId)
                .FirstOrDefaultAsync();

            if (shipment == null)
                return NotFound();

            _context.Shipments.Remove(shipment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
