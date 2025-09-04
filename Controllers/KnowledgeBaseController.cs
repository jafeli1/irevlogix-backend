using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Models;
using System.Security.Claims;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KnowledgeBaseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KnowledgeBaseController> _logger;

        public KnowledgeBaseController(ApplicationDbContext context, ILogger<KnowledgeBaseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetClientId()
        {
            return User.FindFirst("ClientId")?.Value ?? throw new UnauthorizedAccessException("ClientId not found in token");
        }

        private bool IsAdministrator()
        {
            return User.IsInRole("Administrator");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KnowledgeBaseArticle>>> GetArticles(
            [FromQuery] string? search = null,
            [FromQuery] string? category = null,
            [FromQuery] bool publishedOnly = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.KnowledgeBaseArticles
                    .Include(kb => kb.Author)
                    .AsQueryable();

                if (!IsAdministrator())
                {
                    query = query.Where(kb => kb.ClientId == clientId);
                }

                if (publishedOnly)
                    query = query.Where(kb => kb.IsPublished);

                if (!string.IsNullOrEmpty(search))
                    query = query.Where(kb => kb.Title.Contains(search) || kb.Content.Contains(search) || kb.Tags!.Contains(search));

                if (!string.IsNullOrEmpty(category))
                    query = query.Where(kb => kb.Category == category);

                var totalCount = await query.CountAsync();
                var articles = await query
                    .OrderBy(kb => kb.SortOrder)
                    .ThenByDescending(kb => kb.PublishedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving knowledge base articles");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KnowledgeBaseArticle>> GetArticle(int id)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.KnowledgeBaseArticles
                    .Where(kb => kb.Id == id);

                if (!IsAdministrator())
                {
                    query = query.Where(kb => kb.ClientId == clientId);
                }

                var article = await query
                    .Include(kb => kb.Author)
                    .FirstOrDefaultAsync();

                if (article == null)
                    return NotFound();

                article.ViewCount++;
                await _context.SaveChangesAsync();

                return Ok(article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving knowledge base article {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<KnowledgeBaseArticle>> CreateArticle(CreateArticleRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = 1;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsed)) userId = parsed;

                var article = new KnowledgeBaseArticle
                {
                    Title = request.Title,
                    Content = request.Content,
                    Category = request.Category,
                    Tags = request.Tags,
                    Summary = request.Summary,
                    SortOrder = request.SortOrder,
                    IsPublished = request.IsPublished,
                    AuthorUserId = userId,
                    PublishedDate = request.IsPublished ? DateTime.UtcNow : null,
                    ClientId = clientId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.KnowledgeBaseArticles.Add(article);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating knowledge base article");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, UpdateArticleRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = 1;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsed)) userId = parsed;

                var query = _context.KnowledgeBaseArticles
                    .Where(kb => kb.Id == id);

                if (!IsAdministrator())
                {
                    query = query.Where(kb => kb.ClientId == clientId);
                }

                var article = await query.FirstOrDefaultAsync();

                if (article == null)
                    return NotFound();

                var wasPublished = article.IsPublished;

                article.Title = request.Title ?? article.Title;
                article.Content = request.Content ?? article.Content;
                article.Category = request.Category ?? article.Category;
                article.Tags = request.Tags ?? article.Tags;
                article.Summary = request.Summary ?? article.Summary;
                article.SortOrder = request.SortOrder ?? article.SortOrder;
                article.IsPublished = request.IsPublished ?? article.IsPublished;
                article.UpdatedBy = userId;
                article.DateUpdated = DateTime.UtcNow;

                if (!wasPublished && article.IsPublished)
                    article.PublishedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating knowledge base article {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.KnowledgeBaseArticles
                    .Where(kb => kb.Id == id);

                if (!IsAdministrator())
                {
                    query = query.Where(kb => kb.ClientId == clientId);
                }

                var article = await query.FirstOrDefaultAsync();

                if (article == null)
                    return NotFound();

                _context.KnowledgeBaseArticles.Remove(article);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting knowledge base article {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.KnowledgeBaseArticles
                    .Where(kb => !string.IsNullOrEmpty(kb.Category));

                if (!IsAdministrator())
                {
                    query = query.Where(kb => kb.ClientId == clientId);
                }

                var categories = await query
                    .Select(kb => kb.Category!)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving knowledge base categories");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class CreateArticleRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Tags { get; set; }
        public string? Summary { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsPublished { get; set; } = false;
    }

    public class UpdateArticleRequest
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Category { get; set; }
        public string? Tags { get; set; }
        public string? Summary { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsPublished { get; set; }
    }
}
