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
            return User.FindFirst("ClientId")?.Value ?? throw new UnauthorizedAccessException("ClientId not found in token");
        }

        private bool IsAdministrator()
        {
            return User.IsInRole("Administrator");
        }

        public VendorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] string? name, [FromQuery] int? materialTypeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var clientId = GetClientId();

            var q = _context.Vendors.AsQueryable();

            if (!IsAdministrator())
            {
                q = q.Where(x => x.ClientId == clientId);
            }

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

            var query = _context.Vendors.Where(x => x.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(x => x.ClientId == clientId);
            }

            var v = await query.FirstOrDefaultAsync();
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
                v.VendorRating,
                v.VendorTier,
                v.UpstreamTierVendor
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
            public string VendorTier { get; set; }
            public int? UpstreamTierVendor { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVendorDto dto)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int userId = 1;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsed)) userId = parsed;
            var clientId = GetClientId();

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
                VendorTier = dto.VendorTier,
                UpstreamTierVendor = dto.UpstreamTierVendor,
                ClientId = clientId,
                CreatedBy = userId,
                UpdatedBy = userId,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            _context.Vendors.Add(v);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = v.Id }, new { v.Id });
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportVendors(
            [FromQuery] string? name = null,
            [FromQuery] int? materialTypeId = null,
            [FromQuery] string export = "csv")
        {
            var clientId = GetClientId();

            var query = _context.Vendors.AsQueryable();

            if (!IsAdministrator())
            {
                query = query.Where(x => x.ClientId == clientId);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(v => v.VendorName.Contains(name));
            }

            var vendors = await query
                .OrderByDescending(x => x.DateCreated)
                .ToListAsync();

            if (export.ToLower() == "csv")
            {
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("VendorName,ContactPerson,Email,Phone,Address,City,State,PostalCode,Country,MaterialsOfInterest,PaymentTerms,VendorRating,VendorTier,UpstreamTierVendor,CreatedDate");

                foreach (var vendor in vendors)
                {
                    csv.AppendLine($"\"{vendor.VendorName}\",\"{vendor.ContactPerson}\",\"{vendor.Email}\",\"{vendor.Phone}\",\"{vendor.Address}\",\"{vendor.City}\",\"{vendor.State}\",\"{vendor.PostalCode}\",\"{vendor.Country}\",\"{vendor.MaterialsOfInterest}\",\"{vendor.PaymentTerms}\",{vendor.VendorRating},\"{vendor.VendorTier}\",{vendor.UpstreamTierVendor},\"{vendor.DateCreated:yyyy-MM-dd}\"");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                return File(bytes, "text/csv", $"vendors_export_{DateTime.UtcNow:yyyyMMdd}.csv");
            }

            return BadRequest("Unsupported export format");
        }
    }
}
