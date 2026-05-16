# Engineering Audit & Improvement Roadmap

## Codebase Audit Summary

| Category | Assessment | Evidence |
| :--- | :--- | :--- |
| **Architecture Quality** | **Medium** | Clean vertical slices but tight coupling in Security helpers. |
| **Maintainability** | **High** | Consistent naming and small, focused feature files. |
| **Scalability** | **Medium** | Redis caching implemented for Admin, but DB reads are unoptimized. |
| **Security** | **Low** | Obsolete hashing (SHA256) and permissive CORS. |
| **Performance** | **Medium** | Minimal APIs and Redis help, but redundant DB hits exist. |
| **Test Coverage** | **None** | No test project or files detected in the repository. |
| **Dev Experience** | **Medium** | Docker and Swagger are present, but no CI/CD or linting. |

---

## Detailed Findings

### 1. Obsolete Password Hashing
- **Issue:** Use of SHA256 without salt for password storage.
- **Evidence:** `Infrastructure/SecurityHelper.cs:33-39`.
- **Why It Matters:** Vulnerable to rainbow table attacks and fast brute-forcing.
- **Recommended Fix:** Implement `BCrypt.Net-Next` (already in `.csproj`).
- **Risk Level:** High | **Complexity:** Low

### 2. Redundant Authentication Database Hits
- **Issue:** `GetUserId` performs a database lookup on every authenticated request.
- **Evidence:** `Infrastructure/SecurityHelper.cs:41-55`.
- **Why It Matters:** Significant overhead for every protected API call; scales poorly with user growth.
- **Recommended Fix:** Embed `UserId` in JWT claims and extract it directly from the `ClaimsPrincipal`.
- **Risk Level:** Medium | **Complexity:** Medium

### 3. Permissive CORS Policy
- **Issue:** `AllowAnyOrigin`, `AllowAnyHeader`, and `AllowAnyMethod` are active.
- **Evidence:** `Infrastructure/Extensions/ServiceExtensions.cs:15-18`.
- **Why It Matters:** Exposes the API to Cross-Site Request Forgery (CSRF) and data leakage.
- **Recommended Fix:** Define a whitelist of allowed origins (e.g., from `appsettings.json`).
- **Risk Level:** High | **Complexity:** Low

### 4. Lack of Automated Testing
- **Issue:** Zero unit or integration tests.
- **Evidence:** Root directory scan (No `.Tests` projects found).
- **Why It Matters:** High risk of regressions; difficult to refactor business logic safely.
- **Recommended Fix:** Add an xUnit project and implement tests for the `Features` logic.
- **Risk Level:** Medium | **Complexity:** High

---

## Improvement Roadmap

### Phase 1 — Critical Security Fixes
- **Task 1: Migrate to BCrypt**
  - **Files:** `Infrastructure/SecurityHelper.cs`, `Features/Auth/AuthEndpoints.cs`.
  - **Outcome:** Passwords stored with modern hashing and salting.
- **Task 2: Hardened CORS**
  - **Files:** `Infrastructure/Extensions/ServiceExtensions.cs`.
  - **Outcome:** API only accessible from verified domains.

### Phase 2 — Performance Stabilization
- **Task 1: Claims-Based Identity**
  - **Files:** `Infrastructure/SecurityHelper.cs`, `Features/Auth/AuthEndpoints.cs`, `Features/Issues/IssueEndpoints.cs`.
  - **Outcome:** Remove ~1 database query per authenticated request.
- **Task 2: Database Indexing**
  - **Files:** `Infrastructure/AppDbContext.cs`.
  - **Outcome:** Optimize queries for `UserId` and `AssignedToId` in the `Issues` table.

### Phase 3 — Refactoring & Abstraction
- **Task 1: Extract Service Layer**
  - **Files:** `Features/Issues/IssueEndpoints.cs` -> `Features/Issues/IssueService.cs`.
  - **Outcome:** Separate business logic (status transitions, assignment rules) from HTTP handling.
- **Task 2: Repository Pattern**
  - **Outcome:** Abstract data access to allow for easier unit testing with mocks.

### Phase 4 — Scalability & DX
- **Task 1: Implement SignalR**
  - **Outcome:** Real-time notifications for issue assignments and status changes.
- **Task 2: CI/CD Pipeline**
  - **Files:** `.github/workflows/main.yml`.
  - **Outcome:** Automated build and test on every PR.

---

## Task Breakdown

- [ ] **Fix Password Hashing**
  - **Why:** Current SHA256 implementation is unsafe.
  - **Files:** `Infrastructure/SecurityHelper.cs`
  - **Priority:** High | **Risk:** Low | **Dependencies:** None

- [ ] **Implement Claims-based ID**
  - **Why:** Reduces DB load by 1 query per request.
  - **Files:** `Infrastructure/SecurityHelper.cs`, `Features/Auth/AuthEndpoints.cs`
  - **Priority:** Medium | **Risk:** Medium | **Dependencies:** None

- [ ] **Initialize Test Suite**
  - **Why:** No existing tests.
  - **Files:** Create `backend.Tests` project.
  - **Priority:** Medium | **Risk:** Low | **Dependencies:** None

- [ ] **Restrict CORS Origins**
  - **Why:** Prevent unauthorized cross-origin access.
  - **Files:** `Infrastructure/Extensions/ServiceExtensions.cs`
  - **Priority:** High | **Risk:** Low | **Dependencies:** None

---

## Final Engineering Summary

- **Overall Maturity Score:** 6/10
- **Biggest Weaknesses:** Obsolete security patterns and total lack of testing.
- **Highest ROI Improvement:** Switching to claims-based identity and implementing BCrypt.
- **Suggested Milestone:** "V1.1 Security & Performance Patch".
