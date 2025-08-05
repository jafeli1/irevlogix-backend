using System.ComponentModel.DataAnnotations;

namespace irevlogix_backend.Models
{
    public class KnowledgeBaseArticle : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? Category { get; set; }
        
        [MaxLength(500)]
        public string? Tags { get; set; }
        
        public bool IsPublished { get; set; } = false;
        
        public int ViewCount { get; set; } = 0;
        
        public int? AuthorUserId { get; set; }
        public virtual User? Author { get; set; }
        
        public DateTime? PublishedDate { get; set; }
        
        [MaxLength(500)]
        public string? Summary { get; set; }
        
        public int SortOrder { get; set; } = 0;
    }
}
