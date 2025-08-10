namespace irevlogix_backend.DTOs.Reports
{
    public class FinancialSummaryDto
    {
        public decimal TotalActualRevenue { get; set; }
        public decimal TotalExpectedRevenue { get; set; }
        public decimal TotalProcessingCost { get; set; }
        public decimal TotalIncomingMaterialCost { get; set; }
        public decimal NetProfit { get; set; }
    }
}
