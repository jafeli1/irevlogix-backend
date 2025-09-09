namespace irevlogix_backend.DTOs.Reports
{
    public class SummaryMetricsDto
    {
        public int ActiveShipments { get; set; }
        public int ProcessingLots { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int TotalShipments { get; set; }
        public int TotalAssetsProcessed { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class CostRevenueDataDto
    {
        public string Period { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public decimal Revenue { get; set; }
    }

    public class VendorPerformanceDto
    {
        public string VendorName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int ProcessingLots { get; set; }
        public int VendorId { get; set; }
    }

    public class ProcessingTimeDataDto
    {
        public string Stage { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal AverageTime { get; set; }
    }

    public class InboundVolumeDataDto
    {
        public string MaterialType { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public int Count { get; set; }
    }

    public class WasteDiversionDataDto
    {
        public decimal DiversionRate { get; set; }
        public decimal TotalProcessed { get; set; }
        public decimal TotalDiverted { get; set; }
    }

    public class CertificatesIssuedDataDto
    {
        public string Date { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class AssetDispositionDataDto
    {
        public string DispositionType { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class MonthlyRevenueDataDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
}
