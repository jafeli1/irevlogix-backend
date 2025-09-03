using System.Net;
using System.Net.Mail;

namespace irevlogix_backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendContactEmailAsync(string fromName, string fromEmail, string company, string message, string toEmail)
        {
            try
            {
                var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.gmail.com";
                var smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
                var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? "";
                var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS") ?? "";

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser, "iRevLogix Contact Form"),
                    Subject = $"Contact Form Submission from {fromName}",
                    Body = $"Name: {fromName}\nEmail: {fromEmail}\nCompany: {company}\n\nMessage:\n{message}",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(toEmail);
                mailMessage.ReplyToList.Add(new MailAddress(fromEmail, fromName));

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Contact email sent successfully to {toEmail} from {fromEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send contact email to {toEmail} from {fromEmail}");
                return false;
            }
        }

        public async Task<bool> SendEmailConfirmationAsync(string toEmail, string firstName, string confirmationToken, string baseUrl)
        {
            try
            {
                var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.gmail.com";
                var smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
                var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? "";
                var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS") ?? "";

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                var confirmationUrl = $"{baseUrl}/confirm-email?token={confirmationToken}&email={Uri.EscapeDataString(toEmail)}";
                
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser, "iRevLogix"),
                    Subject = "Confirm Your Email Address - iRevLogix",
                    Body = $"Hello {firstName},\n\nThank you for registering with iRevLogix! Please confirm your email address by clicking the link below:\n\n{confirmationUrl}\n\nIf you did not create an account, please ignore this email.\n\nBest regards,\nThe iRevLogix Team",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email confirmation sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email confirmation to {toEmail}");
                return false;
            }
        }
    }
}
