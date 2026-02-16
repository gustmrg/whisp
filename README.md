<p align="center">
  <img src="assets/app-icon.png" alt="Whisp" width="120" />
</p>

<h1 align="center">Whisp</h1>

<p align="center">
  A real-time text chat application built with enterprise-grade architecture and modern technologies.
</p>

---

## About

Whisp is a personal engineering project focused on building a fully functional real-time messaging platform. It supports direct messages, group conversations, and channel-based communication with real-time delivery, presence tracking, and typing indicators.

The project emphasizes learning and demonstrating strong backend and frontend engineering through Clean Architecture, CQRS, domain-driven design principles, and scalable infrastructure patterns.

## Tech Stack

| Layer        | Technology                                  |
| ------------ | ------------------------------------------- |
| Backend      | .NET 10 (ASP.NET Core), SignalR, EF Core    |
| Frontend     | React + TypeScript                          |
| Database     | PostgreSQL                                  |
| Cache        | Redis                                       |
| Auth         | JWT (access + refresh tokens)               |
| File Storage | MinIO (local) / AWS S3 (production)         |
| DevOps       | Docker + Docker Compose                     |

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

Run the full stack with Docker Compose:

```bash
docker compose --env-file .env.compose up -d --build
```

Stop the stack:

```bash
docker compose --env-file .env.compose down
```

Default URLs:

| Service  | URL                            |
| -------- | ------------------------------ |
| Frontend | `http://localhost:3000`        |
| Backend  | `http://localhost:8080`        |
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

The backend uses [Scalar](https://scalar.com/) for interactive API documentation, powered by the OpenAPI specification. When running in Development mode, navigate to `/scalar/` on the backend URL to explore endpoints, view request/response schemas, and test API calls with JWT Bearer authentication.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).
