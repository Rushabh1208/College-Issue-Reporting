# College Issue Reporting Frontend

Production-oriented React frontend for the existing .NET Minimal API backend.

## Stack

- React + Vite
- Tailwind CSS
- React Router
- Axios
- Zustand
- React Hook Form
- Lucide icons

## Setup

```bash
npm install
npm run dev
```

Create `.env` from `.env.example` when using a custom backend URL.

```env
VITE_API_BASE_URL=https://college-issue-reporting.onrender.com
```

## Implemented Flows

- Public login and student registration
- JWT persistence, role decoding, protected routes, auto logout on `401`
- Student issue reporting with image upload and client-side validation
- Student issue tracking
- Staff assigned issue queue and status updates
- Admin issue filtering, assignment, deletion, inferred pagination
- Admin user list with inferred pagination

## Backend Contract Notes

- Issue upload uses multipart form keys expected by the backend: `Title`, `Description`, `Block`, `RoomNumber`, `Image`.
- Admin pagination does not return totals, so next-page availability is inferred from `items.length === pageSize`.
- JWT refresh is not implemented by the backend; expired or unauthorized sessions return to login.
