# Phase 4 — Staff Management by Admin (Final Approved Version)

## Phase Objective

This phase introduces a dedicated Staff Management module that allows administrators to manage operational staff accounts through the application rather than through manual database operations.

Administrators will be able to:

* Create Staff accounts
* View Staff accounts
* Edit Staff account details
* Deactivate Staff accounts
* Reset Staff passwords

This phase is intentionally limited to **Staff management only**.

---

# Out of Scope

The following are explicitly excluded from Phase 4:

* Student management (completed in Phase 3)
* WomenCell management
* Admin management
* Issue assignment workflows
* Issue categories
* Priority handling
* Timelines
* Analytics
* Staff grouping
* Departments
* Assignment groups
* Resolution groups

No additional organizational fields should be introduced.

---

# Dependency Analysis

## Phase 1 Dependency

Provides:

```text
Users table
Role enum
IsActive field
PasswordHash field
```

---

## Phase 2 Dependency

Provides:

```text
Unified authentication
JWT handling
Admin authorization
WomenCell role support
Role-based access control
```

---

## Phase 3 Dependency

Provides:

```text
Admin management patterns
Pagination patterns
Password reset workflow patterns
Activation/deactivation workflow patterns
```

Phase 4 should follow the same architectural approach used in Student Management.

---

# Existing System Analysis

Current user architecture:

## Students

Stored in:

```text
Students table
```

Managed through:

```text
Phase 3 Student Management
```

---

## Administrative Users

Stored in:

```text
Users table
```

Contains:

```text
Admin
Staff
WomenCell
```

Current limitation:

```text
No administrative interface exists for Staff management.
Staff accounts require manual database operations.
```

Phase 4 resolves this limitation.

---

# Architecture Overview

```text
Admin
  ↓
Staff Management Dashboard
  ↓
Create / Edit / Deactivate / Reset Password
  ↓
UserEndpoints
  ↓
Validation
  ↓
Users Table
  ↓
Authentication System
```

---

# Design Constraints

## Single User Table

Continue using:

```csharp
Users
```

No new tables should be introduced.

---

## Staff Only

Every endpoint in this phase must operate exclusively on:

```csharp
UserRole.Staff
```

The following roles must never be managed through Staff Management:

```text
Admin
WomenCell
Student
```

---

# Backend Implementation

## Stage 1 — DTO Layer

Create:

```text
backend/
└── Features/
    └── Users/
        ├── CreateStaffDto.cs
        ├── UpdateStaffDto.cs
        ├── StaffResponseDto.cs
        ├── StaffQueryDto.cs
        └── ResetPasswordResponseDto.cs
```

---

## CreateStaffDto

```csharp
public class CreateStaffDto
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
```

---

## Validation Rules

### Name

```text
Required
Minimum Length: 2
Maximum Length: 100
```

### Email

```text
Required
Valid email format
Unique in Users table
```

---

## UpdateStaffDto

```csharp
public class UpdateStaffDto
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
```

---

## StaffResponseDto

```csharp
public class StaffResponseDto
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}
```

---

## StaffQueryDto

```csharp
public class StaffQueryDto
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
```

---

# Stage 2 — UserEndpoints Expansion

Modify:

```text
backend/Features/Users/UserEndpoints.cs
```

Add the following endpoints.

---

# Endpoint 1 — Create Staff

```http
POST /admin/staff
```

Authorization:

```csharp
.RequireAuthorization("AdminOnly")
```

### Request

```json
{
  "name": "Maintenance Officer",
  "email": "maintenance@college.edu"
}
```

### Processing

1. Validate request
2. Normalize email

```csharp
dto.Email.Trim().ToLowerInvariant()
```

3. Check email uniqueness
4. Generate default password

```text
Staff@123
```

5. Hash password

```csharp
SecurityHelper.HashPassword()
```

6. Create user

```csharp
Role = UserRole.Staff
IsActive = true
```

7. Save

### Response

```json
{
  "message": "Staff account created successfully"
}
```

---

# Endpoint 2 — Update Staff

```http
PUT /admin/staff/{id}
```

Authorization:

```text
AdminOnly
```

### Editable Fields

```text
Name
Email
```

### Non-Editable Fields

```text
Role
PasswordHash
CreatedAt
```

### Validation

Verify:

```text
User exists
Role == Staff
Email uniqueness
```

Reject:

```text
Admin
WomenCell
```

accounts.

---

# Endpoint 3 — Deactivate Staff

```http
PUT /admin/staff/{id}/deactivate
```

### Behavior

```csharp
user.IsActive = false;
```

### Validation

Verify:

```text
User exists
Role == Staff
```

Reject:

```text
Admin
WomenCell
```

accounts.

### Response

```http
204 No Content
```

---

# Endpoint 4 — Reset Password

```http
PUT /admin/staff/{id}/reset-password
```

### Password

```text
Staff@123
```

Implementation:

```csharp
user.PasswordHash =
    SecurityHelper.HashPassword("Staff@123");
```

### Validation

Verify:

```text
User exists
Role == Staff
```

Reject:

```text
Admin
WomenCell
```

accounts.

### Response

```json
{
  "message": "Password reset successfully"
}
```

---

# Endpoint 5 — Staff Listing

```http
GET /admin/staff
```

Authorization:

```text
AdminOnly
```

### Query

```text
?page=1&pageSize=20
```

### Filtering

Only return:

```csharp
Role == UserRole.Staff
```

Exclude:

```text
Admin
WomenCell
Student
```

### Sorting

```text
CreatedAt DESC
```

Newest first.

### Response

```json
{
  "items": [],
  "page": 1,
  "pageSize": 20,
  "total": 15
}
```

---

# Stage 3 — Validation Layer

Create:

```text
backend/Features/Users/Validators/
    CreateStaffDtoValidator.cs
```

Validation Rules:

```text
Name required
Valid email
Email uniqueness checked in endpoint
```

---

# Stage 4 — Frontend API Layer

Create:

```text
frontend/src/features/users/api/staffApi.js
```

Functions:

```javascript
getStaff()
createStaff()
updateStaff()
deactivateStaff()
resetStaffPassword()
```

---

# Stage 5 — Staff Management Page

Create:

```text
frontend/src/features/admin/pages/AdminStaffPage.jsx
```

Responsibilities:

```text
Staff listing
Create staff
Edit staff
Deactivate staff
Reset password
Pagination
```

---

# Layout Structure

```text
+---------------------------------------------------+
| Staff Management                                  |
+---------------------------------------------------+

[ Add Staff ]

+---------------------------------------------------+
| Name | Email | Status | Created Date | Actions |
+---------------------------------------------------+
```

---

# Table Columns

```text
Name
Email
Status
Created Date
Actions
```

---

# Status Badge

Display:

```text
Active
Inactive
```

---

# Add Staff Modal

Fields:

```text
Name
Email
```

All accounts created through this module become:

```csharp
UserRole.Staff
```

---

# Edit Staff Modal

Fields:

```text
Name
Email
```

---

# Actions

```text
Edit
Reset Password
Deactivate
```

---

# Confirmation Dialogs

Required for:

```text
Deactivate
Reset Password
```

---

# Stage 6 — Routing

Modify:

```text
frontend/src/app/router/router.jsx
```

Add:

```jsx
{
  path: "/admin/staff",
  element: <AdminStaffPage />
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

Add:

```javascript
{
  to: "/admin/staff",
  label: "Staff",
  icon: UsersRound
}
```

to the Admin navigation menu.

---

# File-Level Impact Analysis

## New Backend Files

```text
Features/Users/CreateStaffDto.cs
Features/Users/UpdateStaffDto.cs
Features/Users/StaffResponseDto.cs
Features/Users/StaffQueryDto.cs
Features/Users/ResetPasswordResponseDto.cs
Features/Users/Validators/CreateStaffDtoValidator.cs
```

---

## Modified Backend Files

```text
Features/Users/UserEndpoints.cs
Program.cs
Dependency Injection registration files
```

---

## Backend Files That Must Remain Unchanged

```text
Features/Auth/*
Features/Students/*
IssueEndpoints.cs
Issue DTOs
Issue Models
Analytics
Timeline
Categories
```

---

## New Frontend Files

```text
features/users/api/staffApi.js
features/admin/pages/AdminStaffPage.jsx
```

---

## Modified Frontend Files

```text
app/router/router.jsx
app/layouts/AppLayout.jsx
```

---

# Implementation Sequence

1. Create DTOs
2. Create validators
3. Implement GET staff list endpoint
4. Implement POST create staff endpoint
5. Implement PUT update staff endpoint
6. Implement PUT reset password endpoint
7. Implement PUT deactivate endpoint
8. Register endpoints
9. Create frontend API layer
10. Build Staff Management page
11. Add routing
12. Add navigation
13. Execute end-to-end validation

---

# Testing Strategy

## Unit Tests

### CreateStaffDtoValidator

Test:

```text
Valid payload
Empty name
Invalid email
Duplicate email
```

---

## Endpoint Tests

### POST /admin/staff

Verify:

```text
Staff creation succeeds
Duplicate email rejected
Invalid email rejected
```

### PUT /admin/staff/{id}

Verify:

```text
Update succeeds
Duplicate email rejected
Unknown user rejected
```

### PUT /admin/staff/{id}/deactivate

Verify:

```text
Staff deactivated
Admin rejected
WomenCell rejected
```

### PUT /admin/staff/{id}/reset-password

Verify:

```text
Password reset succeeds
Staff can log in with Staff@123
```

---

# Regression Testing

Verify:

```text
Student login works
Admin login works
WomenCell login works
Phase 3 student management works
```

No existing authentication behavior should regress.

---

# Completion Criteria

Phase 4 is complete only when:

* Admin can create Staff accounts.
* Admin can edit Staff details.
* Admin can deactivate Staff accounts.
* Admin can reset Staff passwords.
* Staff list supports pagination.
* Admin and WomenCell accounts cannot be managed through this module.
* All endpoints are Admin-only.
* Frontend Staff Management UI is fully operational.
* No regressions are introduced into Phases 1–3.

**Stop here.**
