# Phase 3 — Student CSV Import

**Source Phase Definition:** Student CSV Import phase from the master implementation roadmap. 

**Dependencies:**

* Phase 1 completed (Students table exists and is migrated)
* Phase 2 completed (Admin authentication, role system, JWT authorization policies available)
* Existing Admin dashboard and routing structure operational

---

# Phase Objective

Introduce a controlled administrative onboarding workflow for student accounts.

This phase eliminates manual student creation and establishes a bulk-import mechanism that:

1. Allows administrators to import student records from CSV.
2. Automatically provisions student accounts.
3. Applies validation and duplicate detection.
4. Creates secure default credentials.
5. Provides lifecycle management for student accounts.
6. Establishes the student administration foundation required by later phases.

This phase is intentionally focused on student account management only.

It does **not** introduce:

* Issue workflows
* Categories
* Upvotes
* Timelines
* Analytics
* WomenCell routing logic beyond what Phase 2 already added

---

# Existing System Analysis

Based on Phase 1 and Phase 2 state:

## Existing Student Model

The `Students` table contains:

```csharp
Id
StudentId
Name
Email
Gender
PasswordHash
IsActive
CreatedAt
```

Student authentication now uses:

```csharp
Students table
```

instead of:

```csharp
Users table
```

Phase 2 unified login already depends on:

```csharp
StudentId
Email
PasswordHash
IsActive
```

Therefore imported students must satisfy those requirements.

---

# Architectural Design

## Import Flow

```text
Admin
  ↓
Upload CSV
  ↓
POST /admin/students/import
  ↓
CsvImportService
  ↓
CSV Validation
  ↓
Duplicate Detection
  ↓
Student Entity Creation
  ↓
Password Hash Generation
  ↓
Bulk Insert
  ↓
Import Summary
  ↓
Admin UI
```

---

# Security Requirements

## Authorization

Only administrators may access student-management endpoints.

Every endpoint must require:

```csharp
.RequireAuthorization("AdminOnly")
```

or equivalent existing admin policy.

---

## Password Security

Never store plaintext passwords.

Imported students receive:

```text
Student@123
```

as temporary password.

Before persistence:

```csharp
SecurityHelper.HashPassword()
```

must be used.

---

## CSV Validation Protection

Reject:

```text
Empty rows
Malformed rows
Duplicate rows
Invalid emails
Invalid gender values
Missing required columns
```

before database insertion.

---

# Backend Implementation

---

# Stage 1 — Student Feature Module Creation

Create:

```text
backend/
└── Features/
    └── Students/
        ├── StudentEndpoints.cs
        ├── StudentResponseDto.cs
        ├── ImportResultDto.cs
        ├── ImportErrorDto.cs
        └── StudentQueryDto.cs
```

---

# StudentResponseDto

Purpose:

Student list API response.

```csharp
public class StudentResponseDto
{
    public long Id { get; set; }

    public string StudentId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}
```

---

# ImportResultDto

Purpose:

Import summary returned after upload.

```csharp
public class ImportResultDto
{
    public int TotalRows { get; set; }

    public int ImportedRows { get; set; }

    public int SkippedRows { get; set; }

    public int DuplicateRows { get; set; }

    public List<ImportErrorDto> Errors { get; set; } = [];
}
```

---

# ImportErrorDto

```csharp
public class ImportErrorDto
{
    public int RowNumber { get; set; }

    public string Message { get; set; } = string.Empty;
}
```

---

# StudentQueryDto

Supports pagination.

```csharp
public class StudentQueryDto
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
```

---

# Stage 2 — CSV Import Service

Create:

```text
backend/
└── Infrastructure/
    └── Services/
        └── CsvImportService.cs
```

---

# Service Responsibilities

The service owns:

```text
File parsing
Header validation
Row validation
Duplicate detection
Entity creation
Password generation
Import summary generation
```

Endpoint must remain thin.

---

# Required Header Format

Expected CSV:

```csv
student_id,student_name,student_email,student_gender
2025001,John Doe,john@college.edu,Male
2025002,Jane Doe,jane@college.edu,Female
```

Header comparison should be:

```csharp
OrdinalIgnoreCase
```

to prevent case issues.

---

# Row Validation Rules

For every row:

Validate:

```text
StudentId present
Name present
Email present
Gender present
```

---

Validate email:

```csharp
MailAddress
```

or equivalent validator.

---

Validate gender:

Allowed values:

```text
Male
Female
Other
```

Any other value:

```text
Skipped
Logged in Errors collection
```

---

# Duplicate Detection

Must detect duplicates in:

### Database

```csharp
Students.StudentId
Students.Email
```

---

### Same Upload File

Example:

```csv
2025001,...
2025001,...
```

Second occurrence skipped.

---

# Performance Strategy

Avoid:

```csharp
SELECT per row
```

Pattern.

Instead:

1. Read CSV.
2. Extract all StudentIds.
3. Extract all Emails.
4. Query DB once.

Example:

```csharp
existingStudentIds
existingEmails
```

using:

```csharp
HashSet<string>
```

for O(1) lookups.

---

# Bulk Insert Strategy

Do not call:

```csharp
SaveChanges()
```

per row.

Instead:

```csharp
AddRange()
SaveChangesAsync()
```

once.

---

# Stage 3 — Student Management Endpoints

Create endpoint registration:

```csharp
app.MapStudentEndpoints();
```

inside Program startup configuration.

---

# Endpoint 1 — Import Students

```http
POST /admin/students/import
```

Content-Type:

```text
multipart/form-data
```

Payload:

```text
file=<csv>
```

---

Validation:

```text
File required
.csv only
Not empty
```

---

Response:

```json
{
  "totalRows": 100,
  "importedRows": 95,
  "skippedRows": 5,
  "duplicateRows": 3,
  "errors": [...]
}
```

---

# Endpoint 2 — Student Listing

```http
GET /admin/students
```

Query:

```text
?page=1&pageSize=20
```

Response:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 20,
  "total": 500
}
```

---

Sorting:

```text
Newest first
```

using:

```csharp
CreatedAt DESC
```

---

# Endpoint 3 — Deactivate Student

```http
PUT /admin/students/{id}/deactivate
```

Behavior:

```csharp
student.IsActive = false;
```

---

Validation:

```text
Student exists
```

---

Response:

```http
204 No Content
```

---

# Endpoint 4 — Reset Password

```http
PUT /admin/students/{id}/reset-password
```

Behavior:

```text
Reset to Student@123
```

Implementation:

```csharp
student.PasswordHash =
    SecurityHelper.HashPassword("Student@123");
```

---

Response:

```json
{
  "message": "Password reset successfully"
}
```

---

# Frontend Implementation

# Stage 4 — API Layer

Create:

```text
frontend/src/features/students/api/studentApi.js
```

Functions:

```javascript
importStudents()
getStudents()
deactivateStudent()
resetPassword()
```

---

# Stage 5 — Student Administration Page

Create:

```text
frontend/src/features/admin/pages/AdminStudentsPage.jsx
```

Responsibilities:

```text
CSV Upload
Import Results
Student Listing
Pagination
Deactivate
Password Reset
```

---

# Upload Section

Components:

```text
File Picker
Upload Button
Import Result Card
```

Accepted:

```html
<input accept=".csv" />
```

only.

---

# Import Summary UI

Display:

```text
Total Rows
Imported Rows
Skipped Rows
Duplicate Rows
```

and

```text
Validation Errors
```

in expandable section.

---

# Student Table

Columns:

```text
Student ID
Name
Email
Gender
Status
Created Date
Actions
```

---

# Status Badge

```text
Active
Inactive
```

visual distinction required.

---

# Actions Column

Buttons:

```text
Reset Password
Deactivate
```

Deactivate hidden when already inactive.

---

# Confirmation Dialogs

Required for:

```text
Deactivate
Reset Password
```

to prevent accidental actions.

---

# Stage 6 — Routing

Modify:

```text
frontend/src/app/router/router.jsx
```

Add:

```jsx
{
  path: "/admin/students",
  element: <AdminStudentsPage />
}
```

Protected by:

```jsx
RequireAuth([ROLES.ADMIN])
```

---

# Stage 7 — Navigation

Modify:

```text
frontend/src/app/layouts/AppLayout.jsx
```

Admin navigation:

```javascript
{
  to: "/admin/students",
  label: "Students",
  icon: GraduationCap
}
```

---

# File-Level Impact Analysis

## New Backend Files

```text
Features/Students/StudentEndpoints.cs
Features/Students/StudentResponseDto.cs
Features/Students/StudentQueryDto.cs
Features/Students/ImportResultDto.cs
Features/Students/ImportErrorDto.cs
Infrastructure/Services/CsvImportService.cs
```

---

## Modified Backend Files

```text
Program.cs
AppDbContext.cs (read-only usage only, no schema changes)
DependencyInjection registration file(s)
```

---

## Files That Must Remain Unchanged

```text
IssueEndpoints.cs
Issue Models
Issue DTOs
Issue Services
IssueCategory logic
Timeline logic
Analytics logic
```

Those belong to later phases. 

---

## New Frontend Files

```text
features/students/api/studentApi.js
features/admin/pages/AdminStudentsPage.jsx
```

---

## Modified Frontend Files

```text
router.jsx
AppLayout.jsx
shared/constants/api.js (only if route constants exist)
```

---

# Implementation Sequence

## Step 1

Create DTOs.

---

## Step 2

Create CsvImportService.

---

## Step 3

Implement import endpoint.

---

## Step 4

Implement student list endpoint.

---

## Step 5

Implement deactivate endpoint.

---

## Step 6

Implement password reset endpoint.

---

## Step 7

Register services and endpoints.

---

## Step 8

Build frontend API layer.

---

## Step 9

Build AdminStudentsPage.

---

## Step 10

Add routing.

---

## Step 11

Add navigation.

---

## Step 12

End-to-end validation.

---

# Testing Strategy

## Unit Tests

CsvImportService:

Test:

```text
Valid CSV
Invalid header
Duplicate StudentId
Duplicate Email
Invalid Gender
Invalid Email
Empty Rows
Mixed Valid/Invalid Records
```

---

## Integration Tests

Verify:

```http
POST /admin/students/import
GET /admin/students
PUT /admin/students/{id}/deactivate
PUT /admin/students/{id}/reset-password
```

Authorization:

```text
Admin = allowed
Staff = denied
Student = denied
WomenCell = denied
```

---

## Regression Testing

Verify Phase 2 login still works:

```text
Student login
Admin login
Staff login
WomenCell login
```

No auth behavior should change.

---

# Manual QA Checklist

### Import

* Upload valid CSV
* Upload empty CSV
* Upload malformed CSV
* Upload duplicate records
* Upload wrong extension

### Student List

* Pagination works
* Correct counts displayed
* Newly imported students visible

### Password Reset

* Reset password
* Login with Student@123

### Deactivation

* Deactivate student
* Verify login denied after deactivation

---

# Completion Criteria

Phase 3 is complete only when:

* CSV import successfully creates student accounts.
* Duplicate detection works.
* Validation errors are reported correctly.
* Student list supports pagination.
* Admin can deactivate students.
* Admin can reset passwords.
* Only Admin users can access all student-management endpoints.
* Frontend management screen is fully operational.
* No regression is introduced into Phase 1 or Phase 2 functionality.
* Backend and frontend builds complete without errors.
