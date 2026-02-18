<p align="center">
  <img src="assets/app-icon.png" alt="Whisp" width="120" />
</p>

<h1 align="center">Whisp</h1>

<p align="center">
  A real-time text chat application built with enterprise-grade architecture and modern technologies.
</p>

---

## About

Whisp is a personal engineering project focused on building a fully functional real-time messaging platform. It supports direct messages with real-time delivery, built on Clean Architecture, domain-driven design principles, and scalable infrastructure patterns.

## Features

**Implemented**

- Real-time direct messaging via SignalR WebSockets
- Conversation and message persistence (PostgreSQL)
- User search and discovery
- Responsive chat UI with conversation list and message history

**Planned**

- Authentication (JWT access + refresh tokens)
- Group conversations and channels
- Typing indicators and presence tracking
- File uploads (MinIO / S3)
- Message reactions and threads

## Tech Stack

| Layer        | Technology                                             |
| ------------ | ------------------------------------------------------ |
| Backend      | .NET 10 (ASP.NET Core), SignalR, EF Core               |
| Frontend     | React 19 + TypeScript, TanStack Router, TanStack Query |
| Styling      | Tailwind CSS v4                                        |
| Database     | PostgreSQL                                             |
| Real-time    | SignalR (WebSockets)                                   |
| Auth         | JWT (access + refresh tokens) — planned                |
| File Storage | MinIO (local) / AWS S3 (production) — planned          |
| DevOps       | Docker + Docker Compose, GitHub Actions                |

## Architecture

The backend follows Clean Architecture with four projects:

- **`Whisp.API`** — presentation layer: REST endpoints, SignalR hubs, middleware
- **`Whisp.Application`** — use-case contracts: repository interfaces, unit of work
- **`Whisp.Domain`** — core business logic: entities, value objects, domain exceptions
- **`Whisp.Infrastructure`** — data access: EF Core, PostgreSQL, repository implementations

The frontend is built with [TanStack Start](https://tanstack.com/start), a full-stack React framework, using file-based routing via TanStack Router and server state management via TanStack Query.

## Repository Structure

```text
src/
  backend/   # .NET API and backend services
  frontend/  # React + TypeScript frontend
docs/
  prd.md     # Product Requirements Document
```

## Getting Started

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/) and Docker Compose
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (for local backend development)
- [Node.js](https://nodejs.org/) and [pnpm](https://pnpm.io/) (for local frontend development)

### Docker Compose (Recommended)

All services (frontend, backend, PostgreSQL) start with a single command. No additional configuration is required — the compose file includes working defaults for every service.

```bash
docker compose --env-file .env.compose up -d --build
```

The `.env.compose` file lets you override the default ports if needed:

```dotenv
FRONTEND_PORT=3000
BACKEND_PORT=8080
```

Stop the stack:

```bash
docker compose --env-file .env.compose down
```

Default URLs:

| Service  | URL                             |
| -------- | ------------------------------- |
| Frontend | `http://localhost:3000`         |
| Backend  | `http://localhost:8080`         |
| API Docs | `http://localhost:8080/scalar/` |

### Backend

```bash
cd src/backend
dotnet run --project Whisp.API/Whisp.API.csproj
```

The API runs at `http://localhost:5265` by default. Interactive API documentation (Scalar) is available at `/scalar/` in the Development environment.

### Frontend

```bash
cd src/frontend
pnpm install
pnpm dev
```

The frontend runs at `http://localhost:3000` by default.

## API Documentation

The backend uses [Scalar](https://scalar.com/) for interactive API documentation, powered by the OpenAPI specification. When running in Development mode, navigate to `/scalar/` on the backend URL to explore endpoints, view request/response schemas, and test API calls.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).
