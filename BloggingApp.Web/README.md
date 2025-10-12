# BloggingApp.Web

Blazor WebAssembly client for the ultra-minimal blogging platform.

## Features

- **Public Pages**: View published blog posts
- **Admin Pages**: Login, create, edit, and delete posts
- **Markdown Editor**: Write posts in Markdown
- **Responsive UI**: Clean, Bootstrap-based design
- **Client-side Routing**: Fast navigation with Blazor

## Prerequisites

- .NET 8.0 SDK
- Running BloggingApp.Api instance

## Getting Started

### 1. Configure API Base URL

Update `wwwroot/appsettings.json` with your API URL:

```json
{
  "ApiBaseUrl": "https://localhost:7194"
}
```

**For production**, change to your production API URL:
```json
{
  "ApiBaseUrl": "https://api.danyaal.net"
}
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Run the Application

```bash
dotnet run
```

The app will start at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

Or use the default Blazor WASM dev server (usually `https://localhost:5173`).

## Application Structure

### Public Pages

- **`/`** - Home page listing all published posts
- **`/p/{slug}`** - Individual post view
- **`/about`** - About page

### Admin Pages (Requires Authentication)

- **`/admin/login`** - Login page
- **`/admin/posts`** - List all posts (drafts and published)
- **`/admin/posts/new`** - Create a new post
- **`/admin/posts/{id}/edit`** - Edit an existing post

## Features

### Authentication

The app uses JWT Bearer token authentication stored in session storage (browser memory).

**Default Login Credentials:**
- Email: `owner@danyaal.net`
- Password: `Owner123!`

(Or whatever you configured via environment variables when running the API)

### Post Management

1. **Create Post**: Click "New Post" from the admin dashboard
   - Enter title (auto-generates slug)
   - Add optional summary
   - Write content in Markdown
   - Set status (Draft or Published)

2. **Edit Post**: Click "Edit" on any post in the admin dashboard
   - Modify any field
   - Change status from Draft to Published
   - PublishedAt timestamp is set automatically on first publish

3. **Delete Post**: Click "Delete" on any post (hard delete in v1)

4. **Logout**: Click "Logout" to clear the session token

### Markdown Support

The editor accepts standard Markdown syntax:

```markdown
# Heading 1
## Heading 2

**Bold text**
*Italic text*

- List item 1
- List item 2

[Link text](https://example.com)

> Blockquote

`inline code`
```

The API converts Markdown to HTML and sanitizes it before storage.

### Slug Auto-Generation

When creating a new post, the slug is automatically generated from the title:
- Converts to lowercase
- Replaces spaces with hyphens
- Removes special characters
- Example: "My First Post!" → "my-first-post"

You can manually edit the slug if needed.

## Services

### AuthService
- `LoginAsync()` - Authenticate user and store JWT token
- `Logout()` - Clear session token
- `GetToken()` - Retrieve stored token
- `IsAuthenticated()` - Check if user is logged in

### PostService
- `GetPublishedPostsAsync()` - Fetch published posts (public)
- `GetPostBySlugAsync()` - Get single post by slug (public)
- `GetAdminPostsAsync()` - Get all posts including drafts (admin)
- `CreatePostAsync()` - Create new post (admin)
- `UpdatePostAsync()` - Update existing post (admin)
- `DeletePostAsync()` - Delete post (admin)

### SlugService
- `GenerateSlug()` - Convert title to URL-friendly slug

## Project Structure

```
BloggingApp.Web/
├── Models/
│   └── ApiModels.cs          # DTOs matching API contracts
├── Pages/
│   ├── Home.razor            # Public: List posts
│   ├── PostDetail.razor      # Public: View post
│   ├── About.razor           # Public: About page
│   ├── AdminLogin.razor      # Admin: Login
│   ├── AdminPosts.razor      # Admin: Post list
│   ├── AdminPostNew.razor    # Admin: Create post
│   └── AdminPostEdit.razor   # Admin: Edit post
├── Services/
│   ├── AuthService.cs        # Authentication service
│   ├── PostService.cs        # Post management service
│   └── SlugService.cs        # Slug generation service
├── Layout/
│   ├── MainLayout.razor      # Main layout
│   └── NavMenu.razor         # Navigation menu
├── wwwroot/
│   ├── appsettings.json      # Configuration
│   └── css/                  # Styles
└── Program.cs                # App entry point
```

## Development

### Hot Reload

The app supports hot reload during development. Changes to `.razor` files will automatically refresh in the browser.

### Debugging

Use browser DevTools to debug the Blazor WebAssembly app:
1. Press F12 to open DevTools
2. Go to Sources tab
3. Find your C# files under `file://` → `dotnet://`

## Production Build

Build for production:

```bash
dotnet publish -c Release -o ./publish
```

The output in `./publish/wwwroot` can be hosted on any static file server (Nginx, Apache, CDN, etc.).

### Hosting Options

1. **Azure Static Web Apps**
2. **GitHub Pages** (with custom domain)
3. **Netlify**
4. **Vercel**
5. **Any web server** serving the `wwwroot` folder

## CORS Configuration

Ensure the API's CORS configuration includes your WASM app's URL:

**Development:**
```json
"Cors": {
  "AllowedOrigins": [
    "https://localhost:5001",
    "http://localhost:5000",
    "https://localhost:5173"
  ]
}
```

**Production:**
```json
"Cors": {
  "AllowedOrigins": [
    "https://danyaal.net"
  ]
}
```

## Session Storage

JWT tokens are stored in session storage (in-memory). Tokens are lost when:
- Browser tab is closed
- User explicitly logs out

For a production app, consider using local storage with proper security measures.

## Known Limitations (v1)

- No preview mode for Markdown
- No image upload support
- No search functionality
- Session storage only (tokens don't persist across tabs)
- Basic confirm() dialog for delete (no custom modal)
- No pagination controls (fixed page size)

## Roadmap

Future enhancements could include:
- Markdown preview
- Image upload and management
- Search and filtering
- Tags and categories
- RSS feed
- SEO optimization
- Analytics integration
# BloggingApp.Api

Ultra-minimal blogging API built with ASP.NET Core 8.0, EF Core, SQLite, and JWT authentication.

## Features

- **Public API**: Read-only access to published posts
- **Admin API**: Full CRUD for posts (requires Owner authentication)
- **Authentication**: JWT Bearer tokens via ASP.NET Core Identity
- **Markdown Support**: Converts Markdown to sanitized HTML
- **SQLite Database**: Lightweight, file-based storage

## Prerequisites

- .NET 8.0 SDK
- Entity Framework Core CLI tools (`dotnet tool install --global dotnet-ef`)

## Getting Started

### 1. Restore Dependencies

```bash
dotnet restore
```

### 2. Apply Database Migrations

The database migration has already been created. Apply it to create the SQLite database:

```bash
dotnet ef database update
```

This will create `Data/blogging.db`.

### 3. Set Owner Credentials (Optional)

On first run, an Owner user will be created automatically. You can customize the credentials using environment variables:

```bash
export OWNER_EMAIL="owner@danyaal.net"
export OWNER_PASSWORD="Owner123!"
```

**Default credentials** (if not set):
- Email: `owner@danyaal.net`
- Password: `Owner123!`

### 4. Run the API

```bash
dotnet run
```

The API will start at:
- HTTPS: `https://localhost:7194`
- HTTP: `http://localhost:5194`

Visit `https://localhost:7194/swagger` to explore the API documentation.

## API Endpoints

### Public Endpoints (No Authentication)

**Get Published Posts (Paginated)**
```bash
GET /api/posts?skip=0&take=10
```

**Get Post by Slug**
```bash
GET /api/posts/{slug}
```

**Health Check**
```bash
GET /healthz
```

### Authentication

**Login**
```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "owner@danyaal.net",
  "password": "Owner123!"
}
```

Response:
```json
{
  "accessToken": "eyJhbGc...",
  "expiresAt": "2025-10-13T04:00:00Z"
}
```

**Logout** (client-side token discard)
```bash
POST /api/auth/logout
```

### Admin Endpoints (Requires Bearer Token)

**Get All Posts (including drafts)**
```bash
GET /api/admin/posts?status=Draft
Authorization: Bearer {token}
```

**Create Post**
```bash
POST /api/admin/posts
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "My First Post",
  "slug": "my-first-post",
  "summary": "A brief summary",
  "bodyMarkdown": "# Hello World\n\nThis is my first post!",
  "status": "Draft"
}
```

**Update Post**
```bash
PUT /api/admin/posts/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "My Updated Post",
  "slug": "my-updated-post",
  "summary": "An updated summary",
  "bodyMarkdown": "# Hello World\n\nThis post has been updated!",
  "status": "Published"
}
```

**Delete Post**
```bash
DELETE /api/admin/posts/{id}
Authorization: Bearer {token}
```

## Example curl Commands

### Login and Get Token
```bash
curl -X POST https://localhost:7194/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"owner@danyaal.net","password":"Owner123!"}' \
  -k
```

### Get Published Posts
```bash
curl https://localhost:7194/api/posts?skip=0&take=10 -k
```

### Get Post by Slug
```bash
curl https://localhost:7194/api/posts/my-first-post -k
```

### Create a Post (Authenticated)
```bash
TOKEN="your-jwt-token-here"

curl -X POST https://localhost:7194/api/admin/posts \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "title": "My First Post",
    "slug": "my-first-post",
    "summary": "A brief summary of my post",
    "bodyMarkdown": "# Hello World\n\nThis is my **first** blog post!",
    "status": "Published"
  }' \
  -k
```

## Configuration

### appsettings.json

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

**Important for Production:**
- Change the JWT `Key` to a strong, unique secret
- Update `AllowedOrigins` to include your production domain (e.g., `https://danyaal.net`)
- Consider using environment variables for sensitive configuration

## Database Schema

### Posts Table
- `Id` (Guid) - Primary key
- `Title` (string, max 200) - Required
- `Slug` (string, max 200) - Required, unique, indexed
- `Summary` (string, max 300) - Optional
- `BodyMarkdown` (text) - Required
- `BodyHtml` (text) - Auto-generated from Markdown
- `Status` (enum: Draft|Published)
- `PublishedAt` (DateTime?) - Set when status changes to Published
- `CreatedAt` (DateTime) - Auto-set on creation
- `UpdatedAt` (DateTime) - Auto-updated on modification
- `ReadingMinutes` (int) - Calculated based on word count (200 words/min)

### Identity Tables
Standard ASP.NET Core Identity tables for users, roles, etc.

## Security Notes

- JWT tokens expire after 8 hours
- Account lockout after 5 failed login attempts (15-minute lockout)
- Password requirements: min 8 characters, uppercase, lowercase, digit
- Markdown is sanitized to prevent XSS attacks
- CORS is configured for specific origins only

## Development

To create additional migrations:
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

To remove the last migration:
```bash
dotnet ef migrations remove
```

## Production Deployment

1. Update `appsettings.json` with production values
2. Set environment variables for sensitive data:
   - `OWNER_EMAIL`
   - `OWNER_PASSWORD`
   - JWT Key (via configuration or secrets)
3. Update CORS allowed origins to include your domain
4. Deploy the application
5. Ensure the `Data` directory is writable for SQLite

