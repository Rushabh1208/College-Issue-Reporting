# College Issue Reporting System — Campus API

A centralized backend system for managing facility issues and maintenance requests within a college campus. This project provides a role-based workflow for students to report issues, staff to resolve them, and administrators to oversee operations.

---

## 🚀 Live Links
- **API Base URL:** [https://college-issue-reporting.onrender.com](https://college-issue-reporting.onrender.com)
- **Interactive API Documentation (Swagger):** [https://college-issue-reporting.onrender.com/swagger/index.html](https://college-issue-reporting.onrender.com/swagger/index.html)

---

## Verified Overview

The system is implemented as a **.NET 8 Minimal API** that manages the lifecycle of campus issues. It supports three distinct user roles (Student, Staff, Admin) and integrates with a cloud-native infrastructure for reliability and performance.

- **Business Purpose:** Digitizing the reporting and tracking of campus maintenance tasks.
- **Problem Solved:** Replaces manual registers and untracked verbal complaints with a transparent, searchable, and accountable digital workflow.

---

## Verified Features

### 1. User Authentication & RBAC
- **Implementation:** JWT-based authentication with custom authorization policies.
- **Security:** Passwords are securely hashed using **BCrypt**.
- **Roles:** `Student`, `Staff`, `Admin`.

### 2. Issue Reporting with Image Uploads
- **Implementation:** Students can submit maintenance requests with an optional **Image Upload**.
- **Storage:** Images are stored on the local filesystem with deterministic naming (`{id}.jpeg`).
- **Evidence:** `Features/Issues/IssueEndpoints.cs:84-110`.

### 3. Issue Management (Admin)
- **Implementation:** Admins can view all issues, filter by status, assign issues to staff, and soft-delete entries.
- **Evidence:** `Features/Issues/IssueEndpoints.cs:15-82`.

### 4. Task Resolution (Staff)
- **Implementation:** Staff can view issues assigned to them and update their resolution status.
- **Evidence:** `Features/Issues/IssueEndpoints.cs:115-150`.

### 5. Automated System Seeding
- **Implementation:** Automatically creates an Admin and default Staff users if they do not exist on startup.

---

## Tech Stack & Infrastructure

- **Framework:** .NET 8.0 (Minimal APIs)
- **Hosting:** [Render](https://render.com) (Native .NET Web Service)
- **Database:** [AWS RDS](https://aws.amazon.com/rds/) (MySQL 8.0)
- **Caching:** [Upstash](https://upstash.com/) (Serverless Redis)
- **Auth:** JWT (System.IdentityModel.Tokens.Jwt)
- **Validation:** FluentValidation
- **Logging:** Serilog (Console & Rolling File)

---

## Project Structure

```text
/
├── Features/                # Vertical Slices (Auth, Issues, Users)
│   ├── Auth/                # Login/Register endpoints and DTOs
│   ├── Issues/              # Core business logic, Image Uploads, and DTOs
│   └── Users/               # User listing and management
├── Infrastructure/          # Cross-cutting concerns
│   ├── Extensions/          # DI and Configuration (DB, Auth, Middleware)
│   ├── Services/            # Abstractions (IStorageService for local/cloud storage)
│   ├── Middleware/          # Global Exception handling
│   ├── SecurityHelper.cs    # Token logic and BCrypt Hashing
│   └── AppDbContext.cs      # EF Core context
├── Models/                  # Shared Domain entities
├── Enums/                   # Role and Status constants
├── Migrations/              # EF Core database migrations
└── uploads/                 # Local directory for stored issue images
```

---

## Setup Instructions

### Environment Variables
The following variables are required in the **Production (Render)** environment:
- `ConnectionStrings__DefaultConnection`: AWS RDS MySQL connection string.
- `Jwt__Key`: Secure secret key for JWT signing.
- `Redis__ConnectionString`: Upstash Redis connection address.
- `PORT`: Automatically handled by Render (defaults to 10000).

### Local Development
```bash
docker-compose up --build
```
*Uses local MySQL and Redis containers as defined in `docker-compose.yml`.*

---

## API Documentation

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| POST | `/login` | Authenticate and get JWT |
| POST | `/register` | Create a new Student account |
| GET | `/admin/issues` | List all issues (Cached) |
| PUT | `/admin/issues/{id}/assign/{sid}` | Assign issue to staff |
| POST | `/issues/report` | Submit an issue + Image (Multipart) |
| PUT | `/staff/issues/{id}/status` | Update issue status |

---

## Database Architecture

### Entities
1. **User**: `Id`, `Name`, `Email`, `PasswordHash`, `Role`.
2. **Issue**: `Id`, `Title`, `Description`, `Status`, `UserId`, `AssignedToId`, `Block`, `RoomNumber`, `ImagePath`.

---

## Future Improvements

- **Cloud Storage Migration**: Transition `IStorageService` to AWS S3 or Azure Blob Storage for persistent image handling.
- **CI/CD**: Fully automate testing and deployment using GitHub Actions.
- **CORS Hardening**: Restrict `AllowAll` origins to a specific frontend whitelist.
- **Global Error Handling**: Implement more granular problem-details responses.
