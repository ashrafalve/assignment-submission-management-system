# AssignmentManagement API

A professional **ASP.NET Core 10** Web API following **Clean Architecture** principles for managing assignments, users, classes, subjects, and submissions.

---

## 🔑 Demo Credentials

The database is automatically seeded on startup in Development mode. You can log in via `/api/auth/login` using the following credentials:

| Role | Full Name | Email | Password | Class Enrollment / Assignment |
|---|---|---|---|---|
| 👑 **Admin** | System Admin | `admin@assignmentmanagement.com` | `Admin@1234` | System Administrator |
| 👨‍🏫 **Teacher** | John Teacher | `john.teacher@assignmentmanagement.com` | `Teacher@1234` | Mathematics & CS (Grade 10 - Sec A) |
| 👩‍🏫 **Teacher** | Sarah Teacher | `sarah.teacher@assignmentmanagement.com` | `Teacher@1234` | Physics & Chem (Grade 10 - Sec A) |
| 🎓 **Student** | Alex Student | `alex.student@assignmentmanagement.com` | `Student@1234` | Enrolled in **Grade 10 - Section A** |
| 🎓 **Student** | Emma Student | `emma.student@assignmentmanagement.com` | `Student@1234` | Enrolled in **Grade 10 - Section A** |
| 🎓 **Student** | Liam Student | `liam.student@assignmentmanagement.com` | `Student@1234` | Enrolled in **Grade 10 - Section B** |

---

## 🏗️ Architecture

```
AssignmentManagement.Api/
├── Controllers/              # API endpoints (Admin, Auth, Teacher, Student)
├── Middleware/               # Global exception handling & security pipeline
├── Domain/
│   ├── Entities/             # User, SchoolClass, Subject, TeacherSubject, Assignment, Submission
│   ├── Enums/                # UserRole, AssignmentStatus, SubmissionStatus
│   ├── Exceptions/           # NotFoundException, ForbiddenException, BusinessRuleException
│   └── Interfaces/           # Repositories & UnitOfWork contracts
├── Application/
│   ├── DTOs/                 # Admin, Auth, Teacher, Student DTOs
│   ├── Services/             # AdminUserService, AuthService, ClassService, SubjectService, TeacherAssignmentService, StudentAssignmentService, TeacherSubmissionService
│   ├── Interfaces/           # Service contracts
│   ├── Validators/           # FluentValidation validators
│   └── Mapping/              # AutoMapper profiles
├── Infrastructure/
│   ├── Persistence/          # ApplicationDbContext, DbSeeder, Migrations
│   ├── Repositories/         # EF Core generic & entity-specific repositories
│   ├── Authentication/       # JwtService & JwtSettings
│   └── Configurations/       # Fluent API entity configurations
├── Shared/                   # ApiResponse<T>, PagedResponse<T>, PaginationParams
└── Tests/                    # xUnit + Moq unit test suite
```

---

## 🛠️ Tech Stack

| Technology | Purpose |
|---|---|
| ASP.NET Core 10 | Web API framework |
| Entity Framework Core 10 | ORM & PostgreSQL Data Access |
| PostgreSQL | Relational Database |
| JWT Bearer | Token-based Authentication |
| AutoMapper | DTO Mapping |
| FluentValidation | Request Validation |
| Swashbuckle / Swagger UI | OpenAPI Interactive Docs |
| Serilog | Structured Logging |
| xUnit + Moq + FluentAssertions | Unit Testing |

---

## 🚀 Getting Started

### 1. Run with Docker Compose

```bash
cd backend
docker-compose up --build
```

- API: `http://localhost:5000`
- Swagger UI: `http://localhost:5000`

### 2. Run Locally

```bash
cd backend/AssignmentManagement.Api
dotnet restore
dotnet ef database update
dotnet run
```

---

## 🧪 Unit Tests

Run the xUnit test suite (14 passing tests):

```bash
cd backend/AssignmentManagement.Tests
dotnet test
```

---

## 📄 License

MIT License
