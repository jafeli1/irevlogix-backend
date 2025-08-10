namespace irevlogix_backend.DTOs.Reports
{
    public class ComplianceSummaryDto
    {
        public int TotalLots { get; set; }
        public int OverdueCertifications { get; set; }
        public int PendingCertifications { get; set; }
    }
}
