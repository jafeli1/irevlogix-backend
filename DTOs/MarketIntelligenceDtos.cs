namespace irevlogix_backend.DTOs
{
    public class ProductAnalysisRequest
    {
        public string? Base64Image { get; set; }
        public string? ProductDescription { get; set; }
    }

    public class ProductAnalysisResult
    {
        public string ProductName { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string Category { get; set; } = "";
        public Dictionary<string, string> Specifications { get; set; } = new();
        public List<RecyclableComponent> Components { get; set; } = new();
        public MarketPriceInfo MarketPrice { get; set; } = new();
        public string Summary { get; set; } = "";
        public List<EbayListingDto> EbayListings { get; set; } = new();
        public List<MatchedRecyclerDto> MatchedRecyclers { get; set; } = new();
        public List<ResaleComponent> ResaleComponents { get; set; } = new();
        public List<GeneralRecyclable> GeneralRecyclables { get; set; } = new();
        public List<HazardousComponent> HazardousComponents { get; set; } = new();
    }

    public class ResaleComponent
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string EstimatedValue { get; set; } = "";
    }

    public class GeneralRecyclable
    {
        public string Name { get; set; } = "";
        public string Material { get; set; } = "";
        public string RecyclingNotes { get; set; } = "";
    }

    public class HazardousComponent
    {
        public string Name { get; set; } = "";
        public string HazardType { get; set; } = "";
        public string SafetyWarning { get; set; } = "";
    }

    public class RecyclableComponent
    {
        public string Name { get; set; } = "";
        public string Material { get; set; } = "";
        public string EstimatedWeight { get; set; } = "";
        public string EstimatedValue { get; set; } = "";
    }

    public class MarketPriceInfo
    {
        public string AveragePrice { get; set; } = "";
        public string PriceRange { get; set; } = "";
        public string MarketTrend { get; set; } = "";
    }

    public class EbayListingDto
    {
        public string Title { get; set; } = "";
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";
        public string Condition { get; set; } = "";
        public string ItemUrl { get; set; } = "";
        public string ImageUrl { get; set; } = "";
    }

    public class MatchedRecyclerDto
    {
        public string CompanyName { get; set; } = "";
        public string Address { get; set; } = "";
        public string CertificationType { get; set; } = "";
        public string ContactPhone { get; set; } = "";
        public string ContactEmail { get; set; } = "";
        public int MatchScore { get; set; }
        public string MatchReason { get; set; } = "";
    }

    public class RecyclerMatchRequest
    {
        public List<string> Materials { get; set; } = new();
        public List<string> Components { get; set; } = new();
    }

    public class ComponentPriceRequest
    {
        public string ComponentName { get; set; } = "";
    }

    public class ComponentPriceResult
    {
        public string ComponentName { get; set; } = "";
        public string AveragePrice { get; set; } = "";
        public string PriceRange { get; set; } = "";
        public List<EbayListingDto> Listings { get; set; } = new();
    }
}
