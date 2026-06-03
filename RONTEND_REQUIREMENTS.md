# FRONTEND_REQUIREMENTS.md

# College Issue Reporting System — Frontend Requirements

## Project Goal

Build a production-grade frontend application for the College Issue Reporting System using:

* React.js
* Vite
* Tailwind CSS
* React Router
* Axios
* Zustand or Context API
* React Hook Form

The frontend must integrate completely with the backend APIs defined in:

```text id="vmd4p3"
API_DOCUMENTATION.md
```

The application must be:

* mobile-first
* modern
* responsive
* scalable
* maintainable
* feature-based
* production quality

---

# CORE FRONTEND PRINCIPLES

## 1. MOBILE-FIRST IS MANDATORY

The majority of users will access the platform from mobile devices.

The UI/UX must therefore prioritize:

1. Mobile phones
2. Tablets
3. Desktop/Laptop

Desktop layouts should be adaptations of the mobile experience — not the reverse.

Avoid desktop-centric dashboard patterns.

---

# DESIGN LANGUAGE

The UI should feel:

* modern
* clean
* lightweight
* minimal
* smooth
* professional

Design inspiration:

* Linear
* Notion
* Stripe
* Vercel
* modern SaaS dashboards
* modern mobile applications

---

# VISUAL DESIGN RULES

## Use

* soft shadows
* rounded corners
* subtle borders
* modern spacing
* responsive typography
* clean card layouts
* modern forms
* subtle animations
* skeleton loaders
* smooth transitions
* proper empty states
* proper error states
* modern status badges
* responsive drawers/sheets

---

## Avoid

* cluttered layouts
* giant tables on mobile
* tiny buttons
* overly dense admin UIs
* excessive borders
* old dashboard styles
* cramped spacing
* inconsistent padding
* nested modal complexity

---

# COLOR SYSTEM

Use a modern accessible palette.

Suggested:

## Primary

```text id="8h8oyh"
#2563eb
```

## Success

```text id="v4z2r8"
#16a34a
```

## Warning

```text id="gtvxyi"
#f59e0b
```

## Error

```text id="6u4xvt"
#dc2626
```

## Neutral

Use Tailwind slate/zinc palette.

---

# RESPONSIVE DESIGN REQUIREMENTS

Every screen must support:

* mobile portrait
* mobile landscape
* tablet
* desktop

---

# MOBILE UX REQUIREMENTS

Implement:

* bottom navigation
* sticky CTA buttons
* touch-friendly spacing
* swipe-friendly cards
* responsive sheets/drawers
* mobile optimized forms
* mobile-friendly issue cards
* large tap targets
* proper safe-area spacing
* optimized keyboard interactions

---

# DESKTOP UX REQUIREMENTS

Desktop should include:

* sidebar navigation
* responsive dashboard layouts
* multi-column layouts where appropriate
* larger content containers

But mobile experience always takes priority.

---

# FEATURE-BASED ARCHITECTURE

The project architecture MUST be feature-separated.

DO NOT create large global folders like:

```text id="6ot9o2"
/pages
/components
/services
/hooks
```

Use domain-based architecture.

---

# REQUIRED FOLDER STRUCTURE

```text id="58b55x"
src/
│
├── app/
│   ├── layouts/
│   ├── router/
│   ├── providers/
│   └── store/
│
├── shared/
│   ├── ui/
│   ├── components/
│   ├── hooks/
│   ├── utils/
│   ├── constants/
│   └── lib/
│
├── features/
│   ├── auth/
│   ├── student/
│   ├── staff/
│   ├── admin/
│   ├── issues/
│   └── users/
│
├── styles/
└── main.jsx
```

Each feature must own:

* API logic
* components
* hooks
* routes
* state
* validation
* feature-specific utilities

---

# AUTHENTICATION REQUIREMENTS

Implement:

* JWT authentication
* persistent login
* session restoration
* protected routes
* role guards
* token expiration handling
* logout flows
* API interceptors
* unauthorized handling

---

# ROLE-BASED APPLICATION REQUIREMENTS

## Student Features

Students must be able to:

* register
* login
* report issues
* upload issue images
* track submitted issues
* view issue statuses

---

## Staff Features

Staff must be able to:

* login
* view assigned issues
* update issue statuses
* manage assigned workload

---

## Admin Features

Admins must be able to:

* view all issues
* filter issues
* paginate issues
* assign staff
* delete issues
* manage users
* monitor issue workflow

---

# API INTEGRATION REQUIREMENTS

Use:

* centralized Axios instance
* interceptors
* token injection
* global error handling

Handle properly:

* 401 Unauthorized
* 403 Forbidden
* 429 Rate Limited
* network errors
* timeout errors

---

# FORM REQUIREMENTS

Use:

* React Hook Form
* schema validation
* live validation feedback
* loading states
* disabled states
* proper error handling
* image previews
* upload progress indicators

---

# ISSUE REPORTING UX

The issue reporting flow must be optimized for mobile usage.

Students should be able to:

* quickly submit issues
* upload images from camera/gallery
* track progress easily
* view status badges clearly

The issue form should feel fast and frictionless.

---

# ISSUE STATUS DESIGN

Use visually distinct status indicators:

| Status     | Suggested Style |
| ---------- | --------------- |
| Open       | Warning         |
| InProgress | Primary         |
| Resolved   | Success         |

---

# NAVIGATION REQUIREMENTS

## Mobile Navigation

Use:

* bottom navigation
* collapsible menus
* floating action buttons where useful

---

## Desktop Navigation

Use:

* sidebar
* responsive topbar
* collapsible navigation

---

# LOADING & FEEDBACK STATES

Implement:

* skeleton loaders
* optimistic UI where safe
* empty states
* retry states
* error boundaries
* toast notifications
* success feedback

---

# PERFORMANCE REQUIREMENTS

Implement:

* lazy loading
* route-based code splitting
* optimized rendering
* image optimization
* minimal unnecessary re-renders

---

# ACCESSIBILITY REQUIREMENTS

Implement:

* keyboard accessibility
* accessible contrast
* proper focus states
* semantic HTML
* screen reader friendly forms

---

# ROUTING REQUIREMENTS

Suggested routes:

```text id="olhx9w"
/login
/register

/student/issues
/student/report

/staff/issues

/admin/issues
/admin/users
```

Use protected role-based routing.

---

# STATE MANAGEMENT REQUIREMENTS

Use Zustand or Context API for:

* auth state
* user state
* UI preferences
* global notifications

Avoid excessive prop drilling.

---

# API BASE URL

Use:

```env id="e1n0sa"
VITE_API_BASE_URL=https://college-issue-reporting.onrender.com
```

---

# FILE UPLOAD REQUIREMENTS

Support:

* image preview
* drag/drop (desktop)
* camera uploads (mobile)
* upload validation
* upload progress

---

# TABLE REQUIREMENTS

Avoid traditional desktop tables on mobile.

For mobile:

* use stacked cards
* responsive data layouts
* collapsible sections

Desktop may use proper tables.

---

# ADMIN DASHBOARD REQUIREMENTS

Admin dashboard should include:

* issue statistics
* filtering
* assignment workflows
* recent issues
* quick actions
* user management

But maintain clean spacing and avoid overcrowding.

---

# ANIMATION REQUIREMENTS

Use subtle animations only.

Examples:

* hover transitions
* page transitions
* loading animations
* modal transitions
* toast animations

Avoid excessive flashy motion.

---

# CODE QUALITY REQUIREMENTS

Generate:

* reusable components
* modular logic
* scalable architecture
* maintainable code
* consistent naming
* clean file organization

Avoid:

* giant components
* duplicated logic
* deeply nested JSX
* unstructured state

---

# IMPORTANT IMPLEMENTATION RULES

Before generating frontend code:

1. Deeply analyze:

   * backend APIs
   * roles
   * workflows
   * validations
   * permissions

2. First plan:

   * routes
   * layouts
   * features
   * state management
   * API architecture
   * responsive behavior

3. Then implement.

Do NOT immediately start generating files without planning the architecture first.

---

# FINAL EXPECTATION

The final frontend should feel like:

* a real production SaaS/mobile application
* scalable for long-term development
* optimized for mobile users
* visually modern
* smooth and intuitive
* cleanly architected

It should NOT feel like:

* a tutorial project
* a hackathon UI
* a generic admin template
* a desktop dashboard shrunk to mobile
