# BloggingApp - Ultra-Minimal Blog Platform

A self-hosted blogging platform with ASP.NET Core Web API + Razor Pages.

## Overview

This is a minimal, production-ready blogging platform where you (the Owner) can:
- Sign in securely with JWT authentication
- Write posts in Markdown
- Publish posts to appear on the public homepage
- Each post gets its own URL based on a slug

The platform consists of two projects:
- **BloggingApp.Api** - ASP.NET Core Web API (backend)
- **BloggingApp.RazorPages** - Razor Pages (frontend)

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

The API will start at `https://localhost:7218`

> **Note**: The latest migration includes performance indexes. If you're cloning this repo fresh, `dotnet ef database update` will automatically apply all migrations including the performance optimizations.

**Default Owner Credentials:**
- Email: `owner@danyaal.net`
- Password: `Owner123!`

### 2. Start the Razor Pages App

Open a new terminal:

```bash
cd BloggingApp.RazorPages

# Run the app
dotnet run
```

The app will start at `https://localhost:7001`

### 3. Use the App

1. Visit `https://localhost:7001` in your browser
2. Go to `/Admin/Login` and sign in with the owner credentials
3. Create your first post at `/Admin/Posts/New`
4. Publish it and see it appear on the homepage!

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                 BloggingApp.RazorPages                      │
│                    (Razor Pages)                            │
│                                                             │
│  Public Pages:        Admin Pages:                          │
│  - / (home)          - /Admin/Login                         │
│  - /Post/{slug}      - /Admin/Posts                         │
│  - /About            - /Admin/Posts/New                     │
│                      - /Admin/Posts/{id}/Edit               │
└────────────────────┬────────────────────────────────────────┘
                     │
                     │ HTTPS + JWT Bearer Token (in Session)
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
└────────────────────┬────────────────────────────────────────┘
                     │
                     │ Entity Framework Core
                     │
┌────────────────────▼────────────────────────────────────────┐
│                  SQLite Database                            │
│                  (Data/blogging.db)                         │
│                                                             │
│  Tables:                                                    │
│  - Users (Owner account)                                    │
│  - Posts (Blog posts with Markdown content)                 │
│                                                             │
│  Performance:                                               │
│  - Indexed slug for fast lookups                            │
│  - Indexed PublishedAt + Status for homepage queries        │
└─────────────────────────────────────────────────────────────┘
```

## Features

### For Visitors
- **Fast Loading** - Server-side rendering with Razor Pages
- **Clean Reading Experience** - Minimal, distraction-free design
- **SEO Friendly** - Server-rendered HTML with proper meta tags
- **Responsive** - Works perfectly on mobile and desktop

### For You (The Owner)
- **Secure Admin Panel** - JWT-based authentication
- **Markdown Editor** - Write posts in Markdown with live preview
- **Draft System** - Save drafts before publishing
- **Easy Management** - View, edit, and delete posts from one place
- **Auto-Slugs** - Automatic URL slug generation from titles

## Technology Stack

- **Backend**: ASP.NET Core 8.0 Web API
- **Frontend**: ASP.NET Core 8.0 Razor Pages
- **Database**: SQLite with Entity Framework Core
- **Authentication**: JWT Bearer tokens (stored in session)
- **Markdown**: Markdig for rendering
- **Styling**: Bootstrap 5 with custom dark theme

## Project Structure

```
BloggingApp/
├── BloggingApp.Api/          # Backend API
│   ├── Controllers/          # API endpoints
│   ├── Data/                 # Database context
│   ├── DTOs/                 # Data transfer objects
│   ├── Migrations/           # EF Core migrations
│   ├── Models/               # Entity models
│   └── Services/             # Business logic
│
├── BloggingApp.RazorPages/   # Frontend
│   ├── Pages/                # Razor Pages
│   │   ├── Admin/           # Admin pages
│   │   └── Shared/          # Layout and shared components
│   ├── Services/             # API communication services
│   ├── Models/               # View models and DTOs
│   └── wwwroot/              # Static files (CSS, JS)
│
└── BloggingApp.sln           # Solution file
```

## API Endpoints

### Public Endpoints
- `GET /api/posts` - Get published posts (paginated)
- `GET /api/posts/{slug}` - Get a specific post by slug
- `GET /healthz` - Health check
- `GET /sitemap.xml` - XML sitemap for SEO

### Authentication
- `POST /api/auth/login` - Login with email/password

### Admin Endpoints (Requires Authentication)
- `GET /api/admin/posts` - Get all posts (including drafts)
- `POST /api/admin/posts` - Create a new post
- `PUT /api/admin/posts/{id}` - Update a post
- `DELETE /api/admin/posts/{id}` - Delete a post

## Configuration

### API Configuration (`BloggingApp.Api/appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=Data/blogging.db"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-here-make-it-long-and-random",
    "ExpiresInHours": 24
  }
}
```

### Razor Pages Configuration (`BloggingApp.RazorPages/appsettings.json`)
```json
{
  "ApiBaseUrl": "https://localhost:7218/"
}
```

## Deployment

### Development
1. Run the API: `cd BloggingApp.Api && dotnet run`
2. Run the frontend: `cd BloggingApp.RazorPages && dotnet run`

### Production
1. Build both projects: `dotnet build -c Release`
2. Publish the API: `dotnet publish BloggingApp.Api -c Release -o ./publish/api`
3. Publish the frontend: `dotnet publish BloggingApp.RazorPages -c Release -o ./publish/web`
4. Deploy to your server using reverse proxy (nginx/Apache)
5. Set up SSL certificates (Let's Encrypt)

## Environment Variables

Set these for production deployment:

```bash
# API
OWNER_EMAIL=your@email.com
OWNER_PASSWORD=YourSecurePassword123!
JWT_SECRET_KEY=your-super-secret-jwt-key-here
ASPNETCORE_ENVIRONMENT=Production

# Database (or use appsettings.Production.json)
ConnectionStrings__DefaultConnection="Data Source=/var/app/blogging.db"
```

## Security Notes

- ⚠️ Change the default owner credentials immediately in production
- ⚠️ Use a strong, random JWT secret key
- ✅ The app uses HTTPS by default
- ✅ Passwords are hashed with BCrypt
- ✅ JWT tokens are stored in secure, HTTP-only session cookies in Razor Pages
- ✅ Admin endpoints require authentication

## License

MIT License - feel free to use this for your own blog!

## Credits

Built with ASP.NET Core, designed for simplicity and speed.
