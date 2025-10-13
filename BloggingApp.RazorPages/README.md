# Blogging App - Razor Pages

This is a Razor Pages version of the Blogging App, converted from Blazor WebAssembly.

## Features

- Home page with list of published blog posts
- Post detail pages with markdown rendering
- Admin login system
- Admin dashboard for managing posts
- Create, edit, and delete posts
- Draft and publish functionality
- Dark OLED theme with minimalistic design

## Running the Application

1. Make sure the API is running (BloggingApp.Api)
2. Run this project:
   ```bash
   dotnet run
   ```
3. Navigate to https://localhost:7001

## Technology Stack

- ASP.NET Core 8.0 Razor Pages
- Bootstrap 5
- Markdig for Markdown parsing
- Session-based authentication

## Admin Credentials

- Email: owner@danyaal.net
- Password: Owner123!

## Project Structure

- `/Pages` - Razor Pages (views and code-behind)
- `/Services` - Business logic and API communication
- `/Models` - DTOs and request/response models
- `/wwwroot` - Static files (CSS, JS, images)
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "http://localhost:5001",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:7001;http://localhost:5001",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}

