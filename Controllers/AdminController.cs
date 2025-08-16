using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using System.Security.Claims;
using System.Security.Cryptography;
using irevlogix_backend.Data;
using irevlogix_backend.Models;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableCors("AllowFrontend")]
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

        [HttpPost("settings/bulk-save")]
        public async Task<IActionResult> BulkSaveSettings(BulkSaveSettingsRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                await UpsertSettings(request.ApplicationSettings, "Application", clientId, userId);
                
                await UpsertSettings(request.GeneralSettings, "General", clientId, userId);
                
                await UpsertSettings(request.SecuritySettings, "Security", clientId, userId);

                await _context.SaveChangesAsync();
                return Ok(new { message = "Settings saved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk saving settings");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task UpsertSettings(Dictionary<string, object> settings, string category, string clientId, int userId)
        {
            foreach (var kvp in settings)
            {
                var existingSetting = await _context.ApplicationSettings
                    .Where(s => s.ClientId == clientId && s.SettingKey == kvp.Key)
                    .FirstOrDefaultAsync();

                if (existingSetting != null)
                {
                    existingSetting.SettingValue = kvp.Value?.ToString();
                    existingSetting.Category = category;
                    existingSetting.UpdatedBy = userId;
                    existingSetting.DateUpdated = DateTime.UtcNow;
                }
                else
                {
                    var newSetting = new ApplicationSettings
                    {
                        SettingKey = kvp.Key,
                        SettingValue = kvp.Value?.ToString(),
                        Category = category,
                        ClientId = clientId,
                        CreatedBy = userId,
                        UpdatedBy = userId
                    };
                    _context.ApplicationSettings.Add(newSetting);
                }
            }
        }

        [HttpPost("settings/upload-logo")]
        public async Task<IActionResult> UploadApplicationLogo(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var uploadsPath = Path.Combine("uploads", clientId, "logos");
                Directory.CreateDirectory(uploadsPath);

                var fileName = $"logo_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                await UpsertSettings(new Dictionary<string, object> { { "ApplicationLogoPath", filePath } }, "General", clientId, userId);
                await _context.SaveChangesAsync();

                return Ok(new { filePath, message = "Logo uploaded successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading logo");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers(
            [FromQuery] string? firstName = null,
            [FromQuery] string? lastName = null,
            [FromQuery] string? email = null,
            [FromQuery] string? clientId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var currentClientId = GetClientId();
                var query = _context.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .Where(u => u.ClientId == currentClientId)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(firstName))
                    query = query.Where(u => u.FirstName.Contains(firstName));
                if (!string.IsNullOrEmpty(lastName))
                    query = query.Where(u => u.LastName.Contains(lastName));
                if (!string.IsNullOrEmpty(email))
                    query = query.Where(u => u.Email.Contains(email));
                if (!string.IsNullOrEmpty(clientId))
                    query = query.Where(u => u.ClientId == clientId);

                var totalCount = await query.CountAsync();
                var users = await query
                    .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new
                    {
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.ClientId,
                        CompanyName = _context.Clients.Where(c => c.ClientId == u.ClientId).Select(c => c.CompanyName).FirstOrDefault(),
                        u.IsActive,
                        LastLogin = u.LastLoginDate ?? u.DateUpdated,
                        u.DateCreated,
                        u.DateUpdated
                    })
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

        [HttpGet("users/{id}")]
        public async Task<ActionResult<object>> GetUser(int id)
        {
            try
            {
                var clientId = GetClientId();
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .Where(u => u.Id == id && u.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound();

                return Ok(new
                {
                    user.Id,
                    user.Username,
                    user.DisplayName,
                    user.FirstName,
                    user.MiddleName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber,
                    user.Address,
                    user.Address2,
                    user.City,
                    user.State,
                    user.PostalCode,
                    user.Country,
                    user.TwoFactorAuthEnabled,
                    user.IsActive,
                    user.ClientId,
                    user.DateCreated,
                    user.DateUpdated,
                    user.CreatedBy,
                    user.UpdatedBy
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("users")]
        public async Task<ActionResult<object>> CreateUser(AdminCreateUserRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (!string.IsNullOrEmpty(request.Username))
                {
                    var existingUser = await _context.Users
                        .Where(u => u.Username == request.Username && u.ClientId == request.ClientId)
                        .FirstOrDefaultAsync();
                    if (existingUser != null)
                        return BadRequest("Username already exists for this client");
                }

                var existingEmail = await _context.Users
                    .Where(u => u.Email == request.Email && u.ClientId == request.ClientId)
                    .FirstOrDefaultAsync();
                if (existingEmail != null)
                    return BadRequest("Email already exists for this client");

                var passwordHash = HashPassword(request.Password);

                var user = new User
                {
                    Username = request.Username,
                    DisplayName = request.DisplayName,
                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    PhoneNumber = request.PhoneNumber,
                    Address = request.Address,
                    Address2 = request.Address2,
                    City = request.City,
                    State = request.State,
                    PostalCode = request.PostalCode,
                    Country = request.Country,
                    TwoFactorAuthEnabled = request.TwoFactorAuthEnabled,
                    IsActive = request.IsActive,
                    ClientId = request.ClientId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, AdminUpdateUserRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var user = await _context.Users
                    .Where(u => u.Id == id && u.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound();

                if (!string.IsNullOrEmpty(request.Username) && request.Username != user.Username)
                {
                    var existingUser = await _context.Users
                        .Where(u => u.Username == request.Username && u.ClientId == user.ClientId && u.Id != id)
                        .FirstOrDefaultAsync();
                    if (existingUser != null)
                        return BadRequest("Username already exists for this client");
                }

                user.Username = request.Username;
                user.DisplayName = request.DisplayName;
                user.FirstName = request.FirstName ?? user.FirstName;
                user.MiddleName = request.MiddleName;
                user.LastName = request.LastName ?? user.LastName;
                user.Email = request.Email ?? user.Email;
                user.PhoneNumber = request.PhoneNumber;
                user.Address = request.Address;
                user.Address2 = request.Address2;
                user.City = request.City;
                user.State = request.State;
                user.PostalCode = request.PostalCode;
                user.Country = request.Country;
                user.TwoFactorAuthEnabled = request.TwoFactorAuthEnabled;
                user.IsActive = request.IsActive;
                user.UpdatedBy = userId;
                user.DateUpdated = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(request.Password))
                {
                    user.PasswordHash = HashPassword(request.Password);
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("clients")]
        public async Task<ActionResult<IEnumerable<object>>> GetClients()
        {
            try
            {
                var clients = await _context.Clients
                    .Where(c => c.IsActive)
                    .Select(c => new { c.Id, c.ClientId, c.CompanyName })
                    .OrderBy(c => c.CompanyName)
                    .ToListAsync();

                return Ok(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clients");
                return StatusCode(500, "Internal server error");
            }
        }

        private string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);
            
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);
            
            var hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);
            
            return Convert.ToBase64String(hashBytes);
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

    public class BulkSaveSettingsRequest
    {
        public Dictionary<string, object> ApplicationSettings { get; set; } = new();
        public Dictionary<string, object> GeneralSettings { get; set; } = new();
        public Dictionary<string, object> SecuritySettings { get; set; } = new();
    }

    public class AdminCreateUserRequest
    {
        public string? Username { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public bool TwoFactorAuthEnabled { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string ClientId { get; set; } = string.Empty;
    }

    public class AdminUpdateUserRequest
    {
        public string? Username { get; set; }
        public string? DisplayName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public bool TwoFactorAuthEnabled { get; set; }
        public bool IsActive { get; set; }
    }
}
