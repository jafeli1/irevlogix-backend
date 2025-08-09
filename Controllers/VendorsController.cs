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
    public class VendorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private string GetClientId()
        {
            return User.FindFirst("ClientId")?.Value ?? string.Empty;
        }

        public VendorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] string? name, [FromQuery] int? materialTypeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var clientId = GetClientId();

            var q = _context.Vendors.Where(x => x.ClientId == clientId).AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                q = q.Where(v => v.VendorName.Contains(name));
            }

            var total = await q.CountAsync();
            Response.Headers["X-Total-Count"] = total.ToString();

            var items = await q
                .OrderByDescending(x => x.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new
                {
                    v.Id,
                    v.VendorName,
                    v.ContactPerson,
                    MaterialsPurchased = "",
                    LastSaleDate = (DateTime?)null,
                    v.VendorRating
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var clientId = GetClientId();

            var v = await _context.Vendors.FirstOrDefaultAsync(x => x.Id == id && (string.IsNullOrEmpty(clientId) || x.ClientId == clientId));
            if (v == null) return NotFound();

            return Ok(new
            {
                v.Id,
                v.VendorName,
                v.ContactPerson,
                v.Email,
                v.Phone,
                v.Address,
                v.City,
                v.State,
                v.PostalCode,
                v.Country,
                v.MaterialsOfInterest,
                v.PaymentTerms,
                v.VendorRating
            });
        }

        public class CreateVendorDto
        {
            public string VendorName { get; set; }
            public string ContactPerson { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string PostalCode { get; set; }
            public string Country { get; set; }
            public string MaterialsOfInterest { get; set; }
            public string PaymentTerms { get; set; }
            public decimal? VendorRating { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVendorDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var clientId = GetClientId();

            int userIdInt = 0;
            if (!string.IsNullOrEmpty(userId))
            {
                _ = int.TryParse(userId, out userIdInt);
            }

            var v = new Vendor
            {
                VendorName = dto.VendorName,
                ContactPerson = dto.ContactPerson,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                MaterialsOfInterest = dto.MaterialsOfInterest,
                PaymentTerms = dto.PaymentTerms,
                VendorRating = dto.VendorRating,
                ClientId = clientId,
                CreatedBy = userIdInt,
                UpdatedBy = userIdInt,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            _context.Vendors.Add(v);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = v.Id }, new { v.Id });
        }
    }
}
