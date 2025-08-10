using System;

namespace irevlogix_backend.DTOs.Reports
{
    public class EsgFactorsDto
    {
        public decimal Co2ePerLb { get; set; }
        public decimal WaterGalPerLb { get; set; }
        public decimal EnergyKwhPerLb { get; set; }
    }

    public class EsgSummaryDto
    {
        public decimal TotalIncomingWeight { get; set; }
        public decimal TotalProcessedWeight { get; set; }
        public decimal DiversionRate { get; set; }
        public decimal Co2eSavedLbs { get; set; }
        public decimal WaterSavedGallons { get; set; }
        public decimal EnergySavedKwh { get; set; }
        public EsgFactorsDto Factors { get; set; } = new EsgFactorsDto();
    }
}
