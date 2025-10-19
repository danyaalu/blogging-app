# BloggingApp - Docker Deployment Guide

This guide covers deploying the BloggingApp using Docker and Docker Compose, configured to run behind a reverse proxy (e.g., Traefik, Nginx Proxy Manager, Caddy).

## Architecture

- **API Service**: ASP.NET Core Web API (port 8080)
- **Web Service**: ASP.NET Core Razor Pages (port 8081)
- **Database**: SQLite (persisted in Docker volume)
- **Reverse Proxy**: External (you manage this separately)

Both services run on HTTP only. Your reverse proxy will handle SSL/TLS termination.

## Quick Start!

### 1. Configure Environment Variables

Copy the example environment file and customize it:

```bash
cp .env.example .env
```

Edit `.env` and update these critical values:

```env
# Your actual domain
DOMAIN_NAME=yourdomain.com

# Owner account credentials
OWNER_EMAIL=admin@yourdomain.com
OWNER_PASSWORD=YourSecurePassword123!

# JWT Secret (generate a strong random key)
JWT_KEY=your-super-secret-key-change-in-production-min-32-chars-please-make-it-secure

# CORS Origins (add your actual domains)
CORS_ALLOWED_ORIGINS=https://yourdomain.com,https://www.yourdomain.com
```

### 2. Build and Start Services

```bash
# Build and start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Stop and remove volumes (CAREFUL: deletes database)
docker-compose down -v
```

### 3. Configure Reverse Proxy

Configure your reverse proxy to forward traffic to the containers:

#### API Service
- **Container**: `blogging-api`
- **Internal URL**: `http://blogging-api:8080`
- **External URL**: `https://api.yourdomain.com`

#### Web Service
- **Container**: `blogging-web`
- **Internal URL**: `http://blogging-web:8080`
- **External URL**: `https://www.yourdomain.com` or `https://yourdomain.com`

#### Example: Traefik Labels

Add these labels to your docker-compose.yml services if using Traefik:

```yaml
services:
  api:
    labels:
      - "traefik.enable=true"
      - "traefik.http.routers.blogging-api.rule=Host(`api.yourdomain.com`)"
      - "traefik.http.routers.blogging-api.entrypoints=websecure"
      - "traefik.http.routers.blogging-api.tls.certresolver=letsencrypt"
      - "traefik.http.services.blogging-api.loadbalancer.server.port=8080"
  
  web:
    labels:
      - "traefik.enable=true"
      - "traefik.http.routers.blogging-web.rule=Host(`www.yourdomain.com`)"
      - "traefik.http.routers.blogging-web.entrypoints=websecure"
      - "traefik.http.routers.blogging-web.tls.certresolver=letsencrypt"
      - "traefik.http.services.blogging-web.loadbalancer.server.port=8080"
```

#### Example: Nginx Configuration

```nginx
# API
server {
    listen 443 ssl http2;
    server_name api.yourdomain.com;
    
    ssl_certificate /path/to/cert.pem;
    ssl_certificate_key /path/to/key.pem;
    
    location / {
        proxy_pass http://localhost:8080;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}

# Web
server {
    listen 443 ssl http2;
    server_name www.yourdomain.com;
    
    ssl_certificate /path/to/cert.pem;
    ssl_certificate_key /path/to/key.pem;
    
    location / {
        proxy_pass http://localhost:8081;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

## Environment Variables Reference

All configuration is centralized in the `.env` file:

| Variable | Description | Default | Required |
|----------|-------------|---------|----------|
| `DOMAIN_NAME` | Your domain name | `localhost` | Yes |
| `API_SUBDOMAIN` | API subdomain | `api` | No |
| `WEB_SUBDOMAIN` | Web subdomain | `www` | No |
| `API_BASE_URL` | Internal API URL for web service | `http://api:8080` | Yes |
| `DATABASE_PATH` | SQLite database path in container | `/app/data/blogging.db` | Yes |
| `JWT_KEY` | JWT signing key (min 32 chars) | - | Yes |
| `JWT_ISSUER` | JWT issuer | `BloggingApp.Api` | Yes |
| `JWT_AUDIENCE` | JWT audience | `BloggingApp.Web` | Yes |
| `OWNER_EMAIL` | Initial admin email | - | Yes |
| `OWNER_PASSWORD` | Initial admin password | - | Yes |
| `CORS_ALLOWED_ORIGINS` | Comma-separated allowed origins | - | Yes |
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Production` | No |
| `LOG_LEVEL` | Logging level | `Information` | No |

## Security Considerations

1. **JWT Key**: Generate a strong random key (at least 32 characters)
   ```bash
   openssl rand -base64 48
   ```

2. **Owner Password**: Use a strong password with uppercase, lowercase, numbers

3. **CORS Origins**: Only add your actual domain names

4. **Environment File**: Never commit `.env` to version control
   ```bash
   echo ".env" >> .gitignore
   ```

## Data Persistence

The API's SQLite database is persisted in a Docker volume named `api-data`:

```bash
# Backup database
docker cp blogging-api:/app/data/blogging.db ./backup-$(date +%Y%m%d).db

# Restore database
docker cp ./backup.db blogging-api:/app/data/blogging.db
docker-compose restart api

# Inspect volume
docker volume inspect blogging-app_api-data
```

## Health Checks

The API includes a health check endpoint:

```bash
# Check API health
curl http://localhost:8080/healthz

# In production (through reverse proxy)
curl https://api.yourdomain.com/healthz
```

## Troubleshooting

### View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f api
docker-compose logs -f web
```

### Access Container Shell
```bash
docker exec -it blogging-api bash
docker exec -it blogging-web bash
```

### Check Database
```bash
docker exec -it blogging-api sqlite3 /app/data/blogging.db ".tables"
```

### Rebuild After Code Changes
```bash
docker-compose down
docker-compose build --no-cache
docker-compose up -d
```

## Development vs Production

For local development without reverse proxy, you can still use the containers:

1. Set `ASPNETCORE_ENVIRONMENT=Development` in `.env`
2. Access services directly:
   - API: http://localhost:8080
   - Web: http://localhost:8081

For production:
- Keep `ASPNETCORE_ENVIRONMENT=Production`
- Access only through your reverse proxy with HTTPS
- The containers run HTTP only; reverse proxy handles SSL/TLS

## Network

Both services communicate on the `blogging-network` Docker network:
- Web service calls API internally via `http://api:8080`
- External traffic goes through your reverse proxy

## Updating

```bash
# Pull latest code
git pull

# Rebuild and restart
docker-compose down
docker-compose build
docker-compose up -d

# Check logs to ensure successful startup
docker-compose logs -f
```

## Complete Example Setup

1. **Configure .env**
   ```env
   DOMAIN_NAME=myblog.com
   OWNER_EMAIL=admin@myblog.com
   OWNER_PASSWORD=MySecurePassword123!
   JWT_KEY=<generated-with-openssl>
   CORS_ALLOWED_ORIGINS=https://myblog.com,https://www.myblog.com
   ```

2. **Start services**
   ```bash
   docker-compose up -d
   ```

3. **Configure reverse proxy** (example with Caddy)
   ```
   api.myblog.com {
       reverse_proxy localhost:8080
   }
   
   www.myblog.com {
       reverse_proxy localhost:8081
   }
   ```

4. **Access your blog**
   - Visit: https://www.myblog.com
   - Login with credentials from `.env`
bin/
obj/
*.db
*.db-shm
*.db-wal
.vs/
.vscode/
.idea/
*.user
*.suo
*.cache
.git/
.gitignore
README.md
*.md
Dockerfile
.dockerignore

