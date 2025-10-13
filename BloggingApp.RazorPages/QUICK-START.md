# Quick Start Guide - Razor Pages Version

## What Was Created

A complete Razor Pages version of your Blazor WebAssembly blogging app with:

✅ **All pages converted:**
- Home page with blog post listings
- Post detail pages
- About page
- Admin login
- Admin dashboard
- Create/Edit/Delete posts

✅ **Complete feature parity:**
- Session-based authentication
- Full CRUD operations for posts
- Dark OLED theme (identical styling)
- Responsive design
- Loading states and error handling

✅ **Production-ready:**
- No build errors
- No warnings
- All dependencies resolved
- Added to solution file

## Running the Application

### Step 1: Start the API (Required)
```bash
cd BloggingApp.Api
dotnet run
```
The API will run at `https://localhost:7218`

### Step 2: Start the Razor Pages App
```bash
cd BloggingApp.RazorPages
dotnet run
```
The app will run at `https://localhost:7001`

### Step 3: Access the Application
Open your browser to: `https://localhost:7001`

## Admin Access

**Login URL:** `https://localhost:7001/Admin/Login`

**Credentials:**
- Email: `owner@danyaal.net`
- Password: `Owner123!`

## Project Location

The new Razor Pages project is located at:
```
/home/danyaal/Documents/git-repos/blogging-app/BloggingApp.RazorPages/
```

## Key Routes

| Page | URL |
|------|-----|
| Home | `/` |
| About | `/About` |
| Post Detail | `/Post/{slug}` |
| Admin Login | `/Admin/Login` |
| Manage Posts | `/Admin/Posts` |
| New Post | `/Admin/Posts/New` |
| Edit Post | `/Admin/Posts/{id}/Edit` |

## Differences from Blazor Version

1. **Server-side rendering** instead of client-side WebAssembly
2. **Session storage** for auth tokens instead of localStorage
3. **Full page reloads** on navigation instead of SPA routing
4. **Separate .cshtml + .cshtml.cs files** instead of single .razor files
5. **Better SEO** out of the box (no pre-rendering needed)
6. **Faster initial load** (no WebAssembly download)

## Running Both Side-by-Side

You can run both versions simultaneously:

**Blazor WASM:** `https://localhost:7109`
**Razor Pages:** `https://localhost:7001`
**API:** `https://localhost:7218`

They both connect to the same API and use the same database!

## Testing the Conversion

1. Start the API
2. Start the Razor Pages app
3. Browse the home page - you should see your blog posts
4. Click on a post to view details
5. Go to `/Admin/Login` and log in
6. Create, edit, or delete posts
7. Everything should work identically to the Blazor version!

## Troubleshooting

If you encounter issues:

1. **API not responding:** Make sure BloggingApp.Api is running on port 7218
2. **CSS not loading:** Run `dotnet build` to ensure wwwroot files are included
3. **Session issues:** Clear browser cookies and try again
4. **Port conflicts:** Check if ports 7001 or 5001 are already in use

## Next Steps

- Compare the code between Blazor and Razor Pages versions
- Run performance tests
- Deploy to production if needed
- See `BLAZOR-TO-RAZOR-CONVERSION.md` for detailed conversion documentation

Enjoy your new Razor Pages version! 🎉

