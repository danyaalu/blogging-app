using System.ComponentModel.DataAnnotations;

namespace BloggingApp.Api.Models;

public class Post
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Summary { get; set; }

    [Required]
    public string BodyMarkdown { get; set; } = string.Empty;

    public string BodyHtml { get; set; } = string.Empty;

    public PostStatus Status { get; set; } = PostStatus.Draft;

    public DateTime? PublishedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int ReadingMinutes { get; set; }
}

public enum PostStatus
{
    Draft,
    Published
}

