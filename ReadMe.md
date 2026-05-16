# College Issue Reporting System — Campus API

A centralized backend system for managing facility issues and maintenance requests within a college campus. This project provides a role-based workflow for students to report issues, staff to resolve them, and administrators to oversee operations.

---

## Verified Overview

The system is implemented as a **.NET 8 Minimal API** that manages the lifecycle of campus issues. It supports three distinct user roles (Student, Staff, Admin) and integrates with MySQL for persistent storage and Redis for high-performance caching of administrative views.

- **Business Purpose:** Digitizing the reporting and tracking of campus maintenance tasks.
- **Problem Solved:** Replaces manual registers and untracked verbal complaints with a transparent, searchable, and accountable digital workflow.

---

## Verified Features

### 1. User Authentication & RBAC
- **Implementation:** JWT-based authentication with custom authorization policies.
- **Roles:** `Student`, `Staff`, `Admin`.
- **Evidence:** `Infrastructure/Extensions/SecurityExtensions.cs:29-34`, `Features/Auth/AuthEndpoints.cs`.

### 2. Issue Reporting (Student)
- **Implementation:** Students can submit maintenance requests tied to specific blocks and rooms.
- **Evidence:** `Features/Issues/IssueEndpoints.cs:84-107`, `Models/Issue.cs`.

### 3. Issue Management (Admin)
- **Implementation:** Admins can view all issues, filter by status, assign issues to staff, and soft-delete entries.
- **Evidence:** `Features/Issues/IssueEndpoints.cs:15-82`.

### 4. Task Resolution (Staff)
- **Implementation:** Staff can view issues assigned to them and update their resolution status.
- **Evidence:** `Features/Issues/IssueEndpoints.cs:109-142`.

### 5. Automated System Seeding
- **Implementation:** Automatically creates an Admin and default Staff users if they do not exist on startup.
- **Evidence:** `Infrastructure/Extensions/DatabaseExtensions.cs:29-65`.

---

## Tech Stack

- **Framework:** .NET 8.0 (Minimal APIs) — *Verified in backend.csproj*
- **Database:** MySQL 8.0 (Pomelo.EntityFrameworkCore.MySql) — *Verified in backend.csproj & docker-compose.yml*
- **Caching:** Redis (StackExchange.Redis) — *Verified in backend.csproj & docker-compose.yml*
- **Auth:** JWT (System.IdentityModel.Tokens.Jwt) — *Verified in backend.csproj*
- **Validation:** FluentValidation — *Verified in backend.csproj*
- **Logging:** Serilog — *Verified in backend.csproj & Program.cs*

---

## Project Structure

```text
/
├── Features/                # Vertical Slices (Auth, Issues, Users)
│   ├── Auth/                # Login/Register endpoints and DTOs
│   ├── Issues/              # Core business logic and DTOs
│   └── Users/               # User listing and management
├── Infrastructure/          # Cross-cutting concerns
│   ├── Extensions/          # DI and Configuration (DB, Auth, Middleware)
│   ├── Middleware/          # Logging and Exception handling
│   ├── SecurityHelper.cs    # Token logic and Hashing
│   └── AppDbContext.cs      # EF Core context
├── Models/                  # Shared Domain entities
├── Enums/                   # Role and Status constants
└── Migrations/              # EF Core database migrations
```

---

## Setup Instructions

### Environment Variables
The following variables are required and can be set in `appsettings.json` or environment variables:
- `ConnectionStrings__DefaultConnection`: MySQL connection string.
- `Jwt__Key`: Secret key for JWT signing.
- `Redis__ConnectionString`: Redis server address.

### Run with Docker (Verified)
```bash
docker-compose up --build
```
*Based on docker-compose.yml configuration.*

---

## API Documentation

| Method | Endpoint | Description | Evidence |
| :--- | :--- | :--- | :--- |
| POST | `/login` | Authenticate and get JWT | `AuthEndpoints.cs:12` |
| POST | `/register` | Create a new Student account | `AuthEndpoints.cs:22` |
| GET | `/admin/issues` | List all issues (Cached) | `IssueEndpoints.cs:17` |
| PUT | `/admin/issues/{id}/assign/{sid}` | Assign issue to staff | `IssueEndpoints.cs:57` |
| POST | `/issues/report` | Submit an issue (Student) | `IssueEndpoints.cs:84` |
| PUT | `/staff/issues/{id}/status` | Update issue status | `IssueEndpoints.cs:123` |

---

## Database Architecture

### Entities
1. **User**: `Id`, `Name`, `Email`, `PasswordHash`, `Role`.
2. **Issue**: `Id`, `Title`, `Description`, `Status`, `UserId`, `AssignedToId`, `Block`, `RoomNumber`.
- **Evidence:** `Models/User.cs`, `Models/Issue.cs`.

---

## Limitations & Missing Areas

- **Security Weakness:** Password hashing uses SHA256 without salt (`Infrastructure/SecurityHelper.cs:33`).
- **Testing:** No unit or integration tests were found in the codebase.
- **CI/CD:** GitHub workflows folder exists but contains no active configurations.
- **File Handling:** The `ImagePath` field exists in the model, but no endpoint implements actual file upload logic.
- **Insecure CORS:** Currently allows all origins (`Infrastructure/Extensions/ServiceExtensions.cs:15`).
