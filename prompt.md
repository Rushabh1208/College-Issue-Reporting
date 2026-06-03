# Generate Complete Frontend for Existing Backend API

You are an expert senior frontend architect.

Using the provided backend implementation and `API_DOCUMENTATION.md`, generate a complete production-grade frontend application using:

* React.js
* Vite
* Tailwind CSS
* React Router
* Axios
* Context API or Zustand for auth/global state
* React Hook Form
* Mobile-first responsive design
* Feature-based architecture

The frontend must fully integrate with ALL APIs documented in `API_DOCUMENTATION.md`.

---

# VERY IMPORTANT ARCHITECTURE RULE

The frontend architecture MUST be:

✅ FEATURE-BASED
❌ NOT LAYER-BASED
❌ NOT PAGE-BASED
❌ NOT COMPONENT-TYPE-BASED

DO NOT create folders like:

```text id="kw55i9"
/components
/pages
/hooks
/utils
/services
```

as global dumping folders.

Instead organize the application by FEATURES / DOMAINS.

---

# REQUIRED FOLDER STRUCTURE

Use a scalable feature-separated architecture like this:

```text id="gqkjb9"
src/
│
├── app/
│   ├── router/
│   ├── providers/
│   ├── layouts/
│   └── store/
│
├── shared/
│   ├── components/
│   ├── ui/
│   ├── lib/
│   ├── utils/
│   ├── constants/
│   └── hooks/
│
├── features/
│   │
│   ├── auth/
│   │   ├── api/
│   │   ├── components/
│   │   ├── hooks/
│   │   ├── pages/
│   │   ├── store/
│   │   ├── schemas/
│   │   ├── types/
│   │   └── routes/
│   │
│   ├── issues/
│   │   ├── api/
│   │   ├── components/
│   │   ├── hooks/
│   │   ├── pages/
│   │   ├── store/
│   │   ├── schemas/
│   │   ├── types/
│   │   └── routes/
│   │
│   ├── admin/
│   ├── staff/
│   ├── student/
│   └── users/
│
├── styles/
└── main.jsx
```

Every feature must contain everything related to itself.

No business logic leakage across modules.

---

# DESIGN REQUIREMENTS

The UI must be:

* Clean
* Modern
* Minimal
* Fast
* Mobile-first
* Fully responsive
* Production quality

---

# MOBILE-FIRST DESIGN IS CRITICAL

MOST USERS WILL USE MOBILE DEVICES.

Design priority must be:

1. Mobile phones
2. Tablets
3. Laptops/Desktop

NOT the reverse.

The UI should feel like a modern mobile application first.

---

# MOBILE UX REQUIREMENTS

Implement:

* Bottom navigation on mobile
* Sticky action buttons
* Touch-friendly spacing
* Large tap targets
* Responsive drawers/sheets
* Mobile optimized forms
* Mobile-first cards
* Optimized mobile tables
* Smooth scrolling
* Proper keyboard handling
* Safe-area spacing
* Responsive typography

Avoid desktop-heavy admin panel designs.

---

# DESIGN STYLE

Use:

* Soft modern shadows
* Rounded corners
* Clean spacing
* Minimal borders
* Smooth hover/transition states
* Professional color palette
* Accessible contrast
* Skeleton loading states
* Empty states
* Error states
* Toast notifications

The design should resemble modern SaaS/mobile dashboards.

---

# IMPLEMENT ALL FEATURES FROM API DOCUMENTATION

Read the entire `API_DOCUMENTATION.md`.

Implement ALL available backend capabilities including:

* Authentication
* Login
* Registration
* JWT handling
* Role-based routing
* Protected routes
* Student issue reporting
* File uploads
* Student issue tracking
* Staff assigned issues
* Issue status updates
* Admin issue management
* Staff assignment
* User management
* Pagination
* Filtering
* Error handling
* Rate limit handling

Do NOT skip any documented API.

---

# ROLE-BASED APPLICATION STRUCTURE

Generate separate UX flows for:

## Student

* Report issue
* View own issues
* Track issue status

## Staff

* View assigned issues
* Update issue status

## Admin

* View all issues
* Assign staff
* Delete issues
* View users
* Manage workflows

---

# AUTHENTICATION REQUIREMENTS

Implement complete auth system:

* JWT login
* Persistent auth
* Token storage
* Auto logout
* Protected routes
* Role guards
* Unauthorized handling
* Session restoration
* API interceptors

---

# API LAYER REQUIREMENTS

Generate a clean API architecture.

Example:

```text id="2g5e6f"
features/issues/api/
├── reportIssue.js
├── getStudentIssues.js
├── getStaffIssues.js
├── updateIssueStatus.js
└── adminIssueApi.js
```

Use centralized Axios configuration with interceptors.

Handle:

* 401
* 403
* 429
* network failures
* timeout handling

---

# COMPONENT REQUIREMENTS

Build reusable UI components such as:

* Buttons
* Inputs
* Form fields
* Mobile navigation
* Sidebar
* Cards
* Tables
* Status badges
* Loaders
* Modals
* Sheets
* Empty states
* Error states
* Pagination
* Toasts

---

# FORM REQUIREMENTS

Use:

* React Hook Form
* Validation schemas
* Real-time validation
* Proper error messages
* File upload previews
* Disabled/loading states

---

# ISSUE MANAGEMENT UX

Implement optimized issue workflows:

## Student

* Quick issue creation
* Camera/image upload
* Issue timeline
* Status tracking

## Staff

* Assigned issue queue
* Quick status update actions

## Admin

* Issue overview dashboard
* Assignment workflow
* Filtering
* Search
* Pagination
* User management

---

# PERFORMANCE REQUIREMENTS

Implement:

* Lazy loading
* Route-based code splitting
* Optimized re-renders
* Image optimization
* Request deduplication
* Efficient state management

---

# RESPONSIVE REQUIREMENTS

Every screen must support:

* Mobile
* Tablet
* Desktop
* Landscape mode

The mobile version must NEVER feel like a shrunk desktop UI.

---

# GENERATED OUTPUT

Generate:

```text id="pmy8vn"
- Complete frontend source code
- Folder structure
- Routing system
- Responsive layouts
- API integration layer
- Auth system
- Protected routes
- Reusable UI system
- Feature modules
- Tailwind configuration
- Environment configuration
- README.md
```

---

# CODE QUALITY REQUIREMENTS

Generate:

* Clean architecture
* Reusable code
* Modular logic
* Proper naming
* Consistent patterns
* Scalable structure
* Production-grade code

Avoid:

* giant files
* duplicated logic
* prop drilling everywhere
* deeply nested JSX
* hardcoded values
* messy state management

---

# IMPORTANT RULES

* Read ALL APIs from `API_DOCUMENTATION.md`
* Fully integrate existing backend
* Do NOT invent APIs
* Do NOT skip endpoints
* Respect role permissions
* Follow backend validations
* Use feature-based architecture strictly
* Optimize for mobile-first usage
* Generate clean production-ready UI
* Ensure frontend is scalable and maintainable
* Ensure architecture is suitable for long-term growth

The final result should feel like a real-world modern production application, not a demo project.