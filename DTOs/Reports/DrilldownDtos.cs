using System;
using System.Collections.Generic;

namespace irevlogix_backend.DTOs.Reports
{
    public class DrilldownRequestDto
    {
        public string Type { get; set; } = "processinglots";
        public string? Status { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class DrilldownItemDto
    {
        public string RecordType { get; set; } = string.Empty;
        public int Id { get; set; }
        public string? NameOrType { get; set; }
        public DateTime? Date { get; set; }
        public decimal? WeightLbs { get; set; }
        public string? Status { get; set; }
    }

    public class DrilldownResponseDto
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<DrilldownItemDto> Items { get; set; } = new List<DrilldownItemDto>();
    }
}
