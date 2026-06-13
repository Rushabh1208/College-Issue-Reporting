# Implementation Plan — Remove Phase 9 (Resolution Evidence) and Eliminate Related Dependencies

## Context

The original roadmap included a Resolution Evidence feature that required staff members to upload images or documents before an issue could be marked as resolved.

The feature introduced:

* File upload requirements
* S3 storage dependencies
* Evidence data models
* Multipart request handling
* Additional frontend workflows
* Resolution-specific validation rules

The project no longer requires evidence collection and there is no business requirement to store proof-of-resolution artifacts.

This implementation removes all evidence-related functionality while preserving:

* Issue lifecycle management
* Status transitions
* Timeline tracking
* Staff workflows
* Analytics
* Resolution tracking
* Existing APIs

The removal must not break any functionality implemented in Phases 1–8.

---

# Objective

Completely remove Resolution Evidence from the system.

The final platform must:

* Allow staff to resolve issues without uploading files
* Remove all evidence-specific database entities
* Remove all evidence upload endpoints
* Remove S3 upload requirements
* Remove evidence validation logic
* Preserve status management and timeline tracking

---

# Architecture Decision

## Resolution Will Become Metadata-Only

Issue resolution will continue to exist.

Resolution evidence will not.

A resolved issue is determined solely by:

```csharp
Issue.Status == IssueStatus.Resolved
```

No supporting file uploads are required.

No storage providers are required.

No evidence records are stored.

---

# Backend Changes

---

## 1. Remove IssueEvidence Model

### Delete File

```text
backend/Models/IssueEvidence.cs
```

Remove entirely.

### Reason

Evidence records are no longer required.

---

## 2. Remove DbSet

### File

```text
AppDbContext.cs
```

Remove:

```csharp
public DbSet<IssueEvidence> IssueEvidences { get; set; }
```

---

## 3. Remove Evidence Configuration

### Files

Any configuration files containing:

```csharp
IssueEvidenceConfiguration
```

or

```csharp
builder.Entity<IssueEvidence>()
```

Remove entirely.

---

## 4. Remove Database Migration Dependencies

### If Migration Not Applied

Delete:

```text
AddIssueEvidenceTable
```

migration.

---

### If Migration Already Applied

Create rollback migration:

```powershell
dotnet ef migrations add RemoveIssueEvidence
```

Drop:

```sql
IssueEvidences
```

table.

### Constraint

Do not modify unrelated tables.

---

## 5. Remove S3 Resolution Upload Logic

### Files

Search for:

```text
resolution/
```

and

```text
IssueEvidence
```

references.

Remove:

```csharp
UploadToS3(...)
```

calls related to resolution evidence.

---

## 6. Simplify Status Update Endpoint

### File

```text
IssueEndpoints.cs
```

### Endpoint

```http
PUT /staff/issues/{id}/status
```

### Remove

Multipart request handling:

```csharp
IFormFile
IFormFileCollection
```

Remove:

```csharp
Evidence
```

parameter.

---

### Replace With

Simple DTO-based request:

```csharp
public class UpdateIssueStatusDto
{
    public IssueStatus Status { get; set; }
}
```

---

## 7. Remove Resolution Validation

### Delete Logic

```csharp
if(status == Resolved && files.Count == 0)
{
    return Results.BadRequest(...);
}
```

---

### New Behavior

Allow:

```csharp
Resolved
```

status transition without file uploads.

Example:

```csharp
{
    "status": "Resolved"
}
```

must succeed.

---

## 8. Preserve Timeline Functionality

### Keep Existing Logic

When status becomes:

```csharp
IssueStatus.Resolved
```

continue creating:

```text
Resolved
```

timeline entries.

### No Changes Required

Phase 8 timeline functionality remains intact.

---

## 9. Preserve Analytics

### Future Analytics

Resolution metrics should continue using:

```csharp
Issue.Status
```

and

```csharp
IssueTimeline
```

instead of evidence uploads.

No analytics dependency should rely on evidence records.

---

## 10. Remove Storage Configuration Dependencies

### Files

Search:

```text
AWS
S3
StorageProvider
IssueEvidence
```

### Remove

Resolution-specific configuration values:

```json
"S3ResolutionBucket"
```

or similar.

---

### Keep

General storage infrastructure if used elsewhere.

Do not remove global storage services unless they are exclusively used for evidence uploads.

---

# Frontend Changes

---

## 11. Remove Evidence Upload Form

### File

```text
StaffIssuesPage.jsx
```

### Remove

Resolution modal containing:

```text
Before Image
After Image
Document Upload
```

controls.

---

## 12. Remove File Validation

Delete:

```javascript
files.length > 0
```

requirements.

Delete:

```javascript
evidenceRequired
```

logic.

---

## 13. Simplify Resolve Action

### Current Workflow

```text
Resolve
→ Upload Evidence
→ Submit
```

---

### New Workflow

```text
Resolve
→ Confirm
→ Submit
```

---

## 14. Replace With Confirmation Dialog

Recommended implementation:

```text
Are you sure you want to mark this issue as resolved?

[Cancel]
[Resolve]
```

---

### Submit Request

```javascript
await updateStatus(issueId, {
  status: "Resolved"
});
```

No multipart payload.

No file uploads.

---

## 15. Remove Upload Components

Delete:

```text
EvidenceUpload.jsx
ResolutionEvidenceModal.jsx
IssueEvidenceForm.jsx
```

if created previously.

---

## 16. Remove Evidence UI References

Search for:

```text
evidence
before image
after image
resolution upload
```

Remove all references.

---

# Phase Dependency Cleanup

---

## Phase 8

No changes required.

Timeline continues working.

---

## Phase 10 Analytics

Update assumptions.

Resolution analytics must use:

```csharp
Resolved timeline entries
```

or

```csharp
Issue.Status
```

instead of evidence records.

---

## Future WomenCell Workflow

No impact.

---

## Future Duplicate Detection

No impact.

---

## Future Upvotes

No impact.

---

# Build Verification

## Backend

```powershell
dotnet build
```

Expected:

```text
Build succeeded.
```

No references remain to:

* IssueEvidence
* EvidenceType
* StorageProvider
* Resolution uploads

---

## Frontend

```bash
npm run build
```

Expected:

```text
built successfully
```

No references remain to:

* Evidence uploads
* Resolution forms
* File selectors

---

# Verification Checklist

### Resolve Issue

```http
PUT /staff/issues/{id}/status
```

Request:

```json
{
  "status": "Resolved"
}
```

Response:

```http
200 OK
```

---

### Timeline

Timeline contains:

```text
StatusChanged
Resolved
```

entries.

---

### Issue State

Issue status becomes:

```text
Resolved
```

without evidence uploads.

---

### Staff Workflow

Staff can resolve issues in a single action.

No upload UI appears.

---

### Database

No:

```text
IssueEvidences
```

table exists.

No evidence records are created.

---

# Constraints and Rules for the Coding Agent

* Do not introduce replacement evidence functionality
* Do not keep partially implemented evidence models
* Do not require uploads for issue resolution
* Preserve all status transition functionality
* Preserve all timeline functionality
* Preserve analytics compatibility
* Preserve API contracts wherever possible
* Keep issue resolution as a simple status change
* Remove only evidence-related dependencies
* Do not modify unrelated issue workflows
* Existing student, staff, admin, upvote, timeline, and category functionality must continue working unchanged
