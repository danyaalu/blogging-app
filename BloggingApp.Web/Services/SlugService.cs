using System.Text.RegularExpressions;

namespace BloggingApp.Web.Services;

public interface ISlugService
{
    string GenerateSlug(string title);
}

public class SlugService : ISlugService
{
    public string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        var slug = title.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", string.Empty);
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');

        return slug;
    }
}
