# Whisp

Whisp is a real-time text chat application being built as a personal engineering project with enterprise-grade architecture and practices.

The project is designed to support:

- Direct messages between two users
- Group conversations
- Channel-based communication
- Real-time delivery, presence, and typing indicators

## Why This Project

Whisp is focused on learning and demonstrating strong backend and frontend engineering through:

- Clean Architecture
- CQRS
- Domain-driven design principles
- Real-time systems with WebSockets/SignalR
- Scalable infrastructure patterns

## Planned Stack

- Backend: .NET 10 (ASP.NET Core), SignalR, EF Core
- Data: PostgreSQL, Redis
- Messaging/async (when needed): RabbitMQ (MassTransit)
- Frontend: React + TypeScript
- Storage: MinIO (local) / AWS S3 (production)
- DevOps: Docker + Docker Compose

## Repository Structure

```text
src/
  backend/   # Backend services and APIs (in progress)
  frontend/  # React frontend

docs/
  prd.md     # Product Requirements Document
```

## Current Status

- Product requirements documented in `docs/prd.md`
- Frontend scaffold is available in `src/frontend`
- Backend implementation is in progress

## Getting Started

### Docker Compose (Dev)

Run frontend and backend together with Docker Compose:

```bash
docker compose --env-file .env.compose up -d --build
```

Stop the stack:

```bash
docker compose --env-file .env.compose down
```

Default URLs:

- Frontend: `http://localhost:3000`
- Backend: `http://localhost:8080/`

### Frontend

```bash
cd src/frontend
pnpm install
pnpm dev
```

Default local URL: `http://localhost:3000`

## Product Requirements

For full scope, requirements, milestones, and architecture decisions, see:

- `docs/prd.md`

## Contributing

Please read `CONTRIBUTING.md` before opening a pull request.

## License

This project is licensed under the MIT License. See `LICENSE`.
