using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using irevlogix_backend.Data;
using irevlogix_backend.Models;
using irevlogix_backend.Services;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IApplicationSettingsService _settingsService;
        private readonly IPasswordValidationService _passwordValidationService;

        public AuthController(ApplicationDbContext context, IConfiguration configuration, 
            IApplicationSettingsService settingsService, IPasswordValidationService passwordValidationService)
        {
            _context = context;
            _configuration = configuration;
            _settingsService = settingsService;
            _passwordValidationService = passwordValidationService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Email and password are required" });
            }

            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var maxAttempts = await _settingsService.GetUnsuccessfulLoginAttemptsBeforeLockout(user.ClientId);
            var lockoutDuration = await _settingsService.GetLockoutDurationMinutes(user.ClientId);
            var passwordExpiryDays = await _settingsService.GetPasswordExpiryDays(user.ClientId);

            if (user.LockoutEndDate.HasValue && user.LockoutEndDate > DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Account is locked. Please try again later." });
            }

            if (!VerifyPassword(request.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= maxAttempts)
                {
                    user.LockoutEndDate = DateTime.UtcNow.AddMinutes(lockoutDuration);
                }
                await _context.SaveChangesAsync();
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var passwordAge = (DateTime.UtcNow - user.DateCreated).Days;

            var isPasswordExpired = passwordAge >= passwordExpiryDays;
            var isPasswordExpiringSoon = passwordAge >= (passwordExpiryDays - 7); // Warning 7 days before expiry

            if (isPasswordExpired)
            {
                return Unauthorized(new { 
                    message = "Password has expired. Please change your password.",
                    passwordExpired = true,
                    requirePasswordChange = true
                });
            }

            user.FailedLoginAttempts = 0;
            user.LockoutEndDate = null;
            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var loginTimeout = await _settingsService.GetLoginTimeoutMinutes(user.ClientId);
            var token = GenerateJwtToken(user, loginTimeout);

            var response = new
            {
                token,
                user = new
                {
                    id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    clientId = user.ClientId,
                    roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
                },
                passwordExpiringSoon = isPasswordExpiringSoon,
                daysUntilPasswordExpiry = passwordExpiryDays - passwordAge,
                sessionTimeoutMinutes = loginTimeout
            };

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<object>> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            var complexityRequirements = await _settingsService.GetPasswordComplexityRequirements(request.ClientId);
            var (isValid, errors) = await _passwordValidationService.ValidatePasswordComplexity(request.Password, complexityRequirements);
            
            if (!isValid)
            {
                return BadRequest(new { message = "Password does not meet complexity requirements", errors = errors });
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                ClientId = request.ClientId,
                CreatedBy = 1,
                UpdatedBy = 1
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }

        private string GenerateJwtToken(User user, int timeoutMinutes = 5)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET") ?? 
                                                      _configuration["Jwt:Key"] ?? 
                                                      "your-secret-key-here-make-it-long-enough");
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("ClientId", user.ClientId),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("LoginTime", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };
            
            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(timeoutMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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

        private bool VerifyPassword(string password, string hash)
        {
            var hashBytes = Convert.FromBase64String(hash);
            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var computedHash = pbkdf2.GetBytes(32);
            
            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != computedHash[i])
                    return false;
            }
            return true;
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<object>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                return BadRequest(new { message = "Current password is incorrect" });
            }

            var complexityRequirements = await _settingsService.GetPasswordComplexityRequirements(user.ClientId);
            var (isValid, errors) = await _passwordValidationService.ValidatePasswordComplexity(request.NewPassword, complexityRequirements);
            
            if (!isValid)
            {
                return BadRequest(new { message = "New password does not meet complexity requirements", errors = errors });
            }

            user.PasswordHash = HashPassword(request.NewPassword);
            user.UpdatedBy = user.Id;
            user.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully" });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
