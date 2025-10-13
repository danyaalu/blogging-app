using BloggingApp.Api.Data;
using BloggingApp.Api.DTOs;
using BloggingApp.Api.Models;
using BloggingApp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloggingApp.Api.Controllers;

[ApiController]
[Route("api/admin/posts")]
[Authorize(Roles = "Owner")]
public class AdminPostsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMarkdownService _markdownService;
    private readonly ISlugService _slugService;

    public AdminPostsController(
        ApplicationDbContext context,
        IMarkdownService markdownService,
        ISlugService slugService)
    {
        _context = context;
        _markdownService = markdownService;
        _slugService = slugService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostAdminDto>>> GetAllPosts([FromQuery] string? status = null)
    {
        var query = _context.Posts.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<PostStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(p => p.Status == parsedStatus);
        }

        var posts = await query
            .OrderByDescending(p => p.UpdatedAt)
            .Select(p => new PostAdminDto(
                p.Id,
                p.Title,
                p.Slug,
                p.Summary,
                p.BodyMarkdown,
                p.BodyHtml,
                p.Status.ToString(),
                p.PublishedAt,
                p.CreatedAt,
                p.UpdatedAt,
                p.ReadingMinutes
            ))
            .ToListAsync();

        return Ok(posts);
    }

    [HttpPost]
    public async Task<ActionResult<PostAdminDto>> CreatePost([FromBody] CreatePostRequest request)
    {
        // Validate slug
        if (string.IsNullOrWhiteSpace(request.Slug) || !_slugService.IsValidSlug(request.Slug))
        {
            return BadRequest(new { message = "Invalid slug format. Use lowercase letters, numbers, and hyphens only." });
        }

        // Check if slug already exists
        if (await _context.Posts.AnyAsync(p => p.Slug == request.Slug))
        {
            return BadRequest(new { message = "A post with this slug already exists." });
        }

        if (!Enum.TryParse<PostStatus>(request.Status, true, out var status))
        {
            return BadRequest(new { message = "Invalid status. Use 'Draft' or 'Published'." });
        }

        var now = DateTime.UtcNow;
        var bodyHtml = _markdownService.ToHtml(request.BodyMarkdown);
        var readingMinutes = CalculateReadingMinutes(request.BodyMarkdown);

        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Slug = request.Slug,
            Summary = request.Summary,
            BodyMarkdown = request.BodyMarkdown,
            BodyHtml = bodyHtml,
            Status = status,
            PublishedAt = status == PostStatus.Published ? now : null,
            CreatedAt = now,
            UpdatedAt = now,
            ReadingMinutes = readingMinutes
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        var dto = new PostAdminDto(
            post.Id,
            post.Title,
            post.Slug,
            post.Summary,
            post.BodyMarkdown,
            post.BodyHtml,
            post.Status.ToString(),
            post.PublishedAt,
            post.CreatedAt,
            post.UpdatedAt,
            post.ReadingMinutes
        );

        return CreatedAtAction(nameof(GetAllPosts), new { id = post.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PostAdminDto>> UpdatePost(Guid id, [FromBody] UpdatePostRequest request)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
        {
            return NotFound(new { message = "Post not found" });
        }

        // Validate slug
        if (string.IsNullOrWhiteSpace(request.Slug) || !_slugService.IsValidSlug(request.Slug))
        {
            return BadRequest(new { message = "Invalid slug format. Use lowercase letters, numbers, and hyphens only." });
        }

        // Check if slug already exists for another post
        if (await _context.Posts.AnyAsync(p => p.Slug == request.Slug && p.Id != id))
        {
            return BadRequest(new { message = "A post with this slug already exists." });
        }

        if (!Enum.TryParse<PostStatus>(request.Status, true, out var status))
        {
            return BadRequest(new { message = "Invalid status. Use 'Draft' or 'Published'." });
        }

        var bodyHtml = _markdownService.ToHtml(request.BodyMarkdown);
        var readingMinutes = CalculateReadingMinutes(request.BodyMarkdown);

        post.Title = request.Title;
        post.Slug = request.Slug;
        post.Summary = request.Summary;
        post.BodyMarkdown = request.BodyMarkdown;
        post.BodyHtml = bodyHtml;
        post.Status = status;
        post.UpdatedAt = DateTime.UtcNow;
        post.ReadingMinutes = readingMinutes;

        // Set PublishedAt if transitioning to Published and not already set
        if (status == PostStatus.Published && post.PublishedAt == null)
        {
            post.PublishedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        var dto = new PostAdminDto(
            post.Id,
            post.Title,
            post.Slug,
            post.Summary,
            post.BodyMarkdown,
            post.BodyHtml,
            post.Status.ToString(),
            post.PublishedAt,
            post.CreatedAt,
            post.UpdatedAt,
            post.ReadingMinutes
        );

        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
        {
            return NotFound(new { message = "Post not found" });
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private int CalculateReadingMinutes(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return 0;

        var wordCount = markdown.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
        var minutes = (int)Math.Ceiling(wordCount / 200.0);
        return Math.Max(1, minutes);
    }
}
