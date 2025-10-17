#!/bin/bash

# BloggingApp Docker Setup Script
# This script helps you get started with the containerized blogging app

set -e

echo "=================================="
echo "BloggingApp Docker Setup"
echo "=================================="
echo ""

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed. Please install Docker first."
    exit 1
fi

# Check if Docker Compose is installed
if ! command -v docker compose &> /dev/null; then
    echo "❌ Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

echo "✅ Docker and Docker Compose are installed"
echo ""

# Check if .env file exists
if [ ! -f .env ]; then
    echo "⚠️  No .env file found. Creating from .env.example..."
    cp .env.example .env
    echo "✅ Created .env file"
    echo ""
    echo "⚠️  IMPORTANT: Please edit .env file with your configuration:"
    echo "   - Set DOMAIN_NAME to your actual domain"
    echo "   - Set OWNER_EMAIL and OWNER_PASSWORD for admin account"
    echo "   - Generate a secure JWT_KEY (run: openssl rand -base64 48)"
    echo "   - Update CORS_ALLOWED_ORIGINS with your actual domains"
    echo ""
    read -p "Press Enter after you've updated .env, or Ctrl+C to exit and edit later..."
else
    echo "✅ Found existing .env file"
fi

echo ""
echo "Building Docker images..."
docker compose build

echo ""
echo "Starting services..."
docker compose up -d

echo ""
echo "Waiting for services to be ready..."
sleep 10

# Check API health
echo "Checking API health..."
for i in {1..30}; do
    if curl -s http://localhost:8080/healthz > /dev/null 2>&1; then
        echo "✅ API is healthy!"
        break
    fi
    if [ $i -eq 30 ]; then
        echo "⚠️  API health check timeout. Check logs with: docker-compose logs api"
    else
        sleep 2
    fi
done

echo ""
echo "=================================="
echo "✅ BloggingApp is running!"
echo "=================================="
echo ""
echo "Services:"
echo "  - API:  http://localhost:8080"
echo "  - Web:  http://localhost:8081"
echo ""