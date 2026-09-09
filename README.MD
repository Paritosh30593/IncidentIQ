# IncidentIQ

IncidentIQ is an incident management system that helps teams quickly report, track, manage, and resolve incidents efficiently.

> **Status:** early scaffolding — backend and frontend boilerplate are in place; local environment orchestration is not yet configured.

## Repository Structure

```plaintext
IncidentIQ/
├── Backend/            # .NET 10 API (Clean Architecture)
│   ├── App/
│   │   ├── IC.Domain/         # Entities, value objects, domain services
│   │   ├── IC.Application/    # Business logic orchestration
│   │   ├── IC.Infrastructure/ # Data access, external services (EF Core)
│   │   └── IC.WebAPI/         # Controllers, HTTP concerns (includes Dockerfile)
│   └── Tests/
│       ├── IC.Unit/           # Unit tests
│       └── IC.Integration/    # Integration tests
├── Frontend/            # React + TypeScript + Vite client (early scaffolding, includes Dockerfile)
└── docker-compose.yml   # Local environment orchestration (not yet configured)
```

## Backend

- **Stack:** .NET 10, ASP.NET Core Web API, Entity Framework Core
- **Architecture:** Clean Architecture with CQRS, Repository, and Service patterns — see [`Backend/.claude/guidelines/architecture.md`](Backend/.claude/guidelines/architecture.md) for details

### Getting Started

```bash
cd Backend
dotnet restore
dotnet build
dotnet run --project App/IC.WebAPI
```

### Running Tests

```bash
cd Backend
dotnet test
```

## Frontend

- **Stack:** React, TypeScript, Vite (React Compiler enabled)
- **Status:** early scaffolding — see [`Frontend/CLAUDE.md`](Frontend/CLAUDE.md) for what's implemented vs. placeholder

### Getting Started

```bash
cd Frontend
npm install
npm run dev
```

### Building

```bash
cd Frontend
npm run build
```

## License

TBD
