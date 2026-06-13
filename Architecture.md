# CampusCare — Project Architecture Documentation

---

## Repository Structure

```
/
├── frontend/                              → React + Vite frontend application
└── backend/                              → .NET 8 Minimal API backend
```

---

## Frontend

```
frontend/
├── index.html                            → HTML entry point, mounts React root
├── vite.config.js                        → Vite build config with React plugin
├── tailwind.config.js                    → Tailwind theme with brand colors and shadows
├── postcss.config.js                     → PostCSS config for Tailwind and Autoprefixer
├── package.json                          → Project dependencies and npm scripts
├── .env.example                          → Example env file for API base URL
└── src/
    ├── main.jsx                          → App bootstrap, mounts RouterProvider and AppProviders
    ├── styles/
    │   └── index.css                     → Global Tailwind base styles and CSS variables
    ├── app/
    │   ├── layouts/
    │   │   ├── AppLayout.jsx             → Authenticated shell with sidebar and mobile nav
    │   │   └── AuthLayout.jsx            → Centered layout for login/register pages
    │   ├── providers/
    │   │   └── AppProviders.jsx          → Root provider wrapping ToastHost
    │   ├── router/
    │   │   ├── router.jsx                → React Router config with lazy-loaded pages
    │   │   └── guards.jsx                → Route guards: RequireAuth, PublicOnly, RoleRedirect
    │   └── store/
    │       └── uiStore.js                → Zustand store for toast notifications
    ├── features/
    │   ├── auth/
    │   │   ├── api/
    │   │   │   └── authApi.js            → POST /login API call
    │   │   ├── pages/
    │   │   │   ├── LoginPage.jsx         → Login form with Student ID or email support
    │   │   │   └── RegisterPage.jsx      → Student registration form (unused in current flow)
    │   │   ├── schemas/
    │   │   │   └── authSchemas.js        → React Hook Form validation rules for login
    │   │   ├── store/
    │   │   │   └── authStore.js          → Zustand store for JWT token and user session
    │   │   └── utils.js                  → JWT decode, expiry check, role-to-home mapping
    │   ├── admin/
    │   │   ├── api/
    │   │   │   └── adminIssueApi.js      → Admin issue API: list, assign, delete
    │   │   ├── components/
    │   │   │   └── AdminIssueActions.jsx → Staff assignment dropdown and delete button
    │   │   ├── hooks/
    │   │   │   └── useAdminIssues.js     → Async hook fetching paginated admin issues
    │   │   └── pages/
    │   │       ├── AdminIssuesPage.jsx   → Admin issue list with filter, assign, delete
    │   │       ├── AdminStaffPage.jsx    → Staff management: create, edit, deactivate
    │   │       └── AdminStudentsPage.jsx → Student roster with CSV import and actions
    │   ├── issues/
    │   │   ├── api/
    │   │   │   ├── issueMappers.js       → Normalize issue response (camelCase/PascalCase)
    │   │   │   └── issuesApi.js          → GET /issues/{id}/timeline API call
    │   │   ├── components/
    │   │   │   ├── IssueCard.jsx         → Issue display card with timeline and upvote toggle
    │   │   │   └── IssueStats.jsx        → Open/InProgress/Resolved count summary cards
    │   │   └── schemas/
    │   │       └── issueSchemas.js       → React Hook Form rules for issue report form
    │   ├── student/
    │   │   ├── api/
    │   │   │   └── studentIssueApi.js    → Student API: get issues, report, categories, upvote
    │   │   ├── components/
    │   │   │   └── ImageUploadField.jsx  → File input with image preview and clear button
    │   │   ├── hooks/
    │   │   │   └── useStudentIssues.js   → Async hook fetching student's own issues
    │   │   └── pages/
    │   │       ├── StudentIssuesPage.jsx → Student issue tracker with upvote and timeline
    │   │       └── ReportIssuePage.jsx   → Issue report form with duplicate detection
    │   ├── staff/
    │   │   ├── api/
    │   │   │   └── staffIssueApi.js      → Staff API: get assigned issues, update status
    │   │   ├── hooks/
    │   │   │   └── useStaffIssues.js     → Async hook fetching staff's assigned issues
    │   │   └── pages/
    │   │       └── StaffIssuesPage.jsx   → Staff issue queue with status update buttons
    │   ├── students/
    │   │   └── api/
    │   │       └── studentApi.js         → Admin student API: import CSV, list, deactivate, reset
    │   ├── users/
    │   │   ├── api/
    │   │   │   ├── usersApi.js           → GET /admin/users paginated list
    │   │   │   ├── userMappers.js        → Normalize user response fields
    │   │   │   └── staffApi.js           → Admin staff CRUD and password reset API calls
    │   │   ├── components/
    │   │   │   └── UserCard.jsx          → User card with role badge display
    │   │   ├── hooks/
    │   │   │   └── useUsers.js           → Async hook fetching paginated users
    │   │   └── pages/
    │   │       └── AdminUsersPage.jsx    → Admin user list with pagination
    │   └── womencell/
    │       ├── api/
    │       │   └── womencellApi.js       → Women Cell API: get issues, update status
    │       ├── hooks/
    │       │   └── useWomenCellIssues.js → Async hook fetching Women Welfare issues
    │       └── pages/
    │           └── WomenCellIssuesPage.jsx → Women Cell issue queue with status actions
    └── shared/
        ├── components/
        │   ├── IssueTimeline.jsx         → Timeline event list with action icons
        │   └── ToastHost.jsx             → Fixed toast notification renderer
        ├── constants/
        │   └── api.js                    → API base URL, ROLES, ISSUE_STATUSES constants
        ├── hooks/
        │   └── useAsync.js               → Generic async data-fetching hook with refetch
        ├── lib/
        │   └── apiClient.js              → Axios instance with JWT interceptor and error handling
        └── ui/
            ├── Button.jsx                → Styled button with loading spinner and variants
            ├── Card.jsx                  → Bordered white card container
            ├── EmptyState.jsx            → Empty list placeholder with icon and action
            ├── ErrorState.jsx            → Error display card with retry button
            ├── FormField.jsx             → Label + error wrapper for form inputs
            ├── Pagination.jsx            → Prev/Next pagination control
            ├── PriorityBadge.jsx         → Colored badge for issue priority level
            ├── Skeleton.jsx              → Animated loading skeleton rows
            └── StatusBadge.jsx           → Colored badge with icon for issue status
        └── utils/
            ├── cn.js                     → Classname concatenation utility
            └── formatters.js            → formatDateTime and compactFileSize helpers
```

---

## Frontend Architecture Summary

**Entry Point & Routing**

- `main.jsx` mounts the app with `RouterProvider` and `AppProviders`
- `router.jsx` uses React Router v6 with lazy-loaded pages and nested route guards
- Guards enforce role-based access: `RequireAuth`, `PublicOnly`, `RoleRedirect`

**Layout Structure**

- `AuthLayout` — centered card layout for public pages (login)
- `AppLayout` — persistent sidebar (desktop) and bottom nav (mobile) with role-based nav items
- Outlet pattern renders feature pages inside layouts

**State Management**

- `authStore.js` (Zustand) — JWT token, decoded user, login/logout, session validation
- `uiStore.js` (Zustand) — toast queue with auto-dismiss timers
- Local `useState` used within components for UI-only state (modals, busy IDs)

**Authentication Flow**

- `POST /login` returns a JWT; stored in `localStorage` via `authStore`
- JWT decoded client-side with `decodeJwt()` to extract role, email, expiry
- `ensureFreshSession()` checks expiry on every protected route
- Axios interceptor attaches `Authorization: Bearer <token>` to all requests; auto-logout on 401

**API Communication**

- Single `apiClient.js` Axios instance with base URL from env
- Feature-specific API modules (`studentIssueApi.js`, `adminIssueApi.js`, etc.) call endpoints
- Responses normalized via mapper utilities to handle PascalCase/camelCase inconsistencies
- `useAsync` hook wraps all API calls with loading/error/data/refetch state

**Component Organization**

- Features are self-contained under `src/features/` (auth, admin, student, staff, womencell)
- Each feature has `api/`, `pages/`, `hooks/`, `components/` sub-folders as needed
- `src/shared/` holds cross-feature UI primitives, hooks, utilities, and constants

---

## Backend

```
backend/
├── Program.cs                                    → App startup: DI registration, migrations, middleware, endpoint mapping
├── backend.csproj                                → .NET project file with NuGet package references
├── appsettings.json                              → Configuration placeholders for DB, JWT, Redis, AWS
├── appsettings.Development.json                  → Development log level overrides
├── Dockerfile                                    → Multi-stage Docker build for production
├── docker-compose.yml                            → Local dev stack: MySQL, Redis, API containers
├── postman_collection.json                       → Postman collection for all API endpoints
├── API_DOCUMENTATION.md                          → Full API reference with request/response examples
├── ReadMe.md                                     → Project overview and setup instructions
├── backend-folder-structure.txt                  → Legacy architecture overview document
├── Properties/
│   └── launchSettings.json                       → Local dev launch profiles for HTTP/HTTPS/Docker
├── Enums/
│   ├── Gender.cs                                 → Gender enum: Male, Female, Other
│   ├── IssuePriority.cs                          → Priority enum: Low, Medium, High, Critical
│   ├── IssueStatus.cs                            → Status enum: Open, InProgress, Resolved, Closed
│   ├── ParentCategory.cs                         → Issue parent category enum
│   ├── TimelineAction.cs                         → Timeline event type enum
│   └── UserRole.cs                               → Role enum: Student, Admin, Staff, WomenCell
├── Models/
│   ├── User.cs                                   → Staff/Admin/WomenCell user entity
│   ├── Student.cs                                → Student entity with StudentId and Gender
│   ├── Issue.cs                                  → Issue entity with image, category, upvote fields
│   ├── IssueCategory.cs                          → Issue category entity with WomenWelfare flag
│   ├── IssueTimeline.cs                          → Timeline event entity linked to Issue
│   └── IssueUpvote.cs                            → Upvote join entity linking Student to Issue
├── Features/
│   ├── Auth/
│   │   ├── AuthEndpoints.cs                      → POST /login — authenticates Students and Users
│   │   └── LoginDto.cs                           → Login request DTO accepting StudentId or email
│   ├── Categories/
│   │   ├── CategoryEndpoints.cs                  → GET /categories — returns grouped categories, filters for male students
│   │   ├── CategoryItemDto.cs                    → Single category item DTO
│   │   └── CategoryResponseDto.cs                → Grouped category response DTO
│   ├── Issues/
│   │   ├── IssueEndpoints.cs                     → All issue routes: student, staff, admin, women cell
│   │   ├── CreateIssueDto.cs                     → Issue creation request DTO
│   │   ├── CreateIssueValidator.cs               → FluentValidation rules for issue creation
│   │   ├── IssueResponseDto.cs                   → Issue response DTO with image and category fields
│   │   ├── AdminIssueResponseDto.cs              → Extends IssueResponseDto with reporter info
│   │   ├── TimelineResponseDto.cs                → Timeline event response DTO
│   │   ├── UpdateStatusDto.cs                    → Status update request DTO
│   │   ├── UpdateStatusValidator.cs              → FluentValidation rules for status update
│   │   └── UpdateIssuePriorityDto.cs             → Priority update request DTO
│   ├── Students/
│   │   ├── StudentEndpoints.cs                   → Admin student routes: import, list, deactivate, reset password
│   │   ├── StudentQueryDto.cs                    → Pagination query DTO for student listing
│   │   ├── StudentResponseDto.cs                 → Student response DTO
│   │   ├── ImportResultDto.cs                    → CSV import summary DTO
│   │   └── ImportErrorDto.cs                     → Per-row CSV import error DTO
│   └── Users/
│       ├── UserEndpoints.cs                      → Admin user and staff management routes
│       ├── UserResponseDto.cs                    → User list response DTO
│       ├── CreateStaffDto.cs                     → Staff creation request DTO
│       ├── UpdateStaffDto.cs                     → Staff update request DTO
│       ├── StaffResponseDto.cs                   → Staff list response DTO
│       ├── StaffQueryDto.cs                      → Pagination query DTO for staff listing
│       ├── ResetPasswordResponseDto.cs           → Password reset confirmation DTO
│       └── Validators/
│           └── CreateStaffDtoValidator.cs        → FluentValidation rules for staff creation
├── Infrastructure/
│   ├── AppDbContext.cs                           → EF Core DbContext with all DbSets and model configuration
│   ├── SecurityHelper.cs                         → JWT generation, BCrypt hashing, claim extraction helpers
│   ├── Extensions/
│   │   ├── ServiceExtensions.cs                  → DI: validators, storage, rate limiter, CORS, memory cache
│   │   ├── DatabaseExtensions.cs                 → DI: MySQL DbContext, Redis, DB seeder
│   │   ├── SecurityExtensions.cs                 → JWT Bearer auth and authorization policy registration
│   │   ├── SwaggerExtensions.cs                  → Swagger/OpenAPI with Bearer auth support
│   │   └── MiddlewareExtensions.cs               → Registers GlobalExceptionMiddleware
│   ├── Filters/
│   │   └── EnumSchemaFilter.cs                   → Swagger filter to render enums as strings
│   ├── Media/
│   │   ├── IImageValidator.cs                    → Interface for image MIME and signature validation
│   │   ├── ImageValidator.cs                     → Validates JPEG/PNG files by content type and byte signature
│   │   ├── IUploadService.cs                     → Interface for uploading issue images
│   │   └── UploadService.cs                      → Validates and uploads images to S3 via storage provider
│   ├── Middleware/
│   │   └── GlobalExceptionMiddleware.cs           → Catches unhandled exceptions, returns 500
│   ├── Services/
│   │   ├── IStorageService.cs                    → Interface for storage delete and URL resolution
│   │   ├── CsvImportService.cs                   → Parses and bulk-imports students from CSV
│   │   └── IssueTimelineService.cs               → Appends timeline entries to issues
│   └── Storage/
│       ├── Abstractions/
│       │   ├── IStorageProvider.cs               → Interface for storage provider (upload, delete, URL)
│       │   └── IStorageProviderResolver.cs       → Interface for resolving storage provider by name
│       ├── Models/
│       │   └── StorageResult.cs                  → Upload result with key, URL, MIME type, size
│       ├── Options/
│       │   ├── StorageOptions.cs                 → Storage provider name and public base URL config
│       │   └── S3Options.cs                      → AWS S3 bucket, region, keys, CloudFront config
│       ├── Providers/
│       │   └── S3StorageProvider.cs              → AWS S3 upload, delete, and URL generation
│       └── Services/
│           ├── StorageProviderResolver.cs        → Resolves registered storage providers by name
│           ├── StorageService.cs                 → Facade over storage provider for delete and URL
│           └── StorageUrlResolver.cs             → Resolves relative object keys to absolute URLs
└── Migrations/                                   → EF Core migration history files (auto-generated)
```

---

## Backend Architecture Summary

**Entry Point & Startup**

- `Program.cs` registers all services, runs migrations with retry logic, seeds the database, configures middleware, and maps feature endpoint groups
- Uses .NET 8 Minimal APIs — all routes defined in static `Map*Endpoints` extension methods per feature

**Routing Structure**

- `/login` — public, rate-limited
- `/categories` — authenticated
- `/issues/report`, `/student/issues`, `/issues/{id}/upvote` — StudentOnly
- `/staff/issues`, `/staff/issues/{id}/status` — StaffOrAdmin
- `/admin/*` — AdminOnly
- `/womencell/*` — WomenCellOnly
- `/issues/{id}/timeline` — any authenticated role with ownership check

**Authentication Flow**

- `POST /login` checks Students table first (by StudentId or email), then Users table
- On match, `SecurityHelper.GenerateToken()` creates a 2-hour JWT with role, userType, and gender claims
- JWT validated by ASP.NET Core JwtBearer middleware; policies enforce role-based access

**State & Caching**

- Admin issue lists cached in Redis for 5 minutes per `status:page:pageSize` key
- Cache invalidated on issue create, assign, status update, or delete
- Issue categories cached in `IMemoryCache` for 7 days

**Storage**

- `IStorageProvider` abstraction implemented by `S3StorageProvider`
- `IUploadService` validates images (MIME type + byte signature) then delegates to the provider
- Object key format: `issues/{issueId}/{guid}.{ext}`

**Database**

- MySQL via Pomelo EF Core; `AppDbContext` configures all relationships and seed data
- No repository layer — data access is inline in endpoint handlers
- 8 migrations covering schema evolution from initial schema to current state

---

## Feature Mapping

### Authentication

| Layer    | Files                                                                                                                                                |
| -------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| Frontend | `features/auth/pages/LoginPage.jsx`, `features/auth/api/authApi.js`, `features/auth/store/authStore.js`, `features/auth/utils.js`                    |
| Backend  | `Features/Auth/AuthEndpoints.cs`, `Features/Auth/LoginDto.cs`, `Infrastructure/SecurityHelper.cs`, `Infrastructure/Extensions/SecurityExtensions.cs` |
| APIs     | `POST /login`                                                                                                                                        |

### Issue Reporting (Student)

| Layer    | Files                                                                                                                                                                                                                             |
| -------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Frontend | `features/student/pages/ReportIssuePage.jsx`, `features/student/api/studentIssueApi.js`, `features/student/components/ImageUploadField.jsx`, `features/issues/schemas/issueSchemas.js`                                            |
| Backend  | `Features/Issues/IssueEndpoints.cs` (MapPost `/issues/report`), `Features/Issues/CreateIssueDto.cs`, `Features/Issues/CreateIssueValidator.cs`, `Infrastructure/Media/UploadService.cs`, `Infrastructure/Media/ImageValidator.cs` |
| APIs     | `POST /issues/report`, `GET /categories`                                                                                                                                                                                          |

### Issue Tracking (Student)

| Layer    | Files                                                                                                                                                                           |
| -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Frontend | `features/student/pages/StudentIssuesPage.jsx`, `features/student/hooks/useStudentIssues.js`, `features/issues/components/IssueCard.jsx`, `shared/components/IssueTimeline.jsx` |
| Backend  | `Features/Issues/IssueEndpoints.cs` (MapGet `/student/issues`, `/issues/{id}/timeline`), `Features/Issues/IssueResponseDto.cs`                                                  |
| APIs     | `GET /student/issues`, `GET /issues/{id}/timeline`, `POST /issues/{id}/upvote`, `DELETE /issues/{id}/upvote`                                                                    |

### Issue Management (Admin)

| Layer    | Files                                                                                                                                                                          |
| -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Frontend | `features/admin/pages/AdminIssuesPage.jsx`, `features/admin/api/adminIssueApi.js`, `features/admin/components/AdminIssueActions.jsx`, `features/admin/hooks/useAdminIssues.js` |
| Backend  | `Features/Issues/IssueEndpoints.cs` (admin group), `Features/Issues/AdminIssueResponseDto.cs`                                                                                  |
| APIs     | `GET /admin/issues`, `PUT /admin/issues/{id}/assign/{staffId}`, `PUT /admin/issues/{id}/priority`, `PUT /admin/issues/{id}/close`, `DELETE /admin/issues/{id}`                 |

### Staff Management (Admin)

| Layer    | Files                                                                                                                                                                            |
| -------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Frontend | `features/admin/pages/AdminStaffPage.jsx`, `features/users/api/staffApi.js`                                                                                                      |
| Backend  | `Features/Users/UserEndpoints.cs` (staffGroup), `Features/Users/CreateStaffDto.cs`, `Features/Users/StaffResponseDto.cs`, `Features/Users/Validators/CreateStaffDtoValidator.cs` |
| APIs     | `GET /admin/staff`, `POST /admin/staff`, `PUT /admin/staff/{id}`, `PUT /admin/staff/{id}/deactivate`, `PUT /admin/staff/{id}/reset-password`                                     |

### Student Management (Admin)

| Layer    | Files                                                                                                                                                                     |
| -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Frontend | `features/admin/pages/AdminStudentsPage.jsx`, `features/students/api/studentApi.js`                                                                                       |
| Backend  | `Features/Students/StudentEndpoints.cs`, `Infrastructure/Services/CsvImportService.cs`, `Features/Students/StudentResponseDto.cs`, `Features/Students/ImportResultDto.cs` |
| APIs     | `GET /admin/students`, `POST /admin/students/import`, `PUT /admin/students/{id}/deactivate`, `PUT /admin/students/{id}/reset-password`                                    |

### Women Cell Portal

| Layer    | Files                                                                                                                                          |
| -------- | ---------------------------------------------------------------------------------------------------------------------------------------------- |
| Frontend | `features/womencell/pages/WomenCellIssuesPage.jsx`, `features/womencell/api/womencellApi.js`, `features/womencell/hooks/useWomenCellIssues.js` |
| Backend  | `Features/Issues/IssueEndpoints.cs` (womenCellGroup)                                                                                           |
| APIs     | `GET /womencell/issues`, `PUT /womencell/issues/{id}/status`                                                                                   |

---

## Key Dependencies

**Frontend**

- React 18, React Router 6 — rendering and routing
- Zustand — global state (auth, toasts)
- Axios — HTTP client with interceptors
- React Hook Form — form state and validation
- Tailwind CSS — utility-first styling
- Lucide React — icon library
- Vite — build tool

**Backend**

- .NET 8 Minimal APIs — framework
- Entity Framework Core 8 + Pomelo MySQL — ORM and database
- StackExchange.Redis — caching
- FluentValidation — DTO validation
- AWSSDK.S3 — S3 image storage
- BCrypt.Net — password hashing
- Serilog — structured logging
- Swashbuckle — Swagger/OpenAPI docs
- System.IdentityModel.Tokens.Jwt — JWT generation
