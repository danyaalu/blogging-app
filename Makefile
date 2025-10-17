.PHONY: help build up down restart logs logs-api logs-web clean rebuild backup-db shell-api shell-web health

help:
	@echo "BloggingApp Docker Commands"
	@echo "============================"
	@echo "make build        - Build Docker images"
	@echo "make up           - Start all services"
	@echo "make down         - Stop all services"
	@echo "make restart      - Restart all services"
	@echo "make logs         - View logs from all services"
	@echo "make logs-api     - View API logs"
	@echo "make logs-web     - View Web logs"
	@echo "make clean        - Stop and remove all containers, networks"
	@echo "make rebuild      - Clean rebuild (no cache)"
	@echo "make backup-db    - Backup SQLite database"
	@echo "make shell-api    - Access API container shell"
	@echo "make shell-web    - Access Web container shell"
	@echo "make health       - Check API health"

build:
	docker-compose build

up:
	docker-compose up -d

down:
	docker-compose down

restart:
	docker-compose restart

logs:
	docker-compose logs -f

logs-api:
	docker-compose logs -f api

logs-web:
	docker-compose logs -f web

clean:
	docker-compose down -v
	docker system prune -f

rebuild:
	docker-compose down
	docker-compose build --no-cache
	docker-compose up -d

backup-db:
	@mkdir -p backups
	docker cp blogging-api:/app/data/blogging.db ./backups/blogging-$(shell date +%Y%m%d-%H%M%S).db
	@echo "Database backed up to backups/ directory"

shell-api:
	docker exec -it blogging-api bash

shell-web:
	docker exec -it blogging-web bash

health:
	@echo "Checking API health..."
	@curl -f http://localhost:8080/healthz || echo "API is not responding"

