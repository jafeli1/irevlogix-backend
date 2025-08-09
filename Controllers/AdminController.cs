using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Models;
using System.Security.Claims;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetClientId()
        {
            return User.FindFirst("ClientId")?.Value ?? throw new UnauthorizedAccessException("ClientId not found in token");
        }

        [HttpGet("settings")]
        public async Task<ActionResult<IEnumerable<ApplicationSettings>>> GetSettings([FromQuery] string? category = null)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.ApplicationSettings
                    .Where(s => s.ClientId == clientId)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(category))
                    query = query.Where(s => s.Category == category);

                var settings = await query.OrderBy(s => s.Category).ThenBy(s => s.SettingKey).ToListAsync();
                return Ok(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving settings");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("settings/{id}")]
        public async Task<ActionResult<ApplicationSettings>> GetSetting(int id)
        {
            try
            {
                var clientId = GetClientId();
                var setting = await _context.ApplicationSettings
                    .Where(s => s.Id == id && s.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (setting == null)
                    return NotFound();

                return Ok(setting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving setting {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("settings")]
        public async Task<ActionResult<ApplicationSettings>> CreateSetting(CreateSettingRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var existingSetting = await _context.ApplicationSettings
                    .Where(s => s.ClientId == clientId && s.SettingKey == request.SettingKey)
                    .FirstOrDefaultAsync();

                if (existingSetting != null)
                    return BadRequest("Setting with this key already exists");

                var setting = new ApplicationSettings
                {
                    SettingKey = request.SettingKey,
                    SettingValue = request.SettingValue,
                    Description = request.Description,
                    Category = request.Category,
                    IsEncrypted = request.IsEncrypted,
                    IsReadOnly = request.IsReadOnly,
                    ClientId = clientId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.ApplicationSettings.Add(setting);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetSetting), new { id = setting.Id }, setting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating setting");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("settings/{id}")]
        public async Task<IActionResult> UpdateSetting(int id, UpdateSettingRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var setting = await _context.ApplicationSettings
                    .Where(s => s.Id == id && s.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (setting == null)
                    return NotFound();

                if (setting.IsReadOnly)
                    return BadRequest("Cannot update read-only setting");

                setting.SettingValue = request.SettingValue ?? setting.SettingValue;
                setting.Description = request.Description ?? setting.Description;
                setting.Category = request.Category ?? setting.Category;
                setting.UpdatedBy = userId;
                setting.DateUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating setting {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("settings/{id}")]
        public async Task<IActionResult> DeleteSetting(int id)
        {
            try
            {
                var clientId = GetClientId();
                var setting = await _context.ApplicationSettings
                    .Where(s => s.Id == id && s.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (setting == null)
                    return NotFound();

                if (setting.IsReadOnly)
                    return BadRequest("Cannot delete read-only setting");

                _context.ApplicationSettings.Remove(setting);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting setting {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.Users
                    .Where(u => u.ClientId == clientId)
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                    query = query.Where(u => u.Email.Contains(search) || u.FirstName.Contains(search) || u.LastName.Contains(search));

                var totalCount = await query.CountAsync();
                var users = await query
                    .OrderBy(u => u.Email)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var clientId = GetClientId();
                var roles = await _context.Roles
                    .Where(r => r.ClientId == clientId)
                    .Select(r => new {
                        r.Id,
                        r.Name,
                        r.Description,
                        r.IsSystemRole,
                        r.DateCreated,
                        r.DateUpdated,
                        RolePermissions = r.RolePermissions.Select(rp => new {
                            rp.Id,
                            rp.PermissionId,
                            Permission = new {
                                rp.Permission.Id,
                                rp.Permission.Name,
                                rp.Permission.Module,
                                rp.Permission.Action,
                                rp.Permission.Description
                            }
                        })
                    })
                    .OrderBy(r => r.Name)
                    .ToListAsync();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("permissions")]
        public async Task<ActionResult<IEnumerable<Permission>>> GetPermissions()
        {
            try
            {
                var clientId = GetClientId();
                var permissions = await _context.Permissions
                    .Where(p => p.ClientId == clientId)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permissions");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class CreateSettingRequest
    {
        public string SettingKey { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool IsEncrypted { get; set; } = false;
        public bool IsReadOnly { get; set; } = false;
    }

    public class UpdateSettingRequest
    {
        public string? SettingValue { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
    }
}
