# College Issue Reporting Backend API Documentation

> Generated from the backend source code and deployed endpoint verification against `https://college-issue-reporting.onrender.com`.

---

## Project Overview

- **Backend framework:** .NET 8 Minimal API
- **Database:** MySQL (via Pomelo.EntityFrameworkCore.MySql)
- **ORM:** Entity Framework Core 8
- **Caching / secondary storage:** Redis via StackExchange.Redis
- **Authentication strategy:** JWT Bearer tokens
- **API base URL:** `https://college-issue-reporting.onrender.com`
- **API versioning strategy:** Swagger documentation version `v1`; no route-based versioning present.
- **Response format conventions:** JSON by default. Property names are preserved from C# models and DTOs. String enums are serialized as strings.
- **Error handling conventions:**
  - `400 Bad Request` for validation and bad data
  - `401 Unauthorized` for missing/invalid JWT
  - `403 Forbidden` for role-based access violations or unauthorized resource actions
  - `404 Not Found` for missing resources
  - `429 Too Many Requests` for configured rate limit quotas
  - `500 Internal Server Error` for unexpected exceptions
- **Rate limiting:**
  - `POST /login`: 5 requests per minute
  - `POST /issues/report`: 5 requests per minute
- **File upload strategy:** S3-backed object storage; image URLs are returned by the API.
- **Realtime support:** None.
- **Webhooks:** None.
- **Queue jobs:** None.

---

## Authentication System

### JWT / Auth flow

- Client authenticates using `POST /login` with email and password.
- Successful login returns a JWT token in the response body:

```json
{
  "token": "<JWT>"
}
```

- The JWT is valid for **2 hours**.
- The token contains these claims:
  - `NameIdentifier` => user id
  - `Name` => user email
  - `Role` => user role string

### Login flow

- `POST /login` is public.
- On success, it returns `200 OK` with a JWT.
- On failure, it returns `401 Unauthorized`.

### Refresh token flow

- Not implemented.
- If the token expires, the frontend must re-authenticate via `/login`.

### Session handling

- Stateless JWT authentication.
- No server-side session store.

### OAuth providers

- None.

### API key usage

- None.

### Cookie usage

- None.

### CSRF handling

- None explicitly configured.
- API is intended for bearer-token usage, not browser cookies.

### Role system

- Roles in code: `Student`, `Staff`, `Admin`
- Role policies configured:
  - `AdminOnly` => Admin role only
  - `StudentOnly` => Student role only
  - `StaffOrAdmin` => Staff or Admin roles

### Permission system

- Role-based policy checks are enforced using `RequireAuthorization` on route groups.

### Authorization header format

```
Authorization: Bearer <token>
```

---

## Roles & Authorization Matrix

| Endpoint | Public | Student | Staff | Admin | Notes |
|---|:---:|:---:|:---:|:---:|---|
| `GET /` | ✅ | ✅ | ✅ | ✅ | Health endpoint |
| `POST /login` | ✅ | ✅ | ✅ | ✅ | Login returns JWT |
| `POST /register` | ✅ | ✅ | ✅ | ✅ | Register creates Student |
| `POST /issues/report` | ❌ | ✅ | ❌ | ❌ | Student only |
| `GET /student/issues` | ❌ | ✅ | ❌ | ❌ | Student only |
| `GET /staff/issues` | ❌ | ❌ | ✅ | ✅ | Staff or Admin |
| `PUT /staff/issues/{id}/status` | ❌ | ❌ | ✅ | ✅ | Staff or Admin; only assigned user may update |
| `GET /admin/issues` | ❌ | ❌ | ❌ | ✅ | Admin only |
| `PUT /admin/issues/{id}/assign/{staffId}` | ❌ | ❌ | ❌ | ✅ | Admin only |
| `DELETE /admin/issues/{id}` | ❌ | ❌ | ❌ | ✅ | Admin only |
| `GET /admin/users` | ❌ | ❌ | ❌ | ✅ | Admin only |

---

## API ENDPOINTS

---

## [GET] /

### Description
Health check endpoint that confirms the API is running.

---

### Verification Status
```
✅ Verified Against Deployment
```

---

### Authorization
- Public

---

### Full Endpoint URL
```
https://college-issue-reporting.onrender.com/
```

---

### Request Headers
| Header | Required | Description |
|---|:---:|---|
| `Accept` | No | Optional client preference |

---

### Path Parameters
None

---

### Query Parameters
None

---

### Request Body
None

---

### Request Body Fields
None

---

### File Uploads
None

---

### Success Response
```json
"Campus API is running and healthy! 🚀"
```

---

### Success Response Fields
| Field | Type | Description |
|---|---|---|
| plain text | string | Health check message |

---

### Error Responses
| Status Code | Meaning | Cause |
|---|---|---|---|
| `500` | Internal Server Error | Unexpected exception in middleware |

---

### Business Logic Notes
- This endpoint is a lightweight readiness check.
- It does not access the database or authorization systems.

---

### Related Models
None

---

### Frontend Integration Notes
- Use it for application startup and monitoring.
- Treat it as a public health probe.
- Do not require authentication or tokens.

---

### Example cURL Request
```bash
curl https://college-issue-reporting.onrender.com/
```

---

### Example Frontend Request
#### Fetch API
```js
const res = await fetch('https://college-issue-reporting.onrender.com/');
const text = await res.text();
console.log(text);
```

#### Axios
```js
const res = await axios.get('https://college-issue-reporting.onrender.com/');
console.log(res.data);
```

---

## [POST] /login

### Description
Authenticates a user and returns a JWT bearer token.

---

### Verification Status
```
⚠️ Code Exists But Direct Runtime Login Verification Was Not Confirmed From Deployment
```

---

### Authorization
- Public

---

### Full Endpoint URL
```
https://college-issue-reporting.onrender.com/login
```

---

### Request Headers
| Header | Required | Description |
|---|:---:|---|
| `Content-Type` | Yes | Must be `application/json` |
| `Accept` | No | Optional |

---

### Path Parameters
None

---

### Query Parameters
None

---

### Request Body
```json
{
  "email": "student@example.com",
  "password": "password123"
}
```

---

### Request Body Fields
| Field | Type | Required | Validation |
|---|---|:---:|---|
| `email` | string | Yes | Must be provided |
| `password` | string | Yes | Must be provided |

---

### File Uploads
None

---

### Success Response
```json
{
  "token": "<JWT_TOKEN>"
}
```

---

### Success Response Fields
| Field | Type | Description |
|---|---|---|
| `token` | string | JWT bearer token valid for 2 hours |

---

### Error Responses
| Status Code | Meaning | Cause |
|---|---|---|
| `400` | Bad Request | Invalid JSON or missing fields |
| `401` | Unauthorized | Invalid email/password |
| `429` | Too Many Requests | Login rate limit exceeded |

---

### Business Logic Notes
- Looks up the user by email.
- Verifies password with BCrypt.
- Returns a signed JWT using `Jwt:Key`.

---

### Related Models
- `backend.Models.User`

---

### Frontend Integration Notes
- Store the returned token securely.
- Use the token in `Authorization: Bearer <token>` for protected endpoints.
- Retry login on 401 after showing credentials error.
- Handle 429 by throttling or showing a rate limit message.

---

### Example cURL Request
```bash
curl -X POST https://college-issue-reporting.onrender.com/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@gmail.com","password":"admin123"}'
```

---

### Example Frontend Request
#### Fetch API
```js
const response = await fetch('https://college-issue-reporting.onrender.com/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email: 'admin@gmail.com', password: 'admin123' })
});
const data = await response.json();
```

#### Axios
```js
const response = await axios.post('https://college-issue-reporting.onrender.com/login', {
  email: 'admin@gmail.com',
  password: 'admin123'
});
```

---

## [POST] /register

### Description
Creates a new student user account.

---

### Verification Status
```
⚠️ Code Exists But Not Verified Against Deployment
```

---

### Authorization
- Public

---

### Full Endpoint URL
```
https://college-issue-reporting.onrender.com/register
```

---

### Request Headers
| Header | Required | Description |
|---|:---:|---|
| `Content-Type` | Yes | `application/json` |

---

### Path Parameters
None

---

### Query Parameters
None

---

### Request Body
```json
{
  "name": "Jane Student",
  "email": "jane.student@college.edu",
  "password": "strongpassword"
}
```

---

### Request Body Fields
| Field | Type | Required | Validation |
|---|---|:---:|---|
| `name` | string | Yes | Must be provided |
| `email` | string | Yes | Must be provided |
| `password` | string | Yes | Must be provided |

---

### File Uploads
None

---

### Success Response
```json
{
  "message": "User registered successfully",
  "id": 123,
  "email": "jane.student@college.edu"
}
```

---

### Success Response Fields
| Field | Type | Description |
|---|---|---|
| `message` | string | Confirmation string |
| `id` | integer | New user id |
| `email` | string | Registered email |

---

### Error Responses
| Status Code | Meaning | Cause |
|---|---|---|
| `400` | Bad Request | Email already registered |

---

### Business Logic Notes
- New users are created with the role `Student`.
- No additional onboarding metadata is stored.
- Passwords are hashed with BCrypt.

---

### Related Models
- `backend.Models.User`

---

### Frontend Integration Notes
- Immediately redirect new users to login after registration.
- Avoid returning the password in any frontend flow.
- Use user-facing email validation before submission.

---

### Example cURL Request
```bash
curl -X POST https://college-issue-reporting.onrender.com/register \
  -H "Content-Type: application/json" \
  -d '{"name":"Jane Student","email":"jane.student@college.edu","password":"strongpassword"}'
```

---

## [POST] /issues/report

### Description
Allows a student to create a new issue report and optionally upload one image.

---

### Verification Status
```
⚠️ Code Exists But Endpoint Not Directly Verified Against Deployment
```

---

### Authorization
- Authenticated Student only

---

### Full Endpoint URL
```
https://college-issue-reporting.onrender.com/issues/report
```

---

### Request Headers
| Header | Required | Description |
|---|:---:|---|
| `Authorization` | Yes | `Bearer <token>` |
| `Content-Type` | Yes | `multipart/form-data` |

---

### Path Parameters
None

---

### Query Parameters
None

---

### Request Body
Form-data fields:
- `Title` (string)
- `Description` (string)
- `Block` (string)
- `RoomNumber` (string)
- `Image` (file, optional)

Example form data structure:
```
Title=Broken classroom fan
Description=The fan in room 302 is not working.
Block=C
RoomNumber=302
Image=@/path/to/photo.png
```

---

### Request Body Fields
| Field | Type | Required | Validation |
|---|---|:---:|---|
| `Title` | string | Yes | max 100 chars |
| `Description` | string | Yes | max 500 chars |
| `Block` | string | Yes | max 10 chars |
| `RoomNumber` | string | Yes | max 10 chars |
| `Image` | file | No | JPEG/PNG only, max 5MB |

---

### File Uploads
| Field | MIME Types | Max Size | Single / Multiple | Storage |
|---|---|---|---|---|
| `Image` | `image/jpeg`, `image/jpg`, `image/png` | 5 MB | Single | Stored in AWS S3 and returned as a cloud URL |

---

### Success Response
```json
{
  "message": "Issue created",
  "id": 456,
  "imageUrl": "https://your-cdn-or-s3-url.example/..."
}
```

---

### Success Response Fields
| Field | Type | Description |
|---|---|---|
| `message` | string | Confirmation string |
| `id` | integer | Created issue id |
| `imageUrl` | string|null | URL to uploaded image if provided |

---

### Error Responses
| Status Code | Meaning | Cause |
|---|---|---|
| `400` | Bad Request | Validation failure or invalid file upload |
| `401` | Unauthorized | Missing or invalid JWT |
| `403` | Forbidden | User is not a student |
| `429` | Too Many Requests | Issue report rate limit exceeded |

---

### Business Logic Notes
- The authenticated user becomes the `UserId` for the issue.
- Status is set to `Open` initially.
- If an image is uploaded, it is saved as `{issueId}.jpeg`.
- Writes to the database are performed before image persistence so the issue id is available.
- Admin issue cache is invalidated after creation.

---

### Related Models
- `backend.Models.Issue`
- `backend.Models.User`

---

### Frontend Integration Notes
- Use `multipart/form-data`.
- Send the JWT in `Authorization` header.
- Always include the required fields.
- Use client-side validation to match backend validator limits.
- Handle 429 by delaying retries.
- Do not assume `imageUrl` is always present.

---

### Example cURL Request
```bash
curl -X POST https://college-issue-reporting.onrender.com/issues/report \
  -H "Authorization: Bearer {{token}}" \
  -F "Title=Broken fan" \
  -F "Description=The fan in classroom A101 is not working." \
  -F "Block=A" \
  -F "RoomNumber=A101" \
  -F "Image=@/path/to/photo.png"
```

---

### Example Frontend Request
#### Fetch API
```js
const formData = new FormData();
formData.append('Title', 'Broken fan');
formData.append('Description', 'The fan in classroom A101 is not working.');
formData.append('Block', 'A');
formData.append('RoomNumber', 'A101');
formData.append('Image', fileInput.files[0]);

const response = await fetch('https://college-issue-reporting.onrender.com/issues/report', {
  method: 'POST',
  headers: {
    Authorization: `Bearer ${token}`
  },
  body: formData
});
const data = await response.json();
```

#### Axios
```js
const formData = new FormData();
formData.append('Title', 'Broken fan');
formData.append('Description', 'The fan in classroom A101 is not working.');
formData.append('Block', 'A');
formData.append('RoomNumber', 'A101');
formData.append('Image', fileInput.files[0]);

const response = await axios.post('https://college-issue-reporting.onrender.com/issues/report', formData, {
  headers: {
    Authorization: `Bearer ${token}`,
    'Content-Type': 'multipart/form-data'
  }
});
```

---

## [GET] /student/issues

### Description
Returns all issues created by the authenticated student.

---

### Verification Status
```
⚠️ Code Exists But Endpoint Not Verified Against Deployment
```

---

### Authorization
- Authenticated Student only

---

### Full Endpoint URL
```
https://college-issue-reporting.onrender.com/student/issues
```

---

### Request Headers
| Header | Required | Description |
|---|:---:|---|
| `Authorization` | Yes | `Bearer <token>` |

---

### Path Parameters
None

---

### Query Parameters
None

---

### Request Body
None

---

### Request Body Fields
None

---

### File Uploads
None

---

### Success Response
```json
[
  {
    "id": 456,
    "title": "Broken fan",
    "description": "The fan in classroom A101 is not working.",
    "status": "Open",
    "block": "A",
    "roomNumber": "A101",
    "assignedStaffName": "Unassigned",
    "imageUrl": "https://your-cdn-or-s3-url.example/...",
    "createdAt": "2026-06-03T12:34:56Z"
  }
]
```

---

### Success Response Fields
| Field | Type | Description |
|---|---|---|
| `id` | integer | Issue id |
| `title` | string | Issue title |
| `description` | string | Issue description |
| `status` | string | `Open`, `InProgress`, or `Resolved` |
| `block` | string | Campus block |
| `roomNumber` | string | Room number |
| `assignedStaffName` | string | Staff name or `Unassigned` |
| `imageUrl` | string|null | Public URL if image exists |
| `createdAt` | string | UTC timestamp |

---

### Error Responses
| Status Code | Meaning | Cause |
|---|---|---|
| `401` | Unauthorized | Missing or invalid JWT |
| `403` | Forbidden | Role is not Student |

---

### Business Logic Notes
- Only non-deleted issues are returned.
- Issues are ordered by `CreatedAt` descending.
- There is no total count metadata returned.

---

### Related Models
- `backend.Models.Issue`

---

### Frontend Integration Notes
- Use this endpoint to populate student issue dashboards.
- Treat `imageUrl` as optional.
- No pagination metadata is returned, so use cursor / incremental load only with client-side logic.

---

### Example cURL Request
```bash
curl -H "Authorization: Bearer {{token}}" \
  https://college-issue-reporting.onrender.com/student/issues
```

---

### Example Frontend Request
#### Fetch API
```js
const response = await fetch('https://college-issue-reporting.onrender.com/student/issues', {
  headers: { Authorization: `Bearer ${token}` }
});
const issues = await response.json();
```

#### Axios
```js
const response = await axios.get('https://college-issue-reporting.onrender.com/student/issues', {
  headers: { Authorization: `Bearer ${token}` }
});
```

---

## [GET] /staff/issues

### Description
Returns all issues assigned to the authenticated staff user.

---

### Verification Status
```
⚠️ Code Exists But Endpoint Not Verified Against Deployment
```

---

### Authorization
- Authenticated Staff or Admin

---

### Full Endpoint URL
```
https://college-issue-reporting.onrender.com/staff/issues
```

---

### Request Headers
| Header | Required | Description |
|---|:---:|---|
| `Authorization` | Yes | `Bearer <token>` |

---

### Path Parameters
None

---

### Query Parameters
None

---

### Request Body
None

---

### Request Body Fields
None

---

### File Uploads
None

---

### Success Response
Same schema as `GET /student/issues`, except `assignedStaffName` may be `Me` for the current assigned staff.

---

### Success Response Fields
| Field | Type | Description |
|---|---|---|
| `assignedStaffName` | string | Returns `Me` or staff display name |

---

### Error Responses
| Status Code | Meaning | Cause |
|---|---|---|
| `401` | Unauthorized | Missing or invalid JWT |
| `403` | Forbidden | Role is not Staff or Admin |

---

### Business Logic Notes
- Returns issues where `AssignedToId == current user id`.
- Only non-deleted issues are returned.

---

### Related Models
- `backend.Models.Issue`

---

### Frontend Integration Notes
- Use this endpoint for staff dashboards.
- If response is empty, the staff user has no assigned issues.

---

### Example cURL Request
```bash
curl -H "Authorization: Bearer {{token}}" \
  https://college-issue-reporting.onrender.com/staff/issues
```

---

### Example Frontend Request
#### Axios
```js
const response = await axios.get('https://college-issue-reporting.onrender.com/staff/issues', {
  headers: { Authorization: `Bearer ${token}` }
});
```

---

## [PUT] /staff/issues/{id}/status

### Description
Updates the status of an assigned issue.

---

### Verification Status
```
⚠️ Code Exists But Endpoint Not Verified Against Deployment
```

---

### Authorization
- Authenticated Staff or Admin

---

### Full Endpoint URL
```
https://college-issue-reporting.onrender.com/staff/issues/{id}/status
```

---

### Request Headers
| Header | Required | Description |
|---|:---:|---|
| `Authorization` | Yes | `Bearer <token>` |
| `Content-Type` | Yes | `application/json` |

---

### Path Parameters
| Parameter | Type | Required | Description |
|---|---|:---:|---|
| `id` | integer | Yes | Issue id |

---

### Query Parameters
None

---

### Request Body
```json
{
  "status": "Resolved"
}
```

---

### Request Body Fields
| Field | Type | Required | Validation |
|---|---|:---:|---|
| `status` | string | Yes | Must be one of `Open`, `InProgress`, `Resolved` |

---

### File Uploads
None

---

### Success Response
```json
{
  "message": "Status updated"
}
```

---

### Success Response Fields
| Field | Type | Description |
|---|---|---|
| `message` | string | Confirmation string |

---

### Error Responses
| Status Code | Meaning | Cause |
|---|---|---|
| `400` | Bad Request | Invalid status or reopen of resolved issue |
| `401` | Unauthorized | Missing or invalid JWT |
| `403` | Forbidden | Not assigned to this issue or role mismatch |
| `404` | Not Found | Issue missing or deleted |

---

### Business Logic Notes
- Only the user assigned to the issue may update it.
- Reopening a resolved issue to `Open` is forbidden.
- Status update invalidates the admin cache.

---

### Related Models
- `backend.Models.Issue`

---

### Frontend Integration Notes
- Use this endpoint after issue assignment.
- Refresh relevant issue lists after success.
- Handle 403 separately from 401.

---

### Example cURL Request
```bash
curl -X PUT https://college-issue-reporting.onrender.com/staff/issues/456/status \
  -H "Authorization: Bearer {{token}}" \
  -H "Content-Type: application/json" \
  -d '{"status":"Resolved"}'
```

---

## [GET] /admin/issues

### Description
Returns a paginated list of all non-deleted issues with optional status filtering.

---

### Verification Status
```
⚠️ Code Exists But Endpoint Not Verified Against Deployment
```

---

### Authorization
- Admin only

---

### Full Endpoint URL
```
https://college-issue-reporting.onrender.com/admin/issues
```

---

### Request Headers
| Header | Required | Description |
|---|:---:|---|
| `Authorization` | Yes | `Bearer <token>` |

---

### Path Parameters
None

---

### Query Parameters
| Parameter | Type | Required | Default | Description |
|---|---|:---:|:---:|---|
| `status` | string | No | none | Filter by `Open`, `InProgress`, or `Resolved` |
| `page` | integer | No | `1` | Page number |
| `pageSize` | integer | No | `10` | Page size |

---

### Request Body
None

---

### Request Body Fields
None

---

### File Uploads
None

---

### Success Response
Array of issue objects using `IssueResponseDto`.

---

### Success Response Fields
Same fields as `GET /student/issues`.

---

### Error Responses
| Status Code | Meaning | Cause |
|---|---|---|
| `401` | Unauthorized | Missing or invalid JWT |
| `403` | Forbidden | Role is not Admin |

---

### Business Logic Notes
- Results are cached in Redis for 5 minutes per status/page/pageSize.
- Cache key: `admin:issues:{status}:{page}:{pageSize}`.
- Cache is invalidated on issue creation, assignment, deletion, and status update.

---

### Related Models
- `backend.Models.Issue`

---

### Frontend Integration Notes
- Use query parameters to page through results.
- There is no total count metadata, so use `pageSize` and list length to infer pagination end.
- Prefer server-side filtering by status before requesting.
- Cache client-side results for short intervals, but refresh after admin actions.

---

### Example cURL Request
```bash
curl -H "Authorization: Bearer {{token}}" \
  "https://college-issue-reporting.onrender.com/admin/issues?page=1&pageSize=20&status=Open"
```

---

## [PUT] /admin/issues/{id}/assign/{staffId}

### Description
Assigns a staff member to an issue and moves the issue into `InProgress`.

---

### Verification Status
```
⚠️ Code Exists But Endpoint Not Verified Against Deployment
```

---

### Authorization
- Admin only

---

### Full Endpoint URL
```
https://college-issue-reporting.onrender.com/admin/issues/{id}/assign/{staffId}
```

---

### Request Headers
| Header | Required | Description |
|---|:---:|---|
| `Authorization` | Yes | `Bearer <token>` |

---

### Path Parameters
| Parameter | Type | Required | Description |
|---|---|:---:|---|
| `id` | integer | Yes | Issue id |
| `staffId` | integer | Yes | Staff user id |

---

### Query Parameters
None

---

### Request Body
None

---

### Request Body Fields
None

---

### File Uploads
None

---

### Success Response
```json
{
  "message": "Issue assigned to staff",
  "issue": 123,
  "status": "InProgress"
}
```

---

### Success Response Fields
| Field | Type | Description |
|---|---|---|
| `message` | string | Confirmation string |
| `issue` | integer | Issue id |
| `status` | string | Updated issue status |

---

### Error Responses
| Status Code | Meaning | Cause |
|---|---|---|
| `400` | Bad Request | `staffId` is not a staff user |
| `401` | Unauthorized | Missing or invalid JWT |
| `403` | Forbidden | Role is not Admin |
| `404` | Not Found | Issue missing or deleted |

---

### Business Logic Notes
- Issue status is set to `InProgress` automatically.
- Admin cache for issues is invalidated.

---

### Related Models
- `backend.Models.Issue`
- `backend.Models.User`

---

### Frontend Integration Notes
- Use this endpoint from an admin assignment UI.
- Confirm the staff member is a valid staff user before calling.
- Refresh issue lists after assignment.

---

### Example cURL Request
```bash
curl -X PUT https://college-issue-reporting.onrender.com/admin/issues/456/assign/3 \
  -H "Authorization: Bearer {{token}}"
```

---

## [DELETE] /admin/issues/{id}

### Description
Soft-deletes an issue and deletes its uploaded image if present.

---

### Verification Status
```
⚠️ Code Exists But Endpoint Not Verified Against Deployment
```

---

### Authorization
- Admin only

---

### Full Endpoint URL
```
https://college-issue-reporting.onrender.com/admin/issues/{id}
```

---

### Request Headers
| Header | Required | Description |
|---|:---:|---|
| `Authorization` | Yes | `Bearer <token>` |

---

### Path Parameters
| Parameter | Type | Required | Description |
|---|---|:---:|---|
| `id` | integer | Yes | Issue id |

---

### Query Parameters
None

---

### Request Body
None

---

### Request Body Fields
None

---

### File Uploads
None

---

### Success Response
```json
{
  "message": "Issue deleted successfully"
}
```

---

### Success Response Fields
| Field | Type | Description |
|---|---|---|
| `message` | string | Confirmation string |

---

### Error Responses
| Status Code | Meaning | Cause |
|---|---|---|
| `401` | Unauthorized | Missing or invalid JWT |
| `403` | Forbidden | Role is not Admin |
| `404` | Not Found | Issue missing or already deleted |

---

### Business Logic Notes
- Sets `IsDeleted = true` on the issue.
- Deletes the image file from local storage if present.
- Does not physically remove the database row.
- Invalidates the admin issue cache.

---

### Related Models
- `backend.Models.Issue`

---

### Frontend Integration Notes
- Confirm deletion intent before calling.
- Reload admin issue lists after deletion.
- The issue remains hidden from all issue list endpoints after soft delete.

---

### Example cURL Request
```bash
curl -X DELETE https://college-issue-reporting.onrender.com/admin/issues/456 \
  -H "Authorization: Bearer {{token}}"
```

---

## [GET] /admin/users

### Description
Returns a paginated list of user records.

---

### Verification Status
```
⚠️ Code Exists But Endpoint Not Verified Against Deployment
```

---

### Authorization
- Admin only

---

### Full Endpoint URL
```
https://college-issue-reporting.onrender.com/admin/users
```

---

### Request Headers
| Header | Required | Description |
|---|:---:|---|
| `Authorization` | Yes | `Bearer <token>` |

---

### Path Parameters
None

---

### Query Parameters
| Parameter | Type | Required | Default | Description |
|---|---|:---:|:---:|---|
| `page` | integer | No | `1` | Page number |
| `pageSize` | integer | No | `10` | Page size |

---

### Request Body
None

---

### Request Body Fields
None

---

### File Uploads
None

---

### Success Response
```json
[
  {
    "id": 1,
    "name": "Admin",
    "email": "admin@gmail.com",
    "role": "Admin"
  }
]
```

---

### Success Response Fields
| Field | Type | Description |
|---|---|---|
| `id` | integer | User id |
| `name` | string | User full name |
| `email` | string | Email address |
| `role` | string | `Student`, `Staff`, or `Admin` |

---

### Error Responses
| Status Code | Meaning | Cause |
|---|---|---|
| `401` | Unauthorized | Missing or invalid JWT |
| `403` | Forbidden | Role is not Admin |

---

### Business Logic Notes
- No total count metadata is returned.
- Results are ordered by user id ascending.

---

### Related Models
- `backend.Models.User`

---

### Frontend Integration Notes
- Use this endpoint for admin user management.
- Since pagination metadata is not included, infer end-of-list by returned array length.

---

### Example cURL Request
```bash
curl -H "Authorization: Bearer {{token}}" \
  "https://college-issue-reporting.onrender.com/admin/users?page=1&pageSize=20"
```

---

## RESPONSE MODELS

### Authentication Response
```json
{
  "token": "<JWT_TOKEN>"
}
```

### User Response
```json
{
  "id": 1,
  "name": "Admin",
  "email": "admin@gmail.com",
  "role": "Admin"
}
```

### Issue Response
```json
{
  "id": 101,
  "title": "Broken faucet",
  "description": "The faucet in B201 leaks constantly.",
  "status": "Open",
  "block": "B",
  "roomNumber": "201",
  "assignedStaffName": "Unassigned",
  "imageUrl": "https://your-cdn-or-s3-url.example/...",
  "createdAt": "2026-06-03T12:00:00Z"
}
```

### Error Response - Validation
```json
[
  "Title is required",
  "Description is required"
]
```

### Error Response - Unauthorized
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "traceId": "..."
}
```

### Error Response - Internal Server Error
Plain text body: `Internal Server Error`

---

## ERROR HANDLING

- Standard validation errors are returned as an array of error messages.
- Authentication errors are returned as `401 Unauthorized` with the framework default response.
- Permission errors are returned as `403 Forbidden`.
- Rate limits return `429 Too Many Requests` on `POST /login` and `POST /issues/report`.
- Server errors are handled by global exception middleware and return `500 Internal Server Error`.

---

## PAGINATION SYSTEM

- Pagination is supported on the admin list endpoints only.
- Query params:
  - `page` (integer, default `1`)
  - `pageSize` (integer, default `10`)
- There is no pagination metadata such as `total`, `hasNext`, or `pageCount` returned by the API.
- Sorting is fixed to `CreatedAt` descending for issue lists.
- Filtering is available only for `GET /admin/issues` via `status`.
- Infinite scrolling is possible but must be managed by frontend logic because only arrays are returned.

---

## REALTIME EVENTS

- No WebSocket or Socket.IO support is implemented.
- No realtime event channels exist in this backend.

---

## WEBHOOKS

- None.

---

## SAFE ENVIRONMENT VARIABLES

| Variable | Description |
|---|---|
| `ConnectionStrings:DefaultConnection` | MySQL database connection string |
| `Jwt:Key` | JWT signing secret (DO NOT expose) |
| `Redis:ConnectionString` | Redis connection string for caching and invalidation |

> Do not expose actual secret values in public documentation.

---

## DATABASE MODEL OVERVIEW

### `User`
- `Id` (long)
- `Name` (string)
- `Email` (string)
- `PasswordHash` (string)
- `Role` (`Student`, `Staff`, `Admin`)

### `Issue`
- `Id` (long)
- `Title` (string)
- `Description` (string)
- `Status` (`Open`, `InProgress`, `Resolved`)
- `UserId` (long) — student who created the issue
- `AssignedToId` (long?) — staff user id
- `Block` (string)
- `RoomNumber` (string)
- `CreatedAt` (DateTime)
- `ImagePath` (string?) — local storage path / filename
- `IsDeleted` (bool)

### Relationships
- `Issue` has one `User` (creator).
- `Issue` has an optional assigned `User` as staff.
- `Issue.Status` is indexed for query performance.

---

## API FLOW EXPLANATIONS

### Authentication flow
1. User sends credentials to `POST /login`.
2. Server validates credentials with BCrypt.
3. Server issues a JWT valid for 2 hours.
4. Frontend stores token and uses it for authenticated endpoints.
5. If token expires, the user must log in again. No refresh token flow exists.

### Issue creation flow
1. Student provides issue details and optionally uploads an image.
2. Frontend sends multipart/form-data to `POST /issues/report`.
3. Server validates required fields and image constraints.
4. Server stores the issue and saves the image locally.
5. Admin issue caches are invalidated.
6. The student receives issue id and image URL.

### Issue resolution flow
1. Staff or Admin requests `PUT /staff/issues/{id}/status`.
2. Server validates the status enum.
3. Server checks the authenticated user matches `AssignedToId`.
4. Server updates the status and invalidates cache.
5. Cannot reopen an issue from `Resolved` back to `Open`.

### Notification flow
- No notification system is implemented in this backend.
- Frontend must poll or refresh lists after changes.

### User onboarding flow
1. New user registers via `POST /register`.
2. Server creates a `Student` user record.
3. User can immediately log in via `/login`.

---

## AI FRONTEND AGENT NOTES

- Use `https://college-issue-reporting.onrender.com` as the base URL.
- Start with user authentication: login and registration.
- Store JWT securely and attach to all protected requests.
- No refresh token exists; implement a re-auth flow when the token expires.
- Use role-specific views:
  - Student: `/issues/report`, `/student/issues`
  - Staff: `/staff/issues`, `/staff/issues/{id}/status`
  - Admin: `/admin/issues`, `/admin/issues/{id}/assign/{staffId}`, `/admin/issues/{id}`, `/admin/users`
- Handle rate limits carefully for `POST /login` and `POST /issues/report`.
- Because pagination metadata is not returned, maintain local state to infer continuation.
- Use `multipart/form-data` for issue reporting when file uploads are involved.
- Avoid optimistic updates on admin issue deletion or assignment without reloading from the server.
- If `imageUrl` is null, render a placeholder instead of an empty image.
- Prefer one request per user action and refresh lists after status or assignment changes.
- Treat `403` as a role/permission issue, and `401` as a login issue.
- Implement retry logic for transient `429` / timeout errors, but do not retry on validation errors.
- For autonomous UI generation, map roles to permissions and present only supported actions.

---

## Deployment and Verification Notes

- Verified deployment health at `https://college-issue-reporting.onrender.com/`.
- Verified Swagger JSON is available at `https://college-issue-reporting.onrender.com/swagger/v1/swagger.json`.
- The Swagger endpoint confirms all route paths exist in deployed code.
- Authentication endpoints were not directly confirmed with a successful login due to runtime request formatting differences during verification.

---

## Seeded Sample Accounts (from source)

- Admin: `admin@gmail.com` / `admin123`
- Staff: `staff1@gmail.com` / `staff@123`
- Staff: `staff2@gmail.com` / `staff@123`

> These accounts are defined in source code seed logic and may exist on fresh deployments.
