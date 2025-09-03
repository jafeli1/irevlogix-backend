namespace irevlogix_backend.Services
{
    public interface IEmailService
    {
        Task<bool> SendContactEmailAsync(string fromName, string fromEmail, string company, string message, string toEmail);
    }
}
