# AssignmentManagement API

A professional **ASP.NET Core 10** Web API following **Clean Architecture** principles for managing assignments, users, and submissions.

---

## 🏗️ Architecture

```
AssignmentManagement.Api/
├── Controllers/              # API endpoints (HTTP layer)
├── Middleware/               # Cross-cutting pipeline concerns
├── Domain/
│   ├── Entities/             # Core business entities
│   ├── Enums/                # Domain enumerations
│   └── Interfaces/           # Repository & UoW contracts
├── Application/
│   ├── DTOs/                 # Data Transfer Objects
│   ├── Services/             # Business logic services
│   ├── Interfaces/           # Service contracts
│   ├── Validators/           # FluentValidation validators
│   └── Mapping/              # AutoMapper profiles
├── Infrastructure/
│   ├── Persistence/          # EF Core DbContext
│   ├── Repositories/         # Repository implementations
│   ├── Authentication/       # JWT settings & helpers
│   └── Configurations/       # EF entity type configurations
├── Shared/                   # Shared models (ApiResponse, PagedResponse)
└── Tests/                    # Unit & Integration tests
```

---

## 🛠️ Tech Stack

| Technology | Purpose |
|---|---|
| ASP.NET Core 10 | Web framework |
| Entity Framework Core 10 | ORM |
| PostgreSQL | Database |
| JWT Bearer | Authentication |
| AutoMapper | Object mapping |
| FluentValidation | Input validation |
| Swashbuckle | Swagger / OpenAPI docs |
| Serilog | Structured logging |
| xUnit + Moq | Testing |

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 16+](https://www.postgresql.org/download/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) *(optional)*

### 1. Clone & Configure

```bash
git clone <repo-url>
cd backend/AssignmentManagement.Api

# Copy and edit environment variables
cp .env.example .env
# Edit .env with your database credentials and JWT secret
```

### 2. Run with Docker (Recommended)

```bash
cd backend
docker-compose up --build
```

API: http://localhost:5000  
Swagger UI: http://localhost:5000 (Development only)

### 3. Run Locally

```bash
cd backend/AssignmentManagement.Api

# Install dependencies
dotnet restore

# Apply database migrations
dotnet ef database update

# Run the API
dotnet run
```

---

## 📋 Environment Variables

| Variable | Description | Default |
|---|---|---|
| `DB_HOST` | PostgreSQL host | `localhost` |
| `DB_PORT` | PostgreSQL port | `5432` |
| `DB_NAME` | Database name | `assignment_management` |
| `DB_USER` | Database user | `postgres` |
| `DB_PASSWORD` | Database password | — |
| `JWT_SECRET_KEY` | JWT signing key (min 32 chars) | — |
| `JWT_ISSUER` | JWT token issuer | `AssignmentManagement.Api` |
| `JWT_AUDIENCE` | JWT token audience | `AssignmentManagement.Client` |

---

## 🔗 Endpoints

| Method | Route | Description |
|---|---|---|
| `GET` | `/` | Swagger UI (Dev only) |
| `GET` | `/health` | Health check (HealthChecks UI format) |
| `GET` | `/api/health` | Health check (JSON API format) |

> Business logic endpoints will be added in subsequent steps.

---

## 🗄️ Database Migrations

```bash
# Add a new migration
dotnet ef migrations add <MigrationName>

# Apply migrations
dotnet ef database update

# Revert last migration
dotnet ef migrations remove
```

---

## 📝 Logging

Logs are written to:
- **Console** — structured output during development
- **`logs/log-{date}.txt`** — rolling daily files (14-day retention)

Log level is controlled via `appsettings.json` → `Serilog.MinimumLevel`.

---

## 🧪 Testing

```bash
cd backend/AssignmentManagement.Api/Tests
dotnet test
```

---

## 📄 License

MIT License — see [LICENSE](LICENSE) for details.
