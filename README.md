# BloggingApp - Ultra-Minimal Blog Platform

A self-hosted blogging platform with ASP.NET Core Web API + Blazor WebAssembly.

## Overview

This is a minimal, production-ready blogging platform where you (the Owner) can:
- Sign in securely with JWT authentication
- Write posts in Markdown
- Publish posts to appear on the public homepage
- Each post gets its own URL based on a slug

The platform consists of two projects:
- **BloggingApp.Api** - ASP.NET Core Web API (backend)
- **BloggingApp.Web** - Blazor WebAssembly (frontend)

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- Entity Framework Core CLI tools: `dotnet tool install --global dotnet-ef`

### 1. Start the API

```bash
cd BloggingApp.Api

# Apply database migrations (creates SQLite database)
dotnet ef database update

# Optional: Set custom owner credentials
export OWNER_EMAIL="your@email.com"
export OWNER_PASSWORD="YourPassword123!"

# Run the API
dotnet run
```

The API will start at `https://localhost:7194`

**Default Owner Credentials:**
- Email: `owner@danyaal.net`
- Password: `Owner123!`

### 2. Start the Blazor WebAssembly App

Open a new terminal:

```bash
cd BloggingApp.Web

# Run the app
dotnet run
```

The app will start at `https://localhost:5001`

### 3. Use the App

1. Visit `https://localhost:5001` in your browser
2. Go to `/admin/login` and sign in with the owner credentials
3. Create your first post at `/admin/posts/new`
4. Publish it and see it appear on the homepage!

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    BloggingApp.Web                          │
│                  (Blazor WebAssembly)                       │
│                                                             │
│  Public Pages:        Admin Pages:                          │
│  - / (home)          - /admin/login                         │
│  - /p/{slug}         - /admin/posts                         │
│  - /about            - /admin/posts/new                     │
│                      - /admin/posts/{id}/edit               │
└────────────────────┬────────────────────────────────────────┘
                     │
                     │ HTTPS + JWT Bearer Token
                     │
┌────────────────────▼────────────────────────────────────────┐
│                   BloggingApp.Api                           │
│              (ASP.NET Core Web API)                         │
│                                                             │
│  Public Endpoints:       Admin Endpoints:                   │
│  - GET /api/posts       - GET /api/admin/posts              │
│  - GET /api/posts/{slug} - POST /api/admin/posts            │
│  - GET /healthz         - PUT /api/admin/posts/{id}         │
│                         - DELETE /api/admin/posts/{id}      │
│  Auth:                                                      │
│  - POST /api/auth/login                                     │
│  - POST /api/auth/logout                                    │
└────────────────────┬────────────────────────────────────────┘
                     │
                     │ Entity Framework Core
                     │
┌────────────────────▼────────────────────────────────────────┐
│                  SQLite Database                            │
│                  (Data/blogging.db)                         │
│                                                             │
│  Tables: Posts, AspNetUsers, AspNetRoles, etc.              │
└─────────────────────────────────────────────────────────────┘
```

## Technology Stack

### Backend (BloggingApp.Api)
- **Framework**: ASP.NET Core 8.0 Web API
- **Database**: SQLite with Entity Framework Core 8.0
- **Authentication**: ASP.NET Core Identity + JWT Bearer tokens
- **Markdown**: Markdig (conversion) + HtmlSanitizer (security)
- **API Documentation**: Swagger/OpenAPI

### Frontend (BloggingApp.Web)
- **Framework**: Blazor WebAssembly (standalone)
- **UI**: Bootstrap 5
- **HTTP Client**: Built-in HttpClient with JWT token support
- **Routing**: Blazor Router

## Features

### Authentication & Authorization
- Single Owner account (local login only)
- JWT Bearer token authentication (8-hour expiration)
- Account lockout after 5 failed attempts
- Password requirements: min 8 chars, uppercase, lowercase, digit

### Post Management
- **Create**: Write posts in Markdown with auto-slug generation
- **Edit**: Modify all post fields, toggle Draft/Published status
- **Delete**: Hard delete (acceptable for v1)
- **List**: Filter by status (Draft/Published)

### Markdown & Content
- Full Markdown support with advanced extensions
- HTML sanitization to prevent XSS attacks
- Auto-generated reading time (based on 200 words/min)
- Cached HTML rendering for performance

### Data Model
- **Post**: Id, Title, Slug (unique), Summary, BodyMarkdown, BodyHtml, Status, PublishedAt, CreatedAt, UpdatedAt, ReadingMinutes
- **User**: ASP.NET Core Identity (Owner role)

### API Features
- RESTful endpoints
- CORS configured for development and production
- Proper HTTP status codes (200, 201, 400, 401, 404, etc.)
- Health check endpoint at `/healthz`
- Paged results for post listings

## Project Structure

```
blogging-app/
├── BloggingApp.sln
├── README.md (this file)
│
├── BloggingApp.Api/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── PostsController.cs (public)
│   │   └── AdminPostsController.cs (owner only)
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   └── blogging.db (created on first run)
│   ├── DTOs/
│   │   └── ApiDtos.cs
│   ├── Models/
│   │   ├── ApplicationUser.cs
│   │   └── Post.cs
│   ├── Services/
│   │   ├── MarkdownService.cs
│   │   └── SlugService.cs
│   ├── Migrations/ (EF Core migrations)
│   ├── Program.cs
│   ├── appsettings.json
│   └── README.md
│
└── BloggingApp.Web/
    ├── Layout/
    │   ├── MainLayout.razor
    │   └── NavMenu.razor
    ├── Models/
    │   └── ApiModels.cs
    ├── Pages/
    │   ├── Home.razor
    │   ├── PostDetail.razor
    │   ├── About.razor
    │   ├── AdminLogin.razor
    │   ├── AdminPosts.razor
    │   ├── AdminPostNew.razor
    │   └── AdminPostEdit.razor
    ├── Services/
    │   ├── AuthService.cs
    │   ├── PostService.cs
    │   └── SlugService.cs
    ├── wwwroot/
    │   ├── appsettings.json
    │   └── css/
    ├── Program.cs
    └── README.md
```

## Configuration

### API Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=Data/blogging.db"
  },
  "Jwt": {
    "Key": "your-super-secret-key-change-in-production-min-32-chars",
    "Issuer": "BloggingApp.Api",
    "Audience": "BloggingApp.Web"
  },
  "Cors": {
    "AllowedOrigins": [
      "https://localhost:5001",
      "http://localhost:5000",
      "https://localhost:5173",
      "http://localhost:5173"
    ]
  }
}
```

### Web Configuration (wwwroot/appsettings.json)

```json
{
  "ApiBaseUrl": "https://localhost:7194"
}
```

## Development Workflow

### Creating a New Post

1. Start both API and Web apps
2. Navigate to `https://localhost:5001/admin/login`
3. Login with owner credentials
4. Click "New Post"
5. Enter:
   - **Title**: "My First Post"
   - **Slug**: Auto-generated as "my-first-post" (editable)
   - **Summary**: Optional brief description
   - **Body**: Markdown content
   - **Status**: "Draft" or "Published"
6. Click "Create Post"
7. If published, visit `https://localhost:5001/p/my-first-post`

### Testing the API Directly

```bash
# Login and get JWT token
curl -X POST https://localhost:7194/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"owner@danyaal.net","password":"Owner123!"}' \
  -k

# Get published posts
curl https://localhost:7194/api/posts -k

# Get post by slug
curl https://localhost:7194/api/posts/my-first-post -k

# Create post (authenticated)
TOKEN="your-jwt-token-here"
curl -X POST https://localhost:7194/api/admin/posts \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "title": "Test Post",
    "slug": "test-post",
    "summary": "A test post",
    "bodyMarkdown": "# Hello\n\nThis is a test.",
    "status": "Published"
  }' \
  -k
```

## Production Deployment

### API Deployment

1. Update `appsettings.json`:
   - Change JWT Key to a strong secret
   - Update CORS origins to your domain
   - Consider using Azure Key Vault or environment variables for secrets

2. Publish:
   ```bash
   cd BloggingApp.Api
   dotnet publish -c Release -o ./publish
   ```

3. Deploy to your hosting platform (Azure App Service, DigitalOcean, AWS, etc.)

4. Ensure the `Data` directory is writable for SQLite

### Web Deployment

1. Update `wwwroot/appsettings.json`:
   ```json
   {
     "ApiBaseUrl": "https://api.danyaal.net"
   }
   ```

2. Build for production:
   ```bash
   cd BloggingApp.Web
   dotnet publish -c Release -o ./publish
   ```

3. Deploy `./publish/wwwroot` to a static hosting service:
   - Azure Static Web Apps
   - GitHub Pages
   - Netlify
   - Vercel
   - Any CDN or web server

### CORS Configuration for Production

Update API's `appsettings.json`:
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://danyaal.net",
      "https://www.danyaal.net"
    ]
  }
}
```

## Security Considerations

- ✅ JWT tokens with 8-hour expiration
- ✅ Password hashing via ASP.NET Core Identity
- ✅ Account lockout protection
- ✅ HTML sanitization for Markdown content
- ✅ CORS restricted to specific origins
- ✅ HTTPS enforced in production
- ⚠️ Change default JWT key before production
- ⚠️ Use environment variables for sensitive data
- ⚠️ Consider rate limiting for production APIs

## Limitations (v1)

Out of scope for this minimal release:
- ❌ Comments, reactions, or social features
- ❌ Tags, categories, or search
- ❌ RSS feed or sitemap
- ❌ Image uploads
- ❌ SEO/Open Graph automation
- ❌ Server-side caching
- ❌ Analytics integration
- ❌ OAuth/social login
- ❌ Email notifications
- ❌ Password reset flow
- ❌ Multi-user roles

## Acceptance Criteria ✅

- ✅ API + WASM run locally
- ✅ Owner seeded from env variables on first run
- ✅ Can log in at `/admin/login`
- ✅ Can create a draft post
- ✅ Can publish a post
- ✅ Published post appears on `/` homepage
- ✅ Published post accessible at `/p/{slug}`
- ✅ Public API returns correct data
- ✅ Invalid slug returns 404
- ✅ JWT required for admin endpoints
- ✅ Unauthenticated requests yield 401
- ✅ CORS works for WASM dev origin

## Troubleshooting

### Database Issues
```bash
# Reset database
cd BloggingApp.Api
rm -rf Data/blogging.db
dotnet ef database update
```

### Migration Issues
```bash
# Remove last migration
dotnet ef migrations remove

# Add new migration
dotnet ef migrations add MigrationName
```

### CORS Errors
Ensure the API's CORS configuration includes the exact origin of your WASM app (including protocol and port).

### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

## License

This is a personal project for danyaal.net. Modify as needed for your use case.

## Support

For issues or questions, refer to the individual README files in each project:
- [BloggingApp.Api README](./BloggingApp.Api/README.md)
- [BloggingApp.Web README](./BloggingApp.Web/README.md)

