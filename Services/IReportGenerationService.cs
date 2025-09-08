namespace irevlogix_backend.Services
{
    public interface IReportGenerationService
    {
        Task<byte[]> GenerateExcelReportAsync(string dataSource, string[] selectedColumns, object? filters, object? sorting, string clientId);
    }
}
