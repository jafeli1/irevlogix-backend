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
}
