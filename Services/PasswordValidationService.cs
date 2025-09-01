using System.Text.RegularExpressions;

namespace irevlogix_backend.Services
{
    public interface IPasswordValidationService
    {
        Task<(bool IsValid, List<string> Errors)> ValidatePasswordComplexity(string password, string complexityRequirements);
    }

    public class PasswordValidationService : IPasswordValidationService
    {
        public async Task<(bool IsValid, List<string> Errors)> ValidatePasswordComplexity(string password, string complexityRequirements)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(password))
            {
                errors.Add("Password is required");
                return (false, errors);
            }

            var requirements = complexityRequirements.ToLower();

            var minLengthMatch = Regex.Match(requirements, @"minimum (\d+) characters");
            var minLength = minLengthMatch.Success ? int.Parse(minLengthMatch.Groups[1].Value) : 8;
            
            if (password.Length < minLength)
            {
                errors.Add($"Password must be at least {minLength} characters long");
            }

            if (requirements.Contains("uppercase") && !password.Any(char.IsUpper))
            {
                errors.Add("Password must contain at least one uppercase letter");
            }

            if (requirements.Contains("lowercase") && !password.Any(char.IsLower))
            {
                errors.Add("Password must contain at least one lowercase letter");
            }

            if (requirements.Contains("number") && !password.Any(char.IsDigit))
            {
                errors.Add("Password must contain at least one number");
            }

            if (requirements.Contains("special character"))
            {
                var specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
                if (!password.Any(c => specialChars.Contains(c)))
                {
                    errors.Add("Password must contain at least one special character");
                }
            }

            return (errors.Count == 0, errors);
        }
    }
}
