using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<IEnumerable<object>>> GetClients(
            [FromQuery] string? search = null,
            [FromQuery] string? companyName = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var query = _context.Clients.AsQueryable();

            var searchTerm = !string.IsNullOrEmpty(companyName) ? companyName : search;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.CompanyName.Contains(searchTerm));
            }

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();
            var clients = await query
                .OrderBy(c => c.CompanyName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new {
                    c.Id,
                    c.ClientId,
                    c.CompanyName,
                    c.ContactFirstName,
                    c.ContactLastName,
                    c.IsActive,
                    DateCreated = c.DateCreated
                })
                .ToListAsync();

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            return Ok(clients);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            
            if (client == null)
                return NotFound();

            return Ok(client);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Client>> CreateClient(CreateClientRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = 1;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsed)) userId = parsed;

            var client = new Client
            {
                CompanyName = request.CompanyName,
                ContactFirstName = request.ContactFirstName,
                ContactLastName = request.ContactLastName,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                Address = request.Address,
                Address2 = request.Address2,
                City = request.City,
                State = request.State,
                ZipCode = request.ZipCode,
                Country = request.Country,
                ClientType = request.ClientType,
                IsActive = request.IsActive,
                ClientId = Guid.NewGuid().ToString(),
                CreatedBy = userId,
                UpdatedBy = userId
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateClient(int id, UpdateClientRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = 1;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsed)) userId = parsed;

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound();

            client.CompanyName = request.CompanyName ?? client.CompanyName;
            client.ContactFirstName = request.ContactFirstName;
            client.ContactLastName = request.ContactLastName;
            client.ContactEmail = request.ContactEmail;
            client.ContactPhone = request.ContactPhone;
            client.Address = request.Address;
            client.Address2 = request.Address2;
            client.City = request.City;
            client.State = request.State;
            client.ZipCode = request.ZipCode;
            client.Country = request.Country;
            client.ClientType = request.ClientType;
            client.IsActive = request.IsActive;
            client.UpdatedBy = userId;
            client.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public class CreateClientRequest
    {
        [Required]
        public string CompanyName { get; set; } = string.Empty;
        public string? ContactFirstName { get; set; }
        public string? ContactLastName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public string? ClientType { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateClientRequest
    {
        public string? CompanyName { get; set; }
        public string? ContactFirstName { get; set; }
        public string? ContactLastName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public string? ClientType { get; set; }
        public bool IsActive { get; set; }
    }
}
