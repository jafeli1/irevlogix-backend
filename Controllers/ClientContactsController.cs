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
    public class ClientContactsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientContactsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientContact>>> GetClientContacts(
            [FromQuery] int? clientId = null,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var tenantClientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(tenantClientId))
                return Unauthorized();

            var query = _context.ClientContacts
                .Where(cc => cc.ClientId == tenantClientId && cc.IsActive)
                .AsQueryable();

            if (clientId.HasValue)
            {
                query = query.Where(cc => cc.Id == clientId.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(cc => cc.FirstName.Contains(search) || 
                                         cc.LastName.Contains(search) || 
                                         cc.Email.Contains(search));
            }

            var contacts = await query
                .OrderBy(cc => cc.FirstName)
                .ThenBy(cc => cc.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(contacts);
        }
    }
}
