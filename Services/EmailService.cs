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

        public async Task<bool> SendScheduledReportAsync(string[] recipients, string reportName, string dataSource, byte[] excelData, string fileName, string tenantName, DateTime generatedDate, string filtersSummary)
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

                var subject = $"Scheduled Report: {reportName} - {tenantName}";
                var body = $@"Dear Recipient,

Please find attached your scheduled report: {reportName}

Report Details:
- Data Source: {dataSource}
- Generated: {generatedDate:yyyy-MM-dd HH:mm} UTC
- Tenant: {tenantName}
{(string.IsNullOrEmpty(filtersSummary) ? "" : $"- Filters Applied: {filtersSummary}")}

This report was automatically generated and delivered as part of your scheduled report subscription.

If you have any questions or need to modify your report schedule, please contact your system administrator.

Best regards,
The iRevLogix Team";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser, "iRevLogix Reports"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };

                foreach (var recipient in recipients)
                {
                    mailMessage.To.Add(recipient);
                }

                using var attachmentStream = new MemoryStream(excelData);
                using var attachment = new Attachment(attachmentStream, fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                mailMessage.Attachments.Add(attachment);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Scheduled report '{reportName}' sent successfully to {string.Join(", ", recipients)}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send scheduled report '{reportName}' to {string.Join(", ", recipients)}");
                return false;
            }
        }
    }
}
