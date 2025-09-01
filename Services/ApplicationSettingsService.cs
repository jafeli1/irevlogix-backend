using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Models;

namespace irevlogix_backend.Services
{
    public interface IApplicationSettingsService
    {
        Task<int> GetLoginTimeoutMinutes(string clientId);
        Task<int> GetPasswordExpiryDays(string clientId);
        Task<int> GetUnsuccessfulLoginAttemptsBeforeLockout(string clientId);
        Task<int> GetLockoutDurationMinutes(string clientId);
        Task<string> GetPasswordComplexityRequirements(string clientId);
    }

    public class ApplicationSettingsService : IApplicationSettingsService
    {
        private readonly ApplicationDbContext _context;

        public ApplicationSettingsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetLoginTimeoutMinutes(string clientId)
        {
            var setting = await GetSettingByKey(clientId, "LoginTimeoutMinutes");
            if (setting?.LoginTimeoutMinutes.HasValue == true)
                return setting.LoginTimeoutMinutes.Value;
            return 5; // Default fallback
        }

        public async Task<int> GetPasswordExpiryDays(string clientId)
        {
            var setting = await GetSettingByKey(clientId, "PasswordExpiryDays");
            if (setting?.PasswordExpiryDays.HasValue == true)
                return setting.PasswordExpiryDays.Value;
            return 45; // Default fallback
        }

        public async Task<int> GetUnsuccessfulLoginAttemptsBeforeLockout(string clientId)
        {
            var setting = await GetSettingByKey(clientId, "UnsuccessfulLoginAttemptsBeforeLockout");
            if (setting?.UnsuccessfulLoginAttemptsBeforeLockout.HasValue == true)
                return setting.UnsuccessfulLoginAttemptsBeforeLockout.Value;
            return 3; // Default fallback
        }

        public async Task<int> GetLockoutDurationMinutes(string clientId)
        {
            var setting = await GetSettingByKey(clientId, "LockoutDurationMinutes");
            if (setting?.LockoutDurationMinutes.HasValue == true)
                return setting.LockoutDurationMinutes.Value;
            return 30; // Default fallback
        }

        public async Task<string> GetPasswordComplexityRequirements(string clientId)
        {
            var setting = await GetSettingByKey(clientId, "PasswordComplexityRequirements");
            if (!string.IsNullOrEmpty(setting?.PasswordComplexityRequirements))
                return setting.PasswordComplexityRequirements;
            return "Minimum 8 characters, at least one uppercase letter, one lowercase letter, one number, and one special character."; // Default fallback
        }

        private async Task<ApplicationSettings?> GetSettingByKey(string clientId, string settingKey)
        {
            var setting = await _context.ApplicationSettings
                .Where(s => s.ClientId == clientId && s.SettingKey == settingKey)
                .FirstOrDefaultAsync();

            if (setting != null)
                return setting;

            return await _context.ApplicationSettings
                .Where(s => s.ClientId == clientId)
                .FirstOrDefaultAsync();
        }
    }
}
