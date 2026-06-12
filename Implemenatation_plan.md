# Implementation Plan — Phase 5: Issue Categories & Priority

## Context

.NET 8 Minimal API, EF Core, MySQL (Pomelo), React + Vite frontend. Phase 1 database schema has already introduced the `IssueCategory` entity and the `CategoryId`, `Priority`, `IsAnonymous`, and `UpvoteCount` fields on the `Issue` model. The current issue reporting flow does not expose categories, priorities, anonymous reporting, or category-based privacy messaging to students. Issue cards also do not display category metadata or engagement metrics.

This phase introduces category management, issue prioritization, anonymous reporting support, and category-aware UI behavior.

---

## Objective

Implement issue categorization and prioritization throughout the system.

Students must be able to:

* Select an issue category while reporting
* Set issue priority
* Report anonymously
* Receive privacy messaging when reporting Women Welfare complaints

The issue listing experience must display:

* Category name
* Priority badge
* Upvote count

A public categories endpoint must provide structured category data for frontend consumption.

---

# Backend Implementation

---

## 1. Seed Issue Categories

### File

`backend/Infrastructure/Extensions/DatabaseExtensions.cs`

### Required Changes

Add seed data for all issue categories defined in the project specification.

Each category record must contain:

```csharp
Name
ParentCategory
IsWomenWelfare
```

Example structure:

```csharp
new IssueCategory
{
    Id = 1,
    Name = "Electrical",
    ParentCategory = "Infrastructure",
    IsWomenWelfare = false
}
```

Women Welfare subcategories:

```csharp
new IssueCategory
{
    Id = 21,
    Name = "Harassment",
    ParentCategory = "Women Welfare",
    IsWomenWelfare = true
}
```

```csharp
new IssueCategory
{
    Id = 22,
    Name = "Safety Concern",
    ParentCategory = "Women Welfare",
    IsWomenWelfare = true
}
```

### Constraints

* Seed only if categories do not already exist
* IDs must remain stable
* No duplicate category names
* All Women Welfare subcategories must have:

```csharp
IsWomenWelfare = true
```

---

## 2. Create Categories Endpoint

### New File

`backend/Features/Categories/CategoryEndpoints.cs`

### Endpoint

```http
GET /categories
```

### Authorization

Public endpoint.

No authentication required.

### Response Structure

```json
[
  {
    "parentCategory": "Infrastructure",
    "categories": [
      {
        "id": 1,
        "name": "Electrical",
        "isWomenWelfare": false
      }
    ]
  }
]
```

### Implementation Logic

Query:

```csharp
db.IssueCategories
```

Group by:

```csharp
ParentCategory
```

Project:

```csharp
ParentCategory
Categories[]
```

Order alphabetically.

### Registration

Register endpoint inside:

```csharp
Program.cs
```

or

```csharp
EndpointExtensions.cs
```

depending on current architecture.

---

## 3. Create Categories DTOs

### New File

`backend/Features/Categories/CategoryResponseDto.cs`

```csharp
public class CategoryResponseDto
{
    public string ParentCategory { get; set; } = string.Empty;

    public List<CategoryItemDto> Categories { get; set; } = [];
}
```

### New File

`backend/Features/Categories/CategoryItemDto.cs`

```csharp
public class CategoryItemDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsWomenWelfare { get; set; }
}
```

---

## 4. Create Priority Enum

### New File

`backend/Enums/IssuePriority.cs`

```csharp
namespace backend.Enums
{
    public enum IssuePriority
    {
        Low,
        Medium,
        High,
        Critical
    }
}
```

### Constraints

* Maintain order exactly as above
* Do not reorder later
* Existing API serialization should return enum names

---

## 5. Update CreateIssueDto

### File

`backend/Features/Issues/CreateIssueDto.cs`

### Add Properties

```csharp
public int CategoryId { get; set; }

public IssuePriority Priority { get; set; } = IssuePriority.Medium;

public bool IsAnonymous { get; set; } = false;
```

### Final Behavior

If frontend does not explicitly provide a priority:

```csharp
Medium
```

must be used automatically.

---

## 6. Update Validation Rules

### File

`backend/Features/Issues/CreateIssueDtoValidator.cs`

### Add Rule

```csharp
RuleFor(x => x.CategoryId)
    .GreaterThan(0)
    .WithMessage("Category is required.");
```

### Validation Requirements

Reject:

```json
{
  "categoryId": 0
}
```

Reject:

```json
{
  "categoryId": -1
}
```

Accept valid category IDs only.

---

## 7. Update Issue Creation Endpoint

### File

`backend/Features/Issues/IssueEndpoints.cs`

### Required Changes

Persist:

```csharp
CategoryId
Priority
IsAnonymous
```

during issue creation.

Example:

```csharp
var issue = new Issue
{
    CategoryId = dto.CategoryId,
    Priority = dto.Priority,
    IsAnonymous = dto.IsAnonymous
};
```

### Additional Validation

Ensure selected category exists:

```csharp
db.IssueCategories
```

If not found:

```http
400 Bad Request
```

Response:

```json
{
  "message": "Invalid category selected."
}
```

---

## 8. Update Issue Response DTO

### File

`backend/Features/Issues/IssueResponseDto.cs`

### Add Properties

```csharp
public string CategoryName { get; set; } = string.Empty;

public IssuePriority Priority { get; set; }

public bool IsAnonymous { get; set; }

public int UpvoteCount { get; set; }
```

---

## 9. Update Issue Mapping Logic

### Files

Any location currently projecting Issue entities to DTOs.

Examples:

```csharp
IssueEndpoints.cs
IssueService.cs
IssueQueries.cs
```

### Include Category Data

Add join:

```csharp
.Include(x => x.Category)
```

Project:

```csharp
CategoryName = issue.Category.Name
```

Project:

```csharp
Priority = issue.Priority

IsAnonymous = issue.IsAnonymous

UpvoteCount = issue.UpvoteCount
```

---

# Frontend Implementation

---

## 10. Categories API Integration

### File

`frontend/src/features/student/api/issuesApi.js`

### Add Method

```javascript
export async function getCategories() {
  const { data } = await apiClient.get("/categories");
  return data;
}
```

---

## 11. Update Report Issue Page

### File

`frontend/src/features/student/pages/ReportIssuePage.jsx`

### Categories Loading

On component mount:

```javascript
useEffect(() => {
  loadCategories();
}, []);
```

Fetch:

```javascript
getCategories()
```

Store in local state.

---

## 12. Category Selection UI

Render grouped categories:

```text
Infrastructure
  ○ Electrical
  ○ Water Supply

Hostel
  ○ Room Maintenance
  ○ Cleaning

Women Welfare
  ○ Harassment
  ○ Safety Concern
```

Implementation options:

### Preferred

Grouped radio sections

### Alternative

Grouped select dropdown

```html
<select>
  <optgroup label="Infrastructure">
  </optgroup>
</select>
```

---

## 13. Priority Selection

Add field:

```jsx
<select>
  <option>Low</option>
  <option>Medium</option>
  <option>High</option>
  <option>Critical</option>
</select>
```

Default:

```javascript
Medium
```

Bind to:

```javascript
priority
```

state.

---

## 14. Anonymous Reporting

Add checkbox:

```jsx
<input type="checkbox" />
```

Label:

```text
Report Anonymously
```

Bind to:

```javascript
isAnonymous
```

Default:

```javascript
false
```

---

## 15. Women Welfare Privacy Notice

When selected category has:

```javascript
isWomenWelfare === true
```

display:

```text
This report will only be visible to WomenCell.
```

### UI Requirements

* Informational card
* Always visible while Women Welfare category remains selected
* Hidden for all other categories

Example:

```jsx
<Alert>
  This report will only be visible to WomenCell.
</Alert>
```

---

## 16. Submit Payload Updates

Update report submission payload.

Current payload:

```javascript
{
  title,
  description
}
```

New payload:

```javascript
{
  title,
  description,
  categoryId,
  priority,
  isAnonymous
}
```

---

## 17. Update Issue Card

### File

`frontend/src/shared/components/IssueCard.jsx`

### Display Category

Example:

```text
Category: Electrical
```

Source:

```javascript
issue.categoryName
```

---

## 18. Display Priority Badge

Priority badge examples:

```text
LOW
MEDIUM
HIGH
CRITICAL
```

Recommended styling:

```javascript
Low       → neutral
Medium    → blue
High      → amber
Critical  → red
```

Display:

```jsx
<PriorityBadge />
```

---

## 19. Display Upvote Count

Show:

```text
↑ 12
```

Source:

```javascript
issue.upvoteCount
```

Position:

* Issue card footer
* Metadata section
* Near status badge

---

# Build Verification

## Backend

```powershell
dotnet build
```

Expected:

```text
Build succeeded.
0 Errors
```

---

## Frontend

```bash
npm run build
```

Expected:

```text
built successfully
```

No ESLint errors.

---

# Verification Checklist

## Categories Endpoint

```http
GET /categories
```

Returns:

```http
200 OK
```

with grouped categories.

---

## Issue Reporting

Student can:

* Select category
* Select priority
* Report anonymously

Issue saves successfully.

---

## Women Welfare

Selecting a Women Welfare category:

* Displays privacy notice
* Submits successfully

---

## Issue List

Issue cards display:

* Category Name
* Priority Badge
* Upvote Count

without layout regressions.

---

# Constraints and Rules for the Coding Agent

* Do not hardcode category lists in the frontend
* Categories must always come from `GET /categories`
* Women Welfare detection must use `IsWomenWelfare` from API response
* `Priority` defaults to `Medium`
* `IsAnonymous` defaults to `false`
* Category validation must occur on both frontend and backend
* Category names displayed on cards must come from the backend response DTO
* Preserve backward compatibility with existing issue listing APIs
* Do not introduce a separate categories administration page in this phase
* Do not add category creation/editing functionality in this phase
* The categories endpoint is read-only and public