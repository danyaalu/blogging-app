using BloggingApp.Api.Data;
using BloggingApp.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloggingApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PostsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "skip", "take" })]
    public async Task<ActionResult<IEnumerable<PostListDto>>> GetPublishedPosts(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10)
    {
        take = Math.Min(take, 50); // Max 50 items

        var posts = await _context.Posts
            .AsNoTracking()
            .Where(p => p.Status == Models.PostStatus.Published)
            .OrderByDescending(p => p.PublishedAt)
            .Skip(skip)
            .Take(take)
            .Select(p => new PostListDto(
                p.Id,
                p.Title,
                p.Slug,
                p.Summary,
                p.PublishedAt,
                p.ReadingMinutes,
                p.Status.ToString()
            ))
            .ToListAsync();

        return Ok(posts);
    }

    [HttpGet("{slug}")]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "slug" })]
    public async Task<ActionResult<PostDetailDto>> GetPostBySlug(string slug)
    {
        var post = await _context.Posts
            .AsNoTracking()
            .Where(p => p.Slug == slug && p.Status == Models.PostStatus.Published)
            .Select(p => new PostDetailDto(
                p.Id,
                p.Title,
                p.Slug,
                p.BodyHtml,
                p.PublishedAt,
                p.ReadingMinutes
            ))
            .FirstOrDefaultAsync();

        if (post == null)
        {
            return NotFound(new { message = "Post not found" });
        }

        return Ok(post);
    }
}
