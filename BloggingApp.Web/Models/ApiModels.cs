namespace BloggingApp.Web.Models;

public record LoginRequest(string Email, string Password);

public record LoginResponse(string AccessToken, DateTime ExpiresAt);

public record PostListDto(
    Guid Id,
    string Title,
    string Slug,
    string? Summary,
    DateTime? PublishedAt,
    int ReadingMinutes,
    string Status
);

public record PostDetailDto(
    Guid Id,
    string Title,
    string Slug,
    string BodyHtml,
    DateTime? PublishedAt,
    int ReadingMinutes
);

public record PostAdminDto(
    Guid Id,
    string Title,
    string Slug,
    string? Summary,
    string BodyMarkdown,
    string BodyHtml,
    string Status,
    DateTime? PublishedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int ReadingMinutes
);

public record CreatePostRequest(
    string Title,
    string Slug,
    string? Summary,
    string BodyMarkdown,
    string Status
);

public record UpdatePostRequest(
    string Title,
    string Slug,
    string? Summary,
    string BodyMarkdown,
    string Status
);

