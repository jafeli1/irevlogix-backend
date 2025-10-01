using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Models;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecyclersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecyclersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetRecyclers(
            [FromQuery] string? companyName = null,
            [FromQuery] string? materialTypesHandled = null,
            [FromQuery] string? certificationType = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Recyclers.AsQueryable();

            if (!string.IsNullOrEmpty(companyName))
            {
                query = query.Where(r => r.CompanyName.Contains(companyName));
            }

            if (!string.IsNullOrEmpty(materialTypesHandled))
            {
                query = query.Where(r => r.MaterialTypesHandled != null && r.MaterialTypesHandled.Contains(materialTypesHandled));
            }

            if (!string.IsNullOrEmpty(certificationType))
            {
                query = query.Where(r => r.CertificationType != null && r.CertificationType.Contains(certificationType));
            }

            var totalCount = await query.CountAsync();
            
            var recyclers = await query
                .OrderBy(r => r.CompanyName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new
                {
                    id = r.Id,
                    companyName = r.CompanyName,
                    address = r.Address,
                    contactPhone = r.ContactPhone,
                    contactEmail = r.ContactEmail,
                    materialTypesHandled = r.MaterialTypesHandled,
                    certificationType = r.CertificationType,
                    servicesOffered = r.ServicesOffered,
                    dateCreated = r.DateCreated,
                    dateUpdated = r.DateUpdated,
                    createdBy = r.CreatedBy,
                    updatedBy = r.UpdatedBy
                })
                .ToListAsync();

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            return Ok(recyclers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetRecycler(int id)
        {
            var recycler = await _context.Recyclers.FindAsync(id);

            if (recycler == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                id = recycler.Id,
                companyName = recycler.CompanyName,
                address = recycler.Address,
                contactPhone = recycler.ContactPhone,
                contactEmail = recycler.ContactEmail,
                materialTypesHandled = recycler.MaterialTypesHandled,
                certificationType = recycler.CertificationType,
                servicesOffered = recycler.ServicesOffered,
                dateCreated = recycler.DateCreated,
                dateUpdated = recycler.DateUpdated,
                createdBy = recycler.CreatedBy,
                updatedBy = recycler.UpdatedBy
            });
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateRecycler([FromBody] CreateRecyclerRequest request)
        {
            var recycler = new Recycler
            {
                CompanyName = request.CompanyName,
                Address = request.Address,
                ContactPhone = request.ContactPhone,
                ContactEmail = request.ContactEmail,
                MaterialTypesHandled = request.MaterialTypesHandled,
                CertificationType = request.CertificationType,
                ServicesOffered = request.ServicesOffered,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                CreatedBy = request.UserId,
                UpdatedBy = request.UserId
            };

            _context.Recyclers.Add(recycler);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRecycler), new { id = recycler.Id }, new
            {
                id = recycler.Id,
                companyName = recycler.CompanyName,
                address = recycler.Address,
                contactPhone = recycler.ContactPhone,
                contactEmail = recycler.ContactEmail,
                materialTypesHandled = recycler.MaterialTypesHandled,
                certificationType = recycler.CertificationType,
                servicesOffered = recycler.ServicesOffered,
                dateCreated = recycler.DateCreated,
                dateUpdated = recycler.DateUpdated,
                createdBy = recycler.CreatedBy,
                updatedBy = recycler.UpdatedBy
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecycler(int id, [FromBody] UpdateRecyclerRequest request)
        {
            var recycler = await _context.Recyclers.FindAsync(id);
            if (recycler == null)
            {
                return NotFound();
            }

            recycler.CompanyName = request.CompanyName;
            recycler.Address = request.Address;
            recycler.ContactPhone = request.ContactPhone;
            recycler.ContactEmail = request.ContactEmail;
            recycler.MaterialTypesHandled = request.MaterialTypesHandled;
            recycler.CertificationType = request.CertificationType;
            recycler.ServicesOffered = request.ServicesOffered;
            recycler.DateUpdated = DateTime.UtcNow;
            recycler.UpdatedBy = request.UserId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecyclerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecycler(int id)
        {
            var recycler = await _context.Recyclers.FindAsync(id);
            if (recycler == null)
            {
                return NotFound();
            }

            _context.Recyclers.Remove(recycler);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RecyclerExists(int id)
        {
            return _context.Recyclers.Any(e => e.Id == id);
        }
    }

    public class CreateRecyclerRequest
    {
        public string CompanyName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? MaterialTypesHandled { get; set; }
        public string? CertificationType { get; set; }
        public string? ServicesOffered { get; set; }
        public int UserId { get; set; }
    }

    public class UpdateRecyclerRequest
    {
        public string CompanyName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? MaterialTypesHandled { get; set; }
        public string? CertificationType { get; set; }
        public string? ServicesOffered { get; set; }
        public int UserId { get; set; }
    }
}
