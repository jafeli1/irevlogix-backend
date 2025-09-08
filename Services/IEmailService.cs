namespace irevlogix_backend.Services
{
    public interface IEmailService
    {
        Task<bool> SendContactEmailAsync(string fromName, string fromEmail, string company, string message, string toEmail);
        Task<bool> SendEmailConfirmationAsync(string toEmail, string firstName, string confirmationToken, string baseUrl);
        Task<bool> SendScheduledReportAsync(string[] recipients, string reportName, string dataSource, byte[] excelData, string fileName, string tenantName, DateTime generatedDate, string filtersSummary);
    }
}
