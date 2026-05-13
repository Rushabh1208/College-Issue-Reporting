# Project Context: College Issue Reporting System

This document provides a comprehensive technical and functional overview of the College Issue Reporting project. It is designed to serve as a complete context for AI assistants and developers to understand the architecture, data flow, and implementation details of the system.

---

## Project Overview
- **Project Name:** College Issue Reporting System
- **Purpose:** A centralized platform for students to report maintenance or facility issues within a college campus and for administrators/staff to track and resolve them.
- **Real-World Problem:** Traditional issue reporting in colleges (via physical registers or word-of-mouth) is often slow, lacks transparency, and is difficult to track.
- **Target Users:** 
    - **Students:** To report and track issues.
    - **Staff:** To view assigned tasks and update status.
    - **Admins:** To manage users, assign issues to staff, and oversee the entire system.
- **Core Functionality:** Issue reporting, role-based access control, issue assignment, status tracking, and automated database seeding.

---

## Tech Stack
- **Backend:** .NET 8.0 (ASP.NET Core Minimal APIs)
- **Database:** MySQL 8.0
- **ORM:** Entity Framework Core (EF Core) with Pomelo.EntityFrameworkCore.MySql
- **Caching:** Redis (StackExchange.Redis)
- **Authentication:** JWT (JSON Web Tokens)
- **Security:** SHA256 Hashing (Note: BCrypt is present in dependencies but SHA256 is currently used in `SecurityHelper.cs`)
- **Validation:** FluentValidation
- **Logging:** Serilog (Console and File sinks)
- **Documentation:** Swagger/OpenAPI
- **Containerization:** Docker & Docker Compose

---

## Project Structure
```text
/
├── Common/                 # Utility classes (SecurityHelper.cs)
├── DTOs/                   # Data Transfer Objects for API requests/responses
├── Enums/                  # System-wide enumerations (UserRole, IssueStatus)
├── Extensions/             # Extension methods for modular configuration
│   ├── DatabaseExtensions.cs # DB context and Redis setup, Seeding logic
│   ├── EndpointExtensions.cs # All API route definitions (Minimal APIs)
│   ├── SecurityExtensions.cs # JWT and Authorization policy configuration
│   ├── ServiceExtensions.cs  # CORS, Rate Limiting, and Validator registration
│   └── SwaggerExtensions.cs  # Swagger/OpenAPI configuration
├── Middleware/             # (Empty, but reserved for custom middleware)
├── Migrations/             # EF Core database migrations
├── Models/                 # Database entities (User.cs, Issue.cs)
├── Properties/             # launchSettings.json for development
├── Validators/             # FluentValidation rules
├── AppDbContext.cs         # EF Core Database Context
├── Program.cs              # Application entry point and middleware pipeline
├── Dockerfile              # Docker image configuration
├── docker-compose.yml      # Multi-container orchestration (API, DB, Redis)
└── backend.csproj          # Project dependencies and configuration
```

---

## Features

### 1. User Authentication & Authorization
- **Implementation:** JWT-based authentication.
- **Roles:** `Student`, `Staff`, `Admin`.
- **Logic:** Users register as Students by default. Login generates a JWT containing email and role claims.
- **Files:** `SecurityExtensions.cs`, `SecurityHelper.cs`, `EndpointExtensions.cs` (MapAuthEndpoints).

### 2. Issue Reporting (Students)
- **Implementation:** Students can submit issues with Title, Description, Block, and Room Number.
- **Validation:** `CreateIssueDtoValidator` ensures all fields are present and within length limits.
- **Files:** `Issue.cs`, `CreateIssueDto.cs`, `EndpointExtensions.cs` (MapIssueEndpoints).

### 3. Issue Management (Admin)
- **Filtering:** Admins can view all issues filtered by status.
- **Caching:** The issue list for admins is cached in Redis for 60 seconds.
- **Assignment:** Admins assign reported issues to specific Staff members. This changes the status from `Open` to `InProgress`.
- **Soft Delete:** Admins can delete issues (sets `IsDeleted = true`).
- **Files:** `EndpointExtensions.cs` (MapAdminEndpoints).

### 4. Issue Resolution (Staff)
- **Workflow:** Staff view issues assigned specifically to them.
- **Status Updates:** Staff can update the status (e.g., to `Resolved` or `Closed`).
- **Constraint:** Staff cannot reopen an issue once it is marked as `Resolved`.
- **Files:** `EndpointExtensions.cs` (MapStaffEndpoints).

### 5. Automated Seeding & Migrations
- **Logic:** On startup, the application automatically applies pending migrations and seeds a default Admin and two Staff members if they don't exist.
- **Files:** `Program.cs`, `DatabaseExtensions.cs`.

---

## Application Flow

1.  **Request Lifecycle:**
    - Request enters `Program.cs` pipeline.
    - Logging (Serilog) records the request.
    - Rate Limiting checks if the user exceeded thresholds.
    - Authentication middleware validates the JWT.
    - Authorization middleware checks role-based policies (`AdminOnly`, `StudentOnly`, etc.).
    - Endpoint handler processes the request using `AppDbContext`.
2.  **Data Flow:**
    - **Frontend -> API:** DTOs are used for input.
    - **API -> Database:** Models are mapped and saved via EF Core.
    - **Database -> API:** Data is projected into Response DTOs before being sent back.
3.  **Caching Flow:**
    - Admin issue list checks Redis first. If hit, returns cached JSON. If miss, queries MySQL, caches result, and returns.

---

## Database Design

### Tables
- **Users:**
    - `Id` (long, PK)
    - `Name`, `Email`, `PasswordHash`
    - `Role` (int mapped to `UserRole` enum)
- **Issues:**
    - `Id` (long, PK)
    - `Title`, `Description`, `Block`, `RoomNumber`
    - `Status` (string, converted from `IssueStatus` enum)
    - `UserId` (long, FK to Users - Reporter)
    - `AssignedToId` (long, FK to Users - Staff)
    - `ImagePath` (string, optional)
    - `CreatedAt` (DateTime)
    - `IsDeleted` (bool)

### Relationships
- `Issue (UserId)` -> `User (Id)` [One-to-Many]
- `Issue (AssignedToId)` -> `User (Id)` [One-to-Many]

---

## API Documentation

### Auth
- `POST /register`: Registers a new student.
- `POST /login`: Returns a JWT token.

### Admin (Requires `AdminOnly` policy)
- `GET /admin/users`: List all users.
- `GET /admin/issues`: List all issues (supports `status` query and pagination). Cached in Redis.
- `PUT /admin/issues/{id}/assign/{staffId}`: Assign an issue to staff.
- `DELETE /admin/issues/{id}`: Soft delete an issue.

### Issues/Student
- `POST /issues/report`: Submit a new issue (Requires `StudentOnly`).
- `GET /student/issues`: Get issues reported by the current student.

### Staff
- `GET /staff/issues`: Get issues assigned to the current staff.
- `PUT /staff/issues/{id}/status`: Update issue status.

---

## State Management
- **Server State:** Handled by EF Core and MySQL.
- **Cache State:** Redis stores temporary JSON snapshots of admin queries.
- **Auth State:** Stateless JWT tokens stored on the client side (e.g., LocalStorage in a frontend).

---

## UI/UX Structure (Inferred)
*Note: This is a backend-focused project, but the endpoints suggest the following UI structure:*
- **Login/Register Pages**
- **Student Dashboard:** Form for reporting issues and a list of "My Reported Issues".
- **Admin Dashboard:** Table of all issues with "Assign" and "Delete" buttons, and a User Management view.
- **Staff Dashboard:** List of "Tasks Assigned to Me" with status update dropdowns.

---

## Important Business Logic
- **Status Progression:** `Open` -> `InProgress` (on assignment) -> `Resolved` -> `Closed`.
- **Validation Logic:** FluentValidation checks for string lengths and mandatory fields.
- **Security Logic:** `SecurityHelper.GetUserId` extracts the user ID from the `ClaimsPrincipal` by matching the email from the token to the database record.
- **Resilient Startup:** `Program.cs` contains a retry loop (5 attempts) for database migrations to handle cases where the DB container isn't ready yet.

---

## Environment & Setup

### Requirements
- .NET 8 SDK
- Docker Desktop

### Commands
1.  **Run with Docker (Recommended):**
    ```bash
    docker-compose up --build
    ```
2.  **Run Locally (Requires local MySQL and Redis):**
    - Update `appsettings.json` connection strings.
    - `dotnet run`

### Environment Variables
- `ConnectionStrings__DefaultConnection`: MySQL connection string.
- `Redis__ConnectionString`: Redis connection string.
- `Jwt__Key`: Secret key for token generation.

---

## Security Considerations
- **Current Practices:** JWT authentication, Role-based authorization, Rate limiting, SHA256 password hashing.
- **Vulnerabilities/Weaknesses:**
    - **Password Hashing:** `SecurityHelper.cs` uses SHA256 without a salt. It should be replaced with **BCrypt** (which is already in the `.csproj`).
    - **Hardcoded Secret:** A default JWT key is hardcoded in `SecurityHelper.cs` as a fallback.
    - **CORS:** Currently set to `AllowAll` (`*`), which should be restricted in production.
    - **Exception Handling:** Custom middleware returns a generic "Internal Server Error" but logs detailed errors to Serilog.

---

## Code Quality Analysis
- **Organization:** Excellent. Use of Extension methods for configuration keeps `Program.cs` clean.
- **Consistency:** High. Naming conventions are standard C#.
- **Maintainability:** Minimal API approach is lightweight. Use of DTOs prevents over-posting and leakage of domain models.
- **Duplication:** Very low. Common logic is abstracted into helpers/extensions.

---

## Missing Features
- **Real-time Notifications:** SignalR integration to notify staff when an issue is assigned or students when resolved.
- **File/Image Uploads:** Implementation for handling actual image files (currently only a string path exists in the model).
- **Comment System:** Allowing students and staff to communicate within an issue thread.
- **Email Notifications:** Sending emails for critical status changes.

---

## Improvement Suggestions
1.  **Security:** Switch to `BCrypt.Net` for password hashing.
2.  **Architecture:** Consider a Repository pattern if the logic grows more complex, though Minimal APIs are sufficient for now.
3.  **UI:** Build a React or Vue.js frontend using the provided DTOs.
4.  **Logging:** Integrate a logging provider like Seq or Azure Application Insights for better log visualization.

---

## Known Issues
- **Redis Dependency:** The application will fail to start if Redis is not reachable, as `AddRedisCache` connects immediately.
- **Seeding Overlap:** If the email of a seeded user is changed manually in the DB, the seeder might create duplicates (it only checks for Role existence).

---

## Future Roadmap
1.  Implement Image Upload to Azure Blob Storage or local storage.
2.  Add a Dashboard with charts (e.g., "Issues by Block", "Average Resolution Time").
3.  Implement a Password Reset flow.
4.  Add Unit and Integration tests using xUnit and Testcontainers.

---

## AI Context Summary
This is a .NET 8 Minimal API backend for a "College Issue Reporting System". It uses MySQL for persistence and Redis for caching the admin issue list. Authentication is JWT-based with roles: Student, Staff, and Admin. Key entities are `User` and `Issue`. Students report issues tied to a campus `Block` and `RoomNumber`. Admins assign these to `Staff`. The project follows a clean, extension-based configuration pattern. Password hashing currently uses SHA256 (needs update to BCrypt). Docker Compose orchestrates the API, DB, and Redis.
